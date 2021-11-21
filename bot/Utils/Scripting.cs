using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;


namespace dcamx.Utils
{
    public static class Scripting
    {
        public static DiscordMember ScrMemberID_DCMember(int _id, dcamx.Scripting.Guild _gld)
        {
            dcamx.Scripting.Member mbr_ = null;
            foreach (dcamx.Scripting.Member mbr in _gld.m_ScriptMembers)
            {
                if (mbr.m_ID == _id) mbr_ = mbr;
            }
            return mbr_.m_DiscordMember;
        }

        public static int ScrMemberDCMember_ID(DiscordUser _member, dcamx.Scripting.Guild _gld)
        {
            dcamx.Scripting.Member mbr_ = null;

            foreach (dcamx.Scripting.Member mbr in _gld.m_ScriptMembers)
            {
                if (mbr.m_DiscordMember.Equals(_member)) mbr_ = mbr;
            }
            return mbr_.m_ID;
        }







        public static dcamx.Scripting.Member DCMember_ScrMember(DiscordMember _member, dcamx.Scripting.Guild _gld)
        {
            dcamx.Scripting.Member mbr_ = null;
            foreach (dcamx.Scripting.Member mbr in _gld.m_ScriptMembers)
            {
                if (mbr.m_DiscordMember.Equals(_member)) mbr_ = mbr;
            }
            return mbr_;
        }

        public static dcamx.Scripting.Member DCMember_ScrMember(DiscordUser _member, dcamx.Scripting.Guild _gld)
        {
            dcamx.Scripting.Member mbr_ = null;
            foreach (dcamx.Scripting.Member mbr in _gld.m_ScriptMembers)
            {
                if (mbr.m_DiscordMember.Equals(_member)) mbr_ = mbr;
            }
            return mbr_;
        }

        public static dcamx.Scripting.Guild DCGuild_ScrGuild(DiscordGuild _guild)
        {
            dcamx.Scripting.Guild mbr_ = null;
            foreach (dcamx.Scripting.Guild mbr in Program.m_ScriptGuilds)
            {
                if (mbr.m_DCGuild == _guild) mbr_ = mbr;
            }
            return mbr_;
        }
    }
}
