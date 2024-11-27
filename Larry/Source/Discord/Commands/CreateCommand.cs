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
            var embedBuilder = new EmbedBuilder()
                .WithAuthor(command.User.Username, command.User.GetAvatarUrl())
                .WithCurrentTimestamp();

            try
            {
                if (command == null)
                {
                    embedBuilder.WithTitle("Command is null")
                                .WithDescription("Command is null, please try again.")
                                .WithColor(Color.Red);
                    await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
                    return;
                }

                var options = command.Data.Options;
                var email = options.First().Value?.ToString();
                var password = options.Skip(1).FirstOrDefault()?.Value?.ToString();

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    embedBuilder.WithTitle("Missing Parameters")
                                .WithDescription("Both email and password are required.")
                                .WithColor(Color.Red);
                    await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
                    return;
                }

                Regex emailRegex = new Regex(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$");
                if (!emailRegex.IsMatch(email))
                {
                    embedBuilder.WithTitle("Invalid email")
                                .WithDescription("The email you provided is invalid.")
                                .WithColor(Color.Red);
                    await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
                    return;
                }

                var discordId = command.User.Id;
                var config = Config.GetConfig();
                var userRepository = new Repository<User>(config.ConnectionUrl);
                var loadoutRepository = new Repository<Loadouts>(config.ConnectionUrl);
                var user = await userRepository.FindByDiscordIdAsync(discordId.ToString());

                if (user != null)
                {
                    embedBuilder.WithTitle("Account already exists")
                                .WithDescription("An account already exists for this user.")
                                .WithColor(Color.Red);
                    await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
                    return;
                }

                var (hashedPassword, salt) = PasswordHasher.HashPassword(password);
                var accountId = Guid.NewGuid().ToString().Replace("-", "");
                var newUser = new User
                {
                    Email = email,
                    Username = command.User.Username,
                    Password = hashedPassword,
                    AccountId = accountId,
                    DiscordId = discordId.ToString(),
                    Roles = Array.Empty<string>(),
                    Banned = false,
                    HasAll = false
                };

                var newLoadout = new Loadouts
                {
                    AccountId = accountId,
                    ProfileId = "athena",
                    TemplateId = "CosmeticLocker:cosmeticlocker_athena",
                    LockerName = "Larry",
                    BannerId = "",
                    BannerColorId = "",
                    CharacterId = "AthenaCharacter:CID_001_Athena_Commando_F_Default",
                    BackpackId = "",
                    GliderId = "AthenaGlider:DefaultGlider",
                    DanceId = new string[6] { "", "", "", "", "", "" },
                    PickaxeId = "AthenaPickaxe:DefaultPickaxe",
                    ItemWrapId = new string[7] { "", "", "", "", "", "", "" },
                    ContrailId = "",
                    LoadingScreenId = "",
                    MusicPackId = ""
                };

                await userRepository.SaveAsync(newUser);
                await loadoutRepository.SaveAsync(newLoadout);

                await ProfileManager.CreateProfileAsync("athena", newUser.AccountId);
                await ProfileManager.CreateProfileAsync("common_core", newUser.AccountId);

                Logger.Information($"Successfully created user with username {newUser.Username} and accountId {newUser.AccountId}");

                embedBuilder.WithTitle("Account Created Successfully")
                            .WithDescription("Your account has been successfully created.")
                            .WithColor(Color.Green)
                            .AddField("Username", newUser.Username, true);

                await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred: {ex.Message}");
                embedBuilder.WithTitle("An error occurred")
                            .WithDescription("An error occurred while creating your account.")
                            .WithColor(Color.Red);

                await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
            }
        }
    }
}
