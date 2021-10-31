using Discord.WebSocket;

namespace CommandFactory
{
  internal interface IModule
  {
    void SetCommand(SocketSlashCommand command);

    // void BeforeExecute(CommandInfo command);
    //     
    // void AfterExecute(CommandInfo command);
    //
    // void OnModuleBuilding(CommandService commandService, ModuleBuilder builder);
  }
}