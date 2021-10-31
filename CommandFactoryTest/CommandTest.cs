using System.Threading.Tasks;
using CommandFactory;
using CommandFactory.Attributes;

namespace CommandFactoryTest
{
  [Command("test")]
  public class CommandTest : SlashModule
  {
    [Execute]
    public async Task test(int yea)
    {
    }

    [SubCommandGroup("ping!")]
    public class Ping : SlashModule
    {
      [Execute]
      public async Task ping(int yea)
      {
      }
    }
  }
}