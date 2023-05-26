using GO.Models;
using GO.ViewModels.Subtasks;
using GO.Views.GoalTask;
using GO.Views.SubTaskView;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.TaskInGoals
{

    //[QueryProperty(nameof(GoalId), (nameof(GoalId)))]
    public class GoalTaskViewModel : BaseViewmodel
    {
        //I cant see where the ObserableRangeCollection/ObservableCor 
        #region Commands
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand OnAddCommand { get; }
        public AsyncCommand<GoalTask> OnUpdateCommand { get; }
        public AsyncCommand<int> PrevDates { get; }
        public AsyncCommand<EventArgs> ToggledCommand { get; set; }
        public AsyncCommand<GoalTask> SendTaskIdCommand { get; }
        public AsyncCommand<GoalTask> PercentageCommand { get; }
        public AsyncCommand<GoalTask> DeleteCommand { get; }
        public AsyncCommand HelpCommand { get; }
        #endregion

        #region observableCollections/PublicProperties
        public ObservableRangeCollection<GoalTask> goalTasks { get; }

        public DateTime selectedDate;
        public int GoalId
        {
            get { return goalId; }
            set
            {
                goalId = value;
            }
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
        private bool all = true;
        private bool notstarted;
        private bool inprogress;
        private bool completed;
        private bool duesoon;
        private bool expired;
        private bool withSubtask;
       


        public bool All { get => all; set => all = value; }
        public bool Notstarted { get => notstarted; set => notstarted = value; }
        public bool Inprogress { get => inprogress; set => inprogress = value; }
        public bool Completed { get => completed; set => completed = value; }
        public bool Duesoon { get => duesoon; set => duesoon = value; }
        public bool Expired { get => expired; set => expired = value; }
        public bool WithSubtask { get => withSubtask; set => withSubtask = value; }
     
        public DateTime SelectedDate { get => selectedDate; set => selectedDate = value; }


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
            HelpCommand = new AsyncCommand(GotoHelpPage);
            // PercentageCommand = new AsyncCommand<GoalTask>(AddPercentage);
            title = "Tasks";
        }
        #endregion

        #region Methods
        public async Task prevdate(DateTime dateTime)
        {
            selectedDate = selectedDate.AddMonths(1);
        }
        public async Task AllGoals()
        {
            all = true;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            expired = false;
            withSubtask = false;
            await Refresh();
        }
        public async Task NotstartedTasks()
        {
            all = false;
            notstarted = true;
            duesoon = false;
            inprogress = false;
            completed = false;
            expired = false;
            withSubtask = false;
            await Refresh();
        }
        public async Task InprogressTasks()
        {
            all = false;
            notstarted = false;
            duesoon = false;
            inprogress = true;
            expired = false;
            completed = false;
            withSubtask = false;
            await Refresh();
        }
        public async Task CompletedTasks()
        {
            all = false;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            expired = false;
            withSubtask = false;
            completed = true;
            await Refresh();
        }
        public async Task DuesoonTasks()
        {
            all = false;
            notstarted = false;
            duesoon = true;
            inprogress = false;
            expired = false;
            completed = false;
            withSubtask = false;
            await Refresh();
        }
        public async Task ExpiredTasks()
        {
            all = false;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            expired = true;
            withSubtask = false;
            completed = false;
            await Refresh();
        }
        public async Task WithSubtasksTasks()
        {
            all = false;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            expired = false;
            withSubtask = true;
            completed = false;
            await Refresh();
        }
        // pass the goal id to add task view model
        async Task OnaddTask()
        {
            var route = $"{nameof(AddTaskPage)}?GetGoalId={GoalId}";
            await Shell.Current.GoToAsync(route);
        }
        async Task GotoHelpPage()
        {
            //var route = $"{nameof(helptaskPage)}";
            //await Shell.Current.GoToAsync(route);
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
            //check if the task has expired with no tasks
            if(goalTask.Status == "Expired" && subtasks.Count().Equals(0))
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Cannot go to Subtask page as the task expired with zero subtask","OK");
                return;
            }
            else 
            {
                var route = $"{nameof(subTaskView)}?SubtaskId={goalTask.Id}";
                await Shell.Current.GoToAsync(route);
            }
         
                       
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
                // check if the task has started 
                if(DateTime.Today < task.StartTask)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "You cannot mark a task as complete if its start date has not yet been reached.", "Ok");
                    await Refresh();
                    return;
                }
                else
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
        async Task AssignTaskPercentage()
        {
            double subtaskpercentage = 0;           
            // get all the tasks having the goalid
            var tasks = await dataTask.GetTasksAsync(goalId);
            // check if they are tasks having the week id
            if (tasks.Count() > 0)
            {
                // loop through the tasks to get their percentage
                foreach (var task in tasks)
                {
                    //calculate task percentage
                    task.Percentage = 100.0 / tasks.Count();
                
                    // get subtasks for this tasks
                    var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
                    task.SubtaskNumber = subtasks.Count();

                    if (subtasks.Count() > 0)
                    {
                        if (subtasks.Any(S => S.Status == "Uncompleted"))
                            task.IsCompleted = false;
                        if (subtasks.Any(S => S.IsCompleted))
                        {
                            // get all completed subtasks
                            var completedSubtasks = subtasks.Where(s => s.IsCompleted).ToList();
                            //loop through the subtasks
                            foreach (var subtask in completedSubtasks)
                            {
                                subtaskpercentage += subtask.Percentage;
                            }
                        }
                    }
                    else if(subtasks.Count() == 0)
                    {
                        if (!task.IsEnabled)
                            task.IsEnabled = true;
                    }
                    task.PendingPercentage = Math.Round(subtaskpercentage, 2);
                    task.Progress = task.PendingPercentage / task.Percentage;
                    await dataTask.UpdateTaskAsync(task);
                    subtaskpercentage = 0;                    
                }
                
            }
            else
                return;

        }
        async Task SetStatus()
        {
            // get a task having the same goal id
            var tasks = await dataTask.GetTasksAsync(goalId);
            // loop through all the tasks
            foreach (var task in tasks)
            {
                if (!task.IsCompleted && task.PendingPercentage == 0 && DateTime.Today <= task.EndTask)
                    task.Status = "Not Started";

                else if (!task.IsCompleted && task.PendingPercentage > 0 && DateTime.Today <= task.EndTask)
                    task.Status = "In Progress";

                else if (task.IsCompleted && DateTime.Today <= task.EndTask)
                    task.Status = "Completed";

                else if (DateTime.Today > task.EndTask)
                    task.Status = "Expired";
                  
                await dataTask.UpdateTaskAsync(task);
            }         
        }
        async Task deleteCategory(GoalTask goalTask)
        {
            if (goalTask == null)
                return;
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Task", "All Subtasks within this Task will be deleted. Continue?", "Yes", "No");
            if(ans)
            {
                await dataTask.DeleteTaskAsync(goalTask.Id);
                // cancel notificaton for this task
                LocalNotificationCenter.Current.Cancel(goalTask.Id);
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
                    // cancel notificaton
                    LocalNotificationCenter.Current.Cancel(subtask.Id);
                }

                await Refresh();
                Datatoast.toast("Task deleted");
            }
            else if (!ans)
                return;
          
        }
     
        public async Task Refresh()
        {
            // set "IsBusy" to true
            IsBusy = true;
            var tasks = await dataTask.GetTasksAsync(goalId);           
            goalTasks.Clear();
            //await Getremainingdays();
            await AssignTaskPercentage();
            await SetStatus();
           

            if (all)
                // retrieve the categories back
                goalTasks.AddRange(tasks);
            //filter goals
            else if (notstarted)
            {
                var notstartedtasks = tasks.Where(g => g.Status == "Not Started").ToList();
                if (notstartedtasks.Count == 0)
                    Datatoast.toast("No tasks!");
                else
                goalTasks.AddRange(notstartedtasks);
            }
            else if (completed)
            {
                var completedtasks = tasks.Where(g => g.Percentage == g.PendingPercentage).ToList();
                if (completedtasks.Count == 0)
                    Datatoast.toast("No tasks!");
                else
                goalTasks.AddRange(completedtasks);
            }
            else if (inprogress)
            {
                var inprogressTasks = tasks.Where(g => g.PendingPercentage > 0 && g.Status != "Expired").ToList();
                if (inprogressTasks.Count == 0)
                    Datatoast.toast("No tasks!");
                goalTasks.AddRange(inprogressTasks);
            }
            else if (duesoon)
            {
                var Date10 = DateTime.Today.AddDays(10);
                var duesoongoals = tasks.Where(g => g.EndTask <= Date10 && g.Status != "Expired").ToList();
                if (duesoongoals.Count == 0)
                    Datatoast.toast("No tasks!");
                goalTasks.AddRange(duesoongoals);
            }
            else if (expired)
            {
                var expiredTasks = tasks.Where(g => g.Status == "Expired").ToList();
                if (expiredTasks.Count == 0)
                    Datatoast.toast("No tasks!");
                else
                goalTasks.AddRange(expiredTasks);
            }
            else if (withSubtask)
            {
                List<GoalTask> tasklist = new List<GoalTask>();
                //loop through the tasks
                foreach (var Task in tasks)
                {
                    // get tasks that have subtasks
                    var subtasks = await dataSubTask.GetSubTasksAsync(Task.Id);
                    if (subtasks.Count() > 0)
                    {
                        tasklist.Add(Task);
                    }
                }
                if (tasklist.Count == 0)
                    Datatoast.toast("No tasks!");
                else                   
                goalTasks.AddRange(tasklist);
            }
            // set "isBusy" to false
            IsBusy = false;
        }
        #endregion
    }
}
