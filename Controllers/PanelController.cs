using Blog3.Data.FileManager;
using Blog3.Data.Repository;
using Blog3.Models;
using Blog3.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.Controllers
{
    [Authorize(Roles ="Admin")]
    public class PanelController : Controller
    {
        private IRepository _repository;
        private IFileManager _fileManager;

        public PanelController(IRepository repository, IFileManager fileManager)
        {
            _repository = repository;
            _fileManager = fileManager;
        }

        [HttpGet]
        public async Task<IActionResult> Remove(int id)
        {
            _repository.RemovePost(id);
            await _repository.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return View(new PostViewModel());
            }
            else
            {
                var post = _repository.GetPost((int)id);
                return View(new PostViewModel
                {
                    Title = post.Title,
                    Id = post.Id,
                    Body = post.Body,
                    CurrentImage = post.Image,
                    Description = post.Description,
                    Category= post.Category,
                    Tags=post.Tags

                }) ; 
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostViewModel vm)
        {
            var post = new Post
            {
                Body = vm.Body,
                Title = vm.Title,
                Id = vm.Id,
                Description = vm.Description,
                Category = vm.Category,
                Tags = vm.Tags
                //Image = ""//Handle image

            };

            if(vm.Image == null)
            {
                post.Image = vm.CurrentImage;
            }
            else
            {
                post.Image = await _fileManager.SaveImage(vm.Image);
            }



            //now in our edit.cshtml it is either we have empty input values(in which case that post does not exist)
            //or we will have input values of the post we want to edit
            //in case one that id is 0 and we want to add the post by filling in the input values and clicking submit
            //in case 2 we cant to update the already exisiting post by modifying the input values which would have the property values of the currently existing post

            if (post.Id > 0)
            {
                _repository.UpdatePost(post);
            }
            else
            {
                _repository.AddPost(post);
            }
            //_repository.AddPost(post);

            if (await _repository.SaveChangesAsync())
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(post);
            }
        }

        


        public IActionResult Index()
        {
            var posts = _repository.GetAllPosts();
            return View(posts);
        }

    }
}
