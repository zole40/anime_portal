using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll.Manager
{
    public class RatingManager
    {
        public static int addRating(int? rating, string friendlyUrl, string userId)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl)).Id;
                if (rating != null) {

                    var rate = context.Rating.FirstOrDefault(anime => anime.AnimeID == animeId && anime.UserID.Equals(userId));
                    if (rate == null)
                    {
                        context.Rating.Add(new Rating
                        {
                            Rate = (int)rating,
                            UserID = userId,
                            AnimeID = animeId
                        });
                    }
                    else
                    {
                        rate.Rate = (int)rating;
                        context.Rating.Attach(rate);
                        context.Entry(rate).State = EntityState.Modified;

                    }
                    context.SaveChanges();
                }
                var animeRating = (from rat in context.Rating
                                   where rat.AnimeID == animeId
                                   select rat.Rate
                                       ).ToList();
                return  (int)Math.Round(animeRating.Average(),MidpointRounding.AwayFromZero);  
            }
        }
    }
}
