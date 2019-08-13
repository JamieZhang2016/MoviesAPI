using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetAll();
        Task<IEnumerable<Rating>> GetRatingsByMovie(int movieId);
        Task<IEnumerable<Rating>> GetRatingByUser(int userId = 0, string userName = "");
        Task<Rating> GetSingleByUserMovie(int userId, int movieId);
        Task<Rating> AddUpdateRating(User user, Movie movie, int rateValue);  // return all ratings of the giving user for verification
    }
}
