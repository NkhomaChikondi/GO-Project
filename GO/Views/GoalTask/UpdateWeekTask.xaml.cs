using GO.Models;
using GO.Services;
using GO.ViewModels.Subtasks;
using GO.ViewModels.TaskInGoals;
using GO.Views.SubTaskView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.GoalTask
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(taskId), nameof(taskId))]
    public partial class UpdateWeekTask : ContentPage
    {
        public string taskId { get; set; }
        Models.GoalTask GoalTask = new Models.GoalTask();
        public IDataWeek<Models.Week> dataWeek { get; }
        public IDataTask<Models.GoalTask> dataTask { get; }
        public IToast GetToast { get; }
        public IDataSubtask<Subtask> datasubTask { get; }
        public UpdateWeekTask()
        {
            InitializeComponent();
            dataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            dataWeek = DependencyService.Get < IDataWeek<Models.Week>>();
            datasubTask = DependencyService.Get<IDataSubtask<Subtask>>();
            GetToast = DependencyService.Get<IToast>();
            BindingContext = new addTaskViewModel();
          //  detaillabel1.TranslateTo(100, 0, 3000, Easing.Linear);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(taskId, out var result);
            // get the task having the task id
            var task = await dataTask.GetTaskAsync(result);
            GoalTask = task;
            Tasknameeditor.Text = task.taskName;
            Desciptioneditor.Text = task.Description;
            createlb.Text = task.CreatedOn.ToString();
            lblstatus.Text = task.Status;

            //get all subtasks having the tasksid
            var allsubtasks = await datasubTask.GetSubTasksAsync(task.Id);
            allStacks.Text = allsubtasks.Count().ToString();
            //get subtasks that are completed
            var allcompletedSubtask = allsubtasks.Where(S => S.IsCompleted).ToList();
            CompletedStacks.Text = allcompletedSubtask.Count().ToString();
            // get all uncompleted subtasks
            var uncompleted = allsubtasks.Where(U => !U.IsCompleted).ToList();
            UncompletedStacks.Text = uncompleted.Count().ToString();
            // get all expired subtasks
            var expired = allsubtasks.Where(E => E.Status == "Expired").ToList();
            ExpiredStacks.Text = expired.Count().ToString();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                // set the application IsBusy to true
                IsBusy = true;
                // create a new task object
                var newtask = new Models.GoalTask
                {
                    taskName = Tasknameeditor.Text,

                    Description = Desciptioneditor.Text,

                };
                // get all tasks in GoalId
                var alltasks = await dataTask.GetTasksAsync(GoalTask.Id);
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newtask.taskName[0]) + newtask.taskName.Substring(1);
                //check if the new task already exist in the database
                if (alltasks.Any(T => T.taskName == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                    return;
                }
                // check if goal has week or not
                // get last inserted week in "this" goal
                var week = await dataWeek.GetWeeksAsync(GoalTask.GoalId);
                // get the last inserted week
                var lastweek = week.ToList().LastOrDefault();
                if (DateTime.Today >= lastweek.StartDate && DateTime.Today <= lastweek.EndDate)
                {
                    // set lastinsertedWeek to active
                    //lastweek.Active = true;
                    // update the database
                    await dataWeek.UpdateWeekAsync(lastweek);
                }

                var newestTask = new Models.GoalTask
                {
                    Id = GoalTask.Id,
                    taskName = UppercasedName,
                    GoalId = GoalTask.GoalId,
                    IsCompleted = GoalTask.IsCompleted,
                    Description = newtask.Description,
                    PendingPercentage = GoalTask.PendingPercentage,
                    Percentage = GoalTask.Percentage,
                    Status = lblstatus.Text,
                    CompletedSubtask = GoalTask.CompletedSubtask,
                    IsEnabled = GoalTask.IsEnabled,
                    CreatedOn = GoalTask.CreatedOn,
                    IsVisible = GoalTask.IsVisible,
                    WeekId = lastweek.Id,
                    DowId = GoalTask.DowId,
                    IsNotVisible = GoalTask.IsNotVisible
                };

                // add the new task to the database                
                await dataTask.UpdateTaskAsync(newestTask);

                // go back to the previous page
                await Shell.Current.GoToAsync("..");
                GetToast.toast("Task Updated");
            }
            catch (Exception ex)
            {
                // Debug.WriteLine($"Failed to add new goal: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update task: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }

        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {        
            var route = $"{nameof(subTaskView)}?{nameof(SubtaskViewModel.Taskid)}={GoalTask.Id}";
            await Shell.Current.GoToAsync(route);
        }
    }
}