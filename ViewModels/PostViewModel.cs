using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.ViewModels
{
    public class PostViewModel
    {
        //just like we used to use dto, it is the postview model that will show on the front end
        //not the whole post that's for the database
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string CurrentImage { get; set; } = "";


        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";
        public string Category { get; set; } = "";
        public IFormFile Image { get; set; } = null;
    }
}
