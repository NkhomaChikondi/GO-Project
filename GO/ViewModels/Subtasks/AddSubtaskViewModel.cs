using GO.Models;
using GO.Views.SubTaskView;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.Subtasks
{
    [QueryProperty(nameof(GetTaskId), (nameof(GetTaskId)))]
    public class AddSubtaskViewModel : BaseViewmodel
    {
        private string name;
        private DateTime startDate = DateTime.Today;
        private DateTime endDate = DateTime.Today;
        private double duration;
        private double percentage;
        private int getTaskId;
        private int remainingDays = 0;
        private int percentageProgress = 0;
        public ObservableRangeCollection<Subtask> subTasks { get; }
        public AsyncCommand subTaskAddCommand { get; }
        public AsyncCommand HelpCommand { get; }
        public AddSubtaskViewModel()
        {
            subTasks = new ObservableRangeCollection<Subtask>();
            subTaskAddCommand = new AsyncCommand(AddSubtask);
            HelpCommand = new AsyncCommand(GotoHelpPage);
        }

        public string Name { get => name; set => name = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public DateTime EndDate { get => endDate; set => endDate = value; }
        public double Duration { get => duration; set => duration = value; }
        public int GetTaskId { get { return getTaskId; } set => getTaskId = value; }
        public double Percentage { get => percentage; set => percentage = value; }
        public int RemainingDays { get => remainingDays; set => remainingDays = value; }

        async Task AddSubtask()
        {
            // check if the app is busy
            if (IsBusy == true)
                return;
            try
            {               
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(name[0]) + name.Substring(1);
               
                //get the task having the task id
                var task = await dataTask.GetTaskAsync(getTaskId);
                // create a new subtask object 
                var newestSubtask = new Subtask
                {
                    CreatedOn = DateTime.Now,
                    Status = "Not Completed",
                    SubEnd = endDate,
                    SubStart = startDate,
                    enddatetostring = endDate.ToLongDateString(),
                    IsCompleted = false,
                    SubName = UppercasedName,
                    Percentage = 0,
                    TaskId = getTaskId
                };
                //check if the new task already exist in the database
                if (subTasks.Any(T => T.SubName == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                    return;
                }
                // check if task has week id 
                if(task.WeekId == 0)
                {
                    //check if newestsubtask has got valid dates
                    if (newestSubtask.SubEnd > task.EndTask)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", $"The end date shouldn't be more than the task's End date ({task.enddatetostring})", "Ok");
                        return;
                    }
                    else if(newestSubtask.SubStart > newestSubtask.SubEnd)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Start date cannot be more than end date", "Ok");
                        return;
                    }                  
                }

                await dataSubTask.AddSubTaskAsync(newestSubtask);
                if (task.WeekId == 0)
                    await SendNotification();
                await TaskEnabled(task);
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
        async Task SendNotification()
        {
            // get all tasks having task id
            var subtasks = await dataSubTask.GetSubTasksAsync(GetTaskId);
            // get the last goal
            var lastSubtask = subtasks.ToList().LastOrDefault();
            var SubtaskId = lastSubtask.Id + 1;

            var notification = new NotificationRequest
            {
                BadgeNumber = 1,
                Description = $"Subtask '{lastSubtask.SubName}' is Due today!",
                Title = "Due-Date!",
                NotificationId = getTaskId,
                Schedule =
                    {
                        //NotifyTime =lastSubtask.SubEnd,
                         NotifyTime  = DateTime.Now.AddSeconds(20),
                    }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }
        async Task GotoHelpPage()
        {
            var route = $"{nameof(Helpaddsubtaskspage)}";
            await Shell.Current.GoToAsync(route);
        }
        async Task TaskEnabled( GoalTask task)
        {          
            if (task.IsEnabled)
            {
                task.IsEnabled = false;
                task.IsCompleted = false;
                await dataTask.UpdateTaskAsync(task);
            }
            else if (task.IsEnabled == false)
                return;
        }


    }
}
