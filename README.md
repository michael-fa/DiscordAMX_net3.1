# DiscordAMX

A .NET Console App that lets you script your Discord Bots using the awesome [PAWN](https://github.com/pawn-lang) scripting language.
```
forward UpdateDiscordActivity();

main()
{
	DC_SetToken("aBzDxYzaBzDxYzaBzDxYz1aBzDxYz");
    DC_SetMinLogLevel(1);
}

public OnInit()
{
    DC_SetActivityText("DiscordAMX!");
    printc("OnInit called.");

    SetTimer("UpdateDiscordActivity", 5000);
    return 1;
}

public UpdateDiscordActivity()
{
	DC_SendChannelMessage("000000000000000000", "My awesome bot!");
}
```

---

## :construction: EARLY ALPHA

The whole project is still in its early days, meaning there will be huge changes and if you are not experienced in PAWN Scripting you should come back later! Leave a watch! 

The current RELEASE is meant for experienced scripters and is only for pure testing.
Run it on windows and make sure you have <b>.NET Framework 4.7.2</b> ! 


## Current Features
* Set the DiscordActivity ('playing' for now only) Text 
* Send Messages to any channel
* Setting timers


## Planned for first release
* DC_OnChannelMessage()
* DC_OnMemberJoin()
* DC_OnReactionAdded()
* DC_RegisterCommand()
* User Functions (finding users by name or ID, sending and receiving PM's)
* Better print function

### Third party info
* Using [PAWN Wrapper by ikkentim](https://github.com/ikkentim/AMXWrapper)
* Using [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)
* Using [Colorful Console](https://github.com/tomakita/Colorful.Console)
