using GO.Models;
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

namespace GO.ViewModels.Subtasks
{
    [QueryProperty(nameof(Taskid), (nameof(Taskid)))]
    public class SubtaskViewModel : BaseViewmodel
    {

        private int taskid;
        private string status;
        private double standbypercentage;
        private string noTasks;

        private bool all = true;
        private bool notstarted;
        private bool inprogress;
        private bool completed;
        private bool duesoon;
        private bool expired;
        


        public bool All { get => all; set => all = value; }
        public bool Notstarted { get => notstarted; set => notstarted = value; }
        public bool Inprogress { get => inprogress; set => inprogress = value; }
        public bool Completed { get => completed; set => completed = value; }
        public bool Duesoon { get => duesoon; set => duesoon = value; }
        public bool Expired { get => expired; set => expired = value; }
        public int Taskid { get { return taskid; } set => taskid = value; }
        public string Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChange();
            }
        }

        public string NoTasks { get => noTasks; set => noTasks = value; }


        public ObservableRangeCollection<Subtask> subtasks { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand sendsubtaskidCommand { get; }
        public AsyncCommand<Subtask> DeleteCommand { get; }       
        public AsyncCommand<Subtask> OnUpdateCommand { get; }
        public AsyncCommand HelpCommand { get; }

        public SubtaskViewModel()
        {
            subtasks = new ObservableRangeCollection<Subtask>();
            RefreshCommand = new AsyncCommand(Refresh);
            sendsubtaskidCommand = new AsyncCommand(selectSubtaskItem);
            DeleteCommand = new AsyncCommand<Subtask>(deleteSubTask);
            OnUpdateCommand = new AsyncCommand<Subtask>(OnUpdateTask);
            title = "Add Sub Task";
            HelpCommand = new AsyncCommand(GotoHelpPage);
        }
        public async Task AllGoals()
        {
            all = true;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            expired = false;
            await Refresh();
        }
        public async Task NotstartedGoals()
        {
            all = false;
            notstarted = true;
            duesoon = false;
            inprogress = false;
            completed = false;
            expired = false;
            await Refresh();
        }
        public async Task InprogressGoals()
        {
            all = false;
            notstarted = false;
            duesoon = false;
            inprogress = true;
            expired = false;
            completed = false;
            await Refresh();
        }
        public async Task CompletedGoals()
        {
            all = false;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            expired = false;
            completed = true;
            await Refresh();
        }
        public async Task DuesoonGoals()
        {
            all = false;
            notstarted = false;
            duesoon = true;
            inprogress = false;
            expired = false;
            completed = false;
            await Refresh();
        }
        public async Task ExpiredGoals()
        {
            all = false;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            expired = true;
            completed = false;
            await Refresh();
        }
      
        async Task selectSubtaskItem()
        {
            // get the task having the result id
            var task = await dataTask.GetTaskAsync(taskid);
            // get the goal having 
            var goal = await datagoal.GetGoalAsync(task.GoalId);
            // check if the goal has a week
            if(goal.Noweek)
            {                
                var route = $"{nameof(AddSubtask)}?GetTaskId={taskid}";
                await Shell.Current.GoToAsync(route);
            }
            if(goal.HasWeek)
            {
                var route = $"{nameof(AddPlannedSubtask)}?{nameof(AddSubtaskViewModel.GetTaskId)}={taskid}";
                await Shell.Current.GoToAsync(route);
            }
        }
        async Task deleteSubTask(Subtask subtask)
        {
            if (subtask == null)
                return;
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Subtask!", "You are deleting the selected subtask. Continue?", "Yes", "No");
            if (ans)
            {
                await dataSubTask.DeleteSubTaskAsync(subtask.Id);
                //cancel notification
                LocalNotificationCenter.Current.Cancel(subtask.Id);
                // get the task with the passed task id
                var task = await dataTask.GetTaskAsync(Taskid);
                // get allsubtasks having the task id
                var allSubtasks = await dataSubTask.GetSubTasksAsync(Taskid);
                if (allSubtasks.Count() > 0)
                {
                    //loop through them
                    foreach (var stask in allSubtasks)
                    {
                        stask.Percentage = task.Percentage / allSubtasks.Count();
                        await dataSubTask.UpdateSubTaskAsync(stask);
                    }
                }
                else if (allSubtasks.Count() == 0)
                {
                    task.IsCompleted = false;
                    task.IsEnabled = true;
                    await dataTask.UpdateTaskAsync(task);
                }
                // check if the deleted subtask was completed
                if (subtask.IsCompleted)
                {
                    task.PendingPercentage -= subtask.Percentage;
                    task.PendingPercentage = Math.Round(task.PendingPercentage, 2);
                    task.Progress = task.PendingPercentage / task.Percentage;
                    await dataTask.UpdateTaskAsync(task);
                }
                await Refresh();
                Datatoast.toast("Subtask deleted");
            }
            else if (!ans)
                return;
            
        }
        async Task OnUpdateTask(Subtask subtask)
        {
            // get the subtask having the subtask id
            var Subtask = await dataSubTask.GetSubTaskAsync(subtask.Id);
            // get the task having the result id
            var task = await dataTask.GetTaskAsync(Subtask.TaskId);
            // get the goal having 
            var goal = await datagoal.GetGoalAsync(task.GoalId);
            if(goal.HasWeek)
            {
                var route = $"{nameof(UpdateWeekSubtask)}?SubtaskId={subtask.Id}";
                await Shell.Current.GoToAsync(route);
            }
            else if(goal.Noweek)
            {
                var route = $"{nameof(UpdateSubtaskPage)}?SubtaskId={subtask.Id}";
                await Shell.Current.GoToAsync(route);
            }    
        }
        async Task GotoHelpPage()
        {
            var route = $"{nameof(HelpSubtaskpage)}";
            await Shell.Current.GoToAsync(route);
        }
        public async Task CompleteSubtask(int SubtaskId, bool IsComplete)
        {
            // get the subtask equal to subtaskId
            var subtask = await dataSubTask.GetSubTaskAsync(SubtaskId);
            // get task having this subtask
            var taskOfSubtask = await dataTask.GetTaskAsync(subtask.TaskId);
           
            if (subtask.IsCompleted)
                return;
            else if(!subtask.IsCompleted)
            {
                if (DateTime.Today < subtask.SubStart)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "You cannot complete a subtask whose start date has not been reached yet", "Ok");
                    await Refresh();
                    return;
                }
                else
                {
                    // check if it has expired
                    if (taskOfSubtask.Status == "Expired")
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to complete this subtask. Its task has expired.", "Ok");
                        return;
                    }
                    // check if the subtask has expired
                    if (subtask.Status == "Expired")
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", " Failed to complete this subtask. It has expired.", "Ok");
                        return;
                    }
                    // get the task equal to to allSubtasks taskId
                    var task = await dataTask.GetTaskAsync(subtask.TaskId);
                    // check if task dayis is greater than zero
                    if (task.DowId > 0)
                    {
                        // get the day for the task
                        var day = await dataDow.GetDOWAsync(task.DowId);
                        // get the week for the dow
                        var week = await dataWeek.GetWeekAsync(day.WeekId);
                        if (week.Active)
                        {
                            // check if day is valid
                            if (DateTime.Today.Date < task.StartTask.Date || DateTime.Today.Date > task.StartTask.Date)
                            {
                                await Application.Current.MainPage.DisplayAlert("Error!", "failed to complete this subtask. the day it was allocated to, has either passed or not reached yet.", "Ok");
                                await Refresh();
                                return;
                            }
                            else
                            {
                                subtask.IsCompleted = IsComplete;
                                subtask.Status = "Completed";
                                await dataSubTask.UpdateSubTaskAsync(subtask);
                                await CheckTaskCompletion(task);
                                await setStatus();
                                await GetCompletedTasks();
                            }

                        }
                        else
                        {
                            //await Refresh();
                            await Application.Current.MainPage.DisplayAlert("Error!", "Failed to complete this subtask. It has expired!", "Ok");
                            return;
                        }
                    }
                    else
                    {
                        // check if all allSubtasks have been completed and turn the task.iscompleted to true
                        subtask.IsCompleted = IsComplete;
                        subtask.Status = "Completed";
                        await dataSubTask.UpdateSubTaskAsync(subtask);
                        await CheckTaskCompletion(task);
                        await GetCompletedTasks();
                    }
                }             
            }            
        }      
        public async Task UnCompleteSubtask(int SubtaskId, bool IsComplete)
        {
            // get the subtask equal to subtaskId
            var subtask = await dataSubTask.GetSubTaskAsync(SubtaskId);
            // get task having this subtask
            var taskOfSubtask = await dataTask.GetTaskAsync(subtask.TaskId);
           
            if (!subtask.IsCompleted)
                return;

            else if (subtask.IsCompleted)
            {
                // check if it has expired
                if (taskOfSubtask.Status == "Expired")
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Failed to Uncomplete a subtask. Its task has expired.", "Ok");
                    return;
                }
                // check if the subtask has expired
                if (subtask.Status == "Expired")
                {
                    await Application.Current.MainPage.DisplayAlert("Error", " Failed to uncomplete a subtask. It has expired.", "Ok");
                    return;
                }
                // get the task equal to to allSubtasks taskId
                var task = await dataTask.GetTaskAsync(subtask.TaskId);
                // check if task dayis is greater than zero
                if (task.DowId > 0)
                {
                    // get the day for the task
                    var day = await dataDow.GetDOWAsync(task.DowId);
                    // get the week for the dow
                    var week = await dataWeek.GetWeekAsync(day.WeekId);
                    if (week.Active)
                    {
                        subtask.IsCompleted = IsComplete;
                        subtask.Status = "Uncompleted";
                        await dataSubTask.UpdateSubTaskAsync(subtask);
                        await CheckTaskCompletion(task);
                        await setStatus();
                        await GetCompletedTasks();
                    }
                    else
                    {
                       // await Refresh();
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to uncomplete a subtask. It has expired!", "Ok");                      
                        return;
                    }

                }
                else
                {
                    subtask.IsCompleted = IsComplete;
                    subtask.Status = "Uncompleted";
                    await dataSubTask.UpdateSubTaskAsync(subtask);
                    await CheckTaskCompletion(task);
                    await GetCompletedTasks();
                }
            }
                
        }
        // setting status for subtask
        async Task CheckTaskCompletion(GoalTask task)
        {            
            // get the allSubtasks that are in there
            var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
            //check if all allSubtasks are completed
            if (subtasks.All(t => t.IsCompleted))
                task.IsCompleted = true;
            else if (subtasks.Any(t => !t.IsCompleted))
                task.IsCompleted = false;
            await dataTask.UpdateTaskAsync(task);
        }      
        async Task CalculateSubtaskPercentage()
        {
                       // get the task 
            var task = await dataTask.GetTaskAsync(taskid);
            // get all allSubtasks having the task id
            var subtasks = await dataSubTask.GetSubTasksAsync(taskid);
            // loop through the allSubtasks
            foreach(var subtask in subtasks)
            {
                subtask.Percentage = task.Percentage / subtasks.Count();
                await dataSubTask.UpdateSubTaskAsync(subtask);
                //if(subtask.IsCompleted)
                //{
                //    completedTaskpercent += subtask.Percentage;
                //}
            }
            //task.PendingPercentage = Math.Round(completedTaskpercent, 2);
            //task.Progress = task.PendingPercentage / task.Percentage;
            //await dataSubtask.UpdateTaskAsync(task);
        }
        async Task setStatus()
        {
           var Allsubtask = await dataSubTask.GetSubTasksAsync(Taskid);
            // loop the tasks
            foreach (var subtask in Allsubtask)
            {
                if (!subtask.IsCompleted && DateTime.Today.Date <= subtask.SubEnd.Date)
                    subtask.Status = "Uncompleted";               

                else if (subtask.IsCompleted && DateTime.Today.Date <= subtask.SubEnd.Date)
                    subtask.Status = "Completed";

                else if (DateTime.Today.Date > subtask.SubEnd.Date)
                    subtask.Status = "Expired";

                await dataSubTask.UpdateSubTaskAsync(subtask);
            }
        }
        async Task GetCompletedTasks()
        {
            double subtaskpercentage = 0;
            // get all allSubtasks having the task id
            var subtasks = await dataSubTask.GetSubTasksAsync(taskid);
            // get the task having the taskid
            var task = await dataTask.GetTaskAsync(taskid);
            //get completed allSubtasks
            var completedSubtasks = subtasks.Where(c => c.IsCompleted).ToList();
            // loop through the task
            foreach (var subtask in  completedSubtasks)
            {
                subtaskpercentage += subtask.Percentage;
            }
            task.Progress = subtaskpercentage / task.Percentage;
            task.PendingPercentage = Math.Round(subtaskpercentage,2);
            await dataTask.UpdateTaskAsync(task);
        }
        public async Task Refresh()
        {
            // set "IsBusy" to true
            IsBusy = true;                          
            // clear Subtasks on the page
            subtasks.Clear();
            await setStatus();
            await CalculateSubtaskPercentage();
            // get all Subtasks
            var Allsubtask = await dataSubTask.GetSubTasksAsync(Taskid);
            // loop through the allSubtasks and check if the subtask has expired
            foreach (var subtask in Allsubtask)
            {
                if (DateTime.Today.Date > subtask.SubEnd.Date)
                    subtask.Status = "Expired";
                await dataSubTask.UpdateSubTaskAsync(subtask);
            }
            if (all)
                // retrieve the categories back
                subtasks.AddRange(Allsubtask);
            //filter goals
            else if (notstarted)
            {
                var notstartedtasks = Allsubtask.Where(g => !g.IsCompleted).ToList();                
                    subtasks.AddRange(notstartedtasks);
            }
            else if (completed)
            {
                var completedtasks = Allsubtask.Where(g => g.IsCompleted).ToList();
                
                subtasks.AddRange(completedtasks);
            }
            else if (inprogress)
            {
                var inprogressTasks = Allsubtask.Where(g => g.Status == "In Progress").ToList();
                subtasks.AddRange(inprogressTasks);
            }
            else if (duesoon)
            {
                var Date10 = DateTime.Today.AddDays(2);
                var duesoongoals = Allsubtask.Where(g => g.SubEnd <= Date10).ToList();
                subtasks.AddRange(duesoongoals);
            }
            else if (expired)
            {
                var expiredTasks = Allsubtask.Where(g => g.Status == "Expired").ToList();
                subtasks.AddRange(expiredTasks);
            }           
            // set "isBusy" to false
            IsBusy = false;

        }
    }
} 

