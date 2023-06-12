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


namespace MyMovieDb.ViewModels
{
    public class MoviePageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IMovieRepository _movieRepo;
        public MoviePageViewModel(INavigationService navigationService, IMovieRepository movieRepo)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _movieRepo = movieRepo;
            Title = "Movie Details";
            LoadMovieInfoCommand.Execute();
        }

        private int _movieId;
        public int MovieId
        {
            get { return _movieId; }
            set { SetProperty(ref _movieId, value); }
        }

        private ObservableCollection<TMDbLib.Objects.General.Video> _videoLinks;
        public ObservableCollection<TMDbLib.Objects.General.Video> VideoLinks
        {
            get { return _videoLinks; }
            set { SetProperty(ref _videoLinks, value); }
        }

        private ObservableCollection<TMDbLib.Objects.Reviews.ReviewBase> _reviews;
        public ObservableCollection<TMDbLib.Objects.Reviews.ReviewBase> Reviews
        {
            get { return _reviews; }
            set { SetProperty(ref _reviews, value); }
        }

        // get id from navigation
        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            MovieId = parameters.GetValue<int>("movieId");
        }

        // get movie info by movie id
        private DelegateCommand _loadMovieInfoCommand;
        public DelegateCommand LoadMovieInfoCommand => _loadMovieInfoCommand ?? (_loadMovieInfoCommand = new DelegateCommand(LoadMovieInfo));

        async void LoadMovieInfo()
        {
            IsBusy = true;
            var movie = await _movieRepo.GetMovie(MovieId);
            VideoLinks = movie.Videos.Results.ToObservableCollection();
            Reviews = movie.Reviews.Results.ToObservableCollection();
            IsBusy = false;
        }

    }
}
