using System.Threading.Tasks;
using CommandFactory;
using CommandFactory.Attributes;
using Discord;
using Discord.WebSocket;

namespace CommandFactoryTest
{
  [Command("test")]
  public class CommandTest : SlashModule
  {
    [Execute]
    public async Task test(int yea)
    {
    }

    public override void Register(DiscordSocketRestClient client, SlashCommandProperties properties)
    {
      base.Register(client, properties);
    }

    [SubCommandGroup("ping")]
    public class Ping : SubSlashGroupModule
    {
      [SubCommand("YEA!")]
      public async Task CPing(int yea)
      {
      }
    }
  }
}