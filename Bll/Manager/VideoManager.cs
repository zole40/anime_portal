using Bll.DTO.AnimeModel;
using Bll.DTO.VideoModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bll.Manager
{
    public class VideoManager
    {
        public static List<VideoHeader> getVideos(int sessionId)
        {
            using (var context = new AnimePortalEntities())
            {
                var animeid = context.Session.FirstOrDefault(session => session.Id == sessionId).AnimeID;
                var anime = context.Animes.FirstOrDefault(an => an.Id == animeid);

                var videos = (from video in context.Video
                              where video.AnimeID == animeid && video.SessionID == sessionId
                              orderby video.Number
                              select new VideoHeader()
                              {
                                  Number = video.Number,
                                  Name = video.Name,
                                  PictureUrl = anime.PictureUrl,
                                  Id = video.ID
                              }
                              ).ToList();
                return videos;
            }
        }

        public static bool remove(int sessionId)
        {
            using (var context = new AnimePortalEntities())
            {
                var videos = (from p in context.Video
                              where p.SessionID == sessionId
                              select p).ToList();
                try
                {
                    foreach (var item in videos) 
                    {

                        if (!CommentManager.remove(item.ID))
                        {
                            return false;
                        }
                    }
                        context.Video.RemoveRange(videos);
                    context.SaveChanges();

                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static List<VideoHeader> getSimilarVideos(int sessionId,int videoId)
        {
            using (var context = new AnimePortalEntities())
            {


                var videos = (from video in context.Video
                              where video.SessionID == sessionId && video.ID != videoId
                              join anime in context.Animes on video.AnimeID equals anime.Id
                              orderby video.Number
                              select new VideoHeader()
                              {
                                  Number = video.Number,
                                  Name = video.Name,
                                  PictureUrl = anime.PictureUrl,
                                  Id = video.ID
                              }
                              ).Take(5).ToList();
                return videos;
            }
        }

        public static List<VideoHeader> getVideosByUser(string userId)
        {
            using (var context = new AnimePortalEntities())
            {
                 var videos = (from video in context.Video
                              where video.Uploder.Equals(userId)
                              join anime in context.Animes on video.AnimeID equals anime.Id
                              orderby video.Number
                              select new VideoHeader()
                              {
                                  Number = video.Number,
                                  Name = video.Name,
                                  PictureUrl = anime.PictureUrl,
                                  Id = video.ID,
                              }
                              ).ToList();
                return videos;
            }
        }


        public static VideoDataHeader getVideoData(int? videoid,string userId)
        {
            bool editor = AnimeManager.isEditor(videoid, userId);
            using (var context = new AnimePortalEntities())
            {
                var videos = (from video in context.Video
                              where video.ID == videoid
                              join anime in context.Animes on video.AnimeID equals anime.Id
                              join session in context.Session on video.SessionID equals session.Id
                              select new VideoDataHeader()
                              {
                                  Number = video.Number,
                                  Name = video.Name,
                                  VideoUrl = video.VideoUrl,
                                  Id = video.ID,
                                  Anime = anime.Name,
                                  Uploader = video.Uploder,
                                  Session = session.Number,
                                  SessionId = video.SessionID,
                                  AnimeFriendlyUrl = anime.FriendlyUrl,
                                  AnimePictureUrl = anime.PictureUrl,
                                  Upload = video.Uplload,
                                  Editor = editor
                              }).SingleOrDefault();
                return videos;
            }
        }
    }
}
