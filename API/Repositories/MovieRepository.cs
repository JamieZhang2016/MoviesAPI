using API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApiContext _context;

        public MovieRepository(ApiContext context)
        {
            _context = context;
        }

        public async Task<List<Movie>> GetAll()
        {
            return await _context.Movies.Include(r => r.Ratings).ToListAsync();
        }

        public async Task<Movie> GetSingleById(int id)
        {
            return await _context.Movies.Include(r=>r.Ratings).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Movie> GetSingleByTitle(string title)
        {
            return await _context.Movies.FirstOrDefaultAsync(m => m.Title == title);
        }
    }
}
