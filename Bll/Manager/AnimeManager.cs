using Bll.DTO.AnimeModel;
using Bll.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Bll.Manager
{
    public class AnimeManager
    {

        public static bool editorRequest(string friendlyUrl, string userId,out AnimeHeaderData anim)
        {
            using (var context = new AnimePortalEntities())
            {
                var editor = (from edit in context.Editors
                              where edit.UserID.Equals(userId)
                              join anime in context.Animes on edit.AnimeID equals anime.Id
                              where anime.FriendlyUrl.Equals(friendlyUrl)
                              select new { }
                              ).FirstOrDefault();
                if (editor != null)
                {
                    anim = null;
                    return false;
                }
                var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl));
                context.Editors.Add(new Editors()
                {
                    AnimeID = animeId.Id,
                    UserID = userId,
                    Edit = 0
                });
                anim = new AnimeHeaderData()
                {
                    Uploader = animeId.Uploader,
                    Name = animeId.Name
                };
                context.SaveChanges();
                return true;
            }
        }

        public static bool havesession(string friendlyUrl)
        {
            using (var context = new AnimePortalEntities())
            {
                var anime = context.Animes.FirstOrDefault(anim => anim.FriendlyUrl.Equals(friendlyUrl));
                var session = context.Session.FirstOrDefault(anim => anim.AnimeID == anime.Id);
                return (session != null);
            }
        }

        public static bool cancelRequest(string friendlyUrl, string userId, out AnimeHeaderData anim)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl));
                var editor = context.Editors.FirstOrDefault(edit => edit.UserID.Equals(userId) && edit.AnimeID == animeId.Id);
                if (editor != null)
                {
                    anim = new AnimeHeaderData()
                    {
                        Uploader = animeId.Uploader,
                        Name = animeId.Name
                    };
                    context.Editors.Remove(editor);
                    context.SaveChanges();
                    return true;
                }
                anim = null;
                return false;

            }
        }

        public static bool acceptRequest(string friendlyUrl, string userId, out AnimeHeaderData anim)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl));
                var editor = context.Editors.FirstOrDefault(edit => edit.UserID.Equals(userId) && edit.AnimeID == animeId.Id && edit.Edit != 2);
                if (editor != null)
                {
                    anim = new AnimeHeaderData()
                    {
                        Uploader = animeId.Uploader,
                        Name = animeId.Name
                    };
                    editor.Edit = 2;
                    context.Editors.Attach(editor);
                    context.Entry(editor).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
                anim = null;
                return false;
            }
        }

        public static bool rejectRequest(string friendlyUrl, string userId, out AnimeHeaderData anim)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl));
                var editor = context.Editors.FirstOrDefault(edit => edit.UserID.Equals(userId) && edit.AnimeID == animeId.Id && edit.Edit != 1);
                if (editor != null)
                {
                    anim = new AnimeHeaderData()
                    {
                        Uploader = animeId.Uploader,
                        Name = animeId.Name
                    };
                    editor.Edit = 1;
                    context.Editors.Attach(editor);
                    context.Entry(editor).State = EntityState.Modified;
                    context.SaveChanges();
                    return true;
                }
                anim = null;
                return false;
            }
        }


        public static List<AnimeManagement> management(string userId)
        {
            using (var context = new AnimePortalEntities())
            {
                var manage = (from anime in context.Animes
                              where anime.Uploader.Equals(userId)
                              join editor in context.Editors on anime.Id equals editor.AnimeID
                              select new AnimeManagement
                              {
                                  AnimeName = anime.Name,
                                  UserId = editor.UserID,
                                  Edit = editor.Edit,
                                  friendlyUrl = anime.FriendlyUrl
                              }
                              ).ToList();
                return manage;
            }
        }


        public static bool delete(string friendlyUrl)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeId = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl));

                var favorite = (from p in context.Favorite
                                where p.AnimeID == animeId.Id
                                select p).ToList();

                context.Favorite.RemoveRange(favorite);

                var edit = (from p in context.Editors
                            where p.AnimeID == animeId.Id
                            select p).ToList();
                context.Editors.RemoveRange(edit);

                var videos = (from p in context.Video
                              where p.AnimeID == animeId.Id
                              select p).ToList();

                context.Video.RemoveRange(videos);

                var session = (from p in context.Session
                               where p.AnimeID == animeId.Id
                               select p).ToList();

                context.Session.RemoveRange(session);

                var rating = (from p in context.Rating
                              where p.AnimeID == animeId.Id
                              select p).ToList();
                context.Rating.RemoveRange(rating);

                context.Animes.Remove(animeId);
                //try
                //{
                context.SaveChanges();
                return true;
                //}
                //catch
                //{
                //    return false;
                //}
            }
        }

        public static string GenerateSeoUrls(AnimeHeaderData header)
        {
            return header.Name.ToSeoFriendlyUrl() + header.Category.ToSeoFriendlyUrl();
        }

        public static bool addFavorite(string friendlyUrl, string userId)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeid = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl)).Id;
                context.Favorite.Add(new Favorite()
                {
                    AnimeID = animeid,
                    UserId = userId
                });
                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }

        public static bool removeFavorite(string friendlyUrl, string userId)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeid = context.Animes.FirstOrDefault(anime => anime.FriendlyUrl.Equals(friendlyUrl)).Id;
                var favorite = context.Favorite.FirstOrDefault(fav => fav.AnimeID.Equals(animeid) && fav.UserId.Equals(userId));
                context.Favorite.Remove(favorite);
                try
                {
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }


        public static List<AnimeHeaderIndex> getAllAnime()
        {
            using (var context = new AnimePortalEntities())
            {
                var allAnime = (from p in context.Animes
                                select new AnimeHeaderIndex
                                {
                                    Name = p.Name,
                                    FriendlyUrl = p.FriendlyUrl,
                                    Category = context.Category.FirstOrDefault(id => id.Id == p.Category).Name,
                                    PictureUrl = p.PictureUrl,
                                    Rating = p.Rating
                                }).ToList();
                return allAnime;
            }
        }

        public static List<AnimeHeaderIndex> getCategory(int? Category)
        {
            using (var context = new AnimePortalEntities())
            {
                var anime = (from p in context.Animes
                             where p.Category == Category
                             select new AnimeHeaderIndex
                             {
                                 Name = p.Name,
                                 FriendlyUrl = p.FriendlyUrl,
                                 Category = context.Category.FirstOrDefault(id => id.Id == p.Category).Name,
                                 PictureUrl = p.PictureUrl,
                                 Rating = p.Rating
                             }).ToList();
                return anime;
            }
        }

        public static List<AnimeHeaderIndex> getCreatedAnimes(string userId)
        {
            using (var context = new AnimePortalEntities())
            {
                var uploadedAnime = (from p in context.Animes
                                     where (p.Uploader.Equals(userId))
                                     select new AnimeHeaderIndex
                                     {
                                         Name = p.Name,
                                         FriendlyUrl = p.FriendlyUrl,
                                         Category = context.Category.FirstOrDefault(id => id.Id == p.Category).Name,
                                         PictureUrl = p.PictureUrl,
                                         Rating = p.Rating,
                                     }).ToList();
                return uploadedAnime;
            }
        }

        public static List<AnimeHeaderIndex> getFavoriteAnimes(string userId)
        {
            using (var context = new AnimePortalEntities())
            {
                var favoriteAnimes = (from favorite in context.Favorite
                                      where favorite.UserId.Equals(userId)
                                      join anime in context.Animes on favorite.AnimeID equals anime.Id
                                      select new AnimeHeaderIndex
                                      {
                                          Name = anime.Name,
                                          FriendlyUrl = anime.FriendlyUrl,
                                          Category = context.Category.FirstOrDefault(id => id.Id == anime.Category).Name,
                                          PictureUrl = anime.PictureUrl,
                                          Rating = anime.Rating,
                                      }).ToList();
                return favoriteAnimes;
            }
        }

        public static List<AnimeHeaderIndex> getEditingAnimes(string userId)
        {
            using (var context = new AnimePortalEntities())
            {
                var favoriteAnimes = (from editor in context.Editors
                                      where editor.UserID.Equals(userId) && editor.Edit == 2
                                      join anime in context.Animes on editor.AnimeID equals anime.Id
                                      select new AnimeHeaderIndex
                                      {
                                          Name = anime.Name,
                                          FriendlyUrl = anime.FriendlyUrl,
                                          Category = context.Category.FirstOrDefault(id => id.Id == anime.Category).Name,
                                          PictureUrl = anime.PictureUrl,
                                          Rating = anime.Rating,
                                      }).ToList();
                return favoriteAnimes;
            }
        }


        public static AnimeHeaderData getAnime(string friendlyUrl, string userId)
        {
            bool creator;
            bool edit = isEditor(friendlyUrl, userId, out creator);
            bool request = hasRequest(friendlyUrl, userId);
            bool favorite = isFavorite(friendlyUrl, userId);
            using (var context = new AnimePortalEntities())
            {
                var anime = (from p in context.Animes
                             where (p.FriendlyUrl.Equals(friendlyUrl))
                             select new AnimeHeaderData
                             {
                                 Name = p.Name,
                                 Description = p.Description,
                                 FriendlyUrl = p.FriendlyUrl,
                                 Category = context.Category.FirstOrDefault(id => id.Id == p.Category).Name,
                                 PictureUrl = p.PictureUrl,
                                 Rating = p.Rating,
                                 Uploader = p.Uploader,
                                 Edit = edit,
                                 Favorite = favorite,
                                 Request = request
                             }).SingleOrDefault();
                return anime;
            }
        }

        private static bool isFavorite(string friendlyUrl, string userId)
        {
            if (userId == null)
            {
                return false;
            }
            using (var context = new AnimePortalEntities())
            {
                var favorite = (from anime in context.Animes
                                where anime.FriendlyUrl.Equals(friendlyUrl)
                                join Favorite in context.Favorite on anime.Id equals Favorite.AnimeID
                                where Favorite.UserId.Equals(userId)
                                select new { }
                                      ).FirstOrDefault();
                return (favorite != null);
            }
        }


        public static List<AnimeHeaderIndex> search(string search)
        {
            using (var context = new AnimePortalEntities())
            {
                var favoriteAnimes = (from anime in context.Animes
                                      join cat in context.Category on anime.Category equals cat.Id
                                      where cat.Name.Contains(search) || anime.Name.Contains(search) || anime.Description.Contains(search)
                                      orderby anime.Name
                                      select new AnimeHeaderIndex
                                      {
                                          Name = anime.Name,
                                          FriendlyUrl = anime.FriendlyUrl,
                                          Category = context.Category.FirstOrDefault(id => id.Id == anime.Category).Name,
                                          PictureUrl = anime.PictureUrl,
                                          Rating = anime.Rating,
                                      }).ToList();
                return favoriteAnimes;
            }
        }

        public static bool isEditor(string friendlyUrl, string userId, out bool creator)
        {
            if (userId == null)
            {
                creator = true;
                return false;
            }
            using (var context = new AnimePortalEntities())
            {
                var edit = (from anime in context.Animes
                            where anime.FriendlyUrl.Equals(friendlyUrl) && anime.Uploader.Equals(userId)
                            select new { }
                     ).FirstOrDefault();
                if (edit == null)
                {

                    edit = (from anime in context.Animes
                            where anime.FriendlyUrl.Equals(friendlyUrl)
                            join user in context.Editors on anime.Id equals user.AnimeID
                            where (user.UserID.Equals(userId) || anime.Uploader.Equals(userId)) && user.Edit == 2
                            select new { }
                                ).FirstOrDefault();

                }
                creator = false;
                return (edit != null);
            }
        }

        public static bool hasRequest(string friendlyUrl, string userId)
        {
            if (userId == null)
            {
                return false;
            }
            using (var context = new AnimePortalEntities())
            {
                var request = (from anime in context.Animes
                               where anime.FriendlyUrl.Equals(friendlyUrl)
                               join user in context.Editors on anime.Id equals user.AnimeID
                               where (user.UserID.Equals(userId) || anime.Uploader.Equals(userId)) && (user.Edit == 0 || user.Edit == 1)
                               select new { }
                                ).FirstOrDefault();


                return (request != null);
            }     
        }

        public static bool isEditor(int? videoId, string userId)
        {
            if (userId == null)
            {

                return false;
            }
            using (var context = new AnimePortalEntities())
            {
                var edit = (from Video in context.Video
                            where Video.ID == videoId
                            join anime in context.Animes on Video.AnimeID equals anime.Id
                            where anime.Uploader.Equals(userId)
                            select new { }
                     ).FirstOrDefault();
                if (edit == null)
                {

                    edit = (from Video in context.Video
                            join Editors in context.Editors on Video.AnimeID equals Editors.AnimeID
                            where Editors.UserID.Equals(userId)
                            select new { }
                                ).FirstOrDefault();

                }
                return (edit != null);
            }
        }
    }
}
