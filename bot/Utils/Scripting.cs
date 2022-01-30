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
                if (mbr.m_ID == _id) return mbr.m_DiscordMember;
            }
            return mbr_.m_DiscordMember;
        }

        public static int ScrMemberDCMember_ID(DiscordUser _member, dcamx.Scripting.Guild _gld)
        {
            dcamx.Scripting.Member mbr_ = null;

            foreach (dcamx.Scripting.Member mbr in _gld.m_ScriptMembers)
            {
                if (mbr.m_DiscordMember.Equals(_member)) return mbr.m_ID;
            }
            return mbr_.m_ID;
        }







        public static dcamx.Scripting.Member DCMember_ScrMember(DiscordMember _member, dcamx.Scripting.Guild _gld)
        {
            dcamx.Scripting.Member mbr_ = null;
            foreach (dcamx.Scripting.Member mbr in _gld.m_ScriptMembers)
            {
                if (mbr.m_DiscordMember.Equals(_member)) return mbr;
            }
            return mbr_;
        }

        public static dcamx.Scripting.Member DCMember_ScrMember(DiscordUser _member, dcamx.Scripting.Guild _gld)
        {
            dcamx.Scripting.Member mbr_ = null;
            foreach (dcamx.Scripting.Member mbr in _gld.m_ScriptMembers)
            {
                if (mbr.m_DiscordMember.Equals(_member)) return mbr;
            }
            return mbr_;
        }

        public static dcamx.Scripting.Guild DCGuild_ScrGuild(DiscordGuild _guild)
        {
            foreach (dcamx.Scripting.Guild mbr in Program.m_ScriptGuilds)
            {
                if (mbr.m_DCGuild == _guild)
                {
                    return mbr;
                }
            }
            return null;
        }

        public static DiscordGuild ScrGuild_DCGuild(int _guild)
        {
            DiscordGuild mbr_ = null;
            foreach (dcamx.Scripting.Guild mbr in Program.m_ScriptGuilds)
            {
                if (mbr.m_ID == _guild)
                {
                    
                    return mbr.m_DCGuild;
                }
            }
            return mbr_;
        }

        public static void ScriptFormat_Resolve(string format, params object[] param)
        {
            int cnt = 0;
            foreach(char x in format.ToCharArray())
            {
                //Utils.Log.Debug("format rule: '" + x + "'");
                switch(x)
                {
                    case 'i':
                        Utils.Log.Debug("  value: " + (Int32)param[cnt]);
                        cnt++;
                        break;
                    case 'f':
                        Utils.Log.Debug("  value: " + Convert.ToSingle(param[cnt]));
                        cnt++;
                        break;
                    case 'a':
                        cnt++;
                        break;
                }
            }
        }
    }
}
