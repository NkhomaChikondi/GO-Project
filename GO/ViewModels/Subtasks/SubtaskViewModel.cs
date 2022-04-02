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
            var route = $"{nameof(AddSubtask)}?{nameof(AddSubtaskViewModel.GetTaskId)}={taskid}";
            await Shell.Current.GoToAsync(route);

        }
        async Task deleteSubTask(Subtask subtask)
        {
            if (subtask == null)
                return;
            await dataSubTask.DeleteSubTaskAsync(subtask.Id);
            await Refresh();
        }
        async Task OnUpdateTask(Subtask subtask)
        {
            var route = $"{nameof(UpdateSubtaskPage)}?SubtaskId={subtask.Id}";

            await Shell.Current.GoToAsync(route);
        }
        public async Task AddSubTaskPercentage(int IscompleteId, bool Iscomplete)
        {

            // get the Id in the database that matches iscompleteId
            var SubTaskId = await dataSubTask.GetSubTaskAsync(IscompleteId);

            // check against the specified condition
            if (SubTaskId.IsCompleted == true && Iscomplete == false)
            {
                // set to false if the condition meets
                SubTaskId.IsCompleted = false;
                //update the item in the database
                await dataSubTask.UpdateSubTaskAsync(SubTaskId);
                //remove percentage from the item
                RemovePercentage(IscompleteId);
                await CompletedSubtask(Taskid);
                await SetStatus();
                await SetStatus(IscompleteId);

            }
            // check against the specified condition
            else if (SubTaskId.IsCompleted == false && Iscomplete == true)
            {
                // set to true if the condition meets
                SubTaskId.IsCompleted = true;
                //update the item in the database
                await dataSubTask.UpdateSubTaskAsync(SubTaskId);
                // the percentage of the completed task
                var tasksid = SubTaskId.Percentage;
                // get the Task that is linked to the targeted subtask
                var Task = await dataTask.GetTaskAsync(Taskid);
                // check if the task is completed
                if (SubTaskId.IsCompleted == true)
                {
                    // pass the task percentage to goal
                    Task.PendingPercentage += tasksid;
                    // assign a value to Task Progress
                    Task.Progress = Task.PendingPercentage / Task.Percentage;
                    await dataTask.UpdateTaskAsync(Task);
                }
                await SetStatus();
                await SetStatus(IscompleteId);
                await CompletedSubtask(Taskid);



            }
            // check against the specified condition
            else if (SubTaskId.IsCompleted == false && Iscomplete == false)
            {
                // set to false if the condition meets
                SubTaskId.IsCompleted = false;
                //update the item in the database
                await dataSubTask.UpdateSubTaskAsync(SubTaskId);
                await SetStatus();
                await SetStatus(IscompleteId);
            }
        }
        async void RemovePercentage(int id)
        {
            //get the Task item having the passed id
            var Task = await dataTask.GetTaskAsync(Taskid);
            // get the subtask item having the passed id
            var Subtask = await dataSubTask.GetSubTaskAsync(id);
            // Subtract task percentage from goal percentage
            Task.PendingPercentage -= Subtask.Percentage;
            // assign a value to Task.progress
            Task.Progress = Task.PendingPercentage / 100;
            //update goal
            await dataTask.UpdateTaskAsync(Task);

        }
        // setting status for the targetted task
        async Task SetStatus()
        {
            // get a task having the same goal id
            var Tasks = await dataTask.GetTaskAsync(taskid);
            // loop through all the tasks

            if (Tasks.PendingPercentage <= 0)
            {
                Tasks.Status = "Not Started";

                await dataTask.UpdateTaskAsync(Tasks);
            }

            else if (Tasks.PendingPercentage != 0 && Tasks.PendingPercentage < Tasks.Percentage)
            {
                Tasks.Status = "In Progress";

                await dataTask.UpdateTaskAsync(Tasks);
            }
            else if (Tasks.PendingPercentage == Tasks.Percentage)
            {
                Tasks.Status = "Completed";

                await dataTask.UpdateTaskAsync(Tasks);
            }

        }
        // setting status for subtask
        async Task SetStatus(int subId)
        {
            // get a task having the same goal id
            var subTask = await dataSubTask.GetSubTaskAsync(subId);
            // loop through all the tasks

            if (subTask.Percentage <= 0)
            {
                subTask.Status = "Not Started";

                await dataSubTask.UpdateSubTaskAsync(subTask);
            }

            else if (subTask.Percentage != 0 && subTask.Percentage < subTask.Percentage)
            {
                subTask.Status = "In Progress";

                await dataSubTask.UpdateSubTaskAsync(subTask);
            
            }
            else if (subTask.Percentage == subTask.Percentage)
            {
                subTask.Status = "Completed";

                await dataSubTask.UpdateSubTaskAsync(subTask);
            }
        

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

        public async Task Refresh()
        {

            // set "IsBusy" to true
            IsBusy = true;
            // make the refreshing process load for 2 seconds
            await Task.Delay(2000);
            // clear Subtasks on the page
            subtasks.Clear();
            // get all Subtasks
            var Allsubtask = await dataSubTask.GetSubTasksAsync(Taskid);
            // retrieve the Subtasks back
            subtasks.AddRange(Allsubtask);
            // set "isBusy" to false
            IsBusy = false;

        }
    }
}
