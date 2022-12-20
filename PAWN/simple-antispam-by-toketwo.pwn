#include <a_dcamx>

//This is a "guessing" thing you have to do yourself. Simply go way higher then your discord server might reach in far future.
#define MAX_PLAYERS             50000

//Note, this is the same than back in SA-MP's enum.
const pSpamData:
{
    message_count = 0,
    last_msg_time,
    mutetime
};
new pSpamData:ClientSpam[MAX_PLAYERS][3];


main()
{
    DC_SetMinLogLevel(1);
}

public OnFilterscriptInit()
{
    DC_SetActivityText("Initialiasing..", DISCORD_ACTIVITY_PLAYING);

    print("Rapid-Send Spamfilter System for DiscordAMX Beta Preview 1.1 - by toketwo (C) 2022");
    DC_SetActivityText("Ready", DISCORD_ACTIVITY_PLAYING);
    return 1;
}

public OnUnload()
{
    print("Rapid-Send Spamfilter script unloaded.");
    return 1;
}

//==========================================================
//==========================================================

public OnChannelMessage(guildid, channelid[], memberid, messageid[], content[])
{
    new tstamp = gettimestamp();
    if((gettimestamp() - _:ClientSpam[1][last_msg_time]) < 2 ) //Has this message been send in short delay to his last message?
    {
        ClientSpam[1][message_count] ++;
        if(_:ClientSpam[1][message_count] == 2)
        {
            DC_DeleteMessage(guildid,channelid, messageid, "AMX: Spam max exceeded!");
            return 1;
        }
        else if(_:ClientSpam[1][message_count] >3 )
        {
             DC_DeleteMessage(guildid, channelid, messageid, "AMX: Spam max exceeded!");
             return 1;
        }
    }
    else ClientSpam[1][message_count] = pSpamData:0, print("\n4\n");

    ClientSpam[1][last_msg_time] = pSpamData:tstamp;
    return 1;
}

//==========================================================
//==========================================================

public OnPrivateMessage(userid[], messageid[], text[])
{
    new tstamp = gettimestamp();
    if((gettimestamp() - _:ClientSpam[1][last_msg_time]) < 2 ) //Has this message been send in short delay to his last message?
    {
        ClientSpam[1][message_count] ++;
        if(_:ClientSpam[1][message_count] == 2)
        {
            ClientSpam[1][message_count] = pSpamData:4;
            DC_DeletePrivateMessage(userid, messageid, "AMX: Spam max exceeded!");
            return 1;
        }
        else if(_:ClientSpam[1][message_count] >3 )
        {
             DC_DeletePrivateMessage(userid, messageid, "AMX: Spam max exceeded!");
             return 1;
        }
    }
    else ClientSpam[1][message_count] = pSpamData:0, print("4");

    ClientSpam[1][last_msg_time] = pSpamData:tstamp;
    return 1;
}

//==========================================================

public OnThreadMessage(guildid, channelid[], threadid[], memberid, messageid[], content[])
{
    new tstamp = gettimestamp();
    if((gettimestamp() - _:ClientSpam[1][last_msg_time]) < 2 ) //Has this message been send in short delay to his last message?
    {
        ClientSpam[1][message_count] ++;
        if(_:ClientSpam[1][message_count] == 2)
        {
            ClientSpam[1][message_count] = pSpamData:4;
            DC_DeleteMessage(guildid,threadid, messageid, "AMX: Spam max exceeded!");
            return 1;
        }
        else if(_:ClientSpam[1][message_count] >3 )
        {
             DC_DeleteMessage(guildid, threadid, messageid, "AMX: Spam max exceeded!");
             return 1;
        }
    }
    else ClientSpam[1][message_count] = pSpamData:0, print("4");

    ClientSpam[1][last_msg_time] = pSpamData:tstamp;
    return 1;
}
