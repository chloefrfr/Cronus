using Discord;
using Discord.WebSocket;
using Larry.Source.Utilities;

namespace Larry.Source.Discord
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;

        public Bot()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                GatewayIntents = GatewayIntents.Guilds,
            });

            _services = new ServiceCollection().AddSingleton(_client).BuildServiceProvider();
        }

        public async Task RunAsync(string token)
        {
            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.SlashCommandExecuted += HandleSlashCommandAsync;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            
            //await Task.Delay(-1);
        }

        private Task LogAsync(LogMessage log)
        {
            Logger.Information($"{log}");
            return Task.CompletedTask;
        }

        private async Task ReadyAsync()
        {
            Logger.Information("Bot is connected.");
            await RegisterSlashCommandsAsync();

            await _client.SetGameAsync("Larry", type: ActivityType.Playing);
            Logger.Information("Slash commands registered!");
        }

        private async Task RegisterSlashCommandsAsync()
        {
            Config config;
            try
            {
                config = Config.Load();
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to load config: {ex.Message}");
                return;
            }

            var guild = _client.GetGuild(ulong.Parse(config.GuildId));
            if (guild == null)
            {
                Logger.Error("Failed to get guild.");
                return;
            }

            var commands = new[]
            {
                new SlashCommandBuilder()
                .WithName("hello")
                .WithDescription("Says hello to the user.")
                .Build()
            };

            foreach (var command in commands)
            {
                await guild.CreateApplicationCommandAsync(command);
            }

            Logger.Information("Slash commands registered!");
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "hello":
                    await command.RespondAsync($"Hello {command.User.Mention}");
                    break;
            }
        }
    }
}
