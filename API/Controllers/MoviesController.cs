using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Models;
using API.Repositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRatingRepository _ratingRepository;

        public MoviesController(IMovieRepository movieRepository, IUserRepository userRepository, IRatingRepository ratingRepository)   //ApiContext context, 
        {
            //_context = context;
            _movieRepository = movieRepository;
            _userRepository = userRepository;
            _ratingRepository = ratingRepository;
        }

        [HttpGet("GetMovies", Name = nameof(GetMovies))]
        public async Task<IActionResult> GetMovies([FromQuery]string title, [FromQuery]string partialTitle, [FromQuery]string releaseYear, [FromQuery]string genres)
        {
            try
            {
                if (String.IsNullOrEmpty(String.Concat(title, partialTitle, releaseYear, genres)))
                {
                    return NotFound(); //Return 404 - Not Found.
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
                        return BadRequest(); //Return 400 - Bad Request.
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(); //Return 400 - Bad Request.
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
                        return BadRequest(); //Return 400 - Bad Request.
                    }
                }
                else
                {
                    //get top 5 ratings for giving user
                    var ratings = await _ratingRepository.GetTop5RatingByUserName(userName);

                    var returnDtos = new List<MovieDto>();
                    if (ratings != null && ratings.Count() > 0)
                    {
                        foreach (Rating rating in ratings)
                        {
                            var movie = rating.Movie;

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
                                    averageRating = GetMovieAvgRating(movie.Ratings)
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

        //[HttpPost("AddRating", Name = nameof(AddRating))]
        [HttpGet("AddRating/{username}/{movietitle}/{rating}", Name = nameof(AddRating))]  //actually it's better to use HttpPost here, since when testing we call this api from url, I keep HttpGet for now.
        public async Task<IActionResult> AddRating(string userName, string movieTitle, string rating)
        {
            try
            {
                if (String.IsNullOrEmpty(userName) || String.IsNullOrEmpty(movieTitle) || String.IsNullOrEmpty(rating))
                {
                    return NotFound(); //Returns 404 - Not Found.
                }
                else
                {
                    var user = await _userRepository.GetSingle(userName);
                    var movie = await _movieRepository.GetSingleByTitle(movieTitle);

                    if (user == null || movie == null)
                    {
                        return NotFound(); //Returns 404 - Not Found.
                    }

                    if (int.TryParse(rating, out var parsedRating) && parsedRating >= 1 && parsedRating <= 5)
                    {
                        var ratingNew = await _ratingRepository.AddUpdateRating(user, movie, parsedRating); 

                        var ratingDto = new RatingDto()
                        {
                            Id = ratingNew.Id,
                            UserId = user.Id,
                            UserName = userName,
                            MovieId = movie.Id,
                            MovieTitle = movieTitle,
                            RatingScore = parsedRating
                        };

                        return Ok(ratingDto);
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
            var movies = await _movieRepository.GetAll();
            if (movies != null && movies.Count() > 0)
            {
                movieDtos = movies.Select(m => new MovieDto
                {
                    id = m.Id,
                    title = m.Title,
                    partialTitle = m.PartialTitle,
                    yearOfRelease = m.YearOfRelease,
                    runningTime = m.RunningTime.ToString(),
                    genres = m.Genres,
                    averageRating = GetMovieAvgRating(m.Ratings)
                    //userRating = null,
                }).OrderBy(m => m.title).ToList();
            }
            return movieDtos;
        }

        private double GetMovieAvgRating(List<Rating> ratings)
        {
            return Math.Round(ratings.Select(x => x.RatingScore).DefaultIfEmpty(0).Average() * 2, MidpointRounding.AwayFromZero) / 2;
        }
    }
}