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
        public int taskid;
        Models.GoalTask GetGoalTask;
        public IDataSubtask<Subtask> datasubtask { get; }
        public IDataTask<Models.GoalTask> datatask { get; }
        public subTaskView()
        {
            InitializeComponent();
            datasubtask = DependencyService.Get<IDataSubtask<Subtask>>();
            datatask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            BindingContext = new SubtaskViewModel();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(SubtaskId, out var result);
            taskid = result;
            // get all subtasks having the tasks id
            var subtasks = await datasubtask.GetSubTasksAsync(result);
            // check if the subtask is from weekly taskly task or not
            // get the task from which this subtasks are created
            var Task = await datatask.GetTaskAsync(result);
            GetGoalTask = Task;
            if(subtasks.Count() == 0)
            {
                headsubtask.IsVisible = false;
                StackSubBlank.IsVisible = true;
                StackSublist.IsVisible = false;
                subtasktopRow.IsVisible = false;                
            }
            else
            {
                StackSublist.IsVisible = true;
                StackSubBlank.IsVisible = false;
                subtasktopRow.IsVisible = true;
                headsubtask.IsVisible = true;
                taskname.Text = Task.taskName;
                 getsubtaskPercentage(subtasks,Task);
                //subtodaydate.Text = DateTime.Today.Date.ToString("dd MMMM yyyy");
                if (DateTime.Today < Task.EndTask)
                {
                    TimeSpan daysleft = Task.EndTask - DateTime.Today;
                    var DaysLeft = (int)daysleft.TotalDays;
                    Subtaskdaysleft.Text = DaysLeft.ToString();
                }
                else if (DateTime.Today == Task.EndTask)
                {
                    var DaysLeft = 1;
                    Subtaskdaysleft.Text = DaysLeft.ToString();
                }
                else
                {
                    var DaysLeft = 0;
                    Subtaskdaysleft.Text = DaysLeft.ToString();
                }

            }
            //ball.BackgroundColor = Color.LightGray;
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
                // get all completed tasks
                var subtasks = await datasubtask.GetSubTasksAsync(Subtask.TaskId);
                var subtaskCompleted = subtasks.Where(T => T.IsCompleted).ToList();
                //completedsubtasks.Text = subtaskCompleted.Count().ToString();
                getsubtaskPercentage(subtasks, GetGoalTask);
            }
            else if (!Subtask.IsCompleted)
            {
                if (BindingContext is SubtaskViewModel viewModel)
                    await viewModel.UnCompleteSubtask(Subtaskid, Subtask.IsCompleted);
                // get all completed tasks
                var subtasks = await datasubtask.GetSubTasksAsync(Subtask.TaskId);
                var subtaskCompleted = subtasks.Where(T => T.IsCompleted == false).ToList();
              //  completedsubtasks.Text = subtaskCompleted.Count().ToString();
                getsubtaskPercentage(subtasks, GetGoalTask);
            }
            return;
        }
         
         void getsubtaskPercentage(IEnumerable<Subtask> subtasks, Models.GoalTask goalTask)
         {
            var subtaskPercentage = 0.0;
            // get only completes subtasks
            var completedSubtasks = subtasks.Where(t => t.IsCompleted).ToList();
            if(completedSubtasks.Count() == 0)
            {
                subtasktotalpercentage.Text = "0";
                taskprogress.Progress = 0;
            }
            else 
            {
                // loop through the completed subtasks to get the total accumulated percentage
                foreach (var subtask in completedSubtasks)
                {
                    subtaskPercentage += subtask.Percentage;
                }
                var TaskRoundedPercentage = Math.Round(subtaskPercentage, 2);
                subtasktotalpercentage.Text = TaskRoundedPercentage.ToString();
                taskprogress.Progress = TaskRoundedPercentage / goalTask.Percentage;
            }          
         }
        //private async void ball_Clicked(object sender, EventArgs e)
        //{
        //    bnotstarted.BackgroundColor = Color.Transparent;
        //    ball.BackgroundColor = Color.LightGray;
        //    bcompleted.BackgroundColor = Color.Transparent;
        // //   binprogress.BackgroundColor = Color.Transparent;
        //    bduesoon.BackgroundColor = Color.Transparent;
        //    bexpired.BackgroundColor = Color.Transparent;
        //    var subtasks = await datasubtask.GetSubTasksAsync(taskid);
        //    if (BindingContext is SubtaskViewModel bvm)
        //    {
        //        await bvm.AllGoals();
        //    }
        //}

        //private async void bnotstarted_Clicked(object sender, EventArgs e)
        //{           
        //    bnotstarted.BackgroundColor = Color.LightGray;
        //    ball.BackgroundColor = Color.Transparent;
        //    bcompleted.BackgroundColor = Color.Transparent;
        //    //binprogress.BackgroundColor = Color.Transparent;
        //    bduesoon.BackgroundColor = Color.Transparent;
        //    bexpired.BackgroundColor = Color.Transparent;
        //    var subtasks = await datasubtask.GetSubTasksAsync(taskid);
        //    // get all subtasks not started
        //    var notStartedsubtasks = subtasks.Where(s => !s.IsCompleted).ToList();
        //    if (notStartedsubtasks.Count() == 0)
        //    {
        //        noSubtasks.Text = " They are no uncompleted Subtasks!";
        //        if (BindingContext is SubtaskViewModel bvm)
        //        {
        //            await bvm.NotstartedGoals();
        //        }
        //    }               
        //    else
        //    {
        //        noSubtasks.Text = "";
        //        if (BindingContext is SubtaskViewModel bvm)
        //        {
        //            await bvm.NotstartedGoals();
        //        }
        //    }
           
        //}
       
        //private async void bcompleted_Clicked(object sender, EventArgs e)
        //{
        //    bnotstarted.BackgroundColor = Color.Transparent;
        //    ball.BackgroundColor = Color.Transparent;
        //    bcompleted.BackgroundColor = Color.LightGray;
        //   // binprogress.BackgroundColor = Color.Transparent;
        //    bduesoon.BackgroundColor = Color.Transparent;
        //    bexpired.BackgroundColor = Color.Transparent;
        //    var subtasks = await datasubtask.GetSubTasksAsync(taskid);
        //    var completedsubtasks = subtasks.Where(t => t.IsCompleted).ToList();
        //    if(completedsubtasks.Count() == 0)
        //    {
        //        noSubtasks.Text = " They are no completed tasks!";
        //        if (BindingContext is SubtaskViewModel bvm)
        //        {
        //            await bvm.CompletedGoals();
        //        }
        //    }
        //    else
        //    {
        //        noSubtasks.Text = "";
        //        if (BindingContext is SubtaskViewModel bvm)
        //        {
        //            await bvm.CompletedGoals();
        //        }
        //    }
           
        //}

        //private async void bduesoon_Clicked(object sender, EventArgs e)
        //{
        //    bnotstarted.BackgroundColor = Color.Transparent;
        //    ball.BackgroundColor = Color.Transparent;
        //    bcompleted.BackgroundColor = Color.Transparent;
        //   // binprogress.BackgroundColor = Color.Transparent;
        //    bduesoon.BackgroundColor = Color.LightGray;
        //    bexpired.BackgroundColor = Color.Transparent;
        //    var subtasks = await datasubtask.GetSubTasksAsync(taskid);
        //    var Date10 = DateTime.Today.AddDays(2);
        //    var duesoonsubtasks = subtasks.Where(g => g.SubEnd <= Date10).ToList();
        //    if(duesoonsubtasks.Count()==0)
        //    {
        //        noSubtasks.Text = "They are subtaks that are due soon!";
        //        if (BindingContext is SubtaskViewModel bvm)
        //        {
        //            await bvm.DuesoonGoals();
        //        }
        //    }
        //    else 
        //    {
        //        noSubtasks.Text = "";
        //        if (BindingContext is SubtaskViewModel bvm)
        //        {
        //            await bvm.DuesoonGoals();
        //        }
        //    }          
        //}

        //private async void bexpired_Clicked(object sender, EventArgs e)
        //{
        //    bnotstarted.BackgroundColor = Color.Transparent;
        //    ball.BackgroundColor = Color.Transparent;
        //    bcompleted.BackgroundColor = Color.Transparent;
        //  //  binprogress.BackgroundColor = Color.Transparent;
        //    bduesoon.BackgroundColor = Color.Transparent;
        //    bexpired.BackgroundColor = Color.LightGray;
        //    var subtasks = await datasubtask.GetSubTasksAsync(taskid);
        //    foreach (var subtask in subtasks)
        //    {
        //        if (DateTime.Now > subtask.SubEnd)
        //            subtask.Status = "Expired";
        //        //await datasubtask.UpdateSubTaskAsync(subtask);
        //    }
        //    var expiredsubtasks = subtasks.Where(e => e.Status.Equals("Expired")).ToList();
        //    if(expiredsubtasks.Count() == 0)
        //    {
        //        noSubtasks.Text = "They are no subtasks that have expired!";
        //        if (BindingContext is SubtaskViewModel bvm)
        //        {
        //            await bvm.ExpiredGoals();
        //        }
        //    }
        //    else
        //    {
        //        noSubtasks.Text = "";
        //        if (BindingContext is SubtaskViewModel bvm)
        //        {
        //            await bvm.ExpiredGoals();
        //        }
        //    }           
        //}
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.DisplayAlert("INFO", "* All task's subtasks will be listed on this page.\n \n * Scroll through the horizontal tab to filter the subtasks list according to your preference." +
               "\n \n * Each listed subtask will have \n  1. Name  \n  2. Toggle switch to either complete or uncomplete a subtask. \n  3. Due date" +
               "\n\n * Long press on a task to edit or delete.", "OK");
        }
    }
}