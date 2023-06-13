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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace MyMovieDb.ViewModels
{
    public class MoviePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;
        private readonly IMovieRepository _movieRepo;
        public event PropertyChangedEventHandler PropertyChanged;

        public MoviePageViewModel(INavigationService navigationService, IMovieRepository movieRepo)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _movieRepo = movieRepo;
            Title = "Movie Details";
            LoadMovieInfoCommand.Execute();

            // add endless scroll
            OnPropertyChanged("Reviews");
            int page = 2;
            LoadMore = new Command(() => {
                var moreList = GetReviews(Movie, 2);
                page += 1;
                foreach (var item in moreList)
                {
                    Reviews.Add(item);
                    OnPropertyChanged("Reviews");
                }
            });
        }

        private int _movieId;
        public int MovieId
        {
            get { return _movieId; }
            set { SetProperty(ref _movieId, value); }
        }

        private TMDbLib.Objects.Movies.Movie _movie;
        public TMDbLib.Objects.Movies.Movie Movie
        {
            get { return _movie; }
            set { SetProperty(ref _movie, value); }
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
        public ICommand LoadMore { get; set; }

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
            Movie = await _movieRepo.GetMovie(MovieId);
            VideoLinks = Movie.Videos.Results.ToObservableCollection();
            Reviews = GetReviews(Movie, 1);
            IsBusy = false;
        }

        // endless scroll review init
        public ObservableCollection<TMDbLib.Objects.Reviews.ReviewBase> GetReviews(TMDbLib.Objects.Movies.Movie movie, int page)
        {
            return movie.Reviews.Results.ToObservableCollection().Take(10).Skip(page - 1).ToObservableCollection();
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
