using Blog3.Models;
using Blog3.Models.Comments;
using Blog3.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Blog3.Data.Repository
{
    public class Repository : IRepository
    {
        private AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public  void AddPost(Post post)
        {
            //throw new NotImplementedException();
            _context.Posts.Add(post);
            //await _context.SaveChangesAsync();
        }

        public void AddSubComment(SubComment comment)
        {
            //throw new NotImplementedException();
            _context.SubComments.Add(comment);
        }

        public Post DeleteMainComment(int postid, int mainid)
        {
            //throw new NotImplementedException();
            var MyPost = _context.Posts.FirstOrDefault(p=> p.Id ==postid);
            var Maincomment = _context.MainComments.FirstOrDefault(m => m.Id == mainid);

            MyPost.MainComments.Remove(Maincomment);
            _context.MainComments.Remove(Maincomment);
            _context.SaveChanges();
            return MyPost;
        }

        public Post DeleteSubComment(int postid, int subid, int mainid)
        {
            //throw new NotImplementedException();
            var MyPost = _context.Posts.FirstOrDefault(p => p.Id == postid);
            var Maincomment = _context.MainComments.FirstOrDefault(m => m.Id == mainid);
            var Subcomment = _context.SubComments.FirstOrDefault(s => s.Id == subid);
            Maincomment.SubComments.Remove(Subcomment);
            _context.SubComments.Remove(Subcomment);
            _context.SaveChanges();
            return MyPost;
        }

        public Post DislikePost(int id,string username)
        {
            var new_post = _context.Posts.FirstOrDefault(p => p.Id == id);
            new_post.DislikedByUser = DislikedByCurrentUser(id, username);
            if (!new_post.DislikedByUser)
            {
                Dislike new_dislike = new Dislike();
                new_dislike.PostId = id;
                new_dislike.Username = username;

                _context.Dislikes.Add(new_dislike);

                new_post.Dislikes = new_post.Dislikes + 1;
                new_post.DislikedByUser = true;

                UpdatePost(new_post);
                _context.SaveChanges();
                return new_post;

            }
            else
            {
                var my_dislike = _context.Dislikes.Where(l => l.PostId == id && l.Username == username).FirstOrDefault();
                _context.Dislikes.Remove(my_dislike);

                new_post.DislikedByUser = false;
                new_post.Dislikes = new_post.Dislikes - 1;
                UpdatePost(new_post);

                _context.SaveChanges();
                //return new_post;
                return new_post;
            }

        }

        public List<Post> GetAllPosts()
        {
            //throw new NotImplementedException();
            return _context.Posts.ToList();
        }

        //this method is for pagination. The basis of pagination is to skip a certain 
        //number of posts in the blog and move to the next
        //if we are doing 5 posts per page we do page 0: skip page number(0) * number of posts (5)
        //next page, page1 skip 1*5 posts so it skips the first 5 posts and shows the next 5, you get
       //however there is nothing like page 0 so instead we do page number -1
       
         public IndexViewModel GetAllPosts(int PageNumber, string category, string search)
        {
            if (!String.IsNullOrEmpty(search))
            {

                search = search.ToLower();
            }
            //Func<Post, bool> InCategory = (post) =>
            //{ return post.Category.ToLower() == category.ToLower(); };
            
            int PageSize = 5;
            int skipAmount = PageSize * (PageNumber - 1);
            int capacity = skipAmount + PageSize;

            //we will boost performance by using asnotracking. It is used when we need to
            //get data from the context without necessarily updating anything use as no tracking
            var query = _context
                        .Posts
                        .AsNoTracking()
                        .AsQueryable();

           
            if (!String.IsNullOrEmpty(category))
            {
                //query = query.Where(x => InCategory(x));
                query = query.Where(post => post.Category == category);
            }

            if (!String.IsNullOrEmpty(search))
            {
                //ideally when creating we are supposed to have a "search" field in our database that con
                //concatenates title body description



                //check the title body and description for the one that contains the string you are searching for
               //EF allows us to  use "like" just the same way we do in sql
                query = query.Where(x => (x.Title + x.Body + x.Description + x.Category).ToLower().Contains(search));

                //query = query.Where(x => EF.Functions.Like((x.Title + x.Body + x.Description),$"%{search}%"));

            }
            int postsCount = query.Count();

            query = query.Skip(skipAmount).Take(PageSize);


            IndexViewModel vm = new IndexViewModel();

            bool CanGoNext = postsCount > capacity;
            vm.Posts = query.ToList();
            vm.PageNumber = PageNumber;
            vm.NextPage = CanGoNext;
            vm.Category = category;
            vm.Search = search;
            vm.PageCount = Convert.ToInt32(Math.Ceiling((double)postsCount / PageSize));
            vm.PopularPosts =  _context.Posts.OrderByDescending(p => p.NoOfViews).Take(10).ToList();


            return vm;

        }

       



        /*  public IndexViewModel GetAllPosts(int pageNumber, string category)
          {
              return _context.Posts.Where(post => post.Category.ToLower() == category.ToLower()).ToList();

          }
          */

        public Post GetPost(int id)
        {
            //throw new NotImplementedException();
           // return _context.Posts.FirstOrDefault(p => p.Id == id);

            return  _context.Posts
                 .Include(mc=>mc.MainComments)
                .ThenInclude(sc=>sc.SubComments)
                .FirstOrDefault(p => p.Id == id);
            
            //return _context.Posts.FirstOrDefault(p => p.Id == id);
            
            ///IN A very simple sense we are trying to get the main comments of the post (include)
            ///and also the subcoments of the maincomments of the post(theninclude)
        }

        public Post IncreasePostViews(int id, string username)
        {
            //throw new NotImplementedException();
            
            var new_post = _context.Posts.FirstOrDefault(p => p.Id == id);
            new_post.ReadByUser = _context.Reads.Where(r=> r.PostId==id && r.Username==username).Any();
            if (!new_post.ReadByUser)
            {
                Read new_read = new Read();
                new_read.PostId = id;
                new_read.Username = username;
                _context.Reads.Add(new_read);

                new_post.NoOfViews = new_post.NoOfViews + 1;
                UpdatePost(new_post);
                _context.SaveChanges();
                return new_post;

            }
            else
            {
                return new_post;

            }
        }

        public bool LikedByCurrentUser(int id, string username)
        {
            return _context.Likes.Where(l => l.PostId == id && l.Username == username).Any();

        }
        public bool DislikedByCurrentUser(int id, string username)
        {
            return _context.Dislikes.Where(l => l.PostId == id && l.Username == username).Any();

        }
        public Post LikePost(int id, string username)
        {
            var new_post = _context.Posts.FirstOrDefault(p => p.Id == id);
            new_post.LikedByUser = LikedByCurrentUser(id, username);
            //remove like we have to do 3 things
            //remove that particular like from the likes table
            //set liked by user to false
            //reduce the count of likes of the post
            if (!new_post.LikedByUser)
            {
                Like new_like = new Like();
                new_like.PostId = id;
                new_like.Username = username;
                _context.Likes.Add(new_like);

                new_post.Likes = new_post.Likes + 1;
                new_post.LikedByUser = true;
                UpdatePost(new_post);
                _context.SaveChanges();
                return new_post;

            }


            else
            {
                 var my_like =_context.Likes.Where(l => l.PostId == id && l.Username == username).FirstOrDefault();
                _context.Likes.Remove(my_like);

                new_post.LikedByUser = false;
                new_post.Likes = new_post.Likes - 1;
                UpdatePost(new_post);

                _context.SaveChanges();
                return new_post;
            }

        }

        public void RemovePost(int id)
        {
            //throw new NotImplementedException();
            _context.Posts.Remove(GetPost(id));
        }

        public async Task<bool> SaveChangesAsync()
        {
            //throw new NotImplementedException();
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }
            return false;
        }

        public void UpdatePost(Post post)
        {
            //throw new NotImplementedException();
            _context.Posts.Update(post);
        }
    }
}
