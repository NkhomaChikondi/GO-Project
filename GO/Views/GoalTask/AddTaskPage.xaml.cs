using GO.Models;
using GO.Services;
using GO.ViewModels.TaskInGoals;
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
    [QueryProperty(nameof(GetGoalId), nameof(GetGoalId))]
    public partial class AddTaskPage : ContentPage
    {
        public string GetGoalId { get; set; }
        public IDataSubtask<Subtask> datasub { get; }
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> datasubtask { get; }
        public AddTaskPage()
        {
            InitializeComponent();
            BindingContext = new addTaskViewModel();
            datasub = DependencyService.Get<IDataSubtask<Subtask>>();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            datasubtask = DependencyService.Get<IDataTask<Models.GoalTask>>();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(GetGoalId, out var result);
            // get the task having the result id
            var goal = await datagoal.GetGoalAsync(result);
            if (BindingContext is addTaskViewModel atv)
            {
                atv.GoalId = goal.Id;
                atv.Starttime = goal.Start;
                atv.Endtime = goal.End;
                taskstartdate.Date = goal.Start;
                taskenddate.Date = goal.End;
            }
        }
    }
}