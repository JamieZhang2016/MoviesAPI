using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApiContext _context;

        public MoviesController(ApiContext context)
        {
            _context = context;
        }

        [HttpGet("GetMovies", Name = nameof(GetMovies))]
        public async Task<IActionResult> GetMovies([FromQuery]string title, [FromQuery]string partialTitle, [FromQuery]string releaseYear, [FromQuery]string genres)
        {
            try
            {
                if (String.IsNullOrEmpty(String.Concat(title, partialTitle, releaseYear, genres)))
                //&& String.IsNullOrEmpty(partialTitle) && String.IsNullOrEmpty(releaseYear) && String.IsNullOrEmpty(genres))
                {
                    return NotFound(); //Returns 404 - Not Found.
                }
                else
                {
                    var movieDtos = new List<MovieDto>();
                    movieDtos = await GetMoviesDto();

                    movieDtos = !String.IsNullOrEmpty(title) ? movieDtos.Where(r => r.title == title).ToList() : movieDtos;
                    movieDtos = !String.IsNullOrEmpty(partialTitle) ? movieDtos.Where(r => r.partialTitle == partialTitle).ToList() : movieDtos;
                    movieDtos = !String.IsNullOrEmpty(releaseYear) ? movieDtos.Where(r => r.yearOfRelease == releaseYear).ToList() : movieDtos;
                    movieDtos = !String.IsNullOrEmpty(genres) ? movieDtos.Where(r => r.genres == genres).ToList() : movieDtos;

                    if (movieDtos != null && movieDtos.Count() > 0)
                    {
                        return Ok(movieDtos); //Return Object - 200 OK
                    }
                    else
                    {
                        return BadRequest(); //Returns 400 - Bad Request.
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(); //Returns 400 - Bad Request.
            }

        }

        [HttpGet("Top5Ratings", Name = nameof(Top5Ratings))]
        public async Task<IActionResult> Top5Ratings([FromQuery]string userName)
        {
            try
            {
                if (String.IsNullOrEmpty(userName))
                {
                    var movieDtos = new List<MovieDto>();
                    movieDtos = await GetMoviesDto();
                    if (movieDtos != null && movieDtos.Count() > 0)
                    {
                        return Ok(movieDtos.OrderByDescending(m => m.averageRating).ThenBy(n => n.title).Take(5));
                    }
                    else
                    {
                        return BadRequest(); //Returns 400 - Bad Request.
                    }
                }
                else
                {
                    //get top 5 ratings for giving user
                    var ratings = await _context.Ratings.Where(r => r.User.UserName == userName)
                        .OrderByDescending(m => m.RatingScore).Take(5).ToArrayAsync();

                    var returnDtos = new List<MovieDto>();
                    if (ratings != null && ratings.Count() > 0)
                    {
                        foreach (Rating rating in ratings)
                        {
                            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == rating.MovieId);
                            if (movie != null)
                            {
                                returnDtos.Add(new MovieDto()
                                {
                                    id = movie.Id,
                                    title = movie.Title,
                                    partialTitle = movie.PartialTitle,
                                    yearOfRelease = movie.YearOfRelease,
                                    runningTime = movie.RunningTime.ToString(),
                                    genres = movie.Genres,
                                    userRating = rating.RatingScore,
                                    averageRating = GetMovieAvgRating(movie.Id)
                                });
                            }
                        }
                    }
                    return Ok(returnDtos.OrderByDescending(r=>r.userRating).ThenBy(s=>s.title));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(); //Returns 400 - Bad Request.
            }
        }

        [HttpGet("AddRating", Name = nameof(AddRating))]
        public async Task<IActionResult> AddRating([FromQuery]string userName, [FromQuery]string movieTitle, [FromQuery]string rating)
        {
            try
            {
                if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(movieTitle) || String.IsNullOrEmpty(rating))
                {
                    return NotFound(); //Returns 404 - Not Found.
                }
                else
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                    var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Title == movieTitle);

                    if (user == null || movie == null)
                    {
                        return NotFound(); //Returns 404 - Not Found.
                    }

                    if (int.TryParse(rating, out var parsedRating) && parsedRating >= 1 && parsedRating <= 5)
                    {
                        var rateExists = await _context.Ratings.FirstOrDefaultAsync(m => m.UserId == user.Id && m.MovieId == movie.Id);

                        if (rateExists == null)
                        {
                            var rate = new Rating()
                            {
                                UserId = user.Id,
                                User = user,
                                MovieId = movie.Id,
                                RatingScore = parsedRating
                            };
                            _context.Ratings.Add(rate);
                        }
                        else
                        {
                            rateExists.RatingScore = parsedRating;
                        }
                        
                        await _context.SaveChangesAsync();

                        return Ok(_context.Ratings.Where(r=>r.UserId == user.Id).ToArrayAsync());  // return all ratings of the giving user for verification
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(); //Returns 400 - Bad Request.
            }
        }

        private async Task<List<MovieDto>> GetMoviesDto()
        {
            var movieDtos = new List<MovieDto>();
            var movies = await _context.Movies.ToListAsync();
            if (movies != null && movies.Count() > 0)
            {
                movieDtos = ConvertMovieDto(movies);
            }
            return movieDtos;
        }

        private List<MovieDto> ConvertMovieDto(List<Movie> movies)
        {
            var movieDtos = new List<MovieDto>();
            movieDtos = movies.Select(m => new MovieDto
            {
                id = m.Id,
                title = m.Title,
                partialTitle = m.PartialTitle,
                yearOfRelease = m.YearOfRelease,
                runningTime = m.RunningTime.ToString(),
                genres = m.Genres,
                averageRating = GetMovieAvgRating(m.Id),
                userRating = null,
            }).OrderBy(m => m.title).ToList();

            return movieDtos;
        }

        private double GetMovieAvgRating(int movieId)
        {
            var ratings = _context.Ratings
                .Where(r => r.MovieId == movieId)
                .ToArray();

            return Math.Round(ratings.Select(x => x.RatingScore).DefaultIfEmpty(0).Average() * 2, MidpointRounding.AwayFromZero) / 2;
        }
    }
}