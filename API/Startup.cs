using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApiContext>(context => context.UseInMemoryDatabase("TestDatabase"));
            //services.AddDbContext<ApiContext>(context => InMemoryContext());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        private ApiContext InMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;
            var context = new ApiContext(options);

            return context;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            var context = serviceProvider.GetService<ApiContext>();
            AddTestData(context);

            // Web API routes
            app.UseMvcWithDefaultRoute();

            app.UseMvc();
        }

        private static void AddTestData(ApiContext context)
        {
            var testUser1 = new Models.User
            {
                Id = 1,
                UserName = "abc123",
            };
            var testUser2 = new Models.User
            {
                Id = 2,
                UserName = "cfd123",
            };
            context.Users.AddRange(new List<User> { testUser1, testUser2});

            var testMovie1 = new Models.Movie
            {
                Id = 1,
                Title = "Movie001",
                PartialTitle = "001",
                YearOfRelease = "2012",
                RunningTime = new TimeSpan(2, 14, 18),
                Genres = "Science Fiction, Romantic"
            };
            var testMovie2 = new Models.Movie
            {
                Id = 2,
                Title = "Movie002",
                PartialTitle = "002",
                YearOfRelease = "2019",
                RunningTime = new TimeSpan(1, 50, 55),
                Genres = "History"
            };
            var testMovie3 = new Models.Movie
            {
                Id = 3,
                Title = "Movie003",
                PartialTitle = "003",
                YearOfRelease = "2014",
                RunningTime = new TimeSpan(2, 50, 55),
                Genres = "History,Science Fiction"
            };
            var testMovie4 = new Models.Movie
            {
                Id = 4,
                Title = "Movie004",
                PartialTitle = "004",
                YearOfRelease = "2019",
                RunningTime = new TimeSpan(1, 33, 55),
                Genres = "Romantic"
            };
            var testMovie5 = new Models.Movie
            {
                Id = 5,
                Title = "Movie005",
                PartialTitle = "005",
                YearOfRelease = "2011",
                RunningTime = new TimeSpan(1, 12, 32),
                Genres = "Funny"
            };
            var testMovie6 = new Models.Movie
            {
                Id = 6,
                Title = "Movie006",
                PartialTitle = "006",
                YearOfRelease = "2001",
                RunningTime = new TimeSpan(3, 12, 55),
                Genres = "Romantic"
            };
            context.Movies.AddRange(new List<Movie>{ testMovie1, testMovie2, testMovie3, testMovie4, testMovie5, testMovie6});

            var testRating1 = new Models.Rating
            {
                Id = 1,
                UserId = testUser1.Id,
                MovieId = testMovie1.Id,
                RatingScore = 5
            };
            var testRating2 = new Models.Rating
            {
                Id = 2,
                UserId = testUser1.Id,
                MovieId = testMovie1.Id,
                RatingScore = 3
            };
            var testRating3 = new Models.Rating
            {
                Id = 3,
                UserId = testUser1.Id,
                MovieId = testMovie3.Id,
                RatingScore = 4
            };
            var testRating4 = new Models.Rating
            {
                Id = 4,
                UserId = testUser1.Id,
                MovieId = testMovie4.Id,
                RatingScore = 2
            };
            var testRating5 = new Models.Rating
            {
                Id = 5,
                UserId = testUser1.Id,
                MovieId = testMovie5.Id,
                RatingScore = 5
            };
            var testRating6 = new Models.Rating
            {
                Id = 6,
                UserId = testUser1.Id,
                MovieId = testMovie6.Id,
                RatingScore = 1
            };

            var testRating7 = new Models.Rating
            {
                Id = 7,
                UserId = testUser2.Id,
                MovieId = testMovie1.Id,
                RatingScore = 3
            };
            var testRating8 = new Models.Rating
            {
                Id = 8,
                UserId = testUser2.Id,
                MovieId = testMovie3.Id,
                RatingScore = 3
            };
            var testRating9 = new Models.Rating
            {
                Id = 9,
                UserId = testUser2.Id,
                MovieId = testMovie4.Id,
                RatingScore = 4
            };

            context.Ratings.AddRange(new List<Rating> { testRating1, testRating2, testRating3,
                testRating4, testRating5, testRating6, testRating7, testRating8, testRating9 });

            context.SaveChanges();
        }
    }
}
