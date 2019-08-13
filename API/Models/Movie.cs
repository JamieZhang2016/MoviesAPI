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
        public string YearOfRelease { get; set; }
        public TimeSpan RunningTime { get; set; }
        public string Genres { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}
