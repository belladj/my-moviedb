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
using Xamarin.Forms;

namespace MyMovieDb.ViewModels
{
    public class GenrePageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IMovieRepository _movieRepo;

        public GenrePageViewModel(INavigationService navigationService, IMovieRepository movieRepo)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _movieRepo = movieRepo;
            Title = "Genres";
            LoadGenresCommand.Execute();
            SelectGenreCommand = new DelegateCommand<Genre>(LoadGenreMovies);
        }

        private ObservableCollection<Genre> _genres;
        public ObservableCollection<Genre> Genres
        {
            get { return _genres; }
            set { SetProperty(ref _genres, value); }
        }

        // get genres
        private DelegateCommand _loadGenresCommand;
        public DelegateCommand LoadGenresCommand => _loadGenresCommand ?? (_loadGenresCommand = new DelegateCommand(LoadGenres));

        async void LoadGenres()
        {
            IsBusy = true;
            var genreData = await _movieRepo.GetGenres();
            Genres = genreData.ToObservableCollection();
            IsBusy = false;
        }

        // get movies by genre
        public DelegateCommand<Genre> SelectGenreCommand { get; private set; }

        private async void LoadGenreMovies(Genre genre)
        {
            var navigationParams = new NavigationParameters
                {
                    {"genre", genre }
                };
            await _navigationService.NavigateAsync("MovieListPage", navigationParams);
        }
    }
}
