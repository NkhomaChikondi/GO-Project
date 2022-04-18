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

    [QueryProperty(nameof(GoalTaskId), (nameof(GoalTaskId)))]
    public class GoalTaskViewModel : BaseViewmodel
    {

        //I cant see where the ObserableRangeCollection/ObservableCor 

        #region Commands
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand OnAddCommand { get; }
        public AsyncCommand<GoalTask> OnUpdateCommand { get; }
        public AsyncCommand<GoalTask> SendTaskIdCommand { get; }
        public AsyncCommand<GoalTask> PercentageCommand { get; }
        public AsyncCommand<GoalTask> DeleteCommand { get; }
        #endregion

        #region observableCollections/PublicProperties
        public ObservableRangeCollection<GoalTask> goalTasks { get; }


        public int GoalTaskId
        {
            get { return goalTaskId; }
            set
            {
                goalTaskId = value;
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

        private int goalTaskId;
        private bool isVisible = true;
        private bool isVisibleTask = false;
        private string name;
        private DateTime start;
        private DateTime end;
        private string status;
        private double percentage;
        private double progress;



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
            var route = $"{nameof(AddTaskPage)}?{nameof(addTaskViewModel.GoalId)}={goalTaskId}";
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
            var route = $"{nameof(subTaskView)}?{nameof(SubtaskViewModel.Taskid)}={goalTask.Id}";
            await Shell.Current.GoToAsync(route);
        }
        // get all tasks
        //public List<GoalTask> alltasks = new List<GoalTask>();

        public async Task AddPercentage(int IscompleteId, bool Iscomplete)
        {
            // lets get all subtasks having 'this' task id
            var subtasks = await dataSubTask.GetSubTasksAsync(IscompleteId);
            // get the Id in the database that matches iscompleteId
            var TaskId = await dataTask.GetTaskAsync(IscompleteId);
            // get the Goal that is linked to the above task
            var Goal = await datagoal.GetGoalAsync(goalTaskId);
            if (subtasks.Count() == 0)
            {



                // check against the specified condition
                if (TaskId.IsCompleted == true && Iscomplete == false)
                {
                    // set to false if the condition meets
                    TaskId.IsCompleted = false;
                    //update the item in the database
                    await dataTask.UpdateTaskAsync(TaskId);
                    if (TaskId.IsCompleted == false)
                    {
                        //remove percentage from the item
                        RemovePercentage(IscompleteId);
                    }


                }
                // check against the specified condition
                else if (TaskId.IsCompleted == false && Iscomplete == true)
                {
                    // set to true if the condition meets
                    TaskId.IsCompleted = true;
                    //update the item in the database
                    await dataTask.UpdateTaskAsync(TaskId);
                    // the percentage of the completed task
                    var tasksid = TaskId.Percentage;

                    // check if the task is completed
                    if (TaskId.IsCompleted == true)
                    {
                        addPercentage(IscompleteId);

                    }



                }
                // check against the specified condition
                else if (TaskId.IsCompleted == false && Iscomplete == false)
                {
                    // set to false if the condition meets
                    TaskId.IsCompleted = false;
                    //update the item in the database
                    await dataTask.UpdateTaskAsync(TaskId);
                }

            }
            else if (subtasks.Count() > 0)
            {
                if (TaskId.IsCompleted == false && Iscomplete == true)
                {
                    await App.Current.MainPage.DisplayAlert("Alert!", "It will automatically switch itself on, after all subtasks are complete!", "Cancel");
                    return;
                }
                if (TaskId.PendingPercentage == TaskId.Percentage)
                {
                    if (TaskId.IsCompleted == false)
                    {
                        TaskId.IsCompleted = true;
                        //update task
                        await dataTask.UpdateTaskAsync(TaskId);
                        addPercentage(IscompleteId);
                    }
                }
                else if (TaskId.PendingPercentage != TaskId.Percentage)
                {
                    if (TaskId.IsCompleted == true)
                    {
                        TaskId.IsCompleted = false;
                        await dataTask.UpdateTaskAsync(TaskId);
                        RemovePercentage(IscompleteId);
                    }
                }

            }
        }
        async void RemovePercentage(int id)
        {
            //get the goal item having the passed id
            var goal = await datagoal.GetGoalAsync(goalTaskId);
            // get the task item having the passed id
            var task = await dataTask.GetTaskAsync(id);
            // Subtract task percentage from goal percentage
            goal.Percentage -= task.Percentage;
            if (goal.Percentage < 0)
                goal.Percentage = 0;
            // recalculate goal progress
            goal.Progress = goal.Percentage / 100;
            //update goal
            await datagoal.UpdateGoalAsync(goal);

        }
        async void addPercentage(int id)
        {
            //get the goal item having the passed id
            var goal = await datagoal.GetGoalAsync(goalTaskId);
            // get the task item having the passed id
            var task = await dataTask.GetTaskAsync(id);
            // make sure the percentage figure in goal is never less than zero
            if (goal.Percentage < 0)
                goal.Percentage = 0;
            // pass the task percentage to the variable
            goal.Percentage += task.Percentage;

            // update goal progress
            goal.Progress = goal.Percentage / 100;
            await datagoal.UpdateGoalAsync(goal);
        }
        async Task deleteCategory(GoalTask goalTask)
        {
            if (goalTask == null)
                return;
            await dataTask.DeleteTaskAsync(goalTask.Id);
            await Refresh();
        }
        async Task SubtaskNumber()
        {
            // get all task having the goaltaskId of goal
            var tasks = await dataTask.GetTasksAsync(goalTaskId);
            //loop through each task and get the number of subtasks in it
            foreach (var task in tasks)
            {

                if (task.SubtaskNumber == 0)
                {
                    IsVisible = true;
                    IsVisibleTask = false;
                }
                else if (task.SubtaskNumber > 0)
                {
                    IsVisibleTask = true;
                    IsVisible = false;
                }
            }
        }

        public async Task Refresh()
        {
            // set "IsBusy" to true
            IsBusy = true;
            // make the refreshing process load for 2 seconds
            await Task.Delay(2000);
            // clear categories on the page
            goalTasks.Clear();
            // get all categories
            var tasks = await dataTask.GetTasksAsync(goalTaskId);
            foreach (var task in tasks)
            {
                if (DateTime.Today <= task.EndTask)
                {
                    TimeSpan daysleft = task.EndTask - DateTime.Today;
                    task.RemainingDays = (int)daysleft.TotalDays;
                    await dataTask.UpdateTaskAsync(task);

                }

                else
                {
                    task.RemainingDays = 0;
                    await dataTask.UpdateTaskAsync(task);
                }
            }

            // retrieve the categories back
            goalTasks.AddRange(tasks);
            // set "isBusy" to false
            IsBusy = false;


        }



        #endregion

    }

}
