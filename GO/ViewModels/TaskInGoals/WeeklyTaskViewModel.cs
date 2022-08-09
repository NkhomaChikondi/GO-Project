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
    [QueryProperty(nameof(GoalId), nameof(GoalId))]
    public class WeeklyTaskViewModel : BaseViewmodel
    {


        private int goalId;
        private int dayNumber;
        private double roundedtask;
        public int GoalId { get => goalId; set => goalId = value; }
      
        public ObservableRangeCollection<DOW> dows { get; }
        public ObservableRangeCollection<GoalTask>dowTasks { get; }
       
        
        public ObservableRangeCollection<GoalTask> tasks { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand SunCommand { get; }
        public AsyncCommand MonCommand { get; }
        public AsyncCommand TueCommand { get; }
        public AsyncCommand WedCommand { get; }
        public AsyncCommand ThuCommand { get; }
        public AsyncCommand FriCommand { get; }
        public AsyncCommand SatCommand { get; }
        public AsyncCommand OnAddCommand { get; }
        public AsyncCommand<GoalTask> SendTaskIdCommand { get; }
        public AsyncCommand<GoalTask> OnUpdateCommand { get; }
        public AsyncCommand<GoalTask> DeleteCommand { get; }
        public int DayNumber { get => dayNumber; set => dayNumber = value; }
       

        public WeeklyTaskViewModel()
        {
            dows = new ObservableRangeCollection<DOW>();
            dowTasks = new ObservableRangeCollection<GoalTask>();
            RefreshCommand = new AsyncCommand(Refresh);
            OnAddCommand = new AsyncCommand(OnaddTask);
            SunCommand = new AsyncCommand(sunButton);
            MonCommand = new AsyncCommand(monButton);
            TueCommand = new AsyncCommand(tueButton);
            WedCommand = new AsyncCommand(wedButton);
            ThuCommand = new AsyncCommand(thuButton);
            FriCommand = new AsyncCommand(friButton);
            SatCommand = new AsyncCommand(satButton);
            SendTaskIdCommand = new AsyncCommand<GoalTask>(SendTaskId);
            OnUpdateCommand = new AsyncCommand<GoalTask>(OnUpdateTask);          
            DeleteCommand = new AsyncCommand<GoalTask>(deleteCategory);

        }
        // seed the days of the week into the database upon startup
        async Task CreateDOW()
        {
            // get week 
            // get the last active week of goal
            var getweeks = await dataWeek.GetWeeksAsync(GoalId);
            // get the last inserted week
            var lastInsertedWeek = getweeks.ToList().LastOrDefault();
            // check if the week is active
            if(lastInsertedWeek.Active)
            {
                // check if days having the goal id are already created
                var alldows = await dataDow.GetDOWsAsync();
                if (alldows.Count() > 0)
                    return;
                else if (alldows.Count() == 0)
                {

                    var DowSunday = new DOW
                    {
                        DOWId = 1,
                        Name = "Sunday",
                      
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowSunday);

                    var DowMonday = new DOW
                    {
                        Name = "Monday",
                       
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowMonday);

                    var DowTuesday = new DOW
                    {
                        Name = "Tuesday",
                      
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowTuesday);

                    var DowWednesday = new DOW
                    {
                        Name = "Wednesday",
                       
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowWednesday);


                    var DowThursday = new DOW
                    {
                        Name = "Thursday",
                      
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowThursday);

                    var DowFriday = new DOW
                    {
                        Name = "Friday",
                       
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowFriday);

                    var DowSaturday = new DOW
                    {
                        Name = "Saturday",
                       
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowSaturday);
                }

            }
            else if(!lastInsertedWeek.Active)
            {
                //check if the todays date is more than or equal to weeks start date and is less than or equal to end date
                if(DateTime.Today >= lastInsertedWeek.StartDate && DateTime.Today <= lastInsertedWeek.EndDate)
                {
                    // set lastinsertedWeek to active
                    lastInsertedWeek.Active = true;
                    // update the database
                    await dataWeek.UpdateWeekAsync(lastInsertedWeek);
                    // check if days having the week id are already created
                    var alldows = await dataDow.GetDOWsAsync();
                    if (alldows.Count() > 0)
                        return;
                    else if (alldows.Count() == 0)
                    {

                        var DowSunday = new DOW
                        {
                            DOWId = 1,
                            Name = "Sunday",
                         
                        };
                        // create the Dow Object
                        await dataDow.AddDOWAsync(DowSunday);

                        var DowMonday = new DOW
                        {
                            Name = "Monday",
                          
                        };
                        // create the Dow Object
                        await dataDow.AddDOWAsync(DowMonday);

                        var DowTuesday = new DOW
                        {
                            Name = "Tuesday",
                           
                        };
                        // create the Dow Object
                        await dataDow.AddDOWAsync(DowTuesday);

                        var DowWednesday = new DOW
                        {
                            Name = "Wednesday",
                            
                        };
                        // create the Dow Object
                        await dataDow.AddDOWAsync(DowWednesday);


                        var DowThursday = new DOW
                        {
                            Name = "Thursday",
                           
                        };
                        // create the Dow Object
                        await dataDow.AddDOWAsync(DowThursday);

                        var DowFriday = new DOW
                        {
                            Name = "Friday",
                           
                        };
                        // create the Dow Object
                        await dataDow.AddDOWAsync(DowFriday);

                        var DowSaturday = new DOW
                        {
                            Name = "Saturday",
                           
                        };
                        // create the Dow Object
                        await dataDow.AddDOWAsync(DowSaturday);
                    }
                }
            }
            else 
            {
                await Application.Current.MainPage.DisplayAlert("Alert!", "Failed to create days of the week", "Ok");
                return;
            
            }

            
        }
        async Task OnaddTask()
        {
            var route = $"{nameof(AddPlannedTask)}?{nameof(addTaskViewModel.GoalId)}={goalId}";
            await Shell.Current.GoToAsync(route);
        }
        async Task SendTaskId(GoalTask goalTask)
        {
            var route = $"{nameof(subTaskView)}?SubtaskId={goalTask.Id}";
            await Shell.Current.GoToAsync(route);

            //// get all subtasks havng the task id 
            //var subtasks = await dataSubTask.GetSubTasksAsync(goalTask.Id);
            //if (subtasks.Count() == 0)
            //{
            //    var route1 = $"{nameof(BlankWeekSubtaskView)}?{nameof(SubtaskViewModel.Taskid)}={goalTask.Id}";
            //    await Shell.Current.GoToAsync(route1);
            //}
            //else
            //{
            //    var route = $"{nameof(subTaskView)}?{nameof(SubtaskViewModel.Taskid)}={goalTask.Id}";
            //    await Shell.Current.GoToAsync(route);
            //}
        }
        async Task OnUpdateTask(GoalTask goalTask)
        {
            var route = $"{nameof(UpdateWeekTask)}?taskId={goalTask.Id}";

            await Shell.Current.GoToAsync(route);
        }
        async Task deleteCategory(GoalTask goalTask)
        {
            if (goalTask == null)
                return;
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Task!", "All Subtasks in this Task will be deleted. Continue?", "Yes", "No");
            if (ans)
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

        //method for dow buttons
        async Task sunButton()
        {
            DayNumber = 1;
            await Refresh();
            return;
        }
        async Task monButton()
        {
            DayNumber = 2;
            await Refresh();
            return;
        }
        async Task tueButton()
        {
            DayNumber = 3;
            await Refresh();
            return;
        }
        async Task wedButton()
        {
            DayNumber = 4;
            await Refresh();
            return;
        }
        async Task thuButton()
        {
            DayNumber = 5;
            await Refresh();
            return;
        }
        async Task friButton()
        {
            DayNumber = 6;
            await Refresh();
            return;
        }
        async Task satButton()
        {
            DayNumber = 7;
            await Refresh();
            return;

        }

        //a method for creating a week

        public async Task CompleteTask(int TaskId, bool IsComplete)
        {
            // get the task having the same id as taskId
            var task = await dataTask.GetTaskAsync(TaskId);
            var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
            // check if the incoming object 
            if (task.IsCompleted)
                return;
            else if (!task.IsCompleted)
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
                if (subtasks.Count() == 0)
                {
                    task.IsEnabled = true;

                }
                // set task.pending percentage to zero
                roundedtask = 0;
                // loop through the subtasks
                foreach (var subtask in subtasks)
                {
                    // check if they are completed
                    if (subtask.IsCompleted)
                    {

                        roundedtask += subtask.Percentage;
                    }
                }
                //change a task pending pecentage to a rounded figure
                task.PendingPercentage = Math.Round(roundedtask, 1);
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

        async Task deleteTask(GoalTask goalTask)
        {
            if (goalTask == null)
                return;
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Task!", "All Subtasks in this Task will be deleted. Continue?", "Yes", "No");
            if (ans)
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
        async Task GetLastTask()
        {

            var tasks = await dataTask.GetTasksAsync(goalId);
            if (tasks.Count() == 0)
                return;
            // get the last task
            var task = tasks.Where(T => T.DowId > 0).ToList();
            if (task.Count == 0)
                return;
            else if (task.Count > 0)
            {
                // get last task
                var lastTask = task.LastOrDefault();
                DayNumber = lastTask.DowId;
                // get the day that match the id of the task dowid
                //var days = await dataDow.GetDOWsAsync();

                //var dayid = days.Where(d => d.DOWId == lastTask.DowId).FirstOrDefault();
                //if (dayid.DOWId == 1)
                //{
                //    await sunButton();
                //    return;
                //}
                  
                //else if (dayid.DOWId == 2)
                //{
                //    await monButton();
                //    return;
                //}
                //else if (dayid.DOWId == 3)
                //{
                //    await tueButton();
                //    return;
                //}
                //else if (dayid.DOWId == 4)
                //{
                //    await wedButton();
                //    return;
                //}                   
                //else if (dayid.DOWId == 5)
                //{
                //    await thuButton();
                //    return;
                //}                   
                //else if (dayid.DOWId == 6)
                //{
                //    await friButton();
                //    return;
                //}                   
                //else if (dayid.DOWId == 7)
                //{
                //    await satButton();
                //    return;
                //}                    
                //else
                //    return;

            }


        }
        public async Task Refresh()
        {
           
            IsBusy = true;          
            await CreateDOW();
            await CalculateSubtaskPercentage();
            dows.Clear();
            dowTasks.Clear();
      
            var Dows = await dataDow.GetDOWsAsync();
            var tasks = await dataTask.GetTasksAsync(goalId, dayNumber);
            await Getremainingdays();
           
            dows.AddRange(Dows);
            dowTasks.AddRange(tasks);

            //await GetLastWeek();
            IsBusy = false;
        }
    }
}
