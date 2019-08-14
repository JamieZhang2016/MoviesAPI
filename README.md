# MoviesAPI
Allows movies to be searched and ranked by users

Framework:
.NET Core 2.1

Test Data:
Test data create by using Entity Framework Core UseInMemoryDatabase

API request url examples: (A,C use queryString to pass parameters)

API A: https://localhost:44300/api/movies?title=Movie001&releaseYear=2012

API B: https://localhost:44300/api/movies/Top5Ratings

API C: https://localhost:44300/api/movies/Top5Ratings/?userName=abc123

API D: https://localhost:44300/api/movies/AddRating/abc123/Movie001/3
