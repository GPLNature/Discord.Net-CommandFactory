using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandFactory.Exception;
using CommandFactory.Info;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace CommandFactory
{
  public class CommandManager
  {
    private ImmutableDictionary<string, ModuleInfo> _module;

    internal CommandManager(List<ModuleInfo> moduleInfos)
    {
      _module = moduleInfos.ToImmutableDictionary(x => x.Name);
    }

    public static async Task<CommandManager> Init(Assembly entryAssembly)
    {
      var modules = await CommandBuilder.BuildAsync(entryAssembly);
      return new CommandManager(modules);
    }

    public async Task BuildAndInstallModulesAsync(DiscordSocketClient client)
    {
      foreach (var (_, module) in _module)
      {
        var command = new SlashCommandBuilder()
          .WithName(module.Name)
          .WithDescription(module.Description);

        // Build Options
        foreach (var parameter in module.Executor.Parameters)
        {
          command.AddOption(parameter.Name, parameter.OptionType, parameter.Description, parameter.IsRequire);
        }
        
        foreach (var subGroup in module.SubGroups)
        {
          command.AddOption(await BuildGroups(subGroup.Value));
        }

        try
        {
          await module.Module.Register(client, command.Build());
        }
        catch (ApplicationCommandException ex)
        {
          var json = JsonConvert.SerializeObject(ex.Error, Formatting.Indented);
          Console.WriteLine(json);
        }
      }

      client.SlashCommandExecuted += ClientOnSlashCommandExecuted;
    }

    private async Task ClientOnSlashCommandExecuted(SocketSlashCommand command)
    {
      var commandEntryName = command.Data.Name;

      var moduleInfo = _module.GetValueOrDefault(commandEntryName);

      if (command.Data.Options == null)
      {
        moduleInfo.Executor.Method.Invoke(moduleInfo.Module, null);
      }
      else
      {
        SubModuleInfo? subModule = null;
        var options = command.Data.Options;
        
        foreachStart:
        foreach (var option in options)
        {
          switch (option.Type)
          {
            case ApplicationCommandOptionType.SubCommandGroup:
              subModule = subModule?.SubGroups.GetValueOrDefault(option.Name) ?? moduleInfo.SubGroups.GetValueOrDefault(option.Name);
              options = option.Options;
              goto foreachStart;
            case ApplicationCommandOptionType.SubCommand:
              var commandInfo = subModule?.SubCommands.GetValueOrDefault(option.Name) ?? moduleInfo.SubCommands.GetValueOrDefault(option.Name);
              var parameters = new object?[commandInfo.Parameters.Count];

              for (int i = 0; i < parameters.Length; i++)
              {
                var parameter = commandInfo.Parameters[i];
                var realType = ParameterChecker.ReverseMappingType(parameter.OptionType);
                if (realType == null) 
                  throw new ReceivedDataException($"Doesn't support {parameter.OptionType}");
                var receivedData = options.SingleOrDefault(x => x.Name == parameter.Name && x.Type == parameter.OptionType);
                if (receivedData == null)
                  throw new ReceivedDataException($"Option {parameter.Name} doesn't received");
                var receivedValue = receivedData.Value;
                if (receivedValue.GetType() != realType)
                  throw new ReceivedDataException($"Option {parameter.Name} can't cast to {realType.Name}");
                parameters[i] = receivedValue;
              }

              commandInfo.Method.Invoke((object?)subModule?.Module ?? moduleInfo.Module, parameters);
              break;
          }
        }
      }
    }

    private async Task<SlashCommandOptionBuilder> BuildGroups(SubModuleInfo info)
    {
      var option = new SlashCommandOptionBuilder()
        .WithName(info.Name)
        .WithDescription(info.Description)
        .WithType(ApplicationCommandOptionType.SubCommandGroup);

      foreach (var subGroup in info.SubGroups)
      {
        option.AddOption(await BuildGroups(subGroup.Value));
      }

      foreach (var subCommand in info.SubCommands)
      {
        option.AddOption(await BuildSubCommand(subCommand.Value));
      }

      return option;
    }

    private async Task<SlashCommandOptionBuilder> BuildSubCommand(CommandInfo subCommand)
    {
      var option = new SlashCommandOptionBuilder()
        .WithName(subCommand.Name)
        .WithDescription(subCommand.Description)
        .WithType(ApplicationCommandOptionType.SubCommand);

      foreach (var parameter in subCommand.Parameters)
      {
        option.AddOption(parameter.Name, parameter.OptionType, parameter.Description, parameter.IsRequire);
      }

      return option;
    }
  }
}