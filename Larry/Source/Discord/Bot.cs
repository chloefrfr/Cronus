using Discord;
using Discord.WebSocket;
using Larry.Source.Interfaces;
using Larry.Source.Utilities;
using System.Reflection;

namespace Larry.Source.Discord
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private readonly Dictionary<string, ISlashCommand> _commands = new();

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

            LoadSlashCommands();
            await RegisterSlashCommandsAsync();

            await _client.SetGameAsync("Larry", type: ActivityType.Playing);
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

            foreach (var command in _commands.Values)
            {
                var slashCommandBuilder = new SlashCommandBuilder()
                    .WithName(command.Name)
                    .WithDescription(command.Description);

                if (command.Options != null)
                {
                    foreach (var option in command.Options)
                    {
                        slashCommandBuilder.AddOption(option);
                    }
                }

                await guild.CreateApplicationCommandAsync(slashCommandBuilder.Build());
            }

            Logger.Information("All slash commands registered!");
        }

        private void LoadSlashCommands()
        {
            var commandTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(ISlashCommand).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in commandTypes)
            {
                var command = (ISlashCommand)Activator.CreateInstance(type);
                _commands[command.Name] = command;
                Logger.Information($"Loaded slash command: {command.Name}");
            }
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            if (_commands.TryGetValue(command.Data.Name, out var slashCommand))
            {
                await slashCommand.BuildAsync(command);
            }
            else
            {
                await command.RespondAsync("Unknown command.");
            }
        }
    }
}
