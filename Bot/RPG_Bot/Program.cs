using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RPG_Bot.Commands;

namespace RPG_Bot
{
    class Program
    {
        public static int UserCount = 462;
        //The bots token.
        private static string TOKEN = "";
        //The bots client.
        public static DiscordSocketClient Client; 
        //The bots commands.
        private CommandService Commands; 
        //The bots services.
        private IServiceProvider services; 

        static void Main(string[] args)
        {
            //for (int i = 0; i < 1000; ++i)
            //    Console.WriteLine(i + ": " + i * i);

            //Open the file that contains the bot login token, this txt file is set up with the launcher.
            System.IO.StreamReader file = new System.IO.StreamReader(@"Resources\BotKey.txt");
            //Read the opened file and set its contents to the token string.
            TOKEN = file.ReadToEnd();
            //Begin the async task for the bot.
            Main2(args);
        }

        //This will start the bots login process, this basically kicks off our whole bot. This will call MainAsync as an asynchronous task.
        static void Main2(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            //Note to self, keep in mind logging is a slow down.
            //https://discord.foxbot.me/docs/guides/concepts/logging.html

            //This will start the client in Debug mode.
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                //Set our clients log level to debug mode.
                LogLevel = LogSeverity.Debug
            });

            //This will start a new command service in debug mode.
            Commands = new CommandService(new CommandServiceConfig
            {
                //Turn case sensitive commands off.
                CaseSensitiveCommands = false,
                //Set our run mode to asynchronous.
                DefaultRunMode = RunMode.Async,
                //Set the log level to debug mode.
                LogLevel = LogSeverity.Debug
            });

            //Subscribe the clients message recieved event to the Client_MessageReceived task.
            Client.MessageReceived += Client_MessageReceived;
            //Add command modules from an System.Reflection.Assembly.
            await Commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: null);
            //Subscribe the clients ready event to the Client_Ready task.
            Client.Ready += Client_Ready;
            //Subscribe the clients log event to the Client_Log task.
            Client.Log += Client_Log;
            //Client.JoinedGuild += Joined_Guild;

            //Log our client in as a bot with our token.
            /*
             * Token contains who we connect to, a password and a timestamp of when the token was generated.
             * We tell the client to log in as a bot and then pass it a token.
             * We then use a AsyncStateMachine to log in, this task is awaited so we do not move on until we connect or fail.
             */
            await Client.LoginAsync(TokenType.Bot, TOKEN);
            //This part physically starts the client, this will start our bot and make it go online.
            await Client.StartAsync();
            //This is used to make sure we have started in time before grabbing our services.
            await Task.Delay(-1);
            //Set up our services for our now logged on bot.
            services = new ServiceCollection().BuildServiceProvider();
        }

        //This will handle the clients logging!
        private async Task Client_Log(LogMessage Message)
        {
            //Write to our command console with a timestamp, the message source and the messages message. 
            //This helps a lot for debugging and moderation.
            Console.WriteLine($"{DateTime.Now} at {Message.Source}] {Message.Message}");
        }

        //This will handle the client being ready!
        private async Task Client_Ready()
        {
            foreach (SocketGuild guilds in RPG_Bot.Program.Client.Guilds)
            {
                foreach (SocketUser users in guilds.Users)
                {
                    ++Program.UserCount;
                }
            }

            //This creates a little activity message in discord that says we are "Playing RPG Bot - Game Master".
           await Client.SetGameAsync("RPG Bot - " + Program.UserCount + " connected users.", "https://discord.gg/jnFfqhm", ActivityType.Streaming);
            //Write to the console we have called the boss spawning loop, this is used to make sure everything is not broken.
            Console.WriteLine($"{DateTime.Now}] Boss spawning loop has started.");
            //Call the boss chance task, this is a one time call that will start a looping task that continues until the program is ended.
            Gameplay.BossChance();
            await Gameplay.ServerConnector();
            //[Deprecated] - Better solution found. See DailyQuestHandler for further detail.
            //DailyQuestHandler.ResetCheck();
        }

        //This will handle things like commands and general text being sent!
        private async Task Client_MessageReceived(SocketMessage MessageParam)
        {
            //Grab the message that is recieved.
            var Message = MessageParam as SocketUserMessage;
            //This will grab the context of the client message recieved.
            var Context = new SocketCommandContext(Client, Message);
            //This will make sure the message isn't null, but in reality I don't think discord could ever send a null message.
            if (Context.Message == null || Context.Message.Content == "") return;
            //If the user is a bot then ignore them.
            if (Context.User.IsBot) return;

            //Check for the command prefix in our sent message, or alternatively check if we pinged the bot instead(e.g. @GameMaster Spawn - will spawn an enemy like -Spawn).
            //If I decided to add additional prefixes for commands I would do it here:
            int ArgPos = 0; if (!(Message.HasStringPrefix("-", ref ArgPos) || Message.HasMentionPrefix(Client.CurrentUser, ref ArgPos))) return;

            //Execute the command and send the command some data.
            var Result = await Commands.ExecuteAsync(Context, ArgPos, services);
            //If the command failed for any reason we will display a message with a timestamp, the command passed and the reason. (E.g. Someone does -Fuuuu - it will pass a message in the console)
            if (!Result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Commands] Something went wrong with executing a command. Text: {Context.Message.Content} | Error: {Result.ErrorReason}");

                await Task.Delay(2000);
                await Context.Channel.DeleteMessageAsync(Context.Message.Id);
            }
        }
    }
}
