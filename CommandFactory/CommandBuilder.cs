using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CommandFactory.Attributes;
using ParameterInfo = CommandFactory.Info.ParameterInfo;

namespace CommandFactory
{
  internal class CommandBuilder
  {
    private static readonly TypeInfo ModuleTypeInfo = typeof(SlashModule).GetTypeInfo();

    public static async Task BuildAsync(Assembly assembly)
    {
      var cache = new ModuleBuilder();
      
      foreach (var typeInfo in assembly.DefinedTypes)
      {
        if (typeInfo.IsPublic || typeInfo.IsNestedPublic)
        {
          if (IsValidModuleDefinition(typeInfo))
            if (IsValidCommand(typeInfo))
            {
              BuildModule(typeInfo);
            }
        }
      }
    }

    public static void BuildModule(TypeInfo commandClass)
    {
      var commandMethods = commandClass.DeclaredMethods.Where(info => IsValidExecutorDefinition(info) || IsValidSubCommandDefinition(info));
      var description = commandClass.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
      var commandName = commandClass.GetCustomAttribute<CommandAttribute>()!.Name;
      
      // Build Commands
      
    }

    public static void BuildCommand(MethodInfo info)
    {
      var attributes = info.GetCustomAttributes();

      foreach (var attribute in attributes) // check the attribute of method
      {
        switch (attribute)
        {
          case ExecuteAttribute executorAttr:
            break;
          case SubCommandAttribute subCommandAttr:
            break;
          case DescriptionAttribute descriptionAttr:
            break;
        }
      }

      var parameters = BuildParameter(info);
    }

    public static List<ParameterInfo> BuildParameter(MethodInfo info)
    {
      var parameters = new List<ParameterInfo>();

      var parameterInfos = info.GetParameters();
      foreach (var parameter in parameterInfos)
      {
        var optionType = ParameterChecker.MappingType(parameter);
        if (!optionType.HasValue)
          throw new ParameterMappingException($"Parameter can't match type on {info.Name} method parameter name : {parameter.Name}");
        var description = parameter.GetCustomAttribute<DescriptionAttribute>()?.Description ?? "";
        
        parameters.Add(new ParameterInfo(parameter.Name ?? "Unknown", description, optionType.Value));
      }
      
      return parameters;
    }
    
    private static bool IsValidMethodDefinition(MethodInfo methodInfo, Type target) => methodInfo.IsDefined(target) &&
      methodInfo.ReturnType == typeof(Task) &&
      !methodInfo.IsStatic &&
      methodInfo.GetParameters().Length == 0 &&
      !methodInfo.IsGenericMethod;
    
    private static bool IsValidSubCommandDefinition(MethodInfo methodInfo) => IsValidMethodDefinition(methodInfo, typeof(SubCommandAttribute));

    private static bool IsValidExecutorDefinition(MethodInfo methodInfo) =>
      IsValidMethodDefinition(methodInfo, typeof(ExecuteAttribute));

    private static bool IsValidModuleDefinition(TypeInfo typeInfo) =>
      ModuleTypeInfo.IsAssignableFrom(typeInfo) &&
      !typeInfo.IsAbstract &&
      !typeInfo.ContainsGenericParameters;

    private static bool IsValidCommand(TypeInfo typeInfo) =>
      typeInfo.DeclaredMethods.Any(info => IsValidExecutorDefinition(info) || IsValidSubCommandDefinition(info)) &&
      typeInfo.GetCustomAttribute<CommandAttribute>() != null;
    
  }
}