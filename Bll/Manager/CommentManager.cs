using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bll.DTO.CommentModel;

namespace Bll.Manager
{
    public class CommentManager
    {
        public static List<CommentData> getComments(int? type, int? CommentId)
        {
            using (var context = new AnimePortalEntities())
            {
                var comments = (from comment in context.Comment
                                where comment.Type == type && comment.CommentId == CommentId
                                select new CommentData
                                {
                                    UserName = comment.UserName,
                                    Text = comment.Text,
                                    UserPictureUrl = comment.UserProfilePicture,
                                    Date = comment.date
                                }).ToList();
                return comments;
            }
        }

        public static bool remove(int videoId)
        {
            using (var context = new AnimePortalEntities())
            {
                var comment = (from p in context.Comment
                               where p.CommentId == videoId
                               select p).ToList();
                try {
                    context.Comment.RemoveRange(comment);
                    context.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public static void CreateComment(int Type,string userName, string ProfilPicture,CommentCreateData data,int commentId)
        {
            using (var context = new AnimePortalEntities())
            {
                    context.Comment.Add(new Comment
                    {
                        UserName = userName,
                        CommentId = commentId,
                        Text = data.Text,
                        Type = Type,
                        UserProfilePicture = ProfilPicture,
                        date = DateTime.Now
                    });
                    context.SaveChanges();
            }
        }
    }
}
    
