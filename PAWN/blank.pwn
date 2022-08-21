#include <a_dcamx>



main()
{
}

public OnInit()
{
    return 1;
}

public OnUnload()
{
    return 1;
}

public OnConsoleInput(input[])
{
    return 1;
}

//==========================================================
//==========================================================

public OnHeartbeat(ping)
{
    return 1;
}

//==========================================================
//==========================================================

public OnGuildAdded(guildid)
{
    return 1;
}

public OnGuildRemoved(guildid, guildname[])
{
    return 1;
}

public OnGuildUpdated(guildid, guild_id_string[], member_count, old_name[], new_name[])
{
    return 1;
}

//==========================================================
//==========================================================

public OnChannelCreated(guildid, channelid[], bool:isPrivate)
{
    return 1;
}

public OnChannelUpdated(guildid, channelid[])
{
    return 1;
}

public OnChannelDeleted(guildid, channelid[], bool:isPrivate)
{
    return 1;
}

//==========================================================
//==========================================================

public OnThreadCreated(guildid, channelid[], threadid[])
{
    return 1;
}

public OnThreadDeleted(guildid, channelid[], threadid[])
{
    return 1;
}

public OnThreadUpdated(guildid, channelid[], threadid[], new_name[])
{
    return 1;
}

//==========================================================
//==========================================================

public OnMemberJoin(guildid, memberid)
{
    return 1;
}

public OnMemberLeave(guildid, memberid)
{
    return 1;
}

//==========================================================
//==========================================================

public OnChannelMessage(guildid, channelid[], memberid, messageid[], content[])
{
    return 1;
}

public OnChannelMessageUpdated(guildid, channelid[], memberid, messageid[], old_text[], new_text[])
{
    return 1;
}

public OnChannelMessageDeleted(guildid, channelid[], messageid[])
{
    return 1;
}

public OnReactionAdded(guildid, emojiid[], messageid[], memberid, channelid[])
{
    return 1;
}

public OnReactionRemoved(guildid, emojiid[], messageid[], memberid, channelid[])
{
    return 1;
}

//==========================================================
//==========================================================

public OnPrivateMessage(userid[], messageid[], text[])
{
    return 1;
}

public OnPrivateMessageDeleted(userid[], messageid[])
{
    return 1;
}

public OnPrivateMessageUpdated(userid[], messageid[], old_text[], new_text[])
{
    return 1;
}

public OnPrivateReactionAdded(emojiid[], messageid[], memberid[], channelid[])
{
    return 1;
}

public OnPrivateReactionRemoved(emojiid[], messageid[], memberid[], channelid[])
{
    return 1;
}

//==========================================================
//==========================================================

public OnThreadMessage(guildid, channelid[], threadid[], memberid, messageid[], content[])
{
    return 1;
}

public OnThreadMessageDeleted(guildid, channelid[], threadid[], messageid[])
{
    return 1;
}

public OnThreadMessageUpdated(guildid, channelid[], threadid[], memberid[], messageid[],  old_text[], new_text[])
{
    return 1;
}

public OnThreadMemberJoined(guildid, channelid[], threadid[], memberid)
{
    return 1;
}

public OnThreadMemberLeft(guildid, channelid[], threadid[], memberid)
{
    return 1;
}

public OnThreadMessageReactionAdded(guildid, emojiid[], messageid[], memberid, channelid[], threadid[])
{
    return 1;
}

public OnThreadMessageReactionRemoved(guildid, emojiid[], messageid[], memberid, channelid[], threadid[])
{
    return 1;
}
