using GO.Models;
using GO.Views.SubTaskView;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.Subtasks
{
    [QueryProperty(nameof(GetTaskId), (nameof(GetTaskId)))]
    public class AddSubtaskViewModel : BaseViewmodel
    {
        private string name;
        private DateTime startDate = DateTime.Today;
        private DateTime endDate = DateTime.Today;
        private double duration;
        private double percentage;
        private int getTaskId;
        private int remainingDays = 0;
        private int percentageProgress = 0;
        public ObservableRangeCollection<Subtask> subTasks { get; }
        public AsyncCommand subTaskAddCommand { get; }
        public AddSubtaskViewModel()
        {
            subTasks = new ObservableRangeCollection<Subtask>();
            subTaskAddCommand = new AsyncCommand(AddSubtask);
        }

        public string Name { get => name; set => name = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public DateTime EndDate { get => endDate; set => endDate = value; }
        public double Duration { get => duration; set => duration = value; }
        public int GetTaskId { get { return getTaskId; } set => getTaskId = value; }
        public double Percentage { get => percentage; set => percentage = value; }
        public int RemainingDays { get => remainingDays; set => remainingDays = value; }

        async Task AddSubtask()
        {
            // check if the app is busy
            if (IsBusy == true)
                return;
            try
            {
                // create a new goal object and save
                var newSubtask = new Subtask
                {
                    SubName = name,
                    CreatedOn = DateTime.Now,
                    SubStart = startDate,
                    SubEnd = endDate,
                    TaskId = getTaskId,
                    RemainingDays = remainingDays,
                    Percentage = percentage


                };
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newSubtask.SubName[0]) + newSubtask.SubName.Substring(1);
                //check if the new task already exist in the database
                if (subTasks.Any(T => T.SubName == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                    return;
                }
                // get task from the task table through the given Id
                var subTaskInTask = await dataTask.GetTaskAsync(GetTaskId);
                // get the goal the task comes from
                var goal = await datagoal.GetGoalAsync(subTaskInTask.GoalId);
                if (goal.Noweek)
                {
                    // verify if the Start date and end date are within the duration of its selected goal
                    if (newSubtask.SubStart >= subTaskInTask.StartTask && newSubtask.SubEnd <= subTaskInTask.EndTask)
                    {
                        TimeSpan ts = newSubtask.SubEnd - newSubtask.SubStart;
                        RemainingDays = (int)ts.TotalDays;

                    }

                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", " make sure the Start date and end date are within the duration of its selected Task", "OK");
                        return;
                    }
                    // pass the uppercased name to the category object
                    var newestSubtask = new Subtask
                    {

                        Id = newSubtask.Id,
                        SubName = UppercasedName,
                        SubStart = newSubtask.SubStart,
                        SubEnd = newSubtask.SubEnd,
                        RemainingDays = remainingDays,
                        CreatedOn = newSubtask.CreatedOn,
                        Percentage = newSubtask.Percentage,
                        IsCompleted = false,
                        TaskId = newSubtask.TaskId,
                        Status = "Not Completed"
                    };
                    await dataSubTask.AddSubTaskAsync(newestSubtask);
                    await RecalculateTaskPercentage(newestSubtask.TaskId);
                    // await TaskEnabled();               
                    await totalSubtask(newestSubtask.TaskId);
                    AddSubTaskPercent();
                    setVisibility();

                    // go back to the previous page
                    await Shell.Current.GoToAsync("..");
                }
                else if (goal.HasWeek)
                {

                    var newestSubtask = new Subtask
                    {

                        Id = newSubtask.Id,
                        SubName = UppercasedName,
                        CreatedOn = newSubtask.CreatedOn,
                        Percentage = newSubtask.Percentage,
                        IsCompleted = false,
                        TaskId = newSubtask.TaskId,
                        Status = "Not Completed"
                    };
                    await dataSubTask.AddSubTaskAsync(newestSubtask);
                    await RecalculateTaskPercentage(newestSubtask.TaskId);
                    // await TaskEnabled();               
                    await totalSubtask(newestSubtask.TaskId);
                    AddSubTaskPercent();
                    setVisibility();
                    // go back to the previous page                  
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add new goal: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        // a method to turn Task's enabled to false
        async Task TaskEnabled ()
        {
            // get the task similar to the passed task id of this page
            var task = await dataTask.GetTaskAsync(getTaskId);
            if (task.IsEnabled == true)
            {
                task.IsEnabled = false;
                task.IsCompleted = false;
                await dataTask.UpdateTaskAsync(task);
            }
            else if (task.IsEnabled == false)
                return;
        }
        async void AddSubTaskPercent()
        {
            // get all subtasks having the specified goal id
            var AllSubtask = await dataSubTask.GetSubTasksAsync(getTaskId);
            // get the task having the gettaskid
            var Task = await dataTask.GetTaskAsync(getTaskId);
            // get the total number of tasks 
            var AllSubTaskCount = AllSubtask.Count();
            // make sure the task count is not zero
            if (AllSubTaskCount == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", "make sure a subtask is created", "OK");
                return;
            }

            //loop through the task and add the percentage
            foreach (var task in AllSubtask)
            {
                var SubTaskpercentage = Task.Percentage / AllSubTaskCount;
                task.Percentage = SubTaskpercentage;
                await dataSubTask.UpdateSubTaskAsync(task);
             
              //  percentageProgress += task.Percentage;
            }
            
            // filter tasks to get subtasks that are completed
            var completedSubTasks = AllSubtask.Where(T => T.IsCompleted == true).ToList();
            // get a goal with the same id as task.goalid
            var goal = await datagoal.GetGoalAsync(Task.GoalId);
            if (completedSubTasks.Count() == 0)
            {
                return;
            }
            else if(completedSubTasks.Count() > 0)
            {
                // loop through the tasks to get their percentage
                foreach (var subtask in completedSubTasks)
                {
                    percentageProgress += (int)subtask.Percentage;
                }
                // a variable to temporaliry store a goal percentage
                double NewGoalpercentage = 0;
                if(goal.Percentage == 0)
                {
                    NewGoalpercentage = goal.Percentage;
                }
                else if(goal.Percentage > 0)
                {
                    // subtract the task pending percentage from its goal
                    NewGoalpercentage = goal.Percentage - Task.PendingPercentage;
                }
               
                Task.PendingPercentage = percentageProgress;
                Task.Progress = percentageProgress / Task.Percentage;
                await dataTask.UpdateTaskAsync(Task);

                // assign to goal a new percentage value
                goal.Percentage = NewGoalpercentage + Task.PendingPercentage;
                goal.Progress = goal.Percentage / goal.ExpectedPercentage;
                await datagoal.UpdateGoalAsync(goal);

                return;
            }       
        }
        async Task RecalculateTaskPercentage(int Id)
        {
            // get task having through the subtask's task id
            var Task = await dataTask.GetTaskAsync(Id);
            // get all subtasks having the Task id
            var subtasks = await dataSubTask.GetSubTasksAsync(Task.Id);
            // check if there is a subtask having the task id
            if (subtasks.Count() > 0)
            {
                // check if its task is not completed
                if (Task.IsCompleted == false)
                {
                    await TaskEnabled();
                    return;
                }
                    
                else if (Task.IsCompleted == true)
                {
                   

                    //// turn it to false
                    //Task.IsCompleted = false;
                    //await dataTask.UpdateTaskAsync(Task);

                    // get the goal that this task belong to
                    var goal = await datagoal.GetGoalAsync(Task.GoalId);
                    // check the number of subtask having the task id
                    if (subtasks.Count() >= 1)
                    {
                        // subtract the task percentage from the goal
                        goal.Percentage -= (int)Task.Percentage;
                        goal.Progress = goal.Percentage / goal.ExpectedPercentage;
                        await datagoal.UpdateGoalAsync(goal);
                        await TaskEnabled();
                    }
                    else if (subtasks.Count() > 1)
                        return;

                }
            }
            else
                return;
        }
        async void setVisibility()
        {
            // get all subtask having "this" task id
            var subtasks = await dataSubTask.GetSubTasksAsync(getTaskId);
            // get the task equivalent to this viewmodel task id
            var task = await dataTask.GetTaskAsync(getTaskId);
            // update the number of subtask in task
        
            await dataTask.UpdateTaskAsync(task);

        }

        public async Task totalSubtask(int taskid)
        {
            // get all subtasks whose percentages are equal to tasks
            var allSubtask = await dataSubTask.GetSubTasksAsync(taskid);
            // get the Task having Taskid id
            var Taskid = await dataTask.GetTaskAsync(taskid);
            var subtaskcount = allSubtask.Where(t => t.IsCompleted == true).ToList();
            var totalcount = subtaskcount.Count();
            Taskid.CompletedSubtask = totalcount;
            // send the count to task
            await dataTask.UpdateTaskAsync(Taskid);

        }
    }
}
