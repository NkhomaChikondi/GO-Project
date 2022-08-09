using GO.Models;
using GO.Views.SubTaskView;
using MvvmHelpers;
using MvvmHelpers.Commands;
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

        public ObservableRangeCollection<Subtask> subtasks { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand sendsubtaskidCommand { get; }
        public AsyncCommand<Subtask> DeleteCommand { get; }       
        public AsyncCommand<Subtask> OnUpdateCommand { get; }


        public SubtaskViewModel()
        {
            subtasks = new ObservableRangeCollection<Subtask>();
            RefreshCommand = new AsyncCommand(Refresh);
            sendsubtaskidCommand = new AsyncCommand(selectSubtaskItem);
            DeleteCommand = new AsyncCommand<Subtask>(deleteSubTask);
            OnUpdateCommand = new AsyncCommand<Subtask>(OnUpdateTask);
            title = "Add Sub Task";
            
        }


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

        async Task selectSubtaskItem()
        {
            // get the task having the result id
            var task = await dataTask.GetTaskAsync(taskid);
            // get the goal having 
            var goal = await datagoal.GetGoalAsync(task.GoalId);
            // check if the goal has a week
            if(goal.Noweek)
            {
                var route = $"{nameof(AddSubtask)}?{nameof(AddSubtaskViewModel.GetTaskId)}={taskid}";
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
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Subtask!", "All Subtasks will be deleted. Continue?", "Yes", "No");
            if (ans)
            {
                await dataSubTask.DeleteSubTaskAsync(subtask.Id);
                // get the task with the passed task id
                var task = await dataTask.GetTaskAsync(Taskid);
                // get allsubtasks having the task id
                var subtasks = await dataSubTask.GetSubTasksAsync(Taskid);
                //loop through them
                foreach (var stask in subtasks)
                {
                    stask.Percentage = task.Percentage / subtasks.Count();
                    await dataSubTask.UpdateSubTaskAsync(stask);
                }
                // check if the deleted subtask was completed
                if (subtask.IsCompleted)
                {
                    task.PendingPercentage -= subtask.Percentage;
                    await dataTask.UpdateTaskAsync(task);
                }
                await Refresh();
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
        public async Task CompleteSubtask(int SubtaskId, bool IsComplete)
        {
            // get the subtask equal to subtaskId
            var subtask = await dataSubTask.GetSubTaskAsync(SubtaskId);

            if (subtask.IsCompleted)
                return;
            else if(!subtask.IsCompleted)
            {
                subtask.IsCompleted = IsComplete;
                subtask.Status = "Completed";
                await dataSubTask.UpdateSubTaskAsync(subtask);
                await CheckTaskCompletion();
            }
            // check if all subtasks have been completed and turn the task.iscompleted to true


        }
        public async Task UnCompleteSubtask(int SubtaskId, bool IsComplete)
        {
            // get the subtask equal to subtaskId
            var subtask = await dataSubTask.GetSubTaskAsync(SubtaskId);

            if (!subtask.IsCompleted)
                return;
            else if (subtask.IsCompleted)
            {
                subtask.IsCompleted = IsComplete;
                subtask.Status = "Not Completed";
                await dataSubTask.UpdateSubTaskAsync(subtask);
                await CheckTaskCompletion();
            }
        }

        // setting status for subtask
        async Task CheckTaskCompletion()
        {
            // get the tasks having the TaskId
            var task = await dataTask.GetTaskAsync(taskid);
            // get the subtasks that are in there
            var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
            //check if all subtasks are completed
            if (subtasks.All(t => t.IsCompleted))
                task.IsCompleted = true;
            else if (!subtasks.All(t => t.IsCompleted))
                task.IsCompleted = false;
            await dataTask.UpdateTaskAsync(task);
        }
        async Task CompletedSubtask(int taskid)
        {
            // get all subtasks whose percentages are equal to tasks
            var allSubtask = await dataSubTask.GetSubTasksAsync(taskid);
            // get the Task having Taskid id
            var Taskid = await dataTask.GetTaskAsync(taskid);
            var completedtask = allSubtask.Where(s => s.IsCompleted == true).ToList();
            var totalcount = completedtask.Count();
            Taskid.CompletedSubtask = totalcount;
            await dataTask.UpdateTaskAsync(Taskid);
        }
       async Task setToDefaultPage()
        {
            // get the tasks from which this tasks derived from
            var task = await dataTask.GetTaskAsync(taskid);
            // get the goal from which the task derive from
            var goal = await datagoal.GetGoalAsync(task.GoalId);
            if(goal.HasWeek)
            {

            }
        }
        public async Task Refresh()
        {
            // set "IsBusy" to true
            IsBusy = true;                          
            // clear Subtasks on the page
            subtasks.Clear();
            // get all Subtasks
            var Allsubtask = await dataSubTask.GetSubTasksAsync(Taskid);
            // loop through the subtasks and check if the subtask has expired
            foreach (var subtask in Allsubtask)
            {
                if (DateTime.Now > subtask.SubEnd)
                    subtask.Status = "Expired";
            }
            // retrieve the Subtasks back
            subtasks.AddRange(Allsubtask);
            var task = await dataTask.GetTaskAsync(taskid);
            // get the goal from which the task derive from
            var goal = await datagoal.GetGoalAsync(task.GoalId);
            //if (goal.HasWeek)
            //{
            //    if(Allsubtask.Count() == 0)
            //    {
            //        var route1 = $"{nameof(BlankWeekSubtaskView)}?{nameof(SubtaskViewModel.Taskid)}={taskid}";
            //        await Shell.Current.GoToAsync(route1);
            //    }                
            //}
            //else if(goal.Noweek)
            //{
            //    if(Allsubtask.Count() == 0)
            //    {
            //        var route1 = $"{nameof(BlankSubtaskView)}?{nameof(SubtaskViewModel.Taskid)}={taskid}";
            //        await Shell.Current.GoToAsync(route1);
            //    }
               
            //}
            // set "isBusy" to false
            IsBusy = false;

        }
    }
} 

