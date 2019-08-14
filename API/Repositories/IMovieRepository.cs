using API.Dto;
using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Repositories
{
    public interface IMovieRepository
    {
        Task<List<Movie>> GetAll();
        Task<Movie> GetSingleById(int id);
        Task<Movie> GetSingleByTitle(string title);
    }
}
