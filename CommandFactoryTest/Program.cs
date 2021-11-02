using System;
using System.Reflection;
using System.Threading.Tasks;

namespace CommandFactoryTest
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var init = await CommandFactory.CommandManager.Init(Assembly.GetEntryAssembly());
      Console.WriteLine();
    }
  }
}