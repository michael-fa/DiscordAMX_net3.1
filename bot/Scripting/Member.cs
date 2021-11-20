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
        public Member(DiscordMember _Member)
        {
            this.m_DiscordMember = _Member;
            Program.m_ScriptMembers.Add(this);
            this.m_ID = Program.m_ScriptMembers.Count;

        }

        public void Remove()
        {
            Program.m_ScriptMembers.Remove(this);
            this.m_ID = 0; 
        }

    }
}
