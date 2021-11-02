using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using CommandFactory.Info;
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

    public async Task BuildAndInstallModulesAsync(BaseSocketClient client)
    {
      foreach (var module in _module)
      {
        
      }
    }
  }
}