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
    [QueryProperty(nameof(goalId), nameof(goalId))]
    public partial class GoalTaskPage : ContentPage
    {

        public string goalId { get; set; }
        private int GoalId;
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataSubtask<Subtask> datasubtask { get; }
        public IDataWeek<Models.Week> dataWeek { get; }
        public GoalTaskPage()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataWeek = DependencyService.Get<IDataWeek<Models.Week>>();
            datasubtask = DependencyService.Get<IDataSubtask<Subtask>>();
            DataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();           
            BindingContext = new GoalTaskViewModel();


        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(goalId, out var result);
           
            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(result);
            GoalId = goal.Id;
            // get all tasks having the goal id
            var tasks = await DataTask.GetTasksAsync(goal.Id);
            if (tasks.Count() == 0)
            {
                StackTasklist.IsVisible = false;                
                StackTaskBlank.IsVisible = true;
                tasktoprow.IsVisible = false;             
            }
            else
            {
                btall.BackgroundColor = Color.LightGray;
                StackTaskBlank.IsVisible = false;
                tasktoprow.IsVisible = true;
                StackTasklist.IsVisible = true;                
            }
            btall.BackgroundColor = Color.LightGray;
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

        private async void btall_Clicked(object sender, EventArgs e)
        {
            btnotstarted.BackgroundColor = Color.Transparent;
            btall.BackgroundColor = Color.LightGray;
            btcompleted.BackgroundColor = Color.Transparent;
            btinprogress.BackgroundColor = Color.Transparent;
            btduesoon.BackgroundColor = Color.Transparent;
            btexpired.BackgroundColor = Color.Transparent;
            btwithsubtasks.BackgroundColor = Color.Transparent;
            if (BindingContext is GoalTaskViewModel bvm)
            {
                await bvm.AllGoals();
            }
        }

        private async void btnotstarted_Clicked(object sender, EventArgs e)
        {

            btnotstarted.BackgroundColor = Color.LightGray;
            btall.BackgroundColor = Color.Transparent;
            btcompleted.BackgroundColor = Color.Transparent;
            btinprogress.BackgroundColor = Color.Transparent;
            btduesoon.BackgroundColor = Color.Transparent;
            btexpired.BackgroundColor = Color.Transparent;
            btwithsubtasks.BackgroundColor = Color.Transparent;
            var tasks = await DataTask.GetTasksAsync(GoalId);
            // get all subtasks not started
            var notstartedTasks = tasks.Where(s => s.Status == "Not Started").ToList();
            if (notstartedTasks.Count() == 0)
            {
                noTasks.Text = " They are no Started tasks!";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.NotstartedTasks();
                }
            }
            else
            {
                noTasks.Text = "";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.NotstartedTasks();
                }
            }
        }

        private async void btinprogress_Clicked(object sender, EventArgs e)
        {

            btnotstarted.BackgroundColor = Color.Transparent;
            btall.BackgroundColor = Color.Transparent;
            btcompleted.BackgroundColor = Color.Transparent;
            btinprogress.BackgroundColor = Color.LightGray;
            btduesoon.BackgroundColor = Color.Transparent;
            btexpired.BackgroundColor = Color.Transparent;
            btwithsubtasks.BackgroundColor = Color.Transparent;
            var tasks = await DataTask.GetTasksAsync(GoalId);
            // get all subtasks not started
            var InprogressTasks = tasks.Where(s => s.PendingPercentage >0).ToList();
            if (InprogressTasks.Count() == 0)
            {
                noTasks.Text = " They are no tasks in progress!";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.InprogressTasks();
                }
            }
            else
            {
                noTasks.Text = "";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.InprogressTasks();
                }
            }
        }

        private async void btncompleted_Clicked(object sender, EventArgs e)
        {

            btnotstarted.BackgroundColor = Color.Transparent;
            btall.BackgroundColor = Color.Transparent;
            btcompleted.BackgroundColor = Color.LightGray;
            btinprogress.BackgroundColor = Color.Transparent;
            btduesoon.BackgroundColor = Color.Transparent;
            btexpired.BackgroundColor = Color.Transparent;
            btwithsubtasks.BackgroundColor = Color.Transparent;
            var tasks = await DataTask.GetTasksAsync(GoalId);
            // get all subtasks not started
            var CompletedTasks = tasks.Where(s => s.IsCompleted).ToList();
            if (CompletedTasks.Count() == 0)
            {
                noTasks.Text = " They are no tasks that are completed!";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.CompletedTasks();
                }
            }
            else
            {
                noTasks.Text = "";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.CompletedTasks();
                }
            }
        }

        private async void btduesoon_Clicked(object sender, EventArgs e)
        {
            btnotstarted.BackgroundColor = Color.Transparent;
            btall.BackgroundColor = Color.Transparent;
            btcompleted.BackgroundColor = Color.Transparent;
            btinprogress.BackgroundColor = Color.Transparent;
            btduesoon.BackgroundColor = Color.LightGray;
            btexpired.BackgroundColor = Color.Transparent;
            btwithsubtasks.BackgroundColor = Color.Transparent;

            var tasks = await DataTask.GetTasksAsync(GoalId);
            var Date10 = DateTime.Today.AddDays(10);
            var duesoongoals = tasks.Where(g => g.EndTask <= Date10 && g.Status != "Expired").ToList();
            if (duesoongoals.Count() == 0)
            {
                noTasks.Text = " They are no tasks that are Due soon!";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.DuesoonTasks();
                }
            }
            else
            {
                noTasks.Text = "";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.DuesoonTasks();
                }
            }
        }

        private async void btexpired_Clicked(object sender, EventArgs e)
        {
            btnotstarted.BackgroundColor = Color.Transparent;
            btall.BackgroundColor = Color.Transparent;
            btcompleted.BackgroundColor = Color.Transparent;
            btinprogress.BackgroundColor = Color.Transparent;
            btduesoon.BackgroundColor = Color.Transparent;
            btexpired.BackgroundColor = Color.LightGray;
            btwithsubtasks.BackgroundColor = Color.Transparent;
            var tasks = await DataTask.GetTasksAsync(GoalId);
            // get all subtasks not started
            var expiredTasks = tasks.Where(s => s.Status == "Expired").ToList();
            if (expiredTasks.Count() == 0)
            {
                noTasks.Text = " They are no Expired tasks!";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.ExpiredTasks();
                }
            }
            else
            {
                noTasks.Text = "";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.ExpiredTasks();
                }
            }
        }

        private async void btwithsubtasks_Clicked(object sender, EventArgs e)
        {
            btnotstarted.BackgroundColor = Color.Transparent;
            btall.BackgroundColor = Color.Transparent;
            btcompleted.BackgroundColor = Color.Transparent;
            btinprogress.BackgroundColor = Color.Transparent;
            btduesoon.BackgroundColor = Color.Transparent;
            btexpired.BackgroundColor = Color.Transparent;
            btwithsubtasks.BackgroundColor = Color.LightGray;
            List<Models.GoalTask> tasklist = new List<Models.GoalTask>();
            var tasks = await DataTask.GetTasksAsync(GoalId);
            //loop through the tasks
            foreach (var Task in tasks)
            {
                // get tasks that have subtasks
                var subtasks = await datasubtask .GetSubTasksAsync(Task.Id);
                if (subtasks.Count() > 0)
                {
                    tasklist.Add(Task);
                }
            }
            if (tasklist.Count() == 0)
            {
                noTasks.Text = " They are no tasks with subtasks!";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.WithSubtasksTasks();
                }
            }
            else
            {
                noTasks.Text = "";
                if (BindingContext is GoalTaskViewModel bvm)
                {
                    await bvm.WithSubtasksTasks();
                }
            }
        }
    }

}
