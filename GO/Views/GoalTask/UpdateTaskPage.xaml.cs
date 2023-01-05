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
        public IToast GetToast { get; }
        public UpdateTaskPage()
        {
            InitializeComponent();
            dataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            datasubTask = DependencyService.Get<IDataSubtask<Subtask>>();
            GetToast = DependencyService.Get<IToast>();
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
                // get goal from the goal table through the given Id
                var TaskInGoalId = await dataGoal.GetGoalAsync(GoalTask.GoalId);
                // get all tasks in GoalId
                var alltasks = await dataTask.GetTasksAsync(GoalTask.Id);
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newtask.taskName[0]) + newtask.taskName.Substring(1);

                // check if the name doesnt already exist in the database
                if (task.taskName != UppercasedName)
                {
                    //check if the new task already exist in the database
                    if (alltasks.Any(T => T.taskName == UppercasedName))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                        return;
                    }
                }
                // get goal from which the task is created
                var goal = await dataGoal.GetGoalAsync(task.GoalId);
                // check if the updated start date is not equal to that from the database
                if(newtask.StartTask.Date != task.StartTask.Date)
                {
                    if(DateTime.Today > task.StartTask)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to change task's start date. You cannot change start date of a task that has already started", "Ok");
                        return;
                    }
                    if(newtask.StartTask > newtask.EndTask)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to change task's start date.Task's start date cannot be more than task's end date.", "Ok");
                        return;
                    }
                    else if(DateTime.Today < task.StartTask)
                    {
                        // make sure startday is not more than end date
                        if (newtask.StartTask > newtask.EndTask)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error!", " Failed to change task's start date.Task's start date cannot be more than Task's end date.", "Ok");
                            return;
                        }
                    }
                    else if (newtask.StartTask < DateTime.Today)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to change task's start date. Task's start date cannot be less than the date of today.", "Ok");
                        return;
                    }
                }
                // check if newtask end date is not more than goal end date
                if (newtask.EndTask > goal.End)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", $" Failed to change task's end date. task's end date, cannot be more than goal's end date ({goal.enddatetostring}).", "Ok");
                    return;
                }      
                // check if the changed end date is below the date of today
                if(newtask.EndTask < DateTime.Today)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Failed to change task's end date. task's end date, cannot be less than the date of today", "Ok");
                    return;
                }
                // check if the task has expired
                if(newtask.EndTask != task.EndTask)                  
                {
                     // make sure you cannot expand the end date of a task that has expired whilst it was completed
                    if (task.IsCompleted && task.Status == "Expired")
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to change task's end date. You Cannot change the end date of a task that has expired whilst completed", "Ok");
                        return;
                    }
                    else if (task.IsCompleted)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", " Failed to change task's end date. You cannot make end date changes to a task that has already been completed, unless, you add more subtasks.", "Ok");
                        return;
                    }
                   

                    else if (DateTime.Today > task.EndTask && newtask.EndTask > task.EndTask)
                    {
                        
                        var result = await Application.Current.MainPage.DisplayAlert("Alert", "You are adding days to a task that has expired. Continue?", "Yes", "No");
                        if (result)
                        {
                            // get subtasks having the task's id
                            var subtasks = await datasubTask.GetSubTasksAsync(task.Id);
                            if (subtasks.Count() > 0)
                                task.Status = "In Progress";
                            else if (subtasks.Count() == 0)
                                task.Status = "Not Started";
                        }
                        else if (!result)
                            return;
                        
                        
                    }
                        
                }
                                         
                if(newtask.EndTask > task.EndTask)
                {
                //cancel task notification
                LocalNotificationCenter.Current.Cancel(task.Id);
                  
                }
              
                if (newtask.EndTask < task.EndTask)
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
                        await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to change task's end date. They are subtask's in this task whose end date, is more than the task's end date. Go to tsubask page, find those tasks and modify their end dates", "OK");
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
                    WeekId = GoalTask.WeekId,
                    enddatetostring = EndPicker.Date.ToLongDateString()
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
                GetToast.toast("Task Updated");
            }
           catch (Exception ex)
           {
                await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update new task: {ex.Message}", "OK");
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