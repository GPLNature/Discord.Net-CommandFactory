using System.Reflection;

namespace CommandFactory
{
  public class CommandManager
  {
    public static async void Init(Assembly entryAssembly)
    {
      await CommandBuilder.BuildAsync(entryAssembly);
    }
  }
}