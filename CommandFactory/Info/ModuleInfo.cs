using System.Collections.Generic;

namespace CommandFactory.Info
{
  internal readonly struct ModuleInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly CommandInfo Executor;
    public readonly List<CommandInfo> SubCommands;
    public readonly List<ModuleInfo> SubGroups;

    public ModuleInfo(string name, string description, CommandInfo executor, List<CommandInfo> subCommands, List<ModuleInfo> subGroups)
    {
      Name = name;
      Description = description;
      Executor = executor;
      SubCommands = subCommands;
      SubGroups = subGroups;
    }
  }
}