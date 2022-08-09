using GO.Models;
using GO.ViewModels.Subtasks;
using GO.Views.GoalTask;
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

namespace GO.ViewModels.TaskInGoals
{
    [QueryProperty(nameof(GoalId), nameof(GoalId))]
    public class addTaskViewModel : BaseViewmodel
    {
        private int goalId;
        private string name;
        private DateTime starttime = DateTime.Now;
        private DateTime endtime = DateTime.Now;
        private string description;
        private int remainingDays = 0;
        private double percentageProgress = 0;
        private int newgoalpercent = 0;
        private double taskPercentage;
        private double totalPendingPercentage;
        private int selectedItem;
        private int DayId;
        public List<DOWPicker> dowpicker { get; set; }
        public string DowName { get; set; }
        public AsyncCommand TaskAddCommand { get; }
        public AsyncCommand TaskAddWeekCommand { get; }
        public ObservableRangeCollection<Goal> goals { get; }
        public ObservableRangeCollection<GoalTask> goalTasks { get; }
        public AsyncCommand<GoalTask> SendTaskIdCommand { get; }

        public addTaskViewModel()
        {
            TaskAddCommand = new AsyncCommand(AddTask);
            TaskAddWeekCommand = new AsyncCommand(AddTaskWeek);
            goals = new ObservableRangeCollection<Goal>();
            goalTasks = new ObservableRangeCollection<GoalTask>();
            SendTaskIdCommand = new AsyncCommand<GoalTask>(SendTaskId);
            GetDows();
        }

        public int GoalId { get { return goalId; } set => goalId = value; }

