using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using System.Reflection;
using System.IO;

namespace RPG_Bot.Commands
{
    public class HelloWorld : ModuleBase<SocketCommandContext>
    {
        [Command("Hello"), Alias("hello", "henlo"), Summary("Hello world command")]
        public async Task Robin()
        {
            await Context.Channel.SendMessageAsync("Hello!");
        }

        [Command("embed"), Summary("Embed test command")]
        public async Task Embed([Remainder]string Input = "None")
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithAuthor("Test embed", Context.User.GetAvatarUrl());
            Embed.WithColor(40, 200, 150);
            Embed.WithFooter("The footer of the embed", Context.Guild.Owner.GetAvatarUrl());
            Embed.WithDescription("This is a **dummy** description, with a cool link.\n" +
                              "[This is my favourite website](https://www.google.com/)");

            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }
    }
}
