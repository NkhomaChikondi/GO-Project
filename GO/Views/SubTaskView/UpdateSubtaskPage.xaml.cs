using GO.Models;
using GO.Services;
using GO.ViewModels.Subtasks;
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
        public IDataSubtask<Subtask> dataTask { get; }   
        public IDataTask<Models.GoalTask> datatask { get; }
        public UpdateSubtaskPage()
        {
            InitializeComponent();
            dataTask = DependencyService.Get<IDataSubtask<Subtask>>();
            datatask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            BindingContext = new AddSubtaskViewModel();
            detaillabel2.TranslateTo(100, 0, 8000, Easing.Linear);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(SubtaskId, out var result);
            // get the subtask having the result id
            var dbsubtask = await dataTask.GetSubTaskAsync(result);
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
                    SubStart = Subtask.SubStart,
                    SubEnd = SubEndDate.Date,
                    TaskId = Subtask.TaskId,
                    RemainingDays = remainingDays,
                    CreatedOn = Subtask.CreatedOn
                    


                };
                // get all subtasks having task id
                var AllSubtasks = await dataTask.GetSubTasksAsync(newSubtask.TaskId);
                // get the task having the TaskId
                var task = await datatask.GetTaskAsync(newSubtask.TaskId);
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newSubtask.SubName[0]) + newSubtask.SubName.Substring(1);
                //check if the new task already exist in the database
                if (AllSubtasks.Any(T => T.SubName == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                    return;
                }
               
             
                // verify if the Start date and end date are within the duration of its selected goal
                if (newSubtask.SubStart >= task.StartTask && newSubtask.SubEnd <= task.EndTask)
                {
                    TimeSpan ts = newSubtask.SubEnd - newSubtask.SubStart;
                    remainingDays = (int)ts.TotalDays;

                }

                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", " make sure the Start date and end date are within the duration of its selected Task", "OK");
                    return;
                }
                // pass the uppercased name to the category object
                var newestSubtask = new Subtask
                {

                    Id = newSubtask.Id,
                    SubName = UppercasedName,
                    SubStart = newSubtask.SubStart,
                    SubEnd = newSubtask.SubEnd,
                    RemainingDays = remainingDays,
                    CreatedOn = Subtask.CreatedOn,
                    Percentage = Subtask.Percentage,
                    IsCompleted = Subtask.IsCompleted,
                    TaskId = newSubtask.TaskId,
                    Status = Subtask.Status
                };
                await dataTask.UpdateSubTaskAsync(newestSubtask); 
                // go back to the previous page
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
    }
}