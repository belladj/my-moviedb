using System.Collections.Generic;
using System.Threading.Tasks;
using MyMovieDb.Models;

namespace MyMovieDb.Contracts.Repositories
{
	public interface IMovieRepository
	{
		Task<List<Genre>> GetGenres();
		Task<List<TMDbLib.Objects.Search.SearchMovie>> GetMoviesByGenre(int id, int page);
		Task<TMDbLib.Objects.Movies.Movie> GetMovie(int id);
	}
}