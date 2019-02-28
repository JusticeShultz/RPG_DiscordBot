# RPG Discord Bot

Wanna see what this is all about? 
The bot uses Discord ID's to function so it will only work in the home server, which can be joined off of this link:
https://discord.gg/jnFfqhm

Feel free to join the Discord to try out the bot and ask questions!

# RPG DISCORD BOT DOCUMENTATION

This bot is now publicly invitable. Read the user documentation here and find a link at the top to invite the bot to your servers!
https://justiceshultz.github.io/pages/DiscordBotDocs.html

# Unstable Builds
Looking for unstable builds of the bot? As you may have noticed I only push fully working states to GitHub. This is due to how my publish system for the bot is and for security reasons with my bots API Token. If you would like a build of the most recent build of the bot please DM me on Discord @ Robin#1000 and I'll get back to you.

I will continue to push changes up to here but minor bug fixes and small additions will not be committed until all large broken features are updated.



# Patch 1.3.2.2412 is live now! 
#### Note: These logs are directly from the home dev server(day by day logs) so sections may be wonky/duplicated
##### Leaderboard
             -Type -lb or -leaderboard see the leaderboard by who's richest
##### Discord did an oopsie
             -Discord had an outage causing player data in the database to disappear - this issue was resolved
##### Bug Fixes
             -Leveling up no longer broken 
             -Fixed time display with questing, I'm aware there are time related bugs but I'm not yet sure why
##### Quality of Life
             -Added #announcements for important stuff
             -Admins can now do -notice "message" and the bot will embed it as an announcement
             -Leveling up is now easier          
##### Rank Up
            -Use :GuildGem:'s to buy new ranks all the way to @Rank - Master I!
##### Guild Gems
            -Most quests now pay out :GuildGem:'s
            -Guild gems are now displayed in user profiles
##### Zones
            -All zones now have monsters
            -#the-aurora completed
##### Scaling
            -Leveling up now gives you more scaled stat increases
            -Multi leveling now increases you the amount it should
            -Leveling up now requires way more XP
            -You may now level more than once at a time
            -Archer got 20 extra damage, no more weak shots
            -Fixed some early game zone healths and damages to be a little more fair
##### Small Changes
            -Added 2 new emojis :GuildGem: :MysticLog: 
            -Using -profile now displays the :MysticLog: emoji beside your current event items collected text
            -Easy link commands added, can we get an -f in the chat?
##### Bug Fixes
            -Fixed other ranks besides bronze not showing chats
##### Inactivity
            -The bot is currently in a working state and all that is left is polish and adding new stuff
##### Balancing
            -Late game monsters debuffed
            -World Bosses debuffed
            -Enemies now give more XP
##### Launcher w/ open source
            -Check #announcements for additional details          
##### General Changes
            -Late game monsters are much easier now
            -Edit to this, rebuffed late game monsters x2, early game could kill late game easy after scale changes
            -Leveling up gives increased stats
            -Goblins now spawn more often in #lv1-5 
            -The -shop command now displays your :GuildGem: 
            -World bosses nerfed again, now 100x weaker
##### New monsters
            -Added a ton of new monsters
##### Switching classes
            -Switch your class with -switch [Class] for 500 gold!        
##### Daily login rewards
            -Type -Daily for your daily quest!
            -Quests now balance off of current gold
            -Quests no longer reset with the bot
            -Daily quest is now named Daily Blessing
            -20 unique gods added to award Blessings
##### World Bosses
            -World bosses are now much weaker for the 4th nerf     
##### Leaderboard Update
            -Leaderboards now have sort methods
                      Sort by different kinds of data!
            -Leaderboards have been fancied up a bit
##### Event Changes
            -Event extended to March 20th
            -Event boss should now correctly spawn, my bad!
##### Balancing
            -Late game monsters are once again stronger
            -World bosses are now half as tanky
##### Quality of life
            -World boss fights now display a thumbnail beside the fight to indicate who is who
##### New enemy
            -Log spider added to zone #lv1-5 
##### New Items
            -Added new items to the shop
            -Added a chest opening system to the shop
##### Lag reduction
            -Fighting now deletes the -f
            -Fighting a monster will now edit the message instead of creating a new one
            -Spawning a monster will not unreference the object pools last spawned enemies message
