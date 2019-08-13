using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApiContext _context;

        public RatingRepository(ApiContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetAll()
        {
            return await _context.Ratings.ToArrayAsync();
        }

        public async Task<IEnumerable<Rating>> GetRatingsByMovie(int movieId)
        {
            return await _context.Ratings.Where(r => r.MovieId == movieId).ToArrayAsync();
        }

        public async Task<IEnumerable<Rating>> GetRatingByUser(int userId = 0, string userName = "")
        {
            if (userId != 0)
            {
                return await _context.Ratings.Where(r => r.UserId == userId).ToArrayAsync();
            }
            else if (userName != "")
            {
                return await _context.Ratings.Where(r => r.User.UserName == userName).ToArrayAsync();
            }

            return await _context.Ratings.Where(r => r.UserId == userId).ToArrayAsync();
        }

        public async Task<Rating> GetSingleByUserMovie(int userId, int movieId)
        {
            return await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
        }

        public async Task<Rating> AddUpdateRating(User user, Movie movie, int rateValue)
        {
            var rateExists = await _context.Ratings.FirstOrDefaultAsync(m => m.UserId == user.Id && m.MovieId == movie.Id);

            if (rateExists == null)
            {
                var rate = new Rating()
                {
                    UserId = user.Id,
                    User = user,
                    MovieId = movie.Id,
                    Movie = movie,
                    RatingScore = rateValue
                };
                _context.Ratings.Add(rate);
            }
            else
            {
                rateExists.RatingScore = rateValue;
            }

            await _context.SaveChangesAsync();
            return rateExists;
            //return _context.Ratings.Where(r => r.UserId == user.Id).ToArrayAsync();  // return all ratings of the giving user for verification
        }
    }
}
