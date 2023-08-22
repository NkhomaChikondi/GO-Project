using GO.Models;
using GO.Services;
using GO.ViewModels.Subtasks;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.SubTaskView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(SubtaskId), nameof(SubtaskId))]
    public partial class UpdateSubtaskPage : ContentPage
    {
        public string SubtaskId { get; set; }
        public Subtask Subtask = new Subtask();
        public IDataSubtask<Subtask> dataSubtask { get; }
        public IToast GetToast { get; }
        public IDataTask<Models.GoalTask> datatask { get; }
        public UpdateSubtaskPage()
        {
            InitializeComponent();
            dataSubtask = DependencyService.Get<IDataSubtask<Subtask>>();
            datatask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            GetToast = DependencyService.Get<IToast>();
            BindingContext = new AddSubtaskViewModel();
            //detaillabel2.TranslateTo(100, 0, 8000, Easing.Linear);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(SubtaskId, out var result);
            // get the subtask having the result id
            var dbsubtask = await dataSubtask.GetSubTaskAsync(result);
            Subtask = dbsubtask;
            editorName.Text = dbsubtask.SubName;
            CreatedDate.Text = dbsubtask.CreatedOn.ToString();
            SubstartDate.Date = dbsubtask.SubStart;
            SubEndDate.Date = dbsubtask.SubEnd;
            Substatus.Text = dbsubtask.Status;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (IsBusy == true)
                return;
            try
            {
                int remainingDays = 0;
                // create a new goal object and save
                var newSubtask = new Subtask
                {
                    Id = Subtask.Id,
                    SubName = editorName.Text,                   
                    SubStart = SubstartDate.Date,
                    SubEnd = SubEndDate.Date,
                    TaskId = Subtask.TaskId,
                    RemainingDays = remainingDays,
                    CreatedOn = Subtask.CreatedOn              
                };
                // get all subtasks having task id
                var AllSubtasks = await dataSubtask.GetSubTasksAsync(newSubtask.TaskId);
                // get the task having the TaskId
                var task = await datatask.GetTaskAsync(newSubtask.TaskId);
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newSubtask.SubName[0]) + newSubtask.SubName.Substring(1);

                //check if the new task already exist in the database
                if(Subtask.SubName != UppercasedName)
                {
                    if (AllSubtasks.Any(T => T.SubName == UppercasedName))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                        return;
                    }
                }     
                
                if (newSubtask.SubStart != Subtask.SubStart)
                {
                    if(newSubtask.SubStart < task.StartTask)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "A subtask start date cannot be less than its task start date.", "OK");
                        return;
                    }
                    // check if the changed end date is below the date of today
                    if (newSubtask.SubStart < DateTime.Today)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to change start date. A subtask cannot start on a day that has passed.", "Ok");
                        return;
                    }
                    // make sure startday is not more than end date
                    if (newSubtask.SubStart > newSubtask.SubEnd)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to Change start date. Subtask's start date cannot be more than it's end date.", "Ok");
                        return;
                    }                 
                }
               
                if (newSubtask.SubEnd != Subtask.SubEnd)
                {
                    // check if newsubtask end date is not more than task
                    if (newSubtask.SubEnd > task.EndTask)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to change end date. End date of a Subtask, cannot be more than task's end date ({task.enddatetostring}).", "Ok");
                        return;
                    }
                   
                    // make sure startday is not more than end date
                    if (newSubtask.SubEnd  < newSubtask.SubStart)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to Change start date. Subtask's end date cannot be less than it's start date.", "Ok");
                        return;
                    }
                    // check if the changed end date is below the date of today
                    if (newSubtask.SubEnd < DateTime.Today)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to change end date. An updated End Date of a Subtask, cannot be below the date of today", "Ok");
                        return;
                    }
                    // make sure you cannot expand the end date of a task that has expired whilst it was completed
                    if (Subtask.IsCompleted && Subtask.Status == "Expired")
                    {
                        await Application.Current.MainPage.DisplayAlert("Failed to change end date", "Failed to change end date. Cannot change the end date of a Subtask that has expired whilst completed", "Ok");
                        return;
                    }
                    else if (Subtask.IsCompleted)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to change end date. You cannot make end date changes to a subtask that has already been completed, unless, you add more subtasks.", "Ok");
                        return;
                    }

                    else if (DateTime.Today > Subtask.SubEnd && newSubtask.SubEnd > Subtask.SubEnd)
                    {
                        var result = await Application.Current.MainPage.DisplayAlert("Alert", "You are adding days to a subtask that has expired. Continue?", "Yes", "No");
                        if (result)
                        {
                            Subtask.Status = "UnCompleted";
                        }
                        else if (!result)
                            return;
                    }
                }
                // pass the uppercased name to the category object
                var newestSubtask = new Subtask
                {       Id = newSubtask.Id,            
                    SubName = UppercasedName,
                    SubStart = newSubtask.SubStart,
                    SubEnd = newSubtask.SubEnd,
                    RemainingDays = remainingDays,
                    CreatedOn = Subtask.CreatedOn,
                    Percentage = Subtask.Percentage,
                    IsCompleted = Subtask.IsCompleted,
                    TaskId = newSubtask.TaskId,
                    Status = Subtask.Status,
                    enddatetostring = newSubtask.SubEnd.ToLongDateString()
                };
                await dataSubtask.UpdateSubTaskAsync(newestSubtask);

                if (newestSubtask.SubEnd > Subtask.SubEnd)
                {
                    //cancel task notification
                    LocalNotificationCenter.Current.Cancel(Subtask.Id);
                    // create a new notification
                    await SendNotification(newestSubtask.SubEnd);
                }
                // go back to the previous page
                await Shell.Current.GoToAsync("..");
                GetToast.toast("Subtask Updated");
                
                
                  
            }
            catch (Exception ex)
            {
               
                await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update subtask: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        async Task SendNotification(DateTime end)
        {
            // create a new notification
            var notification = new NotificationRequest
            {
                BadgeNumber = 1,
                Description = $"Subtask '{Subtask.SubName}' is Due today!",
                Title = "Due-Date!",
                NotificationId = Subtask.Id,
                Schedule =
                {
                    NotifyTime = end,
                }
            };
            await LocalNotificationCenter.Current.Show(notification);

        }
    }
}