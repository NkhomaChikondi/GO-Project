using GO.Models;
using GO.Services;
using GO.ViewModels.Subtasks;
using GO.ViewModels.TaskInGoals;
using GO.Views.SubTaskView;
using Plugin.LocalNotification;
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
    public partial class UpdateTaskPage : ContentPage
    {
        public string taskId { get; set; }
        Models.GoalTask GoalTask = new Models.GoalTask();
        public IDataTask<Models.GoalTask> dataTask { get; }
        public IDataGoal<Models.Goal> dataGoal { get; }
        public IDataSubtask<Subtask> datasubTask { get; }
        public UpdateTaskPage()
        {
            InitializeComponent();
            dataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            datasubTask = DependencyService.Get<IDataSubtask<Subtask>>();
            BindingContext = new addTaskViewModel();
            //detaillabel.TranslateTo(100, 0,3000, Easing.Linear);
        }

        protected async override void OnAppearing()
        {

            base.OnAppearing();
            int.TryParse(taskId, out var result);
            // get the task having the result id
            var task = await dataTask.GetTaskAsync(result);
            // pass the dbTask to the goaltask object
            GoalTask = task;
            // pass data to xaml elements
            NameEditor.Text = task.taskName;
            DescEditor.Text = task.Description;
            Createdlbl.Text = task.CreatedOn.ToString();
            Statuslbl.Text = task.Status;
            StartPicker.Date = task.StartTask;
            EndPicker.Date = task.EndTask;

            //get all subtasks having the tasksid
            var allsubtasks = await datasubTask.GetSubTasksAsync(task.Id);
            AllSubtaskslbl.Text = allsubtasks.Count().ToString();
            //get subtasks that are completed
            var allcompletedSubtask = allsubtasks.Where(S => S.IsCompleted).ToList();
            Completedsubtaskslbl.Text = allcompletedSubtask.Count().ToString();
            // get all uncompleted subtasks
            var uncompleted = allsubtasks.Where(U => !U.IsCompleted).ToList();
            uncompletedTaskslbl.Text = uncompleted.Count().ToString();
            // get all expired subtasks
            var expired = allsubtasks.Where(E => E.Status == "Expired").ToList();
            Expiredlbl.Text = expired.Count().ToString();
        }
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            
           try
           {
                int remainingDays = 0;
                // get the task from the database having taskid
                var task = await dataTask.GetTaskAsync(GoalTask.Id);

                // create a new task object
                var newtask = new Models.GoalTask
                {
                        taskName = NameEditor.Text,
                        StartTask = StartPicker.Date,
                        EndTask = EndPicker.Date
                     
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
                    // get goal from the goal table through the given Id
                    var TaskInGoalId = await dataGoal.GetGoalAsync(GoalTask.GoalId);
                    // verify if the Start date and end date are within the duration of its selected goal
                    if (newtask.StartTask >= TaskInGoalId.Start && newtask.EndTask <= TaskInGoalId.End)
                    {
                        TimeSpan ts = newtask.EndTask - newtask.StartTask;
                        remainingDays = (int)ts.TotalDays;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", " make sure the Start date and end date are within the duration of its selected goal", "OK");
                        return;
                    }
                   if(newtask.EndTask > task.EndTask)
                   {
                    //cancel task notification
                    LocalNotificationCenter.Current.Cancel(task.Id);
                  
                   }
                if (newtask.EndTask > task.EndTask)
                {
                    // make sure tasks end date is not surpassing goals end date
                    if (newtask.EndTask > TaskInGoalId.End)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", $"Failed to update task! make sure the selected task's end date is not more than {TaskInGoalId.enddatetostring}", "OK");
                        return;
                    }
                }
                else if (newtask.EndTask < task.EndTask)
                {
                    // check if they are no subtasks whose end date surpasses the tasks end date
                    // get tasks having the goals id
                    var subtasks = await datasubTask.GetSubTasksAsync(task.Id);
                    // loop through the subtasks
                    var counter = 0;
                    foreach (var subtask in subtasks)
                    {
                        if (subtask.SubEnd > task.EndTask)
                            counter += 1;

                    }
                    if (counter > 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update task, they're subtask's in it, whose end date is more than the task's selected end date. Go to tsubask page, find those tasks and modify their end dates", "OK");
                        return;
                    }
                }
               
                var newTask = new Models.GoalTask
                    {
                        Id = GoalTask.Id,
                        taskName = UppercasedName,
                        Description = DescEditor.Text,
                        CreatedOn = Convert.ToDateTime(Createdlbl.Text),
                        Status = Statuslbl.Text,
                        StartTask = GoalTask.StartTask,
                        EndTask = EndPicker.Date,
                        DowId = GoalTask.DowId,
                        RemainingDays = remainingDays,
                        GoalId = GoalTask.GoalId,
                        IsCompleted = GoalTask.IsCompleted,
                        CompletedSubtask = GoalTask.CompletedSubtask,
                        IsEnabled = GoalTask.IsEnabled,
                        IsNotVisible = GoalTask.IsNotVisible,
                        IsVisible = GoalTask.IsVisible,
                        PendingPercentage = GoalTask.PendingPercentage,
                        Percentage = GoalTask.Percentage,
                        Progress = GoalTask.Progress,
                        WeekId = GoalTask.WeekId
                    };

                // add the new task to the database                
                await dataTask.UpdateTaskAsync(newTask);
                if (newtask.EndTask > task.EndTask)
                {
                    //cancel task notification
                    LocalNotificationCenter.Current.Cancel(task.Id);
                    // create a new notification
                    await SendNotification(newtask.EndTask);
                }               
                    // go back to the previous page
                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {                    
                    await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to add new goal: {ex.Message}", "OK");
                }
            
            }
        async Task SendNotification( DateTime end)
        {
            // create a new notification
            var notification = new NotificationRequest
            {
                BadgeNumber = 1,
                Description = $"Task '{GoalTask.taskName}' is Due today!",
                Title = "Due-Date!",
                NotificationId = GoalTask.Id,
                Schedule =
                {
                    NotifyTime = end,
                }
            };
            await LocalNotificationCenter.Current.Show(notification);

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {          
            var route = $"{nameof(subTaskView)}?SubtaskId={GoalTask.Id}";
            await Shell.Current.GoToAsync(route);
        }
    }
}