        public string Name { get => name; set => name = value; }
        public DateTime Endtime { get => endtime; set => endtime = value; }
        public DateTime Starttime { get => starttime; set => starttime = value; }
        public string Description { get => description; set => description = value; }
        public int RemainingDays { get => remainingDays; set => remainingDays = value; }
        public int SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                if (!selectedItem.Equals(null))
                    selectedDow(value);

            }
        }
        
        public async Task SendTaskId(GoalTask goalTask)
        {
            var route = $"{nameof(subTaskView)}?{nameof(SubtaskViewModel.Taskid)}={goalTask.Id}";
            await Shell.Current.GoToAsync(route);
        }

        public void GetDows()
        {
            dowpicker = new List<DOWPicker>
            {
                new DOWPicker{ Key =1,Name = "Sunday"},
                new DOWPicker{Key = 2, Name ="Monday"},
                new DOWPicker {Key = 3, Name ="Tuesday"},
                new DOWPicker { Key =4 , Name = "Wednesday"},
                new DOWPicker {Key = 5, Name = "Thursday"},
                new DOWPicker {Key = 6, Name = "Friday"},
                new DOWPicker{Key = 7, Name = "Saturday"}
            };
        }
        async Task selectedDow(int id)
        {
            // add 1 to the id
            id += 1;
            var getdow = await dataDow.GetDOWAsync(id);

            DayId = getdow.DOWId;

        }
       

        async Task AddTaskWeek()
        {
            // check if the application is busy
            if (IsBusy == true)
                return;
            try
            {
                // set the application IsBusy to true
                IsBusy = true;
                // create a new task object
                var newtask = new GoalTask
                {
                    taskName = name,
                    StartTask = starttime,
                    EndTask = endtime,
                    RemainingDays = remainingDays,
                    Percentage = 0,
                    Description = description,
                    GoalId = goalId
                };
                // get all tasks in GoalId
                var alltasks = await dataTask.GetTasksAsync(goalId);
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newtask.taskName[0]) + newtask.taskName.Substring(1);
                //check if the new task already exist in the database
                if (alltasks.Any(T => T.taskName == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                    return;
                }
                // get goal from the goal table through the given Id
                var TaskInGoalId = await datagoal.GetGoalAsync(goalId);
              
                if (newtask.Description == null)
                    newtask.Description = $"No Description for {newtask.taskName}";
                //call the add percentage method
                await AddweekTaskPercentage();
                // check if goal has week or not
                // get last inserted week in "this" goal
                var week = await dataWeek.GetWeeksAsync(goalId);
                // get the last inserted week
                var lastweek = week.ToList().LastOrDefault();
                if (DateTime.Today >= lastweek.StartDate && DateTime.Today <= lastweek.EndDate)
                {
                    // set lastinsertedWeek to active
                    lastweek.Active = true;
                    // update the database
                    await dataWeek.UpdateWeekAsync(lastweek);
                }

                    var newestTask = new GoalTask
                {
                    taskName = UppercasedName,
                    StartTask = starttime,
                    EndTask = endtime,
                    RemainingDays = remainingDays,
                    GoalId = goalId,
                    IsCompleted = false,
                    Description = newtask.Description,
                    PendingPercentage = 0,
                    Percentage = taskPercentage,
                    Status = "Not Started",
                    CompletedSubtask = 0,
                    IsEnabled = true,
                    CreatedOn = DateTime.Now,
                    IsVisible = true,
                    WeekId = lastweek.Id,
                    DowId = DayId,
                    IsNotVisible = false
                };
                //check if the task already exist so you can either save or update
                if (alltasks.Any(t => t.Id == newestTask.Id))
                {
                    await dataTask.UpdateTaskAsync(newestTask);
                }
                #region check if task has been assigned a percentage
                // counter value
                int counter = 0;
                // check if task percent has been assigned to task's percentage

                while (counter < 3 && newestTask.Percentage == 0)
                {
                    await AddweekTaskPercentage();
                    newestTask.Percentage = taskPercentage;
                    counter++;
                }

                if (counter == 3 && newestTask.Percentage == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "Failed to add Task, retry", "Ok");
                    return;
                }
                #endregion

                // add the new task to the database                
                await dataTask.AddTaskAsync(newestTask);
                // call the add percentage method
                AddTaskPercent();

                // go back to the task list page
                var route = $"{nameof(WeeklyTask)}?goalId={goalId}";
                await Shell.Current.GoToAsync(route);
                
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
        async Task AddTask()
        {
            // check if the application is busy
            if (IsBusy == true)
                return;
            try
            {
                // set the application IsBusy to true
                IsBusy = true;
                // create a new task object
                var newtask = new GoalTask
                {
                    taskName = name,
                    StartTask = starttime,
                    EndTask = endtime,
                    RemainingDays = remainingDays,
                    Percentage = 0,
                    Description = description,
                    GoalId = goalId

                };
                // get all tasks in GoalId
                var alltasks = goalTasks.Where(T => T.GoalId == GoalId).ToList();
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newtask.taskName[0]) + newtask.taskName.Substring(1);
                //check if the new task already exist in the database
                if (alltasks.Any(T => T.taskName == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                    return;
                }
                // get goal from the goal table through the given Id
                var TaskInGoalId = await datagoal.GetGoalAsync(goalId);
                // verify if the Start date and end date are within the duration of its selected goal
                if (newtask.StartTask >= TaskInGoalId.Start && newtask.EndTask <= TaskInGoalId.End)
                {
                    TimeSpan ts = newtask.EndTask - newtask.StartTask;
                    RemainingDays = (int)ts.TotalDays;

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", " make sure the Start date and end date are within the duration of its selected goal", "OK");
                    return;
                }
                if (newtask.Description == null)
                    newtask.Description = $"No Description for {newtask.taskName}";

                //call the add percentage method
                await AddPercentage();
                // check if goal has week or not
                // get the goalid for this view model

                var newestTask = new GoalTask
                {
                    taskName = UppercasedName,
                    StartTask = starttime,
                    EndTask = endtime,
                    RemainingDays = remainingDays,
                    GoalId = goalId,
                    IsCompleted = false,
                    Description = newtask.Description,
                    PendingPercentage = 0,
                    Percentage = taskPercentage,
                    Status = "Not Started",
                    CompletedSubtask = 0,
                    IsEnabled = true,
                    CreatedOn = DateTime.Now,
                    IsVisible = true,
                    IsNotVisible = false
                };
                //check if the task already exist so you can either save or update
                if (alltasks.Any(t => t.Id == newestTask.Id))
                {
                    await dataTask.UpdateTaskAsync(newestTask);
                }
                #region check if task has been assigned a percentage
                // counter value
                int counter = 0;
                // check if task percent has been assigned to task's percentage

                while (counter < 3 && newestTask.Percentage == 0)
                {
                    await AddPercentage();
                    newestTask.Percentage = taskPercentage;
                    counter++;
                }

                if (counter == 3 && newestTask.Percentage == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "Failed to add Task, retry", "Ok");
                    return;
                }
                #endregion

                // add the new task to the database                
                await dataTask.AddTaskAsync(newestTask);
                // call the add percentage method
                AddTaskPercent();
                // go back to the task list page
               
                await Shell.Current.GoToAsync("..");
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
        async Task AddweekTaskPercentage()
        {
            // get all tasks having "this" goal id from the database 
            var tasks = await dataTask.GetTasksAsync(goalId);
            // get weeks that are in this goal
            var weeks = await dataWeek.GetWeeksAsync(goalId);
            // get the active week
            var activeWeek = weeks.ToList().LastOrDefault();
            // get the total number of tasks 
            double AllTaskCount = tasks.Count() + 1;
            taskPercentage = activeWeek.TargetPercentage / AllTaskCount;

        }
        // a method to assign percentage to the task
        async Task AddPercentage()
        {
            // get all tasks having "this" goal id from the database 
            var tasks = await dataTask.GetTasksAsync(goalId);
            // get the total number of tasks 
            double AllTaskCount = tasks.Count() + 1;
            // divide 100 percent by the number of tasks
            taskPercentage = 100.0 / AllTaskCount;

        }
        // a method to reassign percentage to all tasks in the database
        async void AddTaskPercent()
        {
            // get all tasks having the specified goal id
            var Alltask = await dataTask.GetTasksAsync(goalId);
            // get the goal having the task id
            var goal = await datagoal.GetGoalAsync(goalId);
            // set the percentage progress to zero
            percentageProgress = 0;
            //loop through the task and add the percentage
            foreach (var task in Alltask)
            {
                task.Percentage = taskPercentage;
                await dataTask.UpdateTaskAsync(task);
                // get subtasks having the task id
                var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
                if (subtasks.Count() > 0)
                {
                    // loop through the subtasks and assign new percentages
                    foreach (var subtask in subtasks)
                    {
                        subtask.Percentage = task.Percentage / subtasks.Count();
                        await dataSubTask.UpdateSubTaskAsync(subtask);

                    }
                }

            }

        }
        async Task SetStatus(int goalid)
        {
            // get a goal having the same goal id
            var goal = await datagoal.GetGoalAsync(goalid);

            if (goal.Percentage <= 0)
            {
                goal.Status = "Not Started";

                await datagoal.UpdateGoalAsync(goal);
                // await Refresh();
            }

            else if (goal.Percentage < goal.ExpectedPercentage)
            {
                goal.Status = "In Progress";

                await datagoal.UpdateGoalAsync(goal);
                // await Refresh();
            }
            else if (goal.Percentage == goal.ExpectedPercentage)
            {
                goal.Status = "Completed";

                await datagoal.UpdateGoalAsync(goal);
                // await Refresh();
            }
            else
            {
                goal.Status = "Expired";

                await datagoal.UpdateGoalAsync(goal);
                //  await Refresh();
            }

        }

    }



    
}
