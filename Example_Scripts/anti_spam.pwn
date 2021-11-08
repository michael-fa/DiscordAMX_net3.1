include <a_dcamx>

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
