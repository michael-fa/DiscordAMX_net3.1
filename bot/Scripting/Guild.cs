﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;


namespace dcamx.Scripting
{
    public class Guild
    {
        public int m_ID;
        public DiscordGuild m_DCGuild;

        public Guild(DiscordGuild _dcg)
        {
            m_DCGuild = _dcg;
            Program.m_ScriptGuilds.Add(this);

            m_ID = Program.m_ScriptGuilds.Count();
        }
        
    }
}
