# DiscordAMX

Ein .NET Discord Bot scriptbar mit der [Pawn Scriptsprache](https://github.com/pawn-lang)! Das Projekt befindet sich noch in den Kinderschuhen!

## :de: [English](https://github.com/michael-fa/DiscordAMX/blob/master/readme.md) translation is also available. (Am up-to-datestesten lol)
## [Schau im Wiki nach wenn du Fragen hast.](https://github.com/michael-fa/DiscordAMX/wiki) (Aktuell leider nur in Englisch, tut mir leid!)
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

public OnMemberJoin(memberid[])
{
    return 1;
}

public OnMemberLeave(memberid[])
{
    return 1;
}

public OnMessage(channelid[], memberid[], message[])
{
    return 1;
}

public OnMessageDeleted(messageid[])
{
    return 1;
}
```

---

## :construction: VORABVERSION! KEIN STABILES RELEASE!
DiscordAMX befindet sich noch in der Alpha-Phase! Wir bauen gerade alle nötigen Grundfunktionen auf, und fließen dann langsam in die Bridge-Entwicklung.
Bridge: Spiele API zugriff, wie Minecraft, San Andreas Multiplayer, MTA:SA, CS:GO und so weiter, auch andere Programme kommen in Frage.

## Tester
**Wenn du Pawn bereits mittel- bis gut drauf hast, und über einen Discord Server verfügst, der mehrere Mitglieder aufbringt, ist es möglich mich (michael-fa) nach einem Hosting für Testzwecke zu fragen! Es werden aber nur 2-3 "Offizielle" Tester gesucht - wichtig ist andauerndes Feedback!**

__Desweiteren__:
Du möchtest jetzt schon Testen? Du kannst! Das aktuelleste Release ist für private Tester da! Aber denk daran, DiscordAMX ist ein self-host Bot-Programm!
Das bedeutet, ihr müsst euch einen Linux/Windows (v)Server bereitstellen lassen, und ein wenig Know-How mitbringen.
Da das ganze für jeden Scriptanfänger machbar sein sollte, wird es bald eine Linux - Windows Komplett-Guide geben, was das minimale nötig zum Starten des Bot's auflistet und für euch so der Einstieg einfacher fällt.

⚠️ Bitte geht sicher dass ihr mindestens .NET Runtime 5.0 installiert habt, auf Linux oder Windows!


## Features
* DiscordActivity Text setzen (fürs erste nur als 'playing')  
* DC_Token, DC_Guild setzen, beides notwendig in Script main(). Momentan ist nur eine Guild (Discord Server) pro DiscordAMX Instanz unterstützt.
* Empfange bereits schon: OnMessage, OnMemberJoin, OnMemberLeave, OnCommand, OnReactionAdded
* Nachrichten an beliebigen Channel senden
* Timer verwenden (ansync, multithreadded)
* GetMember -Name/NickName/Discriminator Funktion
* Mehrere Scripts laden unterstützt


## Voraussichtlich geplant ist:
* Mehrere Discord-Server unterstützen (wie für Member, ein für die Script's vereinfachtes ID-Handling einfügen)
* Nativer MySQL Support
* Natives YT->MP3 & Music Streaming
* Datei handling
* Eine eigene, bessere printf  funktion (die aktuelle ist dennoch sehr gut)
* LINUX SUPPORT

### 'Dritthersteller' info
* Verwendung von [PAWN Wrapper by ikkentim](https://github.com/ikkentim/AMXWrapper)
* Verwendung von [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus)


[Gehe auf CompuPhase's Seite](https://www.compuphase.com/pawn/pawn.htm) für mehr zur PAWN Sprache!
