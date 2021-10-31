using System.Threading.Tasks;
using CommandFactory;
using CommandFactory.Attributes;

namespace CommandFactoryTest
{
  [Command("test")]
  public class CommandTest : SlashModule
  {
    [Execute]
    public async Task test()
    {
    }

    [SubCommandGroup("ping!")]
    public class Ping
    {
    }
  }
}