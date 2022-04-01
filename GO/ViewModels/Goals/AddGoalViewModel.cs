using GO.Models;
using GO.Services;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.Goals
{
    [QueryProperty(nameof(GoalId), nameof(GoalId))]

    public class AddGoalViewModel : BaseViewmodel, INotifyPropertyChanged
    {
        private string name;
        private DateTime start = DateTime.Today;
        private DateTime end = DateTime.Today;
        private int goalId;
        private string description;
        private DateTime time = DateTime.Now;
        private double percentage;
        private int notificationNumber = 0;
        INotificationManager notificationManager;

        public AsyncCommand AddGoalCommand { get; set; }
        public string Name { get => name; set => name = value; }
        public DateTime End { get => end; set => end = value; }
        public DateTime Start { get => start; set => start = value; }
        public string Description { get => description; set => description = value; }
        public int GoalId { get => goalId; set => goalId = value; }
        public DateTime Time { get => time; set => time = value; }

        public SelectedItemWrapper<DOW> selectedItem { get; set; }
        public ObservableRangeCollection<SelectedItemWrapper<DOW>> DOWs { get => dOWs; set => dOWs = value; }
        public ObservableRangeCollection<DOW> SelectedDOw { get => selectedDOw; private set => selectedDOw = value; }

        private ObservableRangeCollection<SelectedItemWrapper<DOW>> dOWs;
        private ObservableRangeCollection<DOW> selectedDOw;
        public ObservableRangeCollection<Goal> goals { get; }
        public double Percentage { get => percentage; set => percentage = value; }

        public AddGoalViewModel()
        {


            // get Inotification Manager interface through the dependency service
            notificationManager = DependencyService.Get<INotificationManager>();
            AddGoalCommand = new AsyncCommand(AddGoal);
            DOWs = new ObservableRangeCollection<SelectedItemWrapper<DOW>>(DowList.Select(dow => new SelectedItemWrapper<DOW> { Item = dow }));
            goals = new ObservableRangeCollection<Goal>();
        }

        async Task AddGoal()
        {
            // check if the app is busy
            if (IsBusy == true)
                return;
            try
            {
                // create a new goal object and save
                var newGoal = new Goal
                {
                    Name = name,
                    Description = description,
                    CreatedOn = DateTime.Now,
                    Start = start,
                    End = end,
                    Time = time,
                    Percentage = 0,
                    progress = 0,
                    CategoryId = goalId

                };
                // validate add goal inputs
                if (string.IsNullOrWhiteSpace(newGoal.Name) || newGoal.Start == null || newGoal.End == null || newGoal.CreatedOn == null || newGoal.CategoryId == 0)
                    return;

                // save the new object
                await datagoal.AddGoalAsync(newGoal);

                Notify(newGoal);


                //show all the days selected
                await Application.Current.MainPage.DisplayAlert("days name", $"{GetSelectedDOWs().Count()}", "cancel");


                // go back to the previous page
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"Failed to add new goal: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }

            finally
            {
                IsBusy = false;
            }

        }

        void Notify(Goal goal)
        {
            notificationManager.NotificationReceived += (Sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                ShowNotification(evtData.Title, evtData.Message);
            };
            // call schedule reminder
            ScheduleReminder(goal);
        }

        void ShowNotification(string title, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var msg = new Label()
                {
                    Text = $"Notification Received: \nTitle: {title}\nMessage: {message}"
                };
                var result = msg;
            });
        }
        void ScheduleReminder(Goal goal)
        {
            notificationNumber++;
            string title = $"Local Notification #{notificationNumber}";
            string message = $" you have now received {notificationNumber}notifications!";
            DateTime dateTime = goal.Time;
            notificationManager.SendNotification(title, message, dateTime);
        }


        // creating a list of Dow
        public List<DOW> DowList = new List<DOW>()
       {
        new DOW { DOWId = 1, Name = "Sunday"},
        new DOW{DOWId =2, Name = "Monday"},
        new DOW{DOWId =3, Name ="Tuesday"},
        new DOW{DOWId =4,Name="Wensday"},
        new DOW{DOWId =5, Name="Thursday" },
        new DOW{DOWId =6,Name = "Friday"},
        new DOW{DOWId = 7, Name="Saturday"}

       };
        // get the selected item
        public ObservableRangeCollection<DOW> GetSelectedDOWs()
        {
            var selected = DOWs.Where(D => D.IsSelected).Select(D => D.Item).ToList();
            return new ObservableRangeCollection<DOW>(selected);
        }

        public class NotificationEventArgs : EventArgs
        {
            public string Title { get; set; }
            public string Message { get; set; }
        }
    }
}
