using System.Collections.Generic;
using System.Threading.Tasks;
using MyMovieDb.Models;

namespace MyMovieDb.Contracts.Services
{
	public interface IMovieService
	{
		Task<List<Genre>> GetGenres();
		Task<List<TMDbLib.Objects.Search.SearchMovie>> GetMoviesByGenre(int id);
		Task<TMDbLib.Objects.Movies.Movie> GetMovie(int id);
	}
}