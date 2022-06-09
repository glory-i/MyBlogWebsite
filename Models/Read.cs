using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog3.Models
{
    public class Read
    {

        public int Id { get; set; }
        public int PostId { get; set; }

        public string Username { get; set; }
        public string Familytype { get; set; }
    }
}
