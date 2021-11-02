using System.Collections.Generic;

namespace CommandFactory.Info
{
  internal readonly struct SubModuleInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly SubSlashGroupModule Module;
    public readonly List<CommandInfo> SubCommands;
    public readonly List<SubModuleInfo> SubGroups;

    public SubModuleInfo(string name, string description, SubSlashGroupModule module, List<CommandInfo> subCommands, List<SubModuleInfo> subGroups)
    {
      Name = name;
      Description = description;
      Module = module;
      SubCommands = subCommands;
      SubGroups = subGroups;
    }
  }
}