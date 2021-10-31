using System.Reflection;

namespace CommandFactoryTest
{
  class Program
  {
    static void Main(string[] args)
    {
      CommandFactory.CommandManager.Init(Assembly.GetEntryAssembly());
    }
  }
}