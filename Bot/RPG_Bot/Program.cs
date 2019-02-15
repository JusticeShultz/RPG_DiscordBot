using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RPG_Bot.Commands;

namespace RPG_Bot
{
    class Program
    {
        private static string TOKEN = "";
        public static DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider services;

        static void Main(string[] args)
        {
            System.IO.StreamReader file = new System.IO.StreamReader(@"Resources\BotKey.txt");
            TOKEN = file.ReadToEnd();

            Main2(args);
        }

        static void Main2(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            Client.MessageReceived += Client_MessageReceived;
            await Commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            await Client.LoginAsync(TokenType.Bot, TOKEN);
            await Client.StartAsync();

            await Task.Delay(-1);

            services = new ServiceCollection().BuildServiceProvider();
        }

        private async Task Client_Log(LogMessage Message)
        {
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("RPG Bot - Game Master");
            Console.WriteLine($"{DateTime.Now}] Boss spawning loop has started.");
            Gameplay.BossChance();
        }

        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            var Message = MessageParam as SocketUserMessage;
            var Context = new SocketCommandContext(Client, Message);

            if (Context.Message == null || Context.Message.Content == "") return;
            if (Context.User.IsBot) return;

            int ArgPos = 0;
            if (!(Message.HasStringPrefix("-", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;

            var Result = await Commands.ExecuteAsync(Context, ArgPos, services);
            if (!Result.IsSuccess)
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong with executing a command. Text: {Context.Message.Content} | Error: {Result.ErrorReason}");
        }
    }
}
