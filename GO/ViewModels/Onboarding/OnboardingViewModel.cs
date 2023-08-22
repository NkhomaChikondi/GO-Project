using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace GO.ViewModels.Onboarding
{
    public class OnboardingViewModel : MvvmHelpers.BaseViewModel
    {
        private ObservableCollection<Models.Onboarding> items;
        private int position;
        private string nextButtonText;
        private string skipButtonText;

        public OnboardingViewModel()
        {
            SetNextButtonText("NEXT");
            SetSkipButtonText("SKIP");
            OnBoarding();
            LaunchNextCommand();
            //LaunchSkipCommand();
        }
        private void SetNextButtonText(string nextButtonText) => NextButtonText = nextButtonText;
        private void SetSkipButtonText(string skipButtonText) => SkipButtonText = skipButtonText;

        private void OnBoarding()
        {
            Items = new ObservableCollection<Models.Onboarding>
            {
                new Models.Onboarding
                {
                    Title = "Welcome",
                    Content = "Welcome to Weather. Let's start.",
                    ImageUrl = "welcome.svg"
                },
                new  Models.Onboarding
                {
                    Title = "Latest Data",
                    Content = "Weather shows you the data from \n the API.",
                    ImageUrl = "show.svg"
                },
                new Models.Onboarding
                {
                    Title = "Instant notifications",
                    Content = "Get notified according to the changes \n you want.",
                    ImageUrl = "notification.svg"
                }
            };
        }
        private void LaunchNextCommand()
        {

            NextCommand = new Command(() =>
            {
                if (LastPositionReached())
                {
                    //ExitOnBoarding();
                }
                else
                {
                    MoveToNextPosition();
                }
            });
        }
        //private void LaunchSkipCommand()
        //{
        //    SkipCommand = new Command(() =>
        //    {
        //        ExitOnBoarding();

        //    });
        //}

       // private static void ExitOnBoarding()
          //  => App.Current.MainPage.Navigation.PushModalAsync();

        private void MoveToNextPosition()
        {
            var nextPosition = ++Position;
            Position = nextPosition;
        }

        private bool LastPositionReached()
            => Position == Items.Count - 1;

        public ObservableCollection<Models.Onboarding> Items
        {
            get => items;
            set => SetProperty(ref items, value);
        }

        public string NextButtonText
        {
            get => nextButtonText;
            set => SetProperty(ref nextButtonText, value);
        }
        public string SkipButtonText
        {
            get => skipButtonText;
            set => SetProperty(ref skipButtonText, value);
        }

        public int Position
        {
            get => position;
            set
            {
                if (SetProperty(ref position, value))
                {
                    UpdateNextButtonText();
                }
            }
        }

        private void UpdateNextButtonText()
        {
            if (LastPositionReached())
            {
                SetNextButtonText("GOT IT");
            }
            else
            {
                SetNextButtonText("NEXT");
            }
        }

        public ICommand NextCommand { get; private set; }
        public ICommand SkipCommand { get; private set; }

    }
}
