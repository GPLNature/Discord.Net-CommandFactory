using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using CommandFactory.Info;
using Discord;
using Discord.WebSocket;

namespace CommandFactory
{
  public class CommandManager
  {
    private ImmutableList<ModuleInfo> _module;
    
    internal CommandManager(List<ModuleInfo> moduleInfos)
    {
      _module = moduleInfos.ToImmutableList();
    }
    public static async Task<CommandManager> Init(Assembly entryAssembly)
    {
      var modules = await CommandBuilder.BuildAsync(entryAssembly);
      return new CommandManager(modules);
    }

    public async Task BuildAndInstallModulesAsync()
    {
      foreach (var module in _module)
      {
        var command = new SlashCommandBuilder()
          .WithName(module.Name)
          .WithDescription(module.Description);

        // Build Options
        command.AddOption(await BuildSubCommand(module.Executor));
        
        foreach (var subGroup in module.SubGroups)
        {
          command.AddOption(await BuildGroups(subGroup));
        }
        
        Console.WriteLine();
        
        // module.Module.Register(client.Rest, command.Build());
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
        option.AddOption(await BuildGroups(subGroup));
      }

      foreach (var subCommand in info.SubCommands)
      {
        option.AddOption(await BuildSubCommand(subCommand));
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