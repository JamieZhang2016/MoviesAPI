using System;

namespace API.Dto
{
    public class MovieDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public string partialTitle { get; set; }
        public string yearOfRelease { get; set; }
        public string runningTime { get; set; }
        public string genres { get; set; }
        public double averageRating { get; set; }
        public double? userRating { get; set; }
    }
}
