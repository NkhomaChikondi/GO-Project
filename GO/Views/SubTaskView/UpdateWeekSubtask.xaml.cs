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
    public partial class UpdateWeekSubtask : ContentPage
    {

        public string SubtaskId { get; set; }
        public Subtask GetSubtask = new Subtask();
        public IToast GetToast { get; }
        public IDataSubtask<Subtask> dataTask { get; }
        public IDataTask<Models.GoalTask> datatask { get; }
        public UpdateWeekSubtask()
        {
            InitializeComponent();
            dataTask = DependencyService.Get<IDataSubtask<Subtask>>();
            datatask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            GetToast = DependencyService.Get<IToast>();
            BindingContext = new AddSubtaskViewModel();
            //detaillabel4.TranslateTo(100, 0, 8000, Easing.Linear);
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(SubtaskId, out var result);
            // get the subtask having the result id
            var subtask = await dataTask.GetSubTaskAsync(result);

            GetSubtask = subtask;
            subName.Text = subtask.SubName;
            lblCreatedOn.Text = subtask.CreatedOn.ToString();
            lblSubStatus.Text = subtask.Status;
                 
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
                    Id = GetSubtask.Id,
                    SubName = subName.Text,
                    SubStart = GetSubtask.SubStart,

                    TaskId = GetSubtask.TaskId,
                    RemainingDays = remainingDays,
                    CreatedOn = GetSubtask.CreatedOn



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

                var newestSubtask = new Subtask
                {

                    Id = newSubtask.Id,
                    SubName = UppercasedName,
                    CreatedOn = GetSubtask.CreatedOn,
                    Percentage = GetSubtask.Percentage,
                    IsCompleted = GetSubtask.IsCompleted,
                    TaskId = newSubtask.TaskId,
                    Status = GetSubtask.Status
                };
                await dataTask.UpdateSubTaskAsync(newestSubtask);
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
    }
}