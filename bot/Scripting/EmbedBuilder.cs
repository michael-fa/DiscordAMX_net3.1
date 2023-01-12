using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dcamx.Scripting
{
    public class DiscordEmbedBuilder : IDisposable
    {
        private DSharpPlus.Entities.DiscordEmbedBuilder m_Builder;
        public int m_ID;
        bool disposed = false;
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
            m_Builder.WithThumbnail(szUrl, iH, iW);
        }

        public void SetAuthor(string szAuthor, string szUrl = null)
        {
            m_Builder.WithAuthor(szAuthor, szUrl);
        }

        public void SetFooter(string szText, string szIconUrl)
        {
            m_Builder.WithFooter(szText, szIconUrl);
        }

        public void SetColor(DiscordColor color)
        {
            m_Builder.WithColor(color);
        }

        public void SetImageUrl(string szUrl)
        {
            m_Builder.WithImageUrl(szUrl);
        }
        public void SetUrl(string szUrl)
        {
            m_Builder.WithUrl(szUrl);
        }

        public void AddText(string szTitle, string szText, bool bInline = false)
        {
            m_Builder.AddField(szTitle, szText, bInline);
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

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(disposing: true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SuppressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources
                }

                Program.m_Embeds.Remove(this);
                disposed = true;
            }
        }
    }
}
