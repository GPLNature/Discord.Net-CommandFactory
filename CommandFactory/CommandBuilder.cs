using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CommandFactory.Attributes;
using CommandFactory.Exception;
using CommandFactory.Info;
using ParameterInfo = CommandFactory.Info.ParameterInfo;

namespace CommandFactory
{
  internal static class CommandBuilder
  {
    private static readonly TypeInfo ModuleTypeInfo = typeof(SlashModule).GetTypeInfo();
    private static readonly TypeInfo SubModuleTypeInfo = typeof(SubSlashGroupModule).GetTypeInfo();

    public static async Task<List<ModuleInfo>> BuildAsync(Assembly assembly, int threadCount = 5, int completionPortThreads = 5)
    {
      ThreadPool.SetMaxThreads(threadCount, completionPortThreads);
      var modules = new List<ModuleInfo>();

      foreach (var typeInfo in assembly.DefinedTypes)
        if (typeInfo.IsPublic || typeInfo.IsNestedPublic)
          if (IsValidModuleDefinition(typeInfo))
            modules.Add(BuildModule(typeInfo));

      return modules;
    }

    private static ModuleInfo BuildModule(TypeInfo commandClass)
    {
      var subGroupTypes = commandClass.DeclaredNestedTypes
        .Where(IsValidSubGroups);
      var commandMethods =
        commandClass.DeclaredMethods.Where(info =>
          IsValidExecutorDefinition(info) || IsValidSubCommandDefinition(info)).ToImmutableList();
      var commandAttribute = commandClass.GetCustomAttribute<CommandAttribute>();
      var commandName = commandAttribute?.Name ?? throw new LoadException("Command Name can't be empty string or null");
      var description = commandClass.GetCustomAttribute<DescriptionAttribute>()?.Description ??
                        throw new LoadException("Description cannot be blank or null");
      if (description.Length == 0)
        throw new LoadException("Description cannot be blank or null");

      // Build SubCommandGroup
      var subGroups = subGroupTypes.Select(BuildSubModule).ToList();

      // Build Commands
      var executors = commandMethods.Where(IsValidExecutorDefinition).FirstOrDefault();
      var subCommands = commandMethods.Where(IsValidSubCommandDefinition).Select(BuildSubCommand).ToList();

      if (executors == null)
        throw new EmptyExecutorException($"Empty Executor Methods on {commandName}");

      var executor = BuildCommand(commandName, description, executors);

      var instance = Activator.CreateInstance(commandClass) as SlashModule ??
                     throw new LoadException($"Can't create instance of {commandClass.FullName}");

      return new ModuleInfo(commandName, description, instance, executor, subCommands, subGroups);
    }

    private static SubModuleInfo BuildSubModule(TypeInfo groupClass)
    {
      var subGroupTypes = groupClass.DeclaredNestedTypes
        .Where(IsValidSubGroups);

      var subCommandMethods = groupClass.DeclaredMethods.Where(IsValidSubCommandDefinition);
      var commandAttribute = groupClass.GetCustomAttribute<SubCommandGroupAttribute>();
      var commandName = commandAttribute?.Name ?? throw new LoadException("Command Name can't be empty string or null");
      var description = groupClass.GetCustomAttribute<DescriptionAttribute>()?.Description ??
                        throw new LoadException("Description cannot be blank or null");
      if (description.Length == 0)
        throw new LoadException("Description cannot be blank or null");

      // Build SubCommandGroup
      var subGroups = new List<SubModuleInfo>();

      foreach (var subGroup in subGroupTypes) subGroups.Add(BuildSubModule(subGroup));

      // Build Command
      var commands = subCommandMethods.Select(BuildSubCommand).ToList();

      var instance = Activator.CreateInstance(groupClass) as SubSlashGroupModule ??
                     throw new LoadException($"Can't create instance of {groupClass.FullName}");

      return new SubModuleInfo(commandName, description, instance, commands, subGroups);
    }

    private static CommandInfo BuildSubCommand(MethodInfo info)
    {
      return BuildCommand(info.Name, string.Empty, info);
    }

    private static CommandInfo BuildCommand(string name, string description, MethodInfo info)
    {
      var attributes = info.GetCustomAttributes();

      foreach (var attribute in attributes) // check the attribute of method
        switch (attribute)
        {
          case SubCommandAttribute subCommandAttr:
            name = subCommandAttr.Name;
            break;
          case DescriptionAttribute descriptionAttr:
            description = descriptionAttr.Description;
            break;
        }

      if (description.Length == 0)
        throw new LoadException($"Description cannot be blank or null on {info.Name}");

      var parameters = BuildParameter(info);

      return new CommandInfo(name, description, info, parameters);
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
        var options = parameter.GetCustomAttribute<OptionAttribute>();

        if (description.Length == 0)
          throw new LoadException("Description cannot be blank or null");

        parameters.Add(new ParameterInfo(parameter.Name ?? "Unknown", description, optionType.Value,
          parameter.DefaultValue, parameter.ParameterType, new Options(options)));
      }

      return parameters;
    }

    private static bool IsValidMethodDefinition(MethodInfo methodInfo, Type target)
    {
      return methodInfo.IsDefined(target) &&
             methodInfo.ReturnType == typeof(Task) &&
             !methodInfo.IsStatic &&
             !methodInfo.IsGenericMethod;
    }

    private static bool IsValidSubCommandDefinition(MethodInfo methodInfo)
    {
      return IsValidMethodDefinition(methodInfo, typeof(SubCommandAttribute));
    }

    private static bool IsValidExecutorDefinition(MethodInfo methodInfo)
    {
      return IsValidMethodDefinition(methodInfo, typeof(ExecuteAttribute));
    }

    private static bool IsValidModuleDefinition(TypeInfo typeInfo)
    {
      return ModuleTypeInfo.IsAssignableFrom(typeInfo) && IsValidCommand(typeInfo) &&
             !typeInfo.IsAbstract &&
             !typeInfo.ContainsGenericParameters;
    }

    private static bool IsValidCommand(TypeInfo typeInfo)
    {
      return typeInfo.DeclaredMethods.Any(info => IsValidExecutorDefinition(info) || IsValidExecutorDefinition(info)) &&
             typeInfo.GetCustomAttribute<CommandAttribute>() != null;
    }

    private static bool IsValidSubGroups(TypeInfo typeInfo)
    {
      return SubModuleTypeInfo.IsAssignableFrom(typeInfo) &&
             typeInfo.DeclaredMethods.Any(IsValidSubCommandDefinition) &&
             typeInfo.GetCustomAttribute<SubCommandGroupAttribute>() != null;
    }
  }
}