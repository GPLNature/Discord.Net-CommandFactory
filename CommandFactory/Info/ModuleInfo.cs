using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace CommandFactory.Info
{
  internal readonly struct ModuleInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly SlashModule Module;
    public readonly CommandInfo Executor;
    public readonly ImmutableDictionary<string, CommandInfo> SubCommands;
    public readonly ImmutableDictionary<string, SubModuleInfo> SubGroups;

    public ModuleInfo(string name, string description, SlashModule module, CommandInfo executor, List<CommandInfo> subCommands, List<SubModuleInfo> subGroups)
    {
      Name = name;
      Description = description;
      Module = module;
      Executor = executor;
      SubCommands = subCommands.ToImmutableDictionary(x => x.Name);
      SubGroups = subGroups.ToImmutableDictionary(x => x.Name);
    }
  }
}