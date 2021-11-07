# DiscordAMX  [![Generic badge](https://img.shields.io/github/v/release/michael-fa/DiscordAMX?include_prereleases)](https://github.com/michael-fa/DiscordAMX/releases)

## :de: [German](https://github.com/michael-fa/DiscordAMX/blob/master/german_readme.md) translation is also available.
## [Read the Wiki for help](https://github.com/michael-fa/DiscordAMX/wiki)


A .NET Console App that lets you script your Discord Bots using the awesome [PAWN](https://github.com/pawn-lang) scripting language.
The core goal is to make Discord Bots for everyone!
```
#include <a_dcamx>

#define MY_BOT_TOKEN "YOUR_BOT_TOKEN"
#define MY_GUILD_ID "YOUR_GUILD_ID"
#define EXAMPLE_CHANNEL "CHANNEL_ID_AS_STRING"



main()
{
	DC_SetToken(MY_BOT_TOKEN);
        DC_SetGuild(MY_GUILD_ID);
        DC_SetMinLogLevel(1);
	print(" => Awesome Discord Bot starting!\n");
}

public OnInit()
{
	DC_SendChannelMessage(EXAMPLE_CHANNEL, "I just scripted my own Discord Bot! Hurraay!");
	print(" => Awesome Discord Bot is started!\n");
	return 1;
}

public OnUnload()
{
    return 1;
}

public OnHeartbeat()
{
    return 1;
}
```

---

## :construction: EARLY ALPHA

The whole project is still in its early days, meaning there will be huge changes and if you are not experienced in PAWN Scripting you should come back later! Leave a watch! 

The current RELEASE is meant for experienced scripters and is only for pure testing.
Run it on windows and make sure you have <b>[.NET Runtime 5.0.*](https://dotnet.microsoft.com/download/dotnet/5.0)</b> ! 

## :warning: (ALPHA NOTE: ONLY ONE SCRIPT SUPPORTED FOR NOW! Your own Script must be put in /Scripts/ folder AND renamed to 'main.amx')


## Current Features
* Set the DiscordActivity ('playing' for now only) Text 
* Send Messages to any channel
* Setting timers
* Receive onReaction, onMemberJoin, onMemberLeave, onMessage, on onCommand! **(Note until wiki is done: Commands in DiscordAMX are all handled with / as prefix!)**


## Planned for first release
* Built in MySQL Support
* Music streaming support
* User Functions (finding users by name or ID, sending and receiving PM's)
* Better print function

### Third party info
* Using [PAWN Wrapper by ikkentim](https://github.com/ikkentim/AMXWrapper)
* Using [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)
* Using [Colorful Console](https://github.com/tomakita/Colorful.Console)
