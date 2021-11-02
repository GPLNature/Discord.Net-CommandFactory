using System.ComponentModel;
using System.Threading.Tasks;
using CommandFactory;
using CommandFactory.Attributes;
using Discord;
using Discord.WebSocket;

namespace CommandFactoryTest
{
  [Command("test")]
  [Description("test")]
  public class CommandTest : SlashModule
  {
    [Execute]
    public async Task test([Description("test")] int yea)
    {
    }

    public override void Register(DiscordSocketRestClient client, SlashCommandProperties properties)
    {
      base.Register(client, properties);
    }

    [SubCommandGroup("ping")]
    [Description("test")]
    public class Ping : SubSlashGroupModule
    {
      [SubCommand("YEA")]
      [Description("test")]
      public async Task CPing([Description("test")] int yea)
      { 
      }
    }
  }
}