﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.Models.Comments
{
    public class SubComment//: Comment
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime Created { get; set; }
        public int MainCommentId { get; set; }

       // public string Username { get; set; }
        public string BlogUsername { get; set; }

    }
}
