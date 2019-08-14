using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dto
{
    public class RatingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public int RatingScore { get; set; }
    }
}
