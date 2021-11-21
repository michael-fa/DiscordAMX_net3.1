using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcamx.Scripting
{
    public class Member
    {
        public DiscordMember m_DiscordMember;
        public int m_ID;
        public Scripting.Guild m_DiscordGuild;
        public Member(DiscordMember _Member, Scripting.Guild _Guild)
        {
            this.m_DiscordMember = _Member;
            this.m_DiscordGuild = _Guild;

            m_DiscordGuild.m_ScriptMembers.Add(this);
            this.m_ID = m_DiscordGuild.m_ScriptMembers.Count;

        }

        public void Remove()
        {
            m_DiscordGuild.m_ScriptMembers.Remove(this);
            this.m_ID = 0; 
        }

    }
}