##### Message cleanup
            -Doing -s now deletes the message like -f's
            -Death messages now delete after 5 seconds
            -Doing -s in a non monster channel now has a timed cleanup
            -Doing -shop or -sh now auto cleans up it's messages
            -Removed the spawn and prune the channel feature - this generated too much lag with Discords limiter
##### General Polish
            -Shop messages now have the users profile picture at the bottom to clarify who's who when displaying currency counts
            -Invalid commands will now auto delete, no more -f-f in the chat
##### Idk
            -Added a pepe lootbox to the shop
            -Pepe boxes are now colored depending on rarity
##### New Event
            -Bronze, Silver and Gold pots now spawn in non master ranked channels
            -Event monsters are now correctly named
##### Auto Attacking
            -Doing -f [x] will make you fight the spawned creature x times until you either die or kill it or have repeated the fight that many times         
##### Multi-Server Support
            -Servers now have their own individual monsters
            -You may now begin in any server and all data will port to every server
            -All data and roles now carry over to every set up server
            -Doing -startserver in your server now generates channels and roles which are handled by the bot
            -Cross Platformed servers now work with -begin
             -Enemies will now spawn in channels named correctly in every server
            -World bosses now work in every server. Each server has the same boss spawns but only the first 100 to fight in that server will earn a reward. Participating in multiple bosses will give multiple rewards. World bosses will individually die, health and damage is not shared.
##### The bot is back
            -I was stumped by what was wrong with my server connection method, turns out it was just a simple if statement and data population issue, we are back and no longer dead
##### World boss issues
            -I have disabled world bosses for now as I work out a new algorithm for spawning them on multiservers, more info soon to come when I get to it
[Solved Task] World bosses haven't been worked out yet, but everything else is working flawlessly! I will have world bosses back sometime soon, sorry for any inconveniences caused by this!
##### Bug Fixes
            -Doing -begin with a lowercase class now capitalizes it for you when showing the begin embed message
            -Fixed several bugs affecting things related to cross server capability
##### World Bosses
            -The super complex setup of world bosses has finally been setup and they're back(so is the event boss!)
            -When a world boss shows up it will show up in every other server.
            -Each boss has its own health. Health scales off of how many users are in the server. This means killing one here will be super hard without everyone, but a smaller server could have an easier chance with more active users!
##### Possible bugs
I've added a ton of new server porting and even more with the new world boss system. This means stuff WILL break. Please ping me if an enemy just suddenly disappears or weird bugs like that happen. If you set up a server and there are issues please PM me and send an invite to your server(preferably giving me the permissions to see roles, edit channel permissions, etc so I can debug your issue easier) so I can go and help you get stuff working since we are in early development still.
##### 24/7 Bot Schedule
I will be trying a near 24/7 schedules starting today with a few exceptions. My plans for this custom server hosting may just fail so no promises, but we will see how it goes. I will be pressing public release over the course of the next couple days/weeks, depending on how confident I feel. If you wan't to invite the bot to your server please do, it would help me a ton as I polish up the setup system. Check the docs out here and find the invite link at the top: https://justiceshultz.github.io/pages/DiscordBotDocs.html

##### Discords Rate Limiter
            -Doing -f [x] will only edit the fight message for the last 2 fight simulations now. This is both good and bad. It takes away from the visual but now fighting a ton is super fast. Let me know if there are issues!

I will for now on be editing and using these docs for commands, help, etc, I will be setting up a bug report system soon too so people don't gotta be here to send me bugs

##### Message cleanup
Using -help now sends the message to the users PM. The help message now links to the documentation of the bot. The command also sends the user a notification in the used channel and then auto deletes the message and command after 5 1/2 seconds.

##### World Bosses Fixed
            -Fixed the leaderboard show after defeating a world boss.
            -Damage done to the boss now correctly shown
            -Being top 3 on the world boss leaderboard now gives a gold & xp multiplier to each spot depending on how they placed
            -Only online and registered users make bosses scale now!

##### Double XP Wednesday
I've removed the double XP buff from Wednesday, it will be back next week or over the weekend, depending on whatever features I'm working on Friday
