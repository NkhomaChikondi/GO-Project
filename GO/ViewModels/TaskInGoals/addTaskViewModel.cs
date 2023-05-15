using GO.Models;
using GO.ViewModels.Subtasks;
using GO.Views.GoalTask;
using GO.Views.SubTaskView;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.LocalNotification;
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
        private DateTime starttime;
        private DateTime endtime;
        private string description;
        private int remainingDays = 0;
        private double percentageProgress = 0;
        private int newgoalpercent = 0;
        private double taskPercentage;
        private double totalPendingPercentage;
        private int selectedItem;
        private DOW selectedDay;
        private int DayId;
        public List<DOWPicker> dowpicker { get; set; }
        public string DowName { get; set; }
        public AsyncCommand TaskAddCommand { get; }
        public AsyncCommand TaskAddWeekCommand { get; }
        public ObservableRangeCollection<Goal> goals { get; }
        public ObservableRangeCollection<GoalTask> goalTasks { get; }
        public AsyncCommand<GoalTask> SendTaskIdCommand { get; }
        public AsyncCommand HelpCommand { get; }
        public addTaskViewModel()
        {
            TaskAddCommand = new AsyncCommand(AddTask);
            TaskAddWeekCommand = new AsyncCommand(AddTaskWeek);
            goals = new ObservableRangeCollection<Goal>();
            goalTasks = new ObservableRangeCollection<GoalTask>();
            SendTaskIdCommand = new AsyncCommand<GoalTask>(SendTaskId);
            HelpCommand = new AsyncCommand(GotoHelpPage);
        }

        public int GoalId { get { return goalId; } set => goalId = value; }
        public string Name { get => name; set => name = value; }
        public DateTime Endtime { get => endtime; set => endtime = value; }
        public DateTime Starttime { get => starttime; set => starttime = value; }
        public string Description { get => description; set => description = value; }
        public int RemainingDays { get => remainingDays; set => remainingDays = value; }
       
        
        public async Task SendTaskId(GoalTask goalTask)
        {
            var route = $"{nameof(subTaskView)}?{nameof(SubtaskViewModel.Taskid)}={goalTask.Id}";
            await Shell.Current.GoToAsync(route);
        }
        async Task GotoHelpPage()
        {
            var route = $"{nameof(Helpaddtaskpage)}";
            await Shell.Current.GoToAsync(route);
        }

        async Task SelectedDow()
        {
            // get the last week in goal
            var goal = await datagoal.GetGoalAsync(goalId);
            if (goal.HasWeek)
            {
                // get all weeks in it
                var weeks = await dataWeek.GetWeeksAsync(goal.Id);
                // get the last inserted week
                var lastweek = weeks.ToList().LastOrDefault();
                //check if it active
                if(!lastweek.Active)
                {
                    lastweek.Active = true;
                    await dataWeek.UpdateWeekAsync(lastweek);
                }
                var dows = await dataDow.GetDOWsAsync(lastweek.Id);
                // get the selected dow
                var selecteddow = dows.Where(D => D.IsSelected).FirstOrDefault();
                if(selecteddow == null)
                {
                    if(DateTime.Today.Date < lastweek.StartDate.Date)
                    {
                        // get the dow whose day whose date is equal to
                        var day = dows.Where(d => d.Name == lastweek.StartDate.DayOfWeek.ToString()).FirstOrDefault();                        
                        // dayId will be equal to day's Id
                        DayId = day.DOWId;
                    }
                    else if(DateTime.Today.Date >= lastweek.StartDate.Date)
                    { // get the dow whose day is equal to today's day
                        var day = dows.Where(d => d.Name == DateTime.Today.DayOfWeek.ToString()).FirstOrDefault();
                        // dayId will be equal to day's Id
                        DayId = day.DOWId; 
                    }                  
                }
                else
                {
                    selectedDay = selecteddow;
                    starttime = selectedDay.Date;
                    endtime = selectedDay.Date;
                    DayId = selecteddow.DOWId;
                }               
            }           
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
                await SelectedDow();
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
                if (string.IsNullOrEmpty(name))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Please enter the name for the Task. ", "OK");
                    return;
                }
                //get goal of the task
                //var goal = await datagoal.GetGoalAsync(GoalId);
                //// check if the goal hasnt expired yet
                //if(goal.Status == "Expired")
                //{
                //    await Application.Current.MainPage.DisplayAlert("Alert","Cannot create ")
                //}
                var week = await dataWeek.GetWeeksAsync(goalId);
                // get the last inserted week
                var lastweek = week.ToList().LastOrDefault();
                // get all tasks in GoalId
                var alltasks = await dataTask.GetTasksAsync(goalId,lastweek.Id);                
                var dowtask = alltasks.Where(D => D.DowId == DayId).ToList();
                // check if the tasks for the day has surpassed 5              
                if (dowtask.Count() >= 5)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", "You cannot add any new task, Maximum number of tasks (5) added to a day, has been reached. ", "OK");
                    return;
                }
                else
                {
                    // change the first letter of the Task name to upercase
                    var UppercasedName = char.ToUpper(newtask.taskName[0]) + newtask.taskName.Substring(1);
                    //check if the new task already exist in the database
                    if (dowtask.Any(T => T.taskName == UppercasedName))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                        return;
                    }
                    // get goal from the goal table through the given Id
                    var TaskInGoalId = await datagoal.GetGoalAsync(goalId);

                    if (newtask.Description == null)
                        newtask.Description = $"No Description for {newtask.taskName}";
                    //call the add percentage method
                    //await AddweekTaskPercentage();
                    // check if goal has week or not
                    // get last inserted week in "this" goal

                    if (DateTime.Today >= lastweek.StartDate && DateTime.Today <= lastweek.EndDate)
                    {
                        // set lastinsertedWeek to active
                        lastweek.Active = true;
                        // update the database
                        await dataWeek.UpdateWeekAsync(lastweek);
                    }
                    // get dow having the DayId
                    var dow = await dataDow.GetDOWAsync(DayId);
                    // get date                
                    var newestTask = new GoalTask
                    {
                        taskName = UppercasedName,
                        StartTask = dow.Date,
                        EndTask = dow.Date,
                        enddatetostring = endtime.ToLongDateString(),
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

                    #region check if task has been assigned a percentage
                    // counter value
                    int counter = 0;
                    // check if task percent has been assigned to task's percentage

                    while (counter < 3 && newestTask.Percentage == 0)
                    {
                        await AddweekTaskPercentage(lastweek);
                        newestTask.Percentage = taskPercentage;
                        counter++;
                    }

                    if (counter == 3 && newestTask.Percentage == 0)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Failed to add Task, retry", "Ok");
                        return;
                    }
                    #endregion

                    // add the new task to the database                
                    await dataTask.AddTaskAsync(newestTask);
                    // call the add percentage method
                    AddTaskPercent(lastweek);

                    // get the name of the day having dowId
                    var dbDow = await dataDow.GetDOWAsync(newestTask.DowId);
                    await Shell.Current.GoToAsync("..");
                    Datatoast.toast("New task added");
                }                         
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add new task: {ex.Message}");
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

                // get goal from the goal table through the given Id
                var TaskInGoalId = await datagoal.GetGoalAsync(goalId);
                // check if goal has expired 
                if(TaskInGoalId.Status == "Expire")
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Failed to add task! The goal of this task expired therefore, new tasks cannot be created","OK");
                    return;
                }
                else
                {
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
                    if (string.IsNullOrEmpty(name))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Please enter the name for the goal. ", "OK");
                        return;
                    }
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

                    // verify if the Start date and end date are within the duration of its selected goal
                    if (newtask.StartTask.Date >= TaskInGoalId.Start.Date && newtask.EndTask.Date <= TaskInGoalId.End.Date)
                    {
                        TimeSpan ts = newtask.EndTask - newtask.StartTask;
                        RemainingDays = (int)ts.TotalDays;

                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", $" Make sure the start and end date are on/ within the goal's start and end date. (From {TaskInGoalId.Start.ToLongDateString()} to {TaskInGoalId.End.ToLongDateString()}", "OK");
                        return;
                    }
                    if (newtask.Description == null)
                        newtask.Description = $"No Description for {newtask.taskName}";

                    ////call the add percentage method
                    //await AddPercentage();
                    //// check if goal has week or not
                    //// get the goalid for this view model
                    var newestTask = new GoalTask
                    {
                        taskName = UppercasedName,
                        StartTask = starttime,
                        EndTask = endtime,
                        enddatetostring = endtime.ToLongDateString(),
                        RemainingDays = remainingDays,
                        GoalId = goalId,
                        IsCompleted = false,
                        Description = newtask.Description,
                        PendingPercentage = 0,
                        Percentage = 0, //taskPercentage,
                        Status = "Not Started",
                        CompletedSubtask = 0,
                        IsEnabled = true,
                        CreatedOn = DateTime.Now,
                        IsVisible = true,
                        IsNotVisible = false
                    };


                    // add the new task to the database                
                    await dataTask.AddTaskAsync(newestTask);
                    // call send notification method
                    await SendNotification();
                    //var route = $"{nameof(GoalTaskPage)}?goalId={goalId}";
                    //await Shell.Current.GoToAsync(route);
                    await Shell.Current.GoToAsync("..");
                    Datatoast.toast("New task added");
                }
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add new task: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
          
        }
        async Task AddweekTaskPercentage(Week week)
        {
            double Taskcount = 0;
            // check if the week is active
            if(!week.Active)
            {
                week.Active = true;
                await dataWeek.UpdateWeekAsync(week);
            }
            // get tasks
            var tasks = await dataTask.GetTasksAsync(goalId);
            // get all tasks having the week id
            var weekTasks = tasks.Where(T => T.WeekId == week.Id).ToList();
            // add 1 to weeks count
            Taskcount = weekTasks.Count() + 1;
            // loop through the dows and get their Id           
            taskPercentage = week.TargetPercentage / Taskcount;
            Taskcount = 0;          
        }
        // a method to assign percentage to the task
        //async Task AddPercentage()
        //{
        //    // get all tasks having "this" goal id from the database 
        //    var tasks = await dataSubtask.GetTasksAsync(goalId);
        //    // get the total number of tasks 
        //    double AllTaskCount = tasks.Count() + 1;
        //    // divide 100 percent by the number of tasks
        //    taskPercentage = 100.0 / AllTaskCount;

        //}
        // a method to reassign percentage to all tasks in the database
        async void AddTaskPercent(Week week)
        {
            // check if the week is active
            if (!week.Active)
            {
                week.Active = true;
                await dataWeek.UpdateWeekAsync(week);
            }
            // set the percentage progress to zero
            percentageProgress = 0;
            var tasks = await dataTask.GetTasksAsync(goalId);
            // get all tasks having the week id
            var weekTasks = tasks.Where(T => T.WeekId == week.Id).ToList();
            // loop through the dows
            foreach (var task in weekTasks)            {
               
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
        async Task SendNotification()
        {
            // get all tasks having goal id
            var tasks = await dataTask.GetTasksAsync(GoalId);
            // get the last goal
            var lastTask = tasks.ToList().LastOrDefault();            
            var notification = new NotificationRequest
            {
                BadgeNumber = 1,
                Description = $"Task '{lastTask.taskName}' is Due today!",
                Title = "Due-Date!",
                NotificationId = lastTask.Id,
                Schedule =
                    {
                        NotifyTime =lastTask.EndTask,
                       
                    }
            };
            await LocalNotificationCenter.Current.Show(notification);
        }
      

    }



    
}
