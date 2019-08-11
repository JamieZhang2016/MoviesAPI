using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PartialTitle { get; set; }
        public string ReleaseYear { get; set; }
        public List<string> Genres { get; set; }
    }
}
