using Bll.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll.Manager
{
    public class SessionManager
    {
        public static List<SessionHeaderData> getSessions(string friendlyUrl)
        {

            using (var context = new AnimePortalEntities())
            {
                var sessions = (from anime in context.Animes
                                where anime.FriendlyUrl.Equals(friendlyUrl)
                                join session in context.Session on anime.Id equals session.AnimeID
                                orderby session.Number
                                select new SessionHeaderData
                                {
                                    AnimeID = anime.Id,
                                    Number = session.Number,
                                    SessionID = session.Id

                                }).ToList();
                return sessions;
            }
        }
    }
}

