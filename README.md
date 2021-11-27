# DiscordAMX  [![Generic badge](https://img.shields.io/github/v/release/michael-fa/DiscordAMX?include_prereleases)](https://github.com/michael-fa/DiscordAMX/releases)

## :de: [German](https://github.com/michael-fa/DiscordAMX/blob/master/german_readme.md) translation is also available.
## [Read the Wiki for help](https://github.com/michael-fa/DiscordAMX/wiki)


A .NET Console App that lets you script your Discord Bots using the awesome [PAWN](https://github.com/pawn-lang) scripting language.
The core goal is to make Discord Bots more easier for everyone!
For more about PAWN [click here](https://www.compuphase.com/pawn/pawn.htm).
```
#include <a_dcamx>

#define MY_BOT_TOKEN "YOUR_BOT_TOKEN"
#define MY_GUILD_ID "YOUR_GUILD_ID"
#define EXAMPLE_CHANNEL "CHANNEL_ID_AS_STRING"



main()
{
	DC_SetToken(MY_BOT_TOKEN);
        DC_SetMinLogLevel(1);
	print(" => Awesome Discord Bot starting!\n");
}

public OnInit()
{
	DC_SendChannelMessage(MY_GUILD_ID, EXAMPLE_CHANNEL, "I just scripted my own Discord Bot! Hurraay!");
	print(" => Awesome Discord Bot is started!\n");
	return 1;
}

public OnUnload()
{
    return 1;
}

public OnMemberJoin(guildid, memberid[])
{
    return 1;
}

public OnMemberLeave(guildid, memberid[])
{
    return 1;
}

public OnMessage(guildid, channelid[], memberid[], message[])
{
    return 1;
}

public OnMessageDeleted(guildid, messageid[])
{
    return 1;
}
```

---

## :construction: ~~EARLY~~ ALPHA

The whole project is still in its alpha, meaning there will be many changes and if you are not experienced in PAWN Scripting you should come back later! Leave a watch! 

## Testers
The current RELEASE is meant for experienced scripters and is only for pure testing.
Run it on windows and make sure you have <b>[.NET Runtime 5.0.*](https://dotnet.microsoft.com/download/dotnet/5.0)</b> ! 

**If you are an experienced PAWN coder, and you're interested in testing this (given you can thoroughly test the program on your discord)
I'll offer to host a bot for TESTERS -> I am currently looking for 2-3 official project testers!**

:warning: The current aim of this is self-hosting - remember: NOT SELF-BOT. You need (to rent) a (v)Server running either Linux or Windows on it!
Linux support is coming soon!


## :warning: We're slowly getting towards supporting multiple scripts! If there are problems regarding your scripts, other then your main.amx, report this under Issues! 


## Current Features
* Set the DiscordActivity ('playing, listening, watching') Text 
* Send Messages to any channel
* Setting timers
* Private DM Handling
* Receive onReaction, onMemberJoin, onMemberLeave, onMessage, on onCommand! **(Note until wiki is done: Commands in DiscordAMX are all handled with / as prefix!)**


## Planned for first release
* Built in MySQL Support
* Music streaming support
* User Functions (finding users by name or ID, sending and receiving PM's)
* Better print function
* File handling
* LINUX SUPPORT

### Third party info
* Using [PAWN Wrapper by ikkentim](https://github.com/ikkentim/AMXWrapper)
* Using [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)

[Visit CompuPhase Pawn Language](https://www.compuphase.com/pawn/pawn.htm) site for more info!
