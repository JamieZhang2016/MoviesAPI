using API.Dto;
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
            return await _context.Ratings.Where(r => r.MovieId == movieId).ToListAsync();
        }

        public async Task<IEnumerable<Rating>> GetTop5RatingByUserName(string userName)
        {
            return await _context.Ratings.Include(ra => ra.User).Include(s => s.Movie).Where(r => r.User.UserName == userName).OrderByDescending(m => m.RatingScore).Take(5).ToArrayAsync();
        }

        public async Task<Rating> GetSingleByUserMovie(int userId, int movieId)
        {
            return await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.MovieId == movieId);
        }

        public async Task<Rating> AddUpdateRating(User user, Movie movie, int rateValue)
        {
            var ratingDto = new RatingDto();
            var rateExists = await _context.Ratings.Include(r => r.User).FirstOrDefaultAsync(m => m.UserId == user.Id && m.MovieId == movie.Id);

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
        }
    }
}
