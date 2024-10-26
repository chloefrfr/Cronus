using Discord;
using Discord.WebSocket;
using Larry.Source.Database.Entities;
using Larry.Source.Interfaces;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;

namespace Larry.Source.Discord.Commands
{
    public class GrantAllCommand : ISlashCommand
    {
        public string Name => "grantall";
        public string Description => "Grants a user all cosmetics.";

        private static readonly EmbedBuilder accountNotFoundEmbed = new EmbedBuilder()
            .WithTitle("Account not found")
            .WithDescription("User does not have a registered account.")
            .WithColor(Color.Red);

        private static readonly EmbedBuilder alreadyFullLockerEmbed = new EmbedBuilder()
            .WithTitle("Already has full locker")
            .WithDescription("This user already has full locker.")
            .WithColor(Color.Red);

        public List<SlashCommandOptionBuilder> Options => new List<SlashCommandOptionBuilder>
        {
            new SlashCommandOptionBuilder()
                .WithName("user")
                .WithDescription("The user to grant all cosmetics to.")
                .WithType(ApplicationCommandOptionType.User)
                .WithRequired(true)
        };

        public async Task BuildAsync(SocketSlashCommand command)
        {
            await command.DeferAsync(true).ConfigureAwait(false);

            var discordUser = (SocketUser)command.Data.Options.First().Value;
            var config = Config.GetConfig();
            var userRepository = new Repository<User>(config.ConnectionUrl);

            var user = await userRepository.FindByDiscordIdAsync(discordUser.Id.ToString()).ConfigureAwait(false);
            if (user == null)
            {
                await command.RespondAsync(embed: accountNotFoundEmbed.WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp().Build()).ConfigureAwait(false);
                return;
            }

            try
            {
                var waitEmbed = new EmbedBuilder()
                   .WithTitle("Gathering cosmetics")
                   .WithDescription("Gathering cosmetics, please wait as this could take a while!")
                   .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                   .WithCurrentTimestamp()
                   .WithColor(Color.Gold)
                   .Build();
                await command.FollowupAsync(embed: waitEmbed).ConfigureAwait(false);
                await ProfileManager.GrantAll(user.AccountId).ConfigureAwait(false);

                var userEmbed = new EmbedBuilder()
                    .WithTitle("Granted full locker")
                    .WithDescription("You have been granted full locker.")
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(Color.Purple)
                    .Build();
                await discordUser.SendMessageAsync(embed: userEmbed).ConfigureAwait(false);

                user.HasAll = true;
                await userRepository.UpdateAsync(user).ConfigureAwait(false);

                var successEmbed = new EmbedBuilder()
                    .WithTitle("Success")
                    .WithDescription($"Successfully granted full locker to <@{discordUser.Id}>.")
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(Color.Purple)
                    .Build();
                await command.FollowupAsync(embed: successEmbed).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred while granting all: {ex}");
                var errorEmbed = new EmbedBuilder()
                    .WithTitle("An error occurred")
                    .WithDescription("An error occurred while granting all.")
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(Color.Red)
                    .Build();
                await command.FollowupAsync(embed: errorEmbed).ConfigureAwait(false);
            }
        }
    }
}
