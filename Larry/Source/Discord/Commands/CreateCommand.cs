using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Larry.Source.Database.Entities;
using Larry.Source.Interfaces;
using Larry.Source.Repositories;
using Larry.Source.Utilities;
using Larry.Source.Utilities.Managers;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Larry.Source.Discord.Commands
{
    public class CreateCommand : ISlashCommand
    {
        public string Name => "create";
        public string Description => "Creates your account.";
        public List<SlashCommandOptionBuilder> Options => new List<SlashCommandOptionBuilder>
        {
            new SlashCommandOptionBuilder()
            .WithName("email")
            .WithDescription("The email for your account.")
            .WithType(ApplicationCommandOptionType.String)
            .WithRequired(true),

            new SlashCommandOptionBuilder()
            .WithName("password")
            .WithDescription("The password for your account.")
            .WithType(ApplicationCommandOptionType.String)
            .WithRequired(true)
        };

        public async Task BuildAsync(SocketSlashCommand command)
        {
            await command.DeferAsync();

            if (command == null)
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Command is null")
                    .WithDescription("Command is null, please try again.")
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(Color.Red)
                    .Build();

                await command.FollowupAsync(embed: embed, ephemeral: true);
                return;
            }

            var options = command.Data.Options;
            var email = options.First().Value;
            var password = options.Skip(1).FirstOrDefault()?.Value;

            if (password == null || email == null)
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Password or Email is null")
                    .WithDescription("Password or Email were received as 'null'.")
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(Color.Red)
                    .Build();

                await command.FollowupAsync(embed: embed, ephemeral: true);
                return;
            }

            Regex emailRegex = new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$");

            if (!emailRegex.IsMatch(email.ToString()))
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Invalid email")
                    .WithDescription("The email you provided is invalid.")
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(Color.Red)
                    .Build();

                await command.FollowupAsync(embed: embed, ephemeral: true);
                return;
            }

            var discordId = command.User.Id;

            var config = Config.GetConfig();
            var userRepository = new Repository<User>(config.ConnectionUrl);

            var user = await userRepository.FindByDiscordIdAsync(discordId.ToString());

            if (user != null)
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Account already exists")
                    .WithDescription("An account already exists for this user.")
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(Color.Red)
                    .Build();

                await command.FollowupAsync(embed: embed, ephemeral: true);
                return;
            }

            var (hashedPassword, salt) = PasswordHasher.HashPassword(password.ToString());
            Guid guid = Guid.NewGuid();
            var accountId = guid.ToString().Replace("-", "");

            var member = command.User as IGuildUser;
            string[] userRoles = Array.Empty<string>(); 
            if (member != null)
            {
                userRoles = member.RoleIds
                    .Select(roleId => member.Guild.GetRole(roleId)?.Name) 
                    .Where(name => name != null) 
                    .ToArray();
            }


            try
            {
                var newUser = new User
                {
                    Email = email.ToString(),
                    Username = command.User.Username,
                    Password = hashedPassword,
                    AccountId = accountId,
                    DiscordId = discordId.ToString(),
                    Roles = userRoles,
                    Banned = false,
                    HasAll = false
                };

                await userRepository.SaveAsync(newUser).ConfigureAwait(true);

                await ProfileManager.CreateProfileAsync("athena", newUser.AccountId);
                await ProfileManager.CreateProfileAsync("common_core", newUser.AccountId);
                await ProfileManager.CreateProfileAsync("common_public", newUser.AccountId);


                Logger.Information($"Successfully created user with the username {newUser.Username} and the id {newUser.Id}");

                var embed = new EmbedBuilder()
                    .WithTitle("Successfully created your account.")
                    .WithDescription("Your account has been successfully created.")
                    .WithColor(Color.Green)
                    .WithCurrentTimestamp()
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .AddField("Username", newUser.Username, true)
                    .Build();

                await command.FollowupAsync(embed: embed, ephemeral: true);
                return;
            } catch (Exception ex)
            {
                Logger.Error($"An error occured: {ex.Message}");
                var embed = new EmbedBuilder()
                    .WithTitle("An error occured")
                    .WithDescription("An error occured while creating your account.")
                    .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                    .WithCurrentTimestamp()
                    .WithColor(Color.Red)
                    .Build();

                await command.FollowupAsync(embed: embed, ephemeral: true);
                return;
            }
        }
    }
}
