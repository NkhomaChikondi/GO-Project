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
    [QueryProperty(nameof(goalId), nameof(goalId))]
    public partial class GoalTaskPage : ContentPage
    {

        public string goalId { get; set; }
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataWeek<Models.Week> dataWeek { get; }
        public GoalTaskPage()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataWeek = DependencyService.Get<IDataWeek<Models.Week>>();
            DataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();           
            BindingContext = new GoalTaskViewModel();


        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(goalId, out var result);
           
            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(result);
            // get all tasks having the goal id
            var tasks = await DataTask.GetTasksAsync(goal.Id);
            if (tasks.Count() == 0)
            {
                StackTasklist.IsVisible = false;
                StackTaskBlank.IsVisible = true;
            }
            else
            {
                StackTaskBlank.IsVisible = false;
                StackTasklist.IsVisible = true;
            }
            if (BindingContext is GoalTaskViewModel cvm)
            {
                cvm.GoalId = goal.Id;
                await cvm.Refresh();

            }
        }


        private async void switch_Toggled(object sender, ToggledEventArgs e)
        {

            Switch @switch = (Switch)sender;
            var task = (Models.GoalTask)@switch.BindingContext;
            var taskid = task.Id;
            if (task.IsCompleted)
            {
                if(BindingContext is GoalTaskViewModel viewModel)
                    await viewModel.CompleteTask(taskid, task.IsCompleted);

            }
            else if(!task.IsCompleted)
            {
                if (BindingContext is GoalTaskViewModel viewModel)
                    await viewModel.UncompleteTask(taskid, task.IsCompleted);

            }
            return;

        }


    }

}
