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
    public partial class subTaskView : ContentPage
    {
        public string SubtaskId { get; set; }
        public Subtask Subtask = new Subtask();
        public IDataSubtask<Subtask> dataTask { get; }
        public IDataTask<Models.GoalTask> datatask { get; }
        public subTaskView()
        {
            InitializeComponent();
            dataTask = DependencyService.Get<IDataSubtask<Subtask>>();
            datatask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            BindingContext = new SubtaskViewModel();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(SubtaskId, out var result);
            // get all subtasks having the tasks id
            var subtasks = await dataTask.GetSubTasksAsync(result);
            if(subtasks.Count() == 0)
            {
                StackSubBlank.IsVisible = true;
                StackSublist.IsVisible = false;
            }
            else
            {
                StackSublist.IsVisible = true;
                StackSubBlank.IsVisible = false;
            }
            if (BindingContext is SubtaskViewModel cvm)
            {
                cvm.Taskid = result;
                await cvm.Refresh();

            }
        }
        private async void switchTask_Toggled(object sender, ToggledEventArgs e)
        {

            Switch @switch = (Switch)sender;
            var Subtask = (Models.Subtask)@switch.BindingContext;
            var Subtaskid = Subtask.Id;
            if (Subtask.IsCompleted)
            {
                if (BindingContext is SubtaskViewModel viewModel)
                    await viewModel.CompleteSubtask(Subtaskid, Subtask.IsCompleted);

            }
            else if (!Subtask.IsCompleted)
            {
                if (BindingContext is SubtaskViewModel viewModel)
                    await viewModel.UnCompleteSubtask(Subtaskid, Subtask.IsCompleted);

            }
            return;

        }


    }
}