/*

		Example DISCORDAMX Script:	anti_spam.pwn
		Description:			Deleting the spam messages and cleaning up. This is kinda useless but still working!
		Copyright:			DiscordAMX 2021

*/


include <a_dcamx>

#define MY_BOT_TOKEN "YOUR_BOT_TOKEN"
#define MY_GUILD_ID "YOUR_GUILD_ID"
#define EXAMPLE_CHANNEL "CHANNEL_ID_AS_STRING"

//This is a "guessing" thing you have to do yourself. Simply go way higher then your discord server might reach in far future.
#define MAX_PLAYERS             50000

//Note, this is the same than back in SA-MP's enum.
const pSpamData:
{
    message_count = 0,
    last_msg_time,
    mutetime
}
new pSpamData:ClientSpam[MAX_PLAYERS][3];


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

public OnMessage(channelid[], memberid, messageid[], content[])
{
    new tstamp = gettimestamp();
    if((gettimestamp() - _:ClientSpam[1][last_msg_time]) < 2 )
    {
        ClientSpam[1][message_count] ++;
        if(_:ClientSpam[1][message_count] == 2)
        {
            ClientSpam[1][message_count] = pSpamData:4;
            DC_DeleteMessage(channelid, messageid, "AMX: Spam max exceeded!");
            return 1;
        }
        else if(_:ClientSpam[1][message_count] >3 )
        {
             DC_DeleteMessage(channelid, messageid, "AMX: Spam max exceeded!");
             return 1;
        }
    }
    else ClientSpam[1][message_count] = pSpamData:0, print("4");

    ClientSpam[1][last_msg_time] = pSpamData:tstamp;
    return 1;
}
