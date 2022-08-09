using GO.Services;
using GO.ViewModels.Goals;
using GO.ViewModels.TaskInGoals;
using GO.Views.GoalTask;
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
        public UpdateGoalPage()
        {
            InitializeComponent();
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
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
                if (allGoals.Any(G => G.Name == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "A Goal with that Name already exist! Change. ", "OK");
                    return;
                }
                if (newGoal.Description == null)
                    newGoal.Description = $"No Description for \" {newGoal.Name}\" ";
                // get the number of weeks the goal will from start date
                
                // make sure start date is not more than end date

                if (newGoal.Start > newGoal.End)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", $"Make sure Start Date is not more than End Date ", "OK");
                    return;
                }
                if (newGoal.Description == null)
                    newGoal.Description = $"No Description for {newGoal.Name}";

               // create a new goal object
                   var newestGoal = new Models.Goal
                   {
                       Id = GoalId,
                       Name = Nameeditor.Text,
                       Description = Desclbl.Text,
                       End = enddatepicker.Date,
                       CreatedOn = Convert.ToDateTime(Createdlbl.Text),
                       Start = Startdatepicker.Date,
                       CategoryId = Goal.CategoryId,
                       HasWeek = Goal.HasWeek,
                       Noweek = Goal.Noweek,
                       NumberOfWeeks = Goal.NumberOfWeeks,
                       Percentage = Goal.Percentage,
                       ExpectedPercentage = Goal.ExpectedPercentage,
                       Progress = Goal.Progress,
                       Status = Goal.Status,
                       Time = Goal.Time

                   };
              
                //check if the task already exist so you can either save or update
              

                    await dataGoal.UpdateGoalAsync(newestGoal);
                    await Application.Current.MainPage.DisplayAlert("Alert!", "Updated Successfully", "Ok");
                    await Shell.Current.GoToAsync("..");                
                
            }

                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to add new goal: {ex.Message}", "OK");
                }

                finally
                {
                    IsBusy = false;
                }
           

         



            }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if (Goal.HasWeek && !Goal.Noweek)
            {              
                var route = $"{nameof(WeeklyTask)}?goalId={Goal.Id}";
                await Shell.Current.GoToAsync(route);
            }
            else if (!Goal.HasWeek && Goal.Noweek)
            {
                var route = $"{nameof(GoalTaskPage)}?{nameof(GoalTaskViewModel.GoalId)}={Goal.Id}";
                await Shell.Current.GoToAsync(route);
            }
        }
    }
}
