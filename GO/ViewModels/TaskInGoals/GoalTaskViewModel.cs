using GO.Models;
using GO.ViewModels.Subtasks;
using GO.Views.GoalTask;
using GO.Views.SubTaskView;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.TaskInGoals
{

    [QueryProperty(nameof(GoalId), (nameof(GoalId)))]
    public class GoalTaskViewModel : BaseViewmodel
    {

        //I cant see where the ObserableRangeCollection/ObservableCor 

        #region Commands
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand OnAddCommand { get; }
        public AsyncCommand<GoalTask> OnUpdateCommand { get; }
        public AsyncCommand<EventArgs> ToggledCommand { get; set; }
        public AsyncCommand<GoalTask> SendTaskIdCommand { get; }
        public AsyncCommand<GoalTask> PercentageCommand { get; }
        public AsyncCommand<GoalTask> DeleteCommand { get; }
        #endregion

        #region observableCollections/PublicProperties
        public ObservableRangeCollection<GoalTask> goalTasks { get; }


        public int GoalId
        {
            get { return goalId; }
            set
            {
                goalId = value;
            }
        }



        public bool IsVisible
        {
            get => isVisible;
            set { isVisible = value; OnPropertyChange(); }
        }
        public bool IsVisibleTask
        {
            get => isVisibleTask;
            set { isVisibleTask = value; OnPropertyChange(); }
        }
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChange();
            }
        }
        public DateTime End
        {
            get => end;
            set
            {
                end = value;
                OnPropertyChange();
            }
        }
        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChange();
            }
        }
        public DateTime Start
        {
            get => start;
            set
            {
                start = value;
                OnPropertyChange();
            }
        }

        public double Percentage
        {
            get => percentage;
            set
            {
                percentage = value;
                OnPropertyChange();
            }
        }

        public double Progress
        {
            get => progress;
            set
            {
                progress = value;
            }
        }



        #endregion

        #region private properties

        private int goalId;
        private bool isVisible = true;
        private bool isVisibleTask = false;
        private string name;
        private DateTime start;
        private DateTime end;
        private string status;
        private double percentage;
        private double progress;
        private double roundedtask;
        private double standbypercentage;
        private double remainder;



        #endregion

        #region constractor
        public GoalTaskViewModel()
        {
            goalTasks = new ObservableRangeCollection<GoalTask>();
            RefreshCommand = new AsyncCommand(Refresh);
            OnAddCommand = new AsyncCommand(OnaddTask);
            OnUpdateCommand = new AsyncCommand<GoalTask>(OnUpdateTask);          
            SendTaskIdCommand = new AsyncCommand<GoalTask>(SendTaskId);
            DeleteCommand = new AsyncCommand<GoalTask>(deleteCategory);

            // PercentageCommand = new AsyncCommand<GoalTask>(AddPercentage);
            title = "Tasks";
        }
        #endregion

        #region Methods
        // pass the goal id to add task view model
        async Task OnaddTask()
        {
            var route = $"{nameof(AddTaskPage)}?{nameof(addTaskViewModel.GoalId)}={goalId}";
            await Shell.Current.GoToAsync(route);
        }

        // pass the goal id to update task method
        async Task OnUpdateTask(GoalTask goalTask)
        {
            var route = $"{nameof(UpdateTaskPage)}?taskId={goalTask.Id}";

            await Shell.Current.GoToAsync(route);
        }
        //pass the Task Id to subtask viewmodel
        async Task SendTaskId(GoalTask goalTask)
        {
            // get all subtasks havng the task id 
            var subtasks = await dataSubTask.GetSubTasksAsync(goalTask.Id);
            //if(subtasks.Count() == 0)
            //{
            //    var route1 = $"{nameof(BlankSubtaskView)}?{nameof(SubtaskViewModel.Taskid)}={goalTask.Id}";
            //    await Shell.Current.GoToAsync(route1);
            //}
         
                var route = $"{nameof(subTaskView)}?SubtaskId={goalTask.Id}";
                await Shell.Current.GoToAsync(route);
                       
        }
       
        public async Task CompleteTask(int TaskId,bool IsComplete)
        {
            // get the task having the same id as taskId
            var task = await dataTask.GetTaskAsync(TaskId);
            var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
            // check if the incoming object 
            if (task.IsCompleted)
                return;
            else if(!task.IsCompleted)
            {
                //check if it has subtask
                if (subtasks.Count() > 0)
                    return;
                else if(subtasks.Count() == 0)
                {
                    task.IsCompleted = IsComplete;
                    await dataTask.UpdateTaskAsync(task);
                }             
            
            }
            return;
        }
        public async Task UncompleteTask(int TaskId, bool IsComplete)
        {
            // get the task having the same id as taskId
            var task = await dataTask.GetTaskAsync(TaskId);
            var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
            // check if the incoming object 
            if (!task.IsCompleted)
                return;
            else if (task.IsCompleted)
            {
                //check if it has subtask
                if (subtasks.Count() > 0)
                    return;
                else if (subtasks.Count() == 0)
                {
                    task.IsCompleted = IsComplete;
                    await dataTask.UpdateTaskAsync(task);
                }
            }
            return;
        }
        async Task CalculateSubtaskPercentage()
        {
           
            // get all task having the goal id
            var tasks = await dataTask.GetTasksAsync(goalId);
            //loop through them
            foreach (var task in tasks)
            {
                // get all subtasks having the tasks id
                var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
                //check if there are subtasks having this task's id
                if(subtasks.Count() == 0)
                {
                    task.IsEnabled = true;

                }
                // set task.pending percentage to zero
                roundedtask = 0;
                // loop through the subtasks
                foreach (var subtask in subtasks)
                {
                    // check if they are completed
                    if(subtask.IsCompleted)
                    {
                       
                        roundedtask += subtask.Percentage;
                    }
                }
                //change a task pending pecentage to a rounded figure
             task.PendingPercentage =  Math.Round(roundedtask, 1);
                task.Progress = task.PendingPercentage / task.Percentage;
                await dataTask.UpdateTaskAsync(task);
                await SetStatus();
            }
        }
        async Task SetStatus()
        {
            // get a task having the same goal id
            var tasks = await dataTask.GetTasksAsync(goalId);
            // loop through all the tasks
            foreach (var task in tasks)
            {
                if (!task.IsCompleted && task.PendingPercentage == 0)
                    task.Status = "Not Started";

                else if (!task.IsCompleted && task.PendingPercentage > 0)
                    task.Status = "In Progress";

                else if (task.IsCompleted)
                    task.Status = "Completed";

                else if (DateTime.Now > task.EndTask)
                    task.Status = "Expired";
                  
                await dataTask.UpdateTaskAsync(task);
            }           


        }

        async Task deleteCategory(GoalTask goalTask)
        {
            if (goalTask == null)
                return;
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Task!", "All Subtasks in this Task will be deleted. Continue?", "Yes", "No");
            if(ans)
            {
                await dataTask.DeleteTaskAsync(goalTask.Id);
                // get all tasks in the database
                var tasks = await dataTask.GetTasksAsync(goalId);
                // loop through the tasks
                foreach (var task in tasks)
                {
                    task.Percentage = 100 / tasks.Count();
                    await dataTask.UpdateTaskAsync(task);

                }
                // get all subtasks having the deleted task id
                var subtasks = await dataSubTask.GetSubTasksAsync(goalTask.Id);
                // loop through them and delete
                foreach (var subtask in subtasks)
                {
                    await dataSubTask.DeleteSubTaskAsync(subtask.Id);
                }

                await Refresh();
            }
            else if (!ans)
                return;
          
        }
                  
        async Task Getremainingdays()
        {
            var tasks = await dataTask.GetTasksAsync(goalId);
            foreach (var task in tasks)
            {
                if (DateTime.Today <= task.EndTask)
                {
                    TimeSpan daysleft = task.EndTask - DateTime.Today;
                    task.RemainingDays = (int)daysleft.TotalDays;                   
                }
                else                
                    task.RemainingDays = 0;

               await dataTask.UpdateTaskAsync(task);
                
            }
        }
        public async Task Refresh()
        {
            // set "IsBusy" to true
            IsBusy = true;
            await CalculateSubtaskPercentage();
            goalTasks.Clear();
            // get all categories
            var tasks = await dataTask.GetTasksAsync(goalId);
            await Getremainingdays();
            
            //await SetStatus();
            // retrieve goals back
            goalTasks.AddRange(tasks);
            // set "isBusy" to false
            IsBusy = false;
        }



        #endregion

    }

}
