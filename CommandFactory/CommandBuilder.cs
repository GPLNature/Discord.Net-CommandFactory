﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandFactory.Attributes;
using CommandFactory.Exception;
using CommandFactory.Info;
using Discord.WebSocket;
using ParameterInfo = CommandFactory.Info.ParameterInfo;

namespace CommandFactory
{
  internal class CommandBuilder
  {
    private static readonly TypeInfo ModuleTypeInfo = typeof(SlashModule).GetTypeInfo();
    private static readonly TypeInfo SubModuleTypeInfo = typeof(SubSlashGroupModule).GetTypeInfo();

    public static async Task<List<ModuleInfo>> BuildAsync(Assembly assembly)
    {
      var modules = new List<ModuleInfo>();

      foreach (var typeInfo in assembly.DefinedTypes)
      {
        if (typeInfo.IsPublic || typeInfo.IsNestedPublic)
        {
          if (IsValidModuleDefinition(typeInfo))
            modules.Add(BuildModule(typeInfo));
        }
      }

      return modules;
    }

    private static ModuleInfo BuildModule(TypeInfo commandClass)
    {
      var subGroupTypes = commandClass.DeclaredNestedTypes
        .Where(IsValidSubGroups);
      var commandMethods =
        commandClass.DeclaredMethods.Where(info =>
          IsValidExecutorDefinition(info) || IsValidSubCommandDefinition(info));
      var commandAttribute = commandClass.GetCustomAttribute<CommandAttribute>();
      var commandName = commandAttribute?.Name ?? throw new LoadException("Command Name can't be empty string or null");
      var description = commandClass.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";

      // Build SubCommandGroup
      var subGroups = new List<SubModuleInfo>();

      foreach (var subGroup in subGroupTypes)
      {
        subGroups.Add(BuildSubModule(subGroup));
      }

      // Build Commands
      var commands = commandMethods.Select(BuildCommand).ToList();
      var executors = commands.Where(x => x.Type == CommandType.Executor).ToList();
      var subCommands = commands.Where(x => x.Type == CommandType.SubCommand).ToList();

      if (executors.Count > 1)
        throw new TooManyExecutorException($"Too Many Executor Methods on {commandName}");

      if (!executors.Any())
        throw new EmptyExecutorException($"Empty Executor Methods on {commandName}");

      var executor = executors.First();

      var instance = Activator.CreateInstance(commandClass) as SlashModule ?? throw new LoadException($"Can't create instance of {commandClass.FullName}");

      return new ModuleInfo(commandName, description, instance, executor, subCommands, subGroups);
    }

    private static SubModuleInfo BuildSubModule(TypeInfo groupClass)
    {
      var subGroupTypes = groupClass.DeclaredNestedTypes
        .Where(IsValidSubGroups);
      
      var subCommandMethods = groupClass.DeclaredMethods.Where(IsValidSubCommandDefinition);
      var commandAttribute = groupClass.GetCustomAttribute<SubCommandGroupAttribute>();
      var commandName = commandAttribute?.Name ?? throw new LoadException("Command Name can't be empty string or null");
      var description = groupClass.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
      
      // Build SubCommandGroup
      var subGroups = new List<SubModuleInfo>();

      foreach (var subGroup in subGroupTypes)
      {
        subGroups.Add(BuildSubModule(subGroup));
      }
      
      // Build Command
      var commands = subCommandMethods.Select(BuildCommand).Where(x => x.Type == CommandType.SubCommand).ToList();
      
      var instance = Activator.CreateInstance(groupClass) as SubSlashGroupModule ?? throw new LoadException($"Can't create instance of {groupClass.FullName}");
      
      return new SubModuleInfo(commandName, description, instance, commands, subGroups);
    }

    private static CommandInfo BuildCommand(MethodInfo info)
    {
      var attributes = info.GetCustomAttributes();

      var name = string.Empty;
      var description = string.Empty;
      var type = CommandType.Executor;

      foreach (var attribute in attributes) // check the attribute of method
      {
        switch (attribute)
        {
          case SubCommandAttribute subCommandAttr:
            type = CommandType.SubCommand;
            name = subCommandAttr.Name;
            break;
          case DescriptionAttribute descriptionAttr:
            description = descriptionAttr.Description;
            break;
        }
      }

      var parameters = BuildParameter(info);

      return new CommandInfo(name, description, info, parameters, type);
    }

    private static List<ParameterInfo> BuildParameter(MethodInfo info)
    {
      var parameters = new List<ParameterInfo>();

      var parameterInfos = info.GetParameters();
      foreach (var parameter in parameterInfos)
      {
        var optionType = ParameterChecker.MappingType(parameter);
        if (!optionType.HasValue)
          throw new ParameterMappingException(
            $"Parameter can't match type on {info.Name} method parameter name : {parameter.Name}");
        var description = parameter.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";

        parameters.Add(new ParameterInfo(parameter.Name ?? "Unknown", description, optionType.Value));
      }

      return parameters;
    }

    private static bool IsValidMethodDefinition(MethodInfo methodInfo, Type target) => methodInfo.IsDefined(target) &&
      methodInfo.ReturnType == typeof(Task) &&
      !methodInfo.IsStatic &&
      !methodInfo.IsGenericMethod;

    private static bool IsValidSubCommandDefinition(MethodInfo methodInfo) =>
      IsValidMethodDefinition(methodInfo, typeof(SubCommandAttribute));

    private static bool IsValidExecutorDefinition(MethodInfo methodInfo) =>
      IsValidMethodDefinition(methodInfo, typeof(ExecuteAttribute));

    private static bool IsValidModuleDefinition(TypeInfo typeInfo) =>
      ModuleTypeInfo.IsAssignableFrom(typeInfo) && IsValidCommand(typeInfo) &&
      !typeInfo.IsAbstract &&
      !typeInfo.ContainsGenericParameters;

    private static bool IsValidCommand(TypeInfo typeInfo) =>
      typeInfo.DeclaredMethods.Any(info => IsValidExecutorDefinition(info) || IsValidExecutorDefinition(info)) &&
      typeInfo.GetCustomAttribute<CommandAttribute>() != null;

    private static bool IsValidSubGroups(TypeInfo typeInfo) => 
      SubModuleTypeInfo.IsAssignableFrom(typeInfo) &&
      typeInfo.DeclaredMethods.Any(IsValidSubCommandDefinition) &&
      typeInfo.GetCustomAttribute<SubCommandGroupAttribute>() != null;
  }
}