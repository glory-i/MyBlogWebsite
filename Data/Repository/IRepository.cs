using Blog3.Models;
using Blog3.Models.Comments;
using Blog3.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.Data.Repository
{
    public interface IRepository
    {
        Post GetPost(int id);

        Post IncreasePostViews(int id, string username);
        Post LikePost(int id, string username);
        Post DislikePost(int id, string username);

        bool LikedByCurrentUser(int id, string username);
        bool DislikedByCurrentUser(int id, string username);

        Post DeleteSubComment(int postid, int subid, int mainid);
        Post DeleteMainComment(int postid, int mainid);

      

        List<Post> GetAllPosts();
        IndexViewModel GetAllPosts(int PageNumber, string category, string search);

        void AddPost(Post post);
        void UpdatePost(Post post);
        void RemovePost(int id);
        Task<bool> SaveChangesAsync();

        void AddSubComment(SubComment comment);

    }
}
