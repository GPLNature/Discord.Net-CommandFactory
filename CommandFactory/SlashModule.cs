using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace CommandFactory
{
  public class SlashModule : IModule
  {
    protected SocketSlashCommand Command;
    void IModule.SetCommand(SocketSlashCommand command)
    {
      Command = command;
    }

    public virtual async Task Register(DiscordSocketClient client, SlashCommandProperties properties)
    {
      await client.CreateGlobalApplicationCommandAsync(properties);
    }
  }

  public class SubSlashGroupModule : IModule
  {
    protected SocketSlashCommand Command;
    void IModule.SetCommand(SocketSlashCommand command)
    {
      Command = command;
    }
  }
}