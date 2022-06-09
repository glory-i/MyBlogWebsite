﻿using Blog3.Models.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string Image { get; set; } = "";

        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";
        public string Category { get; set; } = "";

        public DateTime Created { get; set; } = DateTime.Now;
        //public string Title { get; set; } = "";

        public List<MainComment> MainComments { get; set; } = new List<MainComment>();

        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int NoOfViews { get; set; }

        public bool LikedByUser { get; set; } = false;
        public bool DislikedByUser { get; set; } = false;

        public bool ReadByUser { get; set; } = false;


    }
}