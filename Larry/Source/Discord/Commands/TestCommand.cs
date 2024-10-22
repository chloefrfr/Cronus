using Discord;
using Discord.WebSocket;
using Larry.Source.Database.Entities;
using Larry.Source.Interfaces;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;
using System.Text.Json;

namespace Larry.Source.Discord.Commands
{
    public class TestCommand : ISlashCommand
    {
        public string Name => "test";
        public string Description => "Random Tests";
        public List<SlashCommandOptionBuilder> Options => new List<SlashCommandOptionBuilder>();

        public async Task BuildAsync(SocketSlashCommand command)
        {

            var discordId = command.User.Id;

            var config = Config.GetConfig();
            var userRepository = new Repository<User>(config.ConnectionUrl);

            var user = await userRepository.FindByDiscordIdAsync(discordId.ToString());


            await ProfileManager.GrantAll(user.AccountId);
           // Console.WriteLine(JsonSerializer.Serialize(idk.Stats.Attributes));
        }

    }
}
