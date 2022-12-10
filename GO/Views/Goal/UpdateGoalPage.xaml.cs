using GO.Models;
using GO.Services;
using GO.ViewModels.Goals;
using GO.ViewModels.TaskInGoals;
using GO.Views.GoalTask;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.Goal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    [QueryProperty(nameof(goalId), nameof(goalId))]
    public partial class UpdateGoalPage : ContentPage
    {

        public string goalId { get; set; }
        public int CategoryId { get; set; }
        public int GoalId { get; set; }
        Models.Goal Goal = new Models.Goal();
        public IDataGoal<Models.Goal> dataGoal { get; }
        public IDataTask<Models.GoalTask> dataTask { get; }
        public IDataWeek<Week> dataWeek { get; }
        public IToast GetToast { get; }
        public UpdateGoalPage()
        {
            InitializeComponent();
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            dataWeek = DependencyService.Get<IDataWeek<Week>>();
            GetToast = DependencyService.Get<IToast>();
            BindingContext = new GoalViewModel();
            //detaillabel.TranslateTo(100, 0, 3000, Easing.Linear);

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            int.TryParse(goalId, out var result);
            // get the goal having the id
            var goal = await dataGoal.GetGoalAsync(result);
            Goal = goal;
            CategoryId = goal.CategoryId;
            GoalId = goal.Id;
            Nameeditor.Text = goal.Name;
            Desclbl.Text = goal.Description;
            Createdlbl.Text = goal.CreatedOn.ToString();
            Statuslbl.Text = goal.Status;
            Startdatepicker.Date = goal.Start;
            enddatepicker.Date = goal.End;
            weeknumber.Text = goal.NumberOfWeeks.ToString();
            // get all tasks in this goal
            var tasks = await dataTask.GetTasksAsync(GoalId);
            AllTask.Text = tasks.Count().ToString();
            // get all completed tasks
            var completedTasks = tasks.Where(C => C.IsCompleted).ToList();
            CompletedTasks.Text = completedTasks.Count().ToString();
            // get all uncompletedTasks
            var uncompletedTasks = tasks.Where(U => !U.IsCompleted).ToList();
            UncompletedTasks.Text = uncompletedTasks.Count().ToString();
            // get all expiredtask
            var expiredtasks = tasks.Where(E => E.Status == "Expired").ToList();
            Expiredtasks.Text = expiredtasks.Count().ToString();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // check if the app is busy
            if (IsBusy == true)
                return;
            try
            {
                
            // create a new goal object and save
            var newGoal = new Models.Goal
            {
                Name = Nameeditor.Text,
                Description = Desclbl.Text,                    
                Start = Startdatepicker.Date,
                End = enddatepicker.Date,                 
                 
                CategoryId = Goal.CategoryId


            };
            // get all tasks in GoalId
            var allGoals = await dataGoal.GetGoalsAsync(CategoryId);
            // change the first letter of the Task name to upercase
            var UppercasedName = char.ToUpper(newGoal.Name[0]) + newGoal.Name.Substring(1);
            //check if the new task already exist in the database
               
            if (newGoal.Description == null)
                newGoal.Description = $"No Description for \" {UppercasedName}\" ";
            // get the number of weeks the goal will from start date
                
            // make sure start date is not more than end date

            if (newGoal.Start > newGoal.End)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", $"End date should be more than start Date. ", "OK");
                return;
            }

            double weeksNumber = Goal.NumberOfWeeks;
                // check if the updated start date is not equal to that from the database
                if(newGoal.Start != Goal.Start)
                {
                    if(DateTime.Today >= Goal.Start)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "You cannot change start date of a goal that has already started.", "Ok");
                        return;
                    }
                    else if(DateTime.Today < Goal.Start)
                    {
                        // make sure startday is not more than end date
                        if(newGoal.Start > newGoal.End)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error!", "Goal's start date cannot be more than goal's end date.", "Ok");
                            return;
                        }
                    }
                    else if(newGoal.Start < DateTime.Today)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Start date of a goal cannot be less than the date of today.", "Ok");
                        return;
                    }
                }
                // check if the changed end date is below the date of today
                if (newGoal.End < DateTime.Today)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "An updated End Date of a goal, cannot be below the date of today", "Ok");
                    return;
                }
                // make sure you cannot expand the end date of a task that has expired whilst it was completed
                if (Goal.Percentage == Goal.ExpectedPercentage && Goal.Status == "Expired")
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Fialed to change the end date. Cannot change the end date of a goal that has expired whilst completed", "Ok");
                    return;
                }
                // check if the incoming end date is not equal to the end date from the database
                if (newGoal.End != Goal.End)
                {      
                    // check if the goal is completed
                    if(Goal.Percentage == Goal.ExpectedPercentage)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "failed to change the end date. Cannot change the end date of a goal that has already been completed, unless, you add new tasks to it", "Ok");
                        return;
                    }
                
                        // check if todays date is more than goal.end date
                    if (DateTime.Today > Goal.End && newGoal.End > Goal.End)
                    {
                        // make sure the updated end date is more than todays date
                        if (newGoal.End < DateTime.Today)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error!", "End Date of a task cannot be below the date of today", "Ok");
                            return;
                        }
                            var result = await Application.Current.MainPage.DisplayAlert("Alert", "You are adjusting the end date of a goal that has expired. Continue?", "Yes", "No");
                        if (result)
                        {
                            if (Goal.Status == "Expired")
                                Goal.Status = "In Progress";
                        }                                        
                        else if (!result)
                            return;
                    }             
                
                    // recalculate number of weeks in goal
                    var duration = newGoal.End - newGoal.Start;
                    // divide the duration by 7
                    double doubleduration = duration.TotalDays;
                    weeksNumber = doubleduration / 7;
                    // get the remainder if any from the above division

                    var remainder = weeksNumber % 7;
                    if (remainder != 0)
                    {

                        // add 1 to weeknumber
                        weeksNumber = weeksNumber + 1;
                    }
            }
            else if(newGoal.End < Goal.End)
            {
                // check if they are no tasks whose end date surpasses the goals end date
                // get tasks having the goals id
                var tasks = await dataTask.GetTasksAsync(Goal.Id);
                // loop through the tasks
                var counter = 0;
                foreach (var task in tasks)
                {
                    if(task.EndTask > Goal.End)                        
                        counter += 1;       
                        
                }
                if(counter > 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update goal, they are task's in it, whose end date is more than the goal's selected end date. Go to task page, find those tasks and modify their end dates","OK");
                    return;
                }
            }
            // create a new goal object
                var newestGoal = new Models.Goal
                {
                    Id = GoalId,
                    Name = UppercasedName,
                    Description = Desclbl.Text,
                    End = enddatepicker.Date,
                    CreatedOn = Convert.ToDateTime(Createdlbl.Text),
                    Start = Startdatepicker.Date,
                    CategoryId = Goal.CategoryId,
                    HasWeek = Goal.HasWeek,
                    Noweek = Goal.Noweek,
                    NumberOfWeeks = (int)weeksNumber,
                    Percentage = Goal.Percentage,
                    ExpectedPercentage = Goal.ExpectedPercentage,
                    Progress = Goal.Progress,
                    Status = Goal.Status,
                    Time = Goal.Time,
                    enddatetostring = newGoal.End.ToLongDateString(),
                };

            // check if updated goal's end date is more than dbgoal end date
            if (newestGoal.End > Goal.End )
            {
                LocalNotificationCenter.Current.Cancel(newestGoal.Id);
                // create a new notification

            }
                await dataGoal.UpdateGoalAsync(newestGoal);
               // cancel its notification
                 await SendNotification();                
                await Shell.Current.GoToAsync("..");
                GetToast.toast("Goal updated");


            }

            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update goal: {ex.Message}", "OK");
            }

            finally
            {
                IsBusy = false;
            }
        async Task SendNotification()
        {
            // get goal form the database
            var dbgoal = await dataGoal.GetGoalAsync(GoalId);
                
            var notification = new NotificationRequest
            {
                BadgeNumber = 1,
                Description = $"Goal '{dbgoal.Name}' is Due today!",
                Title = "Due-Date!",
                NotificationId = dbgoal.Id,
                Schedule =
                {
                    NotifyTime = dbgoal.End,
                }
            };
            await LocalNotificationCenter.Current.Show(notification);

        };
    }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if (Goal.HasWeek )
            {
                // get the weekid of the last inserted week
                // get all weeks having the goal id
                var weeks = await dataWeek.GetWeeksAsync(Goal.Id);
                var lastweek = weeks.ToList().LastOrDefault();
                var route = $"{nameof(WeeklyTask)}?weekId={lastweek.Id}";
                await Shell.Current.GoToAsync(route);
            }
            else if (Goal.Noweek )
            {
                var route = $"{nameof(GoalTaskPage)}?goalId={Goal.Id}";
                await Shell.Current.GoToAsync(route);
            }
        }
    }
}
