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

    public virtual void Register(DiscordSocketRestClient client, SlashCommandProperties properties)
    {
      client.CreateGlobalCommand(properties);
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