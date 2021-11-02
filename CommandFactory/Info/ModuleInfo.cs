﻿using System.Collections.Generic;
using System.Reflection;

namespace CommandFactory.Info
{
  internal readonly struct ModuleInfo
  {
    public readonly string Name;
    public readonly string Description;
    public readonly SlashModule Module;
    public readonly CommandInfo Executor;
    public readonly List<CommandInfo> SubCommands;
    public readonly List<SubModuleInfo> SubGroups;

    public ModuleInfo(string name, string description, SlashModule module, CommandInfo executor, List<CommandInfo> subCommands, List<SubModuleInfo> subGroups)
    {
      Name = name;
      Description = description;
      Module = module;
      Executor = executor;
      SubCommands = subCommands;
      SubGroups = subGroups;
    }
  }
}