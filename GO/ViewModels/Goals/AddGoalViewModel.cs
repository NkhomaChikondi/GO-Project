using GO.Models;
using GO.Services;
using GO.ViewModels.TaskInGoals;
using GO.Views.Goal;
using GO.Views.GoalTask;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Shiny.Jobs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
        private bool hasWeek = false;
        private bool noweek = false;

        INotificationManager notificationManager;
        int notificationNumber = 0;

        public AsyncCommand AddGoalCommand { get; set; }
        public string Name { get => name; set => name = value; }
        public DateTime End { get => end; set => end = value; }
        public DateTime Start { get => start; set => start = value; }
        public string Description { get => description; set => description = value; }
        public int CategoryId { get => categoryId; set => categoryId = value; }
        public DateTime Time { get => time; set => time = value; }
        public double Percentage { get => percentage; set => percentage = value; }
        public DateTime CreatedOn { get => createdOn; set => createdOn = value; }
        public string Status { get => status; set => status = value; }
        public bool HasWeek { get => hasWeek; set => hasWeek = value; }
        public bool Noweek { get => noweek; set => noweek = value; }

        


        public ObservableRangeCollection<Goal> goals { get; }

        public AddGoalViewModel()
        {

            // get Inotification Manager interface through the dependency service

            AddGoalCommand = new AsyncCommand(AddGoal);

            goals = new ObservableRangeCollection<Goal>();
            notificationManager = DependencyService.Get<INotificationManager>();
            notificationManager.NotificationReceived += (sender, eventArgs) =>
            {
                var evtData = (NotificationEventArgs)eventArgs;
                //ShowNotification(evtData.Title, evtData.Message);
            };
        }      
            

        async Task AddGoal()
        {
            // check if the app is busy
            if (IsBusy == true)
                return;
            try
            {
                // check if either one of the two options required for a goal are selected
                if (!HasWeek && !Noweek)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Please select one of the Goal's Task Type options ", "OK");
                    return;

                }
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
                    Progress = 0,
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
                if (newGoal.Description == null)
                    newGoal.Description = $"No Description for \" {newGoal.Name}\" ";
                // get the number of weeks the goal will from start date
                var duration = end - start;
                // divide the duration by 7
                double doubleduration = duration.TotalDays;
                var weeksNumber = doubleduration / 7;
                // get the remainder if any from the above division

                var remainder = weeksNumber % 7;
                if (remainder != 0)
                {

                    // add 1 to weeknumber
                    weeksNumber = weeksNumber + 1;
                }
                // make sure start date is not more than end date

                if (start > end)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", $"Make sure Start Date is not more than End Date ", "OK");
                    return;
                }
                if (newGoal.Description == null)
                    newGoal.Description = $"No Description for {newGoal.Name}";

                // create the newest goal object
                var newestGoal = new Goal
                {
                    Name = UppercasedName,
                    Description = newGoal.Description,
                    CreatedOn = DateTime.Now,
                    Start = start,
                    End = end,
                    Time = time,
                    Percentage = 0,
                    Status = "Not Started",
                    Progress = 0,
                    ExpectedPercentage = 100,
                    NumberOfWeeks = (int)weeksNumber,
                    CategoryId = categoryId,
                    HasWeek = hasWeek,
                    Noweek = Noweek
                };
                // check if the end date is more than start date and the goal duration has more than 7 days
                if (HasWeek)
                {
                    if (newestGoal.Start == newestGoal.End)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", $"Start Date and End Date cannot be the same! " +
                            $" make sure End Date is more than Start Date", "OK");
                        return;
                    }
                    var goalDuration = newestGoal.End - newestGoal.Start;
                    TimeSpan time = new TimeSpan(6, 0, 0, 0);


                    if (goalDuration < time)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", $"Your goal duration has {goalDuration.TotalDays} days, make sure it is equal to or more than 7 days (a week) ", "OK");
                        return;
                    }

                }
                ////check if the task already exist so you can either save or update
                //if (allGoals.Any(t => t.Id == newestGoal.Id))
                //{

                //    await datagoal.UpdateGoalAsync(newestGoal);
                //    await Application.Current.MainPage.DisplayAlert("Alert!", " Your goal has been successfully updated", "Ok");
                //    await Shell.Current.GoToAsync("..");

                //}
                // save the new object
                await datagoal.AddGoalAsync(newestGoal);
                if (hasWeek)
                {
                    await CreateWeek(newestGoal);
                    SendScheduledNotification();
                    await Application.Current.MainPage.DisplayAlert("Alert!", $"Goal has successfully been Created with {newestGoal.NumberOfWeeks} weeks!, Create Tasks for the first week", "OK");
                }
                else
                {
                    SendScheduledNotification();
                    await Shell.Current.GoToAsync("..");
                }
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


            async Task CreateWeek(Goal goal)
            {
                //* check the number of days that are left in the week*

                // get the start day, day of the week
                var startDay = start.DayOfWeek.ToString();
                int dayValue = 0;
                if (startDay == "Sunday")
                    dayValue = 6;
                else if (startDay == "Monday")
                    dayValue = 5;
                else if (startDay == "Tuesday")
                    dayValue = 4;
                else if (startDay == "Wednesday")
                    dayValue = 3;
                else if (startDay == "Thursday")
                    dayValue = 2;
                else if (startDay == "Friday")
                    dayValue = 1;
                else if (startDay == "Saturday")
                    dayValue = 0;


                // get the total number of goals from the database
                var goals = await datagoal.GetGoalsAsync(categoryId);
                //get the id of the last inserted id
                var lastGoal = goals.ToList().LastOrDefault();
                var goalId = lastGoal.Id;
                //* finding the targeted percentage*
                // get the total number of weeks from goal
                var totalWeeks = goal.NumberOfWeeks;
                // calculate the percentage for every week
                var weekPercentage = 100 / totalWeeks;

                if (startDay == "Saturday")
                {
                    dayValue = 6;
                    start = start.AddDays(1);

                    await Application.Current.MainPage.DisplayAlert("Alert", "you cannot start a goal on a Saturday.Your Goal's start day, will be  moved to Sunday  ", "OK");
                }
                // how to find the end date
                var enddate = start.AddDays(dayValue);

                // create a new week object
                var newWeek = new Week
                {
                    WeekNumber = 1,
                    TargetPercentage = weekPercentage,
                    AccumulatedPercentage = 0,
                    Active = true,
                    StartDate = start,
                    EndDate = enddate,
                    GoalId = goalId
                };

                // save the newly created week to the database
                await dataWeek.AddWeekAsync(newWeek);

                var route = $"{nameof(WeeklyTask)}?goalId={goalId}";
                await Shell.Current.GoToAsync(route);

            }
            void SendScheduledNotification()
            {
                notificationNumber++;
                string title = $"Local Notification #{notificationNumber}";
                string message = $"You have now received {notificationNumber} notifications!";
                notificationManager.SendNotification(title, message, DateTime.Now.AddSeconds(50));
            }
            //void ShowNotification(string title, string message)
            //{
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        var msg = new Label()
            //        {
            //            Text = $"Notification Received:\nTitle: {title}\nMessage: {message}"
            //        };
            //        stackLayout.Children.Add(msg);
            //    });

            //}

        }
    }
}
