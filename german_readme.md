# DiscordAMX

Ein .NET Discord Bot scriptbar mit [PAWN](https://github.com/pawn-lang) Lang! Das Projekt befindet sich noch in den Kinderschuhen!
```
//Beispiel code..
forward Spieleranzahl_Refresh();

main()
{
    DC_SetToken("aBzDxYzaBzDxYzaBzDxYz1aBzDxYz"); // <- sonst funzt nichts.
    DC_SetGuild("012345678912345678"); // <- sonst funzt nichts.
    DC_SetMinLogLevel(1);
}

public OnInit()
{
    DC_SetActivityText("DiscordAMX!");
    printc("OnInit aufgerufen.");

    SetTimer("UpdateDiscordActivity", 5000);
    return 1;
}

public Spieleranzahl_Refresh()
{
	DC_SendChannelMessage("000000000000000000", "Hallo aus PAWNIA!");
}
```

---

## :construction: VORABVERSION! KEIN STABILES RELEASE!

Hier ist noch nichts perfekt, garantiert oder fertig! Bitte schaut später vorbei wenn ich keine PAWN Scripter seid!
Watch anklicken nicht vergessen!

Bitte geht sicher dass ihr mindestens .NET Runtime 5.0 installiert habt!


## Features
* DiscordActivity Text setzen (fürs erste nur als 'playing')  
* Nachrichten an beliebigen Channel senden
* Timer verwenden (ansync, multithreadded)


## Geplant ist
* DC_OnChannelMessage()
* DC_OnMemberJoin()
* DC_OnReactionAdded()
* DC_RegisterCommand()
* User Functions (Nutzer per Name oder ID getten, PMs, usw)
* Eine eigene, bessere printf  funktion (die jetzte ist dennoch sehr gut)

### 'Dritthersteller' info
* Verwendung von [PAWN Wrapper by ikkentim](https://github.com/ikkentim/AMXWrapper)
* Verwendung von [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)
* Verwendung von [Colorful Console](https://github.com/tomakita/Colorful.Console)
