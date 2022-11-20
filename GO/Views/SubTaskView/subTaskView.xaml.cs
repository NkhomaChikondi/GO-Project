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
           
            if(subtasks.Count() == 0)
            {
                StackSubBlank.IsVisible = true;
                StackSublist.IsVisible = false;
                subtasktopRow.IsVisible = false;                
            }
            else
            {
                StackSublist.IsVisible = true;
                StackSubBlank.IsVisible = false;
                subtasktopRow.IsVisible = true;
            }
            ball.BackgroundColor = Color.LightGray;
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

        private async void ball_Clicked(object sender, EventArgs e)
        {
            bnotstarted.BackgroundColor = Color.Transparent;
            ball.BackgroundColor = Color.LightGray;
            bcompleted.BackgroundColor = Color.Transparent;
         //   binprogress.BackgroundColor = Color.Transparent;
            bduesoon.BackgroundColor = Color.Transparent;
            bexpired.BackgroundColor = Color.Transparent;
            var subtasks = await datasubtask.GetSubTasksAsync(taskid);
            if (BindingContext is SubtaskViewModel bvm)
            {
                await bvm.AllGoals();
            }
        }

        private async void bnotstarted_Clicked(object sender, EventArgs e)
        {           
            bnotstarted.BackgroundColor = Color.LightGray;
            ball.BackgroundColor = Color.Transparent;
            bcompleted.BackgroundColor = Color.Transparent;
            //binprogress.BackgroundColor = Color.Transparent;
            bduesoon.BackgroundColor = Color.Transparent;
            bexpired.BackgroundColor = Color.Transparent;
            var subtasks = await datasubtask.GetSubTasksAsync(taskid);
            // get all subtasks not started
            var notStartedsubtasks = subtasks.Where(s => !s.IsCompleted).ToList();
            if (notStartedsubtasks.Count() == 0)
            {
                noSubtasks.Text = " They are no uncompleted Subtasks!";
                if (BindingContext is SubtaskViewModel bvm)
                {
                    await bvm.NotstartedGoals();
                }
            }               
            else
            {
                noSubtasks.Text = "";
                if (BindingContext is SubtaskViewModel bvm)
                {
                    await bvm.NotstartedGoals();
                }
            }
           
        }
       
        private async void bcompleted_Clicked(object sender, EventArgs e)
        {
            bnotstarted.BackgroundColor = Color.Transparent;
            ball.BackgroundColor = Color.Transparent;
            bcompleted.BackgroundColor = Color.LightGray;
           // binprogress.BackgroundColor = Color.Transparent;
            bduesoon.BackgroundColor = Color.Transparent;
            bexpired.BackgroundColor = Color.Transparent;
            var subtasks = await datasubtask.GetSubTasksAsync(taskid);
            var completedsubtasks = subtasks.Where(t => t.IsCompleted).ToList();
            if(completedsubtasks.Count() == 0)
            {
                noSubtasks.Text = " They are no completed tasks!";
                if (BindingContext is SubtaskViewModel bvm)
                {
                    await bvm.CompletedGoals();
                }
            }
            else
            {
                noSubtasks.Text = "";
                if (BindingContext is SubtaskViewModel bvm)
                {
                    await bvm.CompletedGoals();
                }
            }
           
        }

        private async void bduesoon_Clicked(object sender, EventArgs e)
        {
            bnotstarted.BackgroundColor = Color.Transparent;
            ball.BackgroundColor = Color.Transparent;
            bcompleted.BackgroundColor = Color.Transparent;
           // binprogress.BackgroundColor = Color.Transparent;
            bduesoon.BackgroundColor = Color.LightGray;
            bexpired.BackgroundColor = Color.Transparent;
            var subtasks = await datasubtask.GetSubTasksAsync(taskid);
            var Date10 = DateTime.Today.AddDays(2);
            var duesoonsubtasks = subtasks.Where(g => g.SubEnd <= Date10).ToList();
            if(duesoonsubtasks.Count()==0)
            {
                noSubtasks.Text = "They are subtaks that are due soon!";
                if (BindingContext is SubtaskViewModel bvm)
                {
                    await bvm.DuesoonGoals();
                }
            }
            else 
            {
                noSubtasks.Text = "";
                if (BindingContext is SubtaskViewModel bvm)
                {
                    await bvm.DuesoonGoals();
                }
            }          
        }

        private async void bexpired_Clicked(object sender, EventArgs e)
        {
            bnotstarted.BackgroundColor = Color.Transparent;
            ball.BackgroundColor = Color.Transparent;
            bcompleted.BackgroundColor = Color.Transparent;
          //  binprogress.BackgroundColor = Color.Transparent;
            bduesoon.BackgroundColor = Color.Transparent;
            bexpired.BackgroundColor = Color.LightGray;
            var subtasks = await datasubtask.GetSubTasksAsync(taskid);
            foreach (var subtask in subtasks)
            {
                if (DateTime.Now > subtask.SubEnd)
                    subtask.Status = "Expired";
                //await datasubtask.UpdateSubTaskAsync(subtask);
            }
            var expiredsubtasks = subtasks.Where(e => e.Status.Equals("Expired")).ToList();
            if(expiredsubtasks.Count() == 0)
            {
                noSubtasks.Text = "They are no subtasks that have expired!";
                if (BindingContext is SubtaskViewModel bvm)
                {
                    await bvm.ExpiredGoals();
                }
            }
            else
            {
                noSubtasks.Text = "";
                if (BindingContext is SubtaskViewModel bvm)
                {
                    await bvm.ExpiredGoals();
                }
            }
           
        }
    }
}