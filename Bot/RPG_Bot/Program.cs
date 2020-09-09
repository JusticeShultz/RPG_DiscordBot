using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using RPG_Bot.Commands;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace RPG_Bot
{
    class Program
    {
        public static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        public static string ApplicationName = "Google Sheets API .NET Connection";

        public static int UserCount = 170;

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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            string spreadsheetId = "1EFRAzE64e8bZlDs1WWZiG5e1hJgOWlQ3uIezJ_6ZZCQ";//"1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
            string range = "Runtime Spells!A3:T";

            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            // https://docs.google.com/spreadsheets/d/1zBzUJ8TE6jpQRF-RubmZ-p8PmVXqm_o-cjUb4kwnh1U/edit?usp=sharing
            // https://docs.google.com/spreadsheets/d/1EFRAzE64e8bZlDs1WWZiG5e1hJgOWlQ3uIezJ_6ZZCQ/edit?usp=sharing
            ValueRange response = request.Execute();

            IList<IList<object>> values = response.Values;

            int loaded = 0;

            if (values != null && values.Count > 0)
            {
                /*Console.WriteLine("Spell Name, Spell Rarity/How to Obtain, Type, Class Exclusive, Spell Damage Min, Spell Damage Max, Crit Chance, " +
                                    "Modifier ID (See Spell Index), Spell Heal Min (% max health), Spell Heal Max (% max health), Mana Cost, Stamina Cost	Cast Chance (%), " +
                                    "Spell Type, Element, Spell Turns, Spell Description, Spell Icon, Allowed in PvP, Allowed on World Bosses");*/

                foreach (var row in values)
                {
                    //try
                    //{
                        // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("Adding new spell to the database: {0}", row[0] as string);

                    loaded++;

                    float damage_min = 0;
                    float damage_max = 0;
                    float crit_chance = 0;
                    float modID = 0;
                    float spell_heal_min = 0;
                    float spell_heal_max = 0;
                    float mana_cost = 0;
                    float stamina_cost = 0;
                    float cast_chance = 0;
                    float spell_turns = 0;
                    bool allowed_in_pvp = false;
                    bool allowed_on_world_bosses = false;

                    try { damage_min = Convert.ToSingle((row[4] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set damage_min of " + row[0] as string); }
                    try { damage_max = Convert.ToSingle((row[5] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set damage_max of " + row[0] as string); }
                    try { crit_chance = Convert.ToSingle((row[6] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set crit_chance of " + row[0] as string); }
                    try { modID = Convert.ToSingle((row[7] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set modID of " + row[0] as string); }
                    try { spell_heal_min = Convert.ToSingle((row[8] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set spell_heal_min of " + row[0] as string); }
                    try { spell_heal_max = Convert.ToSingle((row[9] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set spell_heal_max of " + row[0] as string); }
                    try { mana_cost = Convert.ToSingle((row[10] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set mana_cost of " + row[0] as string); }
                    try { stamina_cost = Convert.ToSingle((row[11] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set stamina_cost of " + row[0] as string); }
                    try { cast_chance = Convert.ToSingle((row[12] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set cast_chance of " + row[0] as string); }
                    try { spell_turns = Convert.ToSingle((row[15] as string).Replace("%", "").Replace("x", "")); }
                    catch { Console.WriteLine("Failed to set spell_turns of " + row[0] as string); }
                    try { allowed_in_pvp = (row[17] as string) == "Yes" ? true : false;  }
                    catch { Console.WriteLine("Failed to set allowed_in_pvp of " + row[0] as string); }
                    try { allowed_on_world_bosses = (row[18] as string) == "Yes" ? true : false; }
                    catch { Console.WriteLine("Failed to set allowed_on_world_bosses of " + row[0] as string); }
                   
                    Resources.Spell spell = new Resources.Spell
                    (
                        row[0] as string,
                        row[1] as string,
                        row[2] as string,
                        row[3] as string,
                        damage_min,
                        damage_max,
                        crit_chance,
                        modID,
                        spell_heal_min,
                        spell_heal_max,
                        mana_cost,
                        stamina_cost,
                        cast_chance,
                        row[13] as string,
                        row[14] as string,
                        spell_turns,
                        row[16] as string,
                        row[17] as string,
                        allowed_in_pvp,
                        allowed_on_world_bosses
                    );

                    Gameplay.spellDatabase.Add(row[0] as string, spell);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Console.WriteLine("Failed to parse {0}, because {1}", row[0] as string, ex.ToString());
                    //}
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;

            Console.WriteLine("Loaded {0} spells into the spell database in {1}ms", loaded, ts.Milliseconds);

            //Console.Read();

            Console.WriteLine("Loading Dependencies...");

            //Open the file that contains the bot login token, this txt file is set up with the launcher.

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            System.IO.StreamReader file = new System.IO.StreamReader(@"Resources\BotKey.txt");

            //Read the opened file and set its contents to the token string.

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            TOKEN = file.ReadToEnd();

            if (File.Exists(@"Resources\MonsterCollection.AsteriaMonsters"))
            {
                using (StreamReader sr = File.OpenText(@"Resources\MonsterCollection.AsteriaMonsters"))
                {
                    string s = "";

                    while ((s = sr.ReadLine()) != null)
                    {
                        string[] temp = s.Split('Æ');

                        if (temp[0] == "Monster")
                        {
                            //0[Type], 1[ID], 2[x], 3[y] 4[z], 5[Name], [6]minLv, [7]maxLv, [8]difficulty, [9]tankiness, [10]flavor, [11]link
                            Gameplay.designedEnemiesIndex.Add(new Resources.DesignedEnemy(temp[5], temp[10], temp[11], temp[6], temp[7], temp[8], temp[9]));
                        }
                    }
                }
            }
            else
            {
                File.Create(@"Resources\MonsterCollection.AsteriaMonsters");
                Console.WriteLine("Failed to find a valid monster collection...");
            }
            
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

            //Main thread
            while (true)
            {

            }
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
