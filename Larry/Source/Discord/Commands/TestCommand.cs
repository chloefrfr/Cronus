using Discord;
using Discord.WebSocket;
using Larry.Source.Database.Entities;
using Larry.Source.Interfaces;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;

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

            var idk = await ProfileManager.GetProfileAsync(user.AccountId);
            foreach (var item in idk.Items)
            {
                Logger.Information($"Key: {item.Key}, Value: {item.Value}");
            }
        }

    }
}
