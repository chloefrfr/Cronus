using Discord.Commands;
using Discord.WebSocket;
using Larry.Source.Interfaces;

namespace Larry.Source.Discord.Commands
{
    public class CreateCommand : ISlashCommand
    {
        public string Name => "create";
        public string Description => "Creates your account.";

        public async Task BuildAsync(SocketSlashCommand command)
        {
            await command.RespondAsync("Test for handler");
        }
    }
}
