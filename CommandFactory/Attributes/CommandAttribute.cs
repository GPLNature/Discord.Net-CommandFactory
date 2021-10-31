using System;

namespace CommandFactory.Attributes
{
  
  [AttributeUsage(AttributeTargets.Class)]
  public class CommandAttribute : Attribute
  {
    internal string Name;

    public CommandAttribute(string name)
    {
      Name = name;
    }
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class ExecuteAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Class)]
  public class SubCommandGroupAttribute : Attribute
  {
    internal string Name;

    public SubCommandGroupAttribute(string name)
    {
      Name = name;
    }
  }

  [AttributeUsage(AttributeTargets.Method)]
  public class SubCommandAttribute : Attribute
  {
    internal string Name;

    public SubCommandAttribute(string name)
    {
      Name = name;
    }
  }
}