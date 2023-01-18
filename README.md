# ♟️ DiscordAMX ♟️ [![Generic badge](https://img.shields.io/github/v/release/michael-fa/DiscordAMX?include_prereleases)](https://github.com/michael-fa/DiscordAMX/releases)

## [Read the Wiki for help](https://github.com/michael-fa/DiscordAMX/wiki)

# THIS PROJECT HAS REACHED IT'S END. 
### But DiscordAMX and TelePawn will soon be reworked with NET 7.0 and higher as it's platform to support linux hosting.



A .NET Console App that lets you script your Discord Bots using the awesome [PAWN](https://github.com/pawn-lang) scripting language.
The core goal is to make Discord Bots more easier for everyone!
For more about PAWN [click here](https://www.compuphase.com/pawn/pawn.htm).
This project depends heavily on [DSharpPlus](https://dsharpplus.github.io). A .NET Framework package allowing me to interact with Discord's API as easy as possible.

![alt text](https://i.imgur.com/LuMFx5K.png)

---

## :construction: BETA 1.0 is here!

This month, this github repo now is one year old, and has changed a lot. I learned things as I tested stuff out for the first time between the D#+ and AMX sides. I had to go though a lot of points where things weren't how I imaged them, and had to make them act how I wanted. Though I kept it as direct as possible sometimes I later changed things up again because they were doing waaaay too many .."hacky" stuff.

## The BETA 1.0 brings:
 - Many natives related to the guild members
 - Direct Message support for many of the natives, and even extra natives for users/DMs.
 - When scripting, guild members and guildids are handled via normal integer id's. The program itself links the ID to each pre-fetched and also always updated internal guild and member list. Knowing [San Andreas Multiplayer Scripting](https://www.sa-mp.com/) you may already know how to work with this. For any other newcomer there will be following some tutorials soon!
 - internal natives provided to handle inifiles.
 - Support for threads!
 - Sending embedded images!
 - Running multiple scripts, called filterscripts
 - Plugin Support (developers:) adding the ability for other people who know C# to write some plugins adding natives and extending the internal discordamx program.
 - Native timer support (running and stopping timers, that execute a callback. For now, no custom parameter support since I i simply forgot to work that out before releasing beta 1.0...
 - **Many more functions (scripting natives) to control your bot!**
 .. Please refer yourself to the [Wiki](https://github.com/michael-fa/DiscordAMX/wiki) for more!
 
 ## Planned for next releases
  - Forum support
  - More user functions, more channel functions like
    - UpdateChannel
    - UpdateMessage
    - More embedded functions
    
 
 ##  📢  Testers 👋 ♟️
 Looking for some Pawn Scripters who know how they can help testing me with this. I will provide an win32 server for temporary use.
 

### Third party info
* Using [PAWN Wrapper by ikkentim](https://github.com/ikkentim/AMXWrapper) **(He helped me a lot in finding out stuff about float support!)**
* Using [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)

[Visit CompuPhase Pawn Language](https://www.compuphase.com/pawn/pawn.htm) site for more info!
