using System.Collections.Generic;
using System.Collections.Immutable;

namespace CommandFactory.Info
{
  internal readonly struct SubModuleInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly SubSlashGroupModule Module;
    public readonly ImmutableDictionary<string, CommandInfo> SubCommands;
    public readonly ImmutableDictionary<string, SubModuleInfo> SubGroups;

    public SubModuleInfo(string name, string description, SubSlashGroupModule module, List<CommandInfo> subCommands, List<SubModuleInfo> subGroups)
    {
      Name = name;
      Description = description;
      Module = module;
      SubCommands = subCommands.ToImmutableDictionary(x => x.Name);
      SubGroups = subGroups.ToImmutableDictionary(x => x.Name);
    }
  }
}