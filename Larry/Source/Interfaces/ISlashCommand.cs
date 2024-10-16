using Discord;
using Discord.WebSocket;

namespace Larry.Source.Interfaces
{
    public interface ISlashCommand
    {
        string Name { get; }
        string Description { get; }
        List<SlashCommandOptionBuilder> Options { get; }

        Task BuildAsync(SocketSlashCommand command);
    }
}
