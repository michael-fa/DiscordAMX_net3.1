/*  DiscordAMX Scripting Functions
 *
 *  (c) Copyright 2021-2022 Michael Fanter (fanter.eu)
 *
 */

#if defined _dcamx_included
	#endinput
#endif
#define _dcamx_included
#pragma library dcamx

//Remove this if you prefer to take care of core includes your own.
#include <core>
#include <float>
#include <string>
#include <file>
#include <time>
#include <datagram>

// Limits and internal constants
#define DISCORD_ACTIVITY_PLAYING                        (0)
#define DISCORD_ACTIVITY_LISTENING                      (1)
#define DISCORD_ACTIVITY_COMPETING                      (2)


#define CHANNEL_TYPE_TEXT                               (0)
#define CHANNEL_TYPE_PRIVATE                            (1)
#define CHANNEL_TYPE_VOICE                              (2)
#define CHANNEL_TYPE_GROUP                              (3)
#define CHANNEL_TYPE_CATEGORY                           (4)
#define CHANNEL_TYPE_NEWS                               (5)
#define CHANNEL_TYPE_THREAD                             (11)

#define MAX_USERS                      50000
#define MAX_GAMES                      200
#define INVALID_USER_ID                -1
#define INVALID_TIMER_ID               -1


// Natives - DCAMX Core
native SetTimer(callback[], interval, bool:repeat);
native KillTimer(timerid);
native gettimestamp();
native Loadscript(script[]);
native Unloadscript(script[]);
native CallRemoteFunction(function[], format[], {Float,_}:...);
native DC_GetBotPing();

//         - INIFile (DCAMX core)
native INI_Delete(fileid);
native INI_Open(filepath[]);
native INI_Close(fileid);
native INI_Write(fileid, key[], value[], section[]);
native INI_WriteInt(fileid, key[], value, section[]);
native Float:INI_ReadFloat(fileid, key[], section[]);
native INI_WriteFloat(fileid, key[], Float:value, section[]);
native INI_Read(fileid, key[], section[], output[]);
native INI_ReadInt(fileid, key[], section[]);
native INI_KeyExists(fileid, key[], section[]);
native INI_Exists(filepath[]);
native INI_DeleteSection(fileid, section[]);
native INI_DeleteKey(fileid, key[], section[]);

//         - Discord / Bot
native DC_SetMinLogLevel(lvl);
native DC_SetActivityText(text[], activity_type = DISCORD_ACTIVITY_PLAYING);

//         - Guilds
native DC_GetGuildName(guildid, dest_string[]);
native DC_GetMemberCount(guildid);
native DC_GetGuildCount();

//         - Members
native DC_GetMemberName(guildid, memberid, dest_string[]);
native DC_GetMemberDisplayName(guildid, memberid, dest_string[]);
native DC_GetMemberDiscriminator(guildid, memberid, dest_string[]);
native DC_GetMemberID(guildid, memberid, dest_string[]);
native DC_GetMemberAvatarURL(guildid, memberid, dest_string[]);
native DC_BanMember(guildid, memberid, delete_messages_days = 0, reason[]);
native DC_UnbanMember(guildid, memberid);
native DC_TimeoutMember(guildid, memberid, seconds, reason[]);
native DC_MemberHasRole(guildid, memberid, role_name[]);
native DC_SetMemberRole(guildid, memberid, role_name[]);
native DC_RevokeMemberRole(guildid, memberid, role_name[]);
//         - This is special for DM members only
native DC_BanUser(userid[]);
native DC_UnbanUser(userid);
native DC_IsUserBanned(userid);

//         - Channels / Threads / DM
native DC_CreateChannel(guildid, channel_name[], channel_type = 0, parent[], topic[], nfsw = 0);
native DC_DeleteChannel(guildid, channelid[], reason[]);
native DC_SendPrivateMessage(channelid[], message[], out_id[] = '0');
native DC_DeletePrivateMessage(channelid[], messageid[], reason[]);
native DC_SendChannelMessage(guildid, channelid[], message[], out_id[] = '0');
native DC_DeleteMessage(guildid, channelid[], messageid[], reason[] = '0');
native DC_SendEmbeddedImage(guildid = -1, channelid[], image_url[], title[], description[], out_id[] = '0'); //THIS FUNCTION WORKS FOR THREADS, DMs and TEXT CHANNELS!
native DC_FindChannel(guildid = -1, channel_name[], destination[]);
native DC_FindThread(guildid, channel_name[], destination[]);
native DC_GetChannelName(guildid, channelid[], destination[]);
native DC_GetChannelTopic(guildid, channelid[], destination[]);
native DC_GetChannelMention(guildid = -1, channelid[], destination[]);
native DC_GetChannelType(guildid, channelid[]);

//         Reactions
native DC_AddReaction(guildid, channelid[], messageid[], reaction[]);
native DC_AddPrivateReaction(channelid[], messageid[], reaction[]);
native DC_RemoveReaction(guildid, channelid[], messageid[], reaction[]);
native DC_RemovePrivateReaction(channelid[], messageid[], reaction);





// Callbacks - DCAMX Core
forward OnInit();
forward OnUnload();
forward OnHeartbeat(ping);
forward OnConsoleInput(input[]);

//           - Guilds
forward OnGuildAdded(guildid);
forward OnGuildRemoved(guildid, guildname[]);
forward OnGuildUpdated(guildid, guild_id_string[], member_count, old_name[], new_name[]);

//           - Members
forward OnMemberJoin(guildid, memberid);
forward OnMemberLeave(guildid, memberid);

//           - Channels
forward OnChannelMessage(guildid, channelid[], memberid, messageid[], content[]);
forward OnChannelMessageDeleted(guildid, channelid[], messageid[]);
forward OnReactionAdded(guildid, emojiid[], messageid[], memberid, channelid[]);
forward OnReactionRemoved(guildid, emojiid[], messageid[], memberid, channelid[]);
forward OnChannelMessageUpdated(guildid, channelid[], memberid, messageid[], old_text[], new_text[]);
forward OnChannelCreated(guildid, channelid[], bool:isPrivate);
forward OnChannelDeleted(guildid, channelid[], bool:isPrivate);
forward OnChannelUpdated(guildid, channelid[]);

//           - Threads
forward OnThreadCreated(guildid, channelid[], threadid[]);
forward OnThreadDeleted(guildid, channelid[], threadid[]);
forward OnThreadUpdated(guildid, channelid[], threadid[], new_name[]);
forward OnThreadMessage(guildid, channelid[], threadid[], memberid, messageid[], content[]);
forward OnThreadMessageDeleted(guildid, channelid[], threadid[], messageid[]);
forward OnThreadMessageUpdated(guildid, channelid[], threadid[], memberid[], messageid[],  old_text[], new_text[]);
forward OnThreadMessageReactionAdded(guildid, emojiid[], messageid[], memberid, channelid[], threadid[]);
forward OnThreadMessageReactionRemoved(guildid, emojiid[], messageid[], memberid, channelid[], threadid[]);
forward OnThreadMemberJoined(guildid, channelid[], threadid[], memberid);
forward OnThreadMemberLeft(guildid, channelid[], threadid[], memberid);

//           - DM
forward OnPrivateMessage(userid[], messageid[], text[]);
forward OnPrivateMessageDeleted(userid[], messageid[]);
forward OnPrivateMessageUpdated(userid[], messageid[], old_text[], new_text[]);
forward OnPrivateReactionAdded(emojiid[], messageid[], memberid[], channelid[]);
forward OnPrivateReactionRemoved(emojiid[], messageid[], memberid[], channelid[]);
