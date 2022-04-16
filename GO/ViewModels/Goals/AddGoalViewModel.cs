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
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]

    public class AddGoalViewModel : BaseViewmodel, INotifyPropertyChanged
    {
        private string name;
        private DateTime start = DateTime.Today;
        private DateTime end = DateTime.Today;
        private int categoryId;
        private string description;
        private DateTime time = DateTime.Now;
        private string status;
        private double percentage;
        private DateTime createdOn;
        private int notificationNumber = 0;
        INotificationManager notificationManager;

        public AsyncCommand AddGoalCommand { get; set; }
        public string Name { get => name; set => name = value; }
        public DateTime End { get => end; set => end = value; }
        public DateTime Start { get => start; set => start = value; }
        public string Description { get => description; set => description = value; }
        public int CategoryId { get => categoryId; set => categoryId = value; }
        public DateTime Time { get => time; set => time = value; }
        public double Percentage { get => percentage; set => percentage = value; }
        public DateTime CreatedOn { get => createdOn; set => createdOn = value; }


        public ObservableRangeCollection<SelectedItemWrapper<DOW>> DOWs { get => dOWs; set => dOWs = value; }
        public ObservableRangeCollection<DOW> SelectedDOw { get => selectedDOw; private set => selectedDOw = value; }

        private ObservableRangeCollection<SelectedItemWrapper<DOW>> dOWs;
        private ObservableRangeCollection<DOW> selectedDOw;
        public ObservableRangeCollection<Goal> goals { get; }
        public string Status { get => status; set => status = value; }

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
                    CategoryId = categoryId

                };
                // get all tasks in GoalId
                var allGoals = await datagoal.GetGoalsAsync(CategoryId);
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newGoal.Name[0]) + newGoal.Name.Substring(1);
                //check if the new task already exist in the database
                if (allGoals.Any(G => G.Name == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "A Goal with that Name already exist! Change. ", "OK");
                    return;
                }
             
                // create the newest goal object
                var newestGoal = new Goal
                {
                    Name = UppercasedName,
                    Description = description,
                    CreatedOn = DateTime.Now,
                    Start = start,
                    End = end,
                    Time = time,
                    Percentage = 0,
                    progress = 0,
                    CategoryId = categoryId

                };

                // save the new object
                await datagoal.AddGoalAsync(newestGoal);
                // get the number of items if selected on the dowlist
                var selectedList = GetSelectedDOWs().Count();
                if (selectedList > 0)
                {
                    //call addDowGoal method and pass to it a newly created goal
                    await addDOWGoal();
                }
                else
                {
                    await Shell.Current.GoToAsync("..");
                }
                   
               
               //Notify(newGoal);             
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
        // get the selected dow item and assign it to a goal
       
        public ObservableRangeCollection<DOW> GetSelectedDOWs()
        {
            var selected = DOWs.Where(D => D.IsSelected).Select(D => D.Item).ToList();
            
            return new ObservableRangeCollection<DOW>(selected);
        }
        // adding values to DOWGoal
        public async Task addDOWGoal()
        {
            if (IsBusy == true)
                return;
            try
            {
                // get goal whose id matches the incoming goal id
                var Goalid = await datagoal.GetGoalAsync(name);
                //creating a  new instance of DowGoal
                DOWGoal dOWGoal = new DOWGoal();
                // loop through the selected list and add their id to dowgoal
                foreach (var item in GetSelectedDOWs())
                {

                    dOWGoal.DowId = item.DOWId;
                    dOWGoal.GoalId = Goalid.Id;
                }
                await dataDowgoal.AddDowGoalAsync(dOWGoal);

            }
            catch (Exception ex)
            {


                Debug.WriteLine($"Failed to add new goal: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally 
            {
                IsBusy = true;
            }
           
        }
        public async void InitializeProperties(Goal goal)
        {
            Name = goal.Name;
            Description = goal.Description;
            Status = goal.Status;
            Start = goal.Start;
            End = goal.End;
            Time = goal.Time;
        }
        public class NotificationEventArgs : EventArgs
        {
            public string Title { get; set; }
            public string Message { get; set; }
        }
    }
}
