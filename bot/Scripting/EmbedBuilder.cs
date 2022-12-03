using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcamx.Scripting
{
    public class DiscordEmbedBuilder
    {
        private DSharpPlus.Entities.DiscordEmbedBuilder m_Builder;
        public int m_ID;
        public DiscordMessage m_MessageID;

        public DiscordEmbedBuilder(string szTitle, string szDescription = "")
        {
            var embed = new DSharpPlus.Entities.DiscordEmbedBuilder
            {
                Title = szTitle,
                Description = szDescription
            };

            m_Builder = embed;
            Program.m_Embeds.Add(this);
            m_ID = Program.m_Embeds.Count;
        }

        public void SetThumbnail(string szUrl, int iH = 0, int iW = 0)
        {
            m_Builder.AddField("field", "field").WithThumbnail(szUrl, iH, iW);
        }

        public void SetAuthor(string szAuthor, string szUrl, string szIconUrl)
        {
            m_Builder.AddField("field", "field").WithAuthor(szAuthor);
            m_Builder.RemoveFieldAt(m_Builder.Fields.Count-1);
        }

        public void SetFooter(string szText, string szIconUrl)
        {
            m_Builder.AddField("", "").WithFooter(szText, szIconUrl);
        }

        public void SetImageUrl(string szUrl)
        {
            m_Builder.AddField("", "").WithImageUrl(szUrl);
        }
        public void SetUrl(string szUrl)
        {
            m_Builder.AddField("", "").WithUrl(szUrl);
        }

        public void AddText(string szTitle, string szText)
        {
            m_Builder.AddField(szTitle, szText);
        }

        public void ToggleTimestamp()
        {
            if (this.m_Builder.Timestamp == null) this.m_Builder.Timestamp = DateTime.Now;
            else this.m_Builder.Timestamp = null;
        }

        public DiscordMessage Send(DiscordChannel channel)
        {
            try
            {
                m_MessageID = channel.SendMessageAsync(this.m_Builder).Result;
                return m_MessageID;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DiscordMessage Update()
        {
            try
            {
                this.m_MessageID.ModifyAsync(this.m_Builder.Build());
                return m_MessageID;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
