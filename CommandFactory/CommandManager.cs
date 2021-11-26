using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
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
    private readonly ImmutableDictionary<string, ModuleInfo> _module;

    internal CommandManager(List<ModuleInfo> moduleInfos)
    {
      _module = moduleInfos.ToImmutableDictionary(x => x.Name);
    }

    public static async Task<CommandManager> Init(Assembly entryAssembly)
    {
      var modules = await CommandBuilder.BuildAsync(entryAssembly);
      return new CommandManager(modules);
    }

    public async Task BuildAndInstallModulesAsync(DiscordSocketClient client, bool build = false)
    {
      if (build)
      {
        foreach (var (_, module) in _module)
        {
          var command = new SlashCommandBuilder()
            .WithName(module.Name)
            .WithDescription(module.Description);

          // Build Options
          foreach (var parameter in module.Executor.Parameters)
            command.AddOption(parameter.Name, parameter.OptionType, parameter.Description, parameter.IsRequire);

          foreach (var subGroup in module.SubGroups) command.AddOption(await BuildGroups(subGroup.Value));

          try
          {
            await ((SlashModule) module.Module).Register(client, command.Build());
          }
          catch (HttpException ex)
          {
            var json = JsonConvert.SerializeObject(ex.GetBaseException(), Formatting.Indented);
            Console.WriteLine(json);
          }
        }
      }

      client.SlashCommandExecuted += ClientOnSlashCommandExecuted;
    }

    private async Task ClientOnSlashCommandExecuted(SocketSlashCommand command)
    {
      var commandEntryName = command.Data.Name;

      IModuleInfo? moduleInfo = _module.GetValueOrDefault(commandEntryName);
      if (moduleInfo == null)
        return;

      if (command.Data.Options == null)
      {
        ((ModuleInfo) moduleInfo).Executor.Method.Invoke(moduleInfo.Module, null);
      }
      else
      {
        var options = command.Data.Options;

        foreachStart:
        foreach (var option in options)
        {
          if (moduleInfo == null) return;
          var module = moduleInfo.Module;
          module.SetCommand(command);
          switch (option.Type)
          {
            case ApplicationCommandOptionType.SubCommandGroup:
              moduleInfo = moduleInfo.SubGroups.GetValueOrDefault(option.Name);
              options = option.Options;
              goto foreachStart;
            case ApplicationCommandOptionType.SubCommand:
              var commandInfo = moduleInfo.SubCommands.GetValueOrDefault(option.Name);
              await ExecuteCommand(commandInfo, options, module);
              break;
            default:
              await ExecuteCommand(((ModuleInfo) moduleInfo).Executor, options, module);
              break;
          }
        }
      }
    }

    private static Task ExecuteCommand(CommandInfo commandInfo,
      IReadOnlyCollection<SocketSlashCommandDataOption> options, object? module)
    {
      ThreadPool.QueueUserWorkItem(_ =>
      {
        var parameters = new object?[commandInfo.Parameters.Count];

        for (var i = 0; i < parameters.Length; i++)
        {
          var parameter = commandInfo.Parameters[i];
          var receivedData =
            options.SingleOrDefault(x => x.Name == parameter.Name && x.Type == parameter.OptionType);
          if (receivedData == null)
            throw new ReceivedDataException($"Option {parameter.Name} doesn't received");
          parameters[i] = Convert.ChangeType(receivedData.Value, parameter.Type);
        }

        try
        {
          commandInfo.Method.Invoke(module, parameters);
        }
        catch (System.Exception e)
        {
          Console.Error.WriteLine(e);
        }
      });

      return Task.CompletedTask;
    }

    private async Task<SlashCommandOptionBuilder> BuildGroups(IModuleInfo info)
    {
      var option = new SlashCommandOptionBuilder()
        .WithName(info.Name)
        .WithDescription(info.Description)
        .WithType(ApplicationCommandOptionType.SubCommandGroup);

      foreach (var subGroup in info.SubGroups) option.AddOption(await BuildGroups(subGroup.Value));

      foreach (var subCommand in info.SubCommands) option.AddOption(await BuildSubCommand(subCommand.Value));

      return option;
    }

    private async Task<SlashCommandOptionBuilder> BuildSubCommand(CommandInfo subCommand)
    {
      var option = new SlashCommandOptionBuilder()
        .WithName(subCommand.Name)
        .WithDescription(subCommand.Description)
        .WithType(ApplicationCommandOptionType.SubCommand);

      foreach (var parameter in subCommand.Parameters)
        option.AddOption(parameter.Name, parameter.OptionType, parameter.Description, parameter.IsRequire);

      return option;
    }
  }
}