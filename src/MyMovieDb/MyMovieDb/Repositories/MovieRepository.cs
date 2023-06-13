using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MyMovieDb.Contracts.Repositories;
using MyMovieDb.Models;
using TMDbLib.Client;

namespace MyMovieDb.Repositories
{
	public class MovieRepository : IMovieRepository
	{
		TMDbClient _client = new TMDbClient("2e4b42db41639f614d7c69a0e5ff2473");

		public async Task<List<Genre>> GetGenres()
		{
			var genres = await _client.GetMovieGenresAsync();
			var result = Mapper.Map<List<TMDbLib.Objects.General.Genre>, List<Models.Genre>>(genres);
			return result;
		}

		public async Task<List<TMDbLib.Objects.Search.SearchMovie>> GetMoviesByGenre(int id, int page)
        {
			var movies = await _client.GetGenreMoviesAsync(id);
			var result = movies.Results.Take(10).Skip(page - 1).ToList();
			return result;
        }

		public async Task<TMDbLib.Objects.Movies.Movie> GetMovie(int id)
        {
			var movie = await _client.GetMovieAsync(id);
			return movie;
        }

	}
}