using GO.Models;
using GO.Services;
using GO.ViewModels.Subtasks;
using GO.ViewModels.TaskInGoals;
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
  [QueryProperty(nameof(GetTaskId), nameof(GetTaskId))]
    public partial class AddSubtask : ContentPage
    {
        public string GetTaskId { get; set; }
        public IDataSubtask<Subtask> datasub { get; }
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> datasubtask { get; }

        public AddSubtask() 
        {
            InitializeComponent();
            BindingContext = new AddSubtaskViewModel();
            datasub = DependencyService.Get<IDataSubtask<Subtask>>();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            datasubtask = DependencyService.Get<IDataTask<Models.GoalTask>>();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(GetTaskId, out var result);
            // get the task having the result id
            var task = await datasubtask.GetTaskAsync(result);
            if(BindingContext is AddSubtaskViewModel asv)
            {
                asv.GetTaskId = task.Id;
                asv.StartDate = task.StartTask;
                asv.EndDate = task.EndTask;
                subtaskenddate.Date = task.EndTask;
                subtaskstartdate.Date = task.StartTask;
            }

        }
    }
}