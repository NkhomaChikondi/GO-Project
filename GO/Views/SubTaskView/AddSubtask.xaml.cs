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
  //  [QueryProperty(nameof(GetTaskId), nameof(GetTaskId))]
    public partial class AddSubtask : ContentPage
    {
        //public string GetTaskId { get; set; }
        //public IDataSubtask<Subtask> datasub { get; }
        //public IDataGoal<Models.Goal> datagoal { get; }
        //public IDataTask<Models.GoalTask> dataTask { get; }

        public AddSubtask() 
        {
            InitializeComponent();
           // BindingContext = new  AddSubtaskViewModel();
            //datasub = DependencyService.Get<IDataSubtask<Subtask>>();
            //datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            //dataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    int.TryParse(GetTaskId, out var result);
        //    // get the task having the result id
        //    var task = await dataTask.GetTaskAsync(result);
        //    // get the goal having 
        //    var goal = await datagoal.GetGoalAsync(task.GoalId);
        //    // check if the goal has a week
        //    if(goal.HasWeek)
        //    {
        //        Durationlbl.IsVisible = false;
        //        StartDateStack.IsVisible = false;
        //        Enddatestack.IsVisible  = false;
        //        noweekStack.IsVisible = false;
        //        HasweekStack.IsVisible = true;
        //    }
        //    if(BindingContext is AddSubtaskViewModel subtaskviewmodel)
        //    {
        //        subtaskviewmodel.GetTaskId = result;
        //    }

        //}
    }
}