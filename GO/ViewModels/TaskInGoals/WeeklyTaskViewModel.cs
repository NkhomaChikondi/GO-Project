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
        private int dowId;
        private double roundedtask;
        private int weekId;
        private bool all = true;
        private bool notstarted;
        private bool inprogress;
        private bool withSubtasks;
        private bool completed;
        private bool duesoon;
        private bool expired;

      
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
        public AsyncCommand HelpCommand { get; }
        public AsyncCommand HelpWeekCommand { get; }

        public int DowId { get => dowId; set => dowId = value; }
        public int WeekId { get => weekId; set => weekId = value; }
        public bool All { get => all; set => all = value; }
        public bool Notstarted { get => notstarted; set => notstarted = value; }
        public bool Inprogress { get => inprogress; set => inprogress = value; }
        public bool Completed { get => completed; set => completed = value; }
        public bool Duesoon { get => duesoon; set => duesoon = value; }
        public bool Expired { get => expired; set => expired = value; }
        public int GoalId { get => goalId; set => goalId = value; }
        public bool WithSubtasks { get => withSubtasks; set => withSubtasks = value; }

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
            DeleteCommand = new AsyncCommand<GoalTask>(deleteTask);
            HelpCommand = new AsyncCommand(GotoHelpPage);
            HelpWeekCommand = new AsyncCommand(GotoHelpweekPage);
        }
        public async Task AllGoals()
        {
            all = true;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            expired = false;
            WithSubtasks = false;
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
            WithSubtasks = false;
            await Refresh();
        }
        public async Task WithsubtasksTask()
        {
            all = false;
            notstarted = false;
            withSubtasks = true;
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
            WithSubtasks = false;
            completed = false;
            await Refresh();
        }
        public async Task CompletedGoals()
        {
            all = false;
            notstarted = false;
            duesoon = false;
            inprogress = false;
            WithSubtasks = false;
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
            WithSubtasks = false;
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
            WithSubtasks = false;
            expired = true;
            completed = false;
            await Refresh();
        }
        // seed the days of the week into the database upon startup
        async Task CreateDOW()
        {
            // get week 
            // get the last active week of goal
            var getweeks = await dataWeek.GetWeeksAsync(GoalId);
            // get the last inserted week
            var lastInsertedWeek = getweeks.ToList().LastOrDefault();
            async Task createDow()
            {
                var alldows = await dataDow.GetDOWsAsync(lastInsertedWeek.Id);
                if (alldows.Count() > 0)
                    return;
                else if (alldows.Count() == 0)
                {

                    var DowSunday = new DOW
                    {

                        Name = "Sunday",
                        WeekId = lastInsertedWeek.Id,
                        IsSelected = false
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowSunday);

                    var DowMonday = new DOW
                    {
                        Name = "Monday",
                        WeekId = lastInsertedWeek.Id,
                        IsSelected = false
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowMonday);

                    var DowTuesday = new DOW
                    {
                        Name = "Tuesday",
                        WeekId = lastInsertedWeek.Id,
                        IsSelected = false
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowTuesday);

                    var DowWednesday = new DOW
                    {
                        Name = "Wednesday",
                        WeekId = lastInsertedWeek.Id,
                        IsSelected = false,
                        ValidDay = false
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowWednesday);


                    var DowThursday = new DOW
                    {
                        Name = "Thursday",
                        WeekId = lastInsertedWeek.Id,
                        IsSelected = false,
                        ValidDay = false
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowThursday);

                    var DowFriday = new DOW
                    {
                        Name = "Friday",
                        WeekId = lastInsertedWeek.Id,
                        IsSelected = false,
                        ValidDay = false
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowFriday);

                    var DowSaturday = new DOW
                    {
                        Name = "Saturday",
                        WeekId = lastInsertedWeek.Id,
                        IsSelected = false,
                        ValidDay = false
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowSaturday);
                }
            }
            // check if the week is active
            if(lastInsertedWeek.Active)
            {
                // call the createdow method
                await createDow();
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
                    // call createdow method
                    await createDow();
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
            // get the day for the task
            var day = await dataDow.GetDOWAsync(goalTask.DowId);
            // get the week for the day
            var week = await dataWeek.GetWeekAsync(day.WeekId);
            //check if they are subtasks inside the goaltask
            var subtasks = await dataSubTask.GetSubTasksAsync(goalTask.Id);
            if(week.Active || subtasks.Count() > 0)
            {
                var route = $"{nameof(subTaskView)}?SubtaskId={goalTask.Id}";
                await Shell.Current.GoToAsync(route);
            }
            else 
            {               
                await Application.Current.MainPage.DisplayAlert("Alert!", "There no subtasks in this task!", "Ok");
                return;            
            }                     
                   
        }       
        async Task OnUpdateTask(GoalTask goalTask)
        {
            var route = $"{nameof(UpdateWeekTask)}?taskId={goalTask.Id}";

            await Shell.Current.GoToAsync(route);
        }
        async Task deleteTask(GoalTask goalTask)
        {
            if (goalTask == null)
                return;
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Task!", "All Subtasks in this Task will also be deleted. Continue?", "Yes", "No");
            if (ans)
            {
                await dataTask.DeleteTaskAsync(goalTask.Id);
                // check if the goal task had a day id
            
                    // get all tasks from the database that has the goal id
                    var tasks = await dataTask.GetTasksAsync(goalTask.GoalId);
                    // get week
                    var week = await dataWeek.GetWeekAsync(goalTask.WeekId);                    
                    // get tasks for the week
                    var weeklyTasks = tasks.Where(T => T.WeekId == week.Id).ToList();
                    // loop through the tasks and recalculate their task percentage
                    foreach(var task in weeklyTasks)
                    {
                        task.Percentage = week.TargetPercentage / weeklyTasks.Count();
                         await dataTask.UpdateTaskAsync(task);
                    }                    
               
                await CalculateTotalWeekPercentage(week);
                await Refresh();
            }
            else if (!ans)
                return;

        }

        //method for dow buttons
        async Task sunButton()
        {
            // get week having the weekid
            var week = await dataWeek.GetWeekAsync(weekId);
            // get all dows in the database
            var dows = await dataDow.GetDOWsAsync(weekId);          
            // loop through the dows
            foreach (var dow in dows)
            {
                if (dow.Name == "Sunday")
                {

                    // get the Id
                    dowId = dow.DOWId;
                    // set is selectio to true
                    dow.IsSelected = true;
                }
                    
                else
                    dow.IsSelected = false;
                //update dow
                await dataDow.UpdateDOWAsync(dow);
            }
            // get all tasks having sundayId
            var tasks = await dataTask.GetTasksAsync(goalId, dowId);
            // check if week is active
            if (week.Active)
            {
                if(DateTime.Today >= week.StartDate && DateTime.Today <= week.EndDate)
                {
                    //check if today is sunday
                    if (DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                    {
                        var route = $"{nameof(weekTasks)}?weekId={weekId}";
                        await Shell.Current.GoToAsync(route);                       
                    }
                    else if(DateTime.Today.DayOfWeek != DayOfWeek.Sunday)
                    {
                        // check if it has tasks 
                        if(tasks.Count().Equals(0))
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot create tasks! Sunday tasks can only be created on Sunday", "Ok");
                            return;
                        }
                        else 
                        {
                            var route = $"{nameof(weekTasks)}?weekId={weekId}";
                            await Shell.Current.GoToAsync(route);
                        }                       
                    }
                }
               
            }
            else if(!week.Active)
            {                
                if (tasks.Count() == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "This week expired with 0 Tasks for Sunday", "Ok");
                    return;
                }
                else
                {
                    var route = $"{nameof(weekTasks)}?weekId={weekId}";
                    await Shell.Current.GoToAsync(route);
                }
            }
          
            
        }
        async Task monButton()
        {
            // get week having the weekid
            var week = await dataWeek.GetWeekAsync(weekId);
            var dows = await dataDow.GetDOWsAsync(weekId);
            // loop through the dows
            foreach (var dow in dows)
            {
                if (dow.Name == "Monday")
                {
                    // get the Id
                    dowId = dow.DOWId;
                    // set is selectio to true
                    dow.IsSelected = true;
                }

                else
                    dow.IsSelected = false;
                //update dow
                await dataDow.UpdateDOWAsync(dow);
            }
            // get all tasks having sundayId
            var tasks = await dataTask.GetTasksAsync(goalId, dowId);
            // check if week is active
            if (week.Active)
            {
                if (DateTime.Today >= week.StartDate && DateTime.Today <= week.EndDate)
                {
                    //check if today is sunday
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Monday)
                    {
                        var route = $"{nameof(weekTasks)}?weekId={weekId}";
                        await Shell.Current.GoToAsync(route);
                    }
                    else if (DateTime.Today.DayOfWeek > DayOfWeek.Monday)
                    {
                        // check if it has tasks 
                        if (tasks.Count().Equals(0))
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot create tasks! Monday tasks can only be created before or on Monday", "Ok");
                            return;
                        }
                        else
                        {
                            var route = $"{nameof(weekTasks)}?weekId={weekId}";
                            await Shell.Current.GoToAsync(route);
                        }
                    }
                }

            }
            else if (!week.Active)
            {
                if (tasks.Count() == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "They're no tasks to view for Monday", "Ok");
                    return;
                }
                else
                {
                    var route = $"{nameof(weekTasks)}?weekId={weekId}";
                    await Shell.Current.GoToAsync(route);
                }
            }


        }
        async Task tueButton()
        {
            // get week having the weekid
            var week = await dataWeek.GetWeekAsync(weekId);
            var dows = await dataDow.GetDOWsAsync(weekId);
            // loop through the dows
            foreach (var dow in dows)
            {
                if (dow.Name == "Tuesday")
                {
                    // get the Id
                    dowId = dow.DOWId;
                    // set is selectio to true
                    dow.IsSelected = true;
                }

                else
                    dow.IsSelected = false;
                //update dow
                await dataDow.UpdateDOWAsync(dow);
            }
            // get all tasks having sundayId
            var tasks = await dataTask.GetTasksAsync(goalId, dowId);
            // check if week is active
            if (week.Active)
            {
                if (DateTime.Today >= week.StartDate && DateTime.Today <= week.EndDate)
                {
                    //check if today is sunday
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Tuesday)
                    {
                        var route = $"{nameof(weekTasks)}?weekId={weekId}";
                        await Shell.Current.GoToAsync(route);
                    }
                    else if (DateTime.Today.DayOfWeek > DayOfWeek.Tuesday)
                    {
                        // check if it has tasks 
                        if (tasks.Count().Equals(0))
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot create tasks! Tuesday tasks can only be created before or on Tuesday", "Ok");
                            return;
                        }
                        else
                        {
                            var route = $"{nameof(weekTasks)}?weekId={weekId}";
                            await Shell.Current.GoToAsync(route);
                        }
                    }
                }

            }
            else if (!week.Active)
            {
                if (tasks.Count() == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "They're no tasks to view for Tuesday", "Ok");
                    return;
                }
                else
                {
                    var route = $"{nameof(weekTasks)}?weekId={weekId}";
                    await Shell.Current.GoToAsync(route);
                }
            }

        }
        async Task wedButton()
        {
            // get week having the weekid
            var week = await dataWeek.GetWeekAsync(weekId);
            var dows = await dataDow.GetDOWsAsync(weekId);
            // loop through the dows
            foreach (var dow in dows)
            {
                if (dow.Name == "Wednesday")
                {
                    // get the Id
                    dowId = dow.DOWId;
                    // set is selectio to true
                    dow.IsSelected = true;
                }

                else
                    dow.IsSelected = false;
                //update dow
                await dataDow.UpdateDOWAsync(dow);
            }
            // get all tasks having sundayId
            var tasks = await dataTask.GetTasksAsync(goalId, dowId);
            // check if week is active
            if (week.Active)
            {
                if (DateTime.Today >= week.StartDate && DateTime.Today <= week.EndDate)
                {
                    //check if today is sunday
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Wednesday)
                    {
                        var route = $"{nameof(weekTasks)}?weekId={weekId}";
                        await Shell.Current.GoToAsync(route);
                    }
                    else if (DateTime.Today.DayOfWeek > DayOfWeek.Wednesday)
                    {
                        // check if it has tasks 
                        if (tasks.Count().Equals(0))
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot create tasks! Wednesday tasks can only be created before or on Wednesday", "Ok");
                            return;
                        }
                        else
                        {
                            var route = $"{nameof(weekTasks)}?weekId={weekId}";
                            await Shell.Current.GoToAsync(route);
                        }
                    }
                }

            }
            else if (!week.Active)
            {
                if (tasks.Count() == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "They're no tasks to view for wednesday", "Ok");
                    return;
                }
                else
                {
                    var route = $"{nameof(weekTasks)}?weekId={weekId}";
                    await Shell.Current.GoToAsync(route);
                }
            }


        }
        async Task thuButton()
        {

            // get week having the weekid
            var week = await dataWeek.GetWeekAsync(weekId);
            var dows = await dataDow.GetDOWsAsync(weekId);
            // loop through the dows
            foreach (var dow in dows)
            {
                if (dow.Name == "Thursday")
                {
                    // get the Id
                    dowId = dow.DOWId;
                    // set is selectio to true
                    dow.IsSelected = true;
                }

                else
                    dow.IsSelected = false;
                //update dow
                await dataDow.UpdateDOWAsync(dow);
            }
            // get all tasks having sundayId
            var tasks = await dataTask.GetTasksAsync(goalId, dowId);
            // check if week is active
            if (week.Active)
            {
                if (DateTime.Today >= week.StartDate && DateTime.Today <= week.EndDate)
                {
                    //check if today is sunday
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Thursday)
                    {
                        var route = $"{nameof(weekTasks)}?weekId={weekId}";
                        await Shell.Current.GoToAsync(route);
                    }
                    else if (DateTime.Today.DayOfWeek > DayOfWeek.Thursday)
                    {
                        // check if it has tasks 
                        if (tasks.Count().Equals(0))
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot create tasks! Thursday tasks can only be created before or on Thursday", "Ok");
                            return;
                        }
                        else
                        {
                            var route = $"{nameof(weekTasks)}?weekId={weekId}";
                            await Shell.Current.GoToAsync(route);
                        }
                    }
                }

            }
            else if (!week.Active)
            {
                if (tasks.Count() == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "They're no tasks to view for Thursday", "Ok");
                    return;
                }
                else
                {
                    var route = $"{nameof(weekTasks)}?weekId={weekId}";
                    await Shell.Current.GoToAsync(route);
                }
            }

        }
        async Task friButton()
        {

            // get week having the weekid
            var week = await dataWeek.GetWeekAsync(weekId);
            var dows = await dataDow.GetDOWsAsync(weekId);
            // loop through the dows
            foreach (var dow in dows)
            {
                if (dow.Name == "Friday")
                {
                    // get the Id
                    dowId = dow.DOWId;
                    // set is selectio to true
                    dow.IsSelected = true;
                }

                else
                    dow.IsSelected = false;
                //update dow
                await dataDow.UpdateDOWAsync(dow);
            }
            // get all tasks having sundayId
            var tasks = await dataTask.GetTasksAsync(goalId, dowId);
            // check if week is active
            if (week.Active)
            {
                if (DateTime.Today >= week.StartDate && DateTime.Today <= week.EndDate)
                {
                    //check if today is sunday
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Friday)
                    {
                        var route = $"{nameof(weekTasks)}?weekId={weekId}";
                        await Shell.Current.GoToAsync(route);
                    }
                    else if (DateTime.Today.DayOfWeek > DayOfWeek.Tuesday)
                    {
                        // check if it has tasks 
                        if (tasks.Count().Equals(0))
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot create tasks! Friday tasks can only be created before or on Friday", "Ok");
                            return;
                        }
                        else
                        {
                            var route = $"{nameof(weekTasks)}?weekId={weekId}";
                            await Shell.Current.GoToAsync(route);
                        }
                    }
                }

            }
            else if (!week.Active)
            {
                if (tasks.Count() == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "They're no tasks to view for Friday", "Ok");
                    return;
                }
                else
                {
                    var route = $"{nameof(weekTasks)}?weekId={weekId}";
                    await Shell.Current.GoToAsync(route);
                }
            }

        }
        async Task satButton()
        {

            // get week having the weekid
            var week = await dataWeek.GetWeekAsync(weekId);
            var dows = await dataDow.GetDOWsAsync(weekId);
            // loop through the dows
            foreach (var dow in dows)
            {
                if (dow.Name == "Saturday")
                {
                    // get the Id
                    dowId = dow.DOWId;
                    // set is selectio to true
                    dow.IsSelected = true;
                }

                else
                    dow.IsSelected = false;
                //update dow
                await dataDow.UpdateDOWAsync(dow);
            }
            // get all tasks having sundayId
            var tasks = await dataTask.GetTasksAsync(goalId, dowId);
            // check if week is active
            if (week.Active)
            {
                if (DateTime.Today >= week.StartDate && DateTime.Today <= week.EndDate)
                {
                    //check if today is sunday
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Saturday)
                    {
                        var route = $"{nameof(weekTasks)}?weekId={weekId}";
                        await Shell.Current.GoToAsync(route);
                    }
                    else if (DateTime.Today.DayOfWeek > DayOfWeek.Saturday)
                    {
                        // check if it has tasks 
                        if (tasks.Count().Equals(0))
                        {
                            await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot create tasks! Saturday tasks can only be created before or on Saturday", "Ok");
                            return;
                        }
                        else
                        {
                            var route = $"{nameof(weekTasks)}?weekId={weekId}";
                            await Shell.Current.GoToAsync(route);
                        }
                    }
                }

            }
            else if (!week.Active)
            {
                if (tasks.Count() == 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", "They're no tasks to view for saturday", "Ok");
                    return;
                }
                else
                {
                    var route = $"{nameof(weekTasks)}?weekId={weekId}";
                    await Shell.Current.GoToAsync(route);
                }
            }


        }
        async Task GotoHelpPage()
        {
            var route = $"{nameof(HelpweeklyTaskspage)}";
            await Shell.Current.GoToAsync(route);
        }
        async Task GotoHelpweekPage()
        {
            var route = $"{nameof(HelpWeekPage)}";
            await Shell.Current.GoToAsync(route);
        }
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
                //get the day the task is assigned to
                var day = await dataDow.GetDOWAsync(task.DowId);
                // check if the day name is less than or equal to the day of today                
                // get the week the day is assigned to
                var week = await dataWeek.GetWeekAsync(day.WeekId);
                if (week.Active)
                {
                    if(day.ValidDay)
                    {
                        if (subtasks.Count() > 0)
                            return;
                        //check if it has subtask
                        else if (subtasks.Count() == 0)
                        {
                            task.IsCompleted = IsComplete;
                            await dataTask.UpdateTaskAsync(task);
                            await SetStatus();
                            await CalculateTotalWeekPercentage(week);
                        }
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert!", "You cannot complete a task for a day that has surpassed!", "Ok");
                        return;
                    }                 
                }
                else
                {
                    //await Refresh();
                    await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot complete this Task. The Task has expired!", "OK");                   
                    return;
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
                //get the day the task is assigned to
                var day = await dataDow.GetDOWAsync(task.DowId);
                // get the week the day is assigned to
                var week = await dataWeek.GetWeekAsync(day.WeekId);
                if (week.Active)
                {
                    if(day.ValidDay)
                    { //check if it has subtask
                        if (subtasks.Count() > 0)
                            return;
                        else if (subtasks.Count() == 0)
                        {
                            task.IsCompleted = IsComplete;
                            await dataTask.UpdateTaskAsync(task);
                            await SetStatus();
                            await CalculateTotalWeekPercentage(week);
                        }
                    }
                    else 
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert!", "You cannot Uncomplete a task for a day that has surpassed!", "Ok");
                        return;
                    }
                }
                else
                {
                    //await Refresh();
                    await Application.Current.MainPage.DisplayAlert("Alert!", "Cannot Uncomplete this task. The Task has expired!", "OK");                    
                    return;
                }
                   
               
            }
            return;
        }
        async Task CalculateSubtaskPercentage()
        {

            // get all week having the goal id
            var weeks = await dataWeek.GetWeeksAsync(goalId);
            // loop through the week and get the days id
            foreach(var week in weeks)
            {
                // get all days having the week id
                var days = await dataDow.GetDOWsAsync(week.Id);
                // loop through the days to get tasks having the day id
                foreach(var day in days)
                {
                    var Daytasks = await dataTask.GetTasksAsync(GoalId, day.DOWId);
                    //loop through the tasks to get their subtasks
                    foreach(var task in Daytasks)
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
                        task.PendingPercentage = Math.Round(roundedtask, 1);
                        task.Progress = task.PendingPercentage / task.Percentage;
                        await dataTask.UpdateTaskAsync(task);
                        await SetStatus();
                    }

                }
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
        async Task CalculateTotalWeekPercentage(Week week)
        {
            var getweeks = await dataWeek.GetWeeksAsync(GoalId);            
            double TaskPercentage = 0;
            double subtaskpercentage = 0;
            double Accumulated = 0;
             
            // get all tasks having the goal id
            var tasks = await dataTask.GetTasksAsync(GoalId);
            // get all tasks having the week id
            var weekTasks = tasks.Where(T => T.WeekId == week.Id).ToList();
            // loop through the tasks
            foreach(var task in weekTasks)
            {
                //check if task is completed
                if (task.IsCompleted)
                {
                    TaskPercentage += task.Percentage;

                }
                else if (!task.IsCompleted)
                {
                    // check task has subtasks
                    //get all subtasks having the tasks Id
                    var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);

                    if (subtasks.Count() > 0)
                    {
                        // get the task's pending percentage
                        subtaskpercentage += task.PendingPercentage;
                    }
                }
            }
            Accumulated = TaskPercentage + subtaskpercentage;
            if (weekTasks.All(A => A.IsCompleted))
                Accumulated = week.TargetPercentage;
            week.AccumulatedPercentage = Math.Round(Accumulated, 1);
            week.Progress = week.AccumulatedPercentage / week.TargetPercentage;
            // check if todays date is less than the weeks end date and update status accordingly
            if (DateTime.Today < week.EndDate)
            {
                if (week.AccumulatedPercentage == 0)
                    week.Status = "Not Started";
                else if (week.AccumulatedPercentage > 0 && week.AccumulatedPercentage < week.TargetPercentage)
                    week.Status = "InProgress";
                else if (week.AccumulatedPercentage == week.TargetPercentage)
                    week.Status = "Completed";
            }
            else if (DateTime.Today > week.EndDate)
                week.Status = "Expired";
            await dataWeek.UpdateWeekAsync(week);                    
                
                        //reset the below variables
                TaskPercentage = 0;
                subtaskpercentage = 0;
                Accumulated = 0;               
                
            
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
            IsBusy = true;          
            await CreateDOW();
            //await CalculateSubtaskPercentage();          
           // dows.Clear();
            dowTasks.Clear();      
            //var Dows = await dataDow.GetDOWsAsync();
            var tasks = await dataTask.GetTasksAsync(goalId, dowId);
            if (all)
                // retrieve the categories back
                dowTasks.AddRange(tasks);

            //filter goals
            else if (notstarted)
            {
                var notstartedtasks = tasks.Where(g => g.Status == "Not Started").ToList();
                dowTasks.AddRange(notstartedtasks);
            }
            else if (completed)
            {
                var completedtasks = tasks.Where(g => g.IsCompleted).ToList();
                dowTasks.AddRange(completedtasks);
            }
            else if(withSubtasks)
            {
                List<GoalTask> tasklist = new List<GoalTask>();
                //loop through the tasks
                foreach(var Task in tasks)
                {
                    // get tasks that have subtasks
                    var subtasks = await dataSubTask.GetSubTasksAsync(Task.Id);
                    if(subtasks.Count() >0)
                    {
                        tasklist.Add(Task);
                    }
                }
                dowTasks.AddRange(tasklist);               
            }
            else if (inprogress)
            {
                var inprogressTasks = tasks.Where(g => g.Status == "In Progress").ToList();
                dowTasks.AddRange(inprogressTasks);
            }         
           // await Getremainingdays();           
            //dows.AddRange(Dows);           
            //weekId = 0;
            IsBusy = false;
        }
    }
}
