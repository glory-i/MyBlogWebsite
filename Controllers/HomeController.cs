using Blog3.Data;
using Blog3.Data.FileManager;
using Blog3.Data.Repository;
using Blog3.Models;
using Blog3.Models.Comments;
using Blog3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.Controllers
{
    //name of the controller is the name of the folder in the views folder
    //name of the method is the name of the html page
    public class HomeController: Controller
    {
        private IRepository _repository;
        private IFileManager _fileManager;

        public HomeController(IRepository repository, IFileManager fileManager)
        {
            _repository = repository;
            _fileManager = fileManager;
           
        }

       /* public IActionResult Index(string category )=>
            View(String.IsNullOrEmpty(category) ? 
                _repository.GetAllPosts() :
                _repository.GetAllPosts(category));
       */  
       //now we are also taking page number
        public IActionResult Index(int pageNumber, string category, string search)
        {
            //we can't have less than page 1, redirect it here again with page number 1
            if(pageNumber < 1)
            {
                return RedirectToAction("Index", new { pageNumber = 1, category });
            }

           
            //we are no longer passing an IEnumerable of Posts to the Index view like before
            // we are passing an Index View Model which contains an IEnumerable of Posts so let us create an index view model
            
            IndexViewModel vm = new IndexViewModel();
            vm = _repository.GetAllPosts(pageNumber, category, search);
            
            return View(vm);

        }

        public IActionResult Post(int id)
        {

            var post = _repository.GetPost(id);
            post.LikedByUser = _repository.LikedByCurrentUser(id, User.Identity.Name);
            post.DislikedByUser = _repository.DislikedByCurrentUser(id, User.Identity.Name);
            _repository.UpdatePost(post);
            _repository.SaveChangesAsync();
            return View(post);
        }

        public IActionResult IncrementPostView(int id, string username)
        {
            var new_post = _repository.IncreasePostViews(id,username);
            return RedirectToAction("Post","Home", new { id = new_post.Id});
        }
       
        public IActionResult Like(int id, string username)
        {
            
            var new_post = _repository.LikePost(id,username);
            
            return RedirectToAction("Post", "Home", new { id = new_post.Id });
            
        }
        public IActionResult Dislike(int id, string username)
        {
            var new_post = _repository.DislikePost(id,username);
            return RedirectToAction("Post", "Home", new { id = new_post.Id });
        }

        public IActionResult RemoveMainComment(int postid, int mainid)
        {
            var new_post = _repository.DeleteMainComment(postid, mainid);
            return RedirectToAction("Post", "Home", new { id = new_post.Id });
        }

        public IActionResult RemoveSubComment(int postid, int subid, int mainid)
        {
            var new_post = _repository.DeleteSubComment(postid, subid, mainid);
            return RedirectToAction("Post", "Home", new { id = new_post.Id });
        }

        /*public IActionResult Post(int id) => View(_repository.GetPost(id));*/


        /* [HttpGet("/Image/{image}")]
         public IActionResult Image(string image)
         {
             var mine = image.Substring(image.LastIndexOf('.')+1);

             return new FileStreamResult(_fileManager.ImageStream(image),$"image/{mine}");
         }
        */
        [HttpGet("/Image/{image}")]
        public IActionResult Image(string image) => new FileStreamResult(_fileManager.ImageStream(image), $"image/{image.Substring(image.LastIndexOf('.') + 1)}");
        

        [HttpPost]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid)
            {
               // return Post(vm.PostId); //if the state is not valid we just want to return the original post
                return RedirectToAction("Post", new { id = vm.PostId });
            }
            var post = _repository.GetPost(vm.PostId);

            //now each post can have many MAIN comments, each Main comment can have Many SUB comments
            //each maincomment has a post ID which is the post they are tied to
            // each subcomment has a maincomment ID which is the main comment they are tied to
            //we want to know if the comment is a maincomment or a subcomment
            //if it has a main comment id value greater than 0 then it is a sub comment from our understanding

            if (vm.MainCommentId == 0)
            {
                //if it is a maincomment
                //check if the maincomments list of the post is null.
                //if it is create a new list of maincomment for the post
                //add the new main comment to the list of main comments for the post
               
                //post.MainComments = post.MainComments ?? new List<MainComment>();
                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                    BlogUsername = User.Identity.Name
                }) ;
                _repository.UpdatePost(post);
            }
            else
            {
                var comment = new SubComment
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                    BlogUsername = User.Identity.Name
                };
                _repository.AddSubComment(comment);
            }
            await _repository.SaveChangesAsync();
            return RedirectToAction("Post", new { id = vm.PostId });
        }
    }
}
