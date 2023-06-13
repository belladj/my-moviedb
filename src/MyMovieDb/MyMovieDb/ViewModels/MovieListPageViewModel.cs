using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MyMovieDb.Contracts.Repositories;
using MyMovieDb.Extensions;
using MyMovieDb.Models;
using System.Windows.Input;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace MyMovieDb.ViewModels
{
    public class MovieListPageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private readonly IMovieRepository _movieRepo;
        public event PropertyChangedEventHandler PropertyChanged;

        public MovieListPageViewModel(INavigationService navigationService, IMovieRepository movieRepo)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _movieRepo = movieRepo;
            Title = Genre.Name;
            LoadGenreMoviesCommand.Execute();
            SelectMovieCommand = new DelegateCommand<TMDbLib.Objects.Search.SearchMovie>(LoadMovieDetails);

            // add endless scroll
            OnPropertyChanged("GenreMovies");
            int page = 2;
            this.LoadMore = new Command(async () => {
                var moreList = await _movieRepo.GetMoviesByGenre(Genre.Id, 2);
                page += 1;
                foreach (var item in moreList)
                {
                    GenreMovies.Add(item);
                    OnPropertyChanged("GenreMovies");
                }
            });
        }

        private Genre _genre;
        public Genre Genre
        {
            get { return _genre; }
            set { SetProperty(ref _genre, value); }
        }
        private ObservableCollection<TMDbLib.Objects.Search.SearchMovie> _genremovies;
        public ObservableCollection<TMDbLib.Objects.Search.SearchMovie> GenreMovies
        {
            get { return _genremovies; }
            set { SetProperty(ref _genremovies, value); }
        }

        public ICommand LoadMore {get; set; }

        // get data upon navigation
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            Genre = parameters.GetValue<Genre>("genre");
        }

        // get movie list by genre
        private DelegateCommand _loadGenreMoviesCommand;
        public DelegateCommand LoadGenreMoviesCommand => _loadGenreMoviesCommand ?? (_loadGenreMoviesCommand = new DelegateCommand(LoadMoviesByGenre));

        async void LoadMoviesByGenre()
        {
            IsBusy = true;
            var movieList = await _movieRepo.GetMoviesByGenre(Genre.Id,1);
            GenreMovies = movieList.ToObservableCollection();
            IsBusy = false;
        }

        async void LoadMoreMovies()
        {
            IsBusy = true;
            var movieList = await _movieRepo.GetMoviesByGenre(Genre.Id, 1);
            GenreMovies = movieList.ToObservableCollection();
            IsBusy = false;
        }

        //get movie details
        public DelegateCommand<TMDbLib.Objects.Search.SearchMovie> SelectMovieCommand { get; private set; }
        private async void LoadMovieDetails(TMDbLib.Objects.Search.SearchMovie movie)
        {
            var navigationParams = new NavigationParameters
                {
                    {"movieId", movie.Id }
                };
            await _navigationService.NavigateAsync("MoviePage", navigationParams);
         }


        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
