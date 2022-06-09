using Blog3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.ViewModels
{
    public class IndexViewModel
    {
        public int PageNumber { get; set; }
        
        public int PageCount { get; set; }
        public bool NextPage { get; set; }
        public string Category { get; set; }
        public string Search { get; set; }
        public IEnumerable<Post> Posts { get; set; }
        public IEnumerable<Post> PopularPosts { get; set; }


    }
}
