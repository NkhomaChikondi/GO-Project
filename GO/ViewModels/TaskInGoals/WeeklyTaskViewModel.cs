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
        private double roundedtask = 0;
        private int weekId;
        private bool all = true;
        private bool notstarted;
        private bool inprogress;
        private bool withSubtasks; 
        private bool completed;
        public List<GoalTask> goalTaskslist = new List<GoalTask>();
        private int daySelected;
        private string dayName;


        public ObservableRangeCollection<DOW> dows { get; }
        public ObservableRangeCollection<GoalTask>dowTasks { get; }
        public ObservableRangeCollection<Task_Day> task_Days { get; }
       
        
        public ObservableRangeCollection<GoalTask> tasks { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand SunCommand { get; }
        public AsyncCommand MonCommand { get; }
        public AsyncCommand TueCommand { get; }
        public AsyncCommand WedCommand { get; }
        public AsyncCommand ThuCommand { get; }
        public AsyncCommand FriCommand { get; }
        public AsyncCommand weekStatsCommand { get; }
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
      
        public int GoalId { get => goalId; set => goalId = value; }
        public bool WithSubtasks { get => withSubtasks; set => withSubtasks = value; }
        public int DaySelected { get => daySelected; set => daySelected = value; }
        public string DayName { get => dayName; set => dayName = value; }

        public WeeklyTaskViewModel()
        {
            dows = new ObservableRangeCollection<DOW>();
            dowTasks = new ObservableRangeCollection<GoalTask>();
            task_Days = new ObservableRangeCollection<Task_Day>();

            RefreshCommand = new AsyncCommand(Refresh);
            OnAddCommand = new AsyncCommand(OnaddTask);
            SunCommand = new AsyncCommand(sunButton);
            MonCommand = new AsyncCommand(monButton);
            TueCommand = new AsyncCommand(tueButton);
            WedCommand = new AsyncCommand(wedButton);
            ThuCommand = new AsyncCommand(thuButton);
            FriCommand = new AsyncCommand(friButton);
            SatCommand = new AsyncCommand(satButton);
            weekStatsCommand = new AsyncCommand(gotoWeekstats);
            SendTaskIdCommand = new AsyncCommand<GoalTask>(SendTaskId);
            OnUpdateCommand = new AsyncCommand<GoalTask>(OnUpdateTask);          
            DeleteCommand = new AsyncCommand<GoalTask>(deleteTask);
            HelpCommand = new AsyncCommand(GotoHelpPage);
            HelpWeekCommand = new AsyncCommand(GotoHelpweekPage);
        }
        public async Task AllTasks()
        {
            all = true;
            notstarted = false;          
            inprogress = false;           
            WithSubtasks = false;
            await Refresh();
        }
        public async Task NotstartedTasks()
        {
            all = false;
            notstarted = true;          
            inprogress = false;
            completed = false;            
            WithSubtasks = false;
            await Refresh();
        }
        public async Task Withsubtasks()
        {
            all = false;
            notstarted = false;
            withSubtasks = true;            
            inprogress = false;
            completed = false;           
            await Refresh();
        }
        public async Task InprogressTasks()
        {
            all = false;
            notstarted = false;           
            inprogress = true;           
            WithSubtasks = false;
            completed = false;
            await Refresh();
        }
        public async Task CompletedTasks()
        {
            all = false;
            notstarted = false;           
            inprogress = false;
            WithSubtasks = false;
            completed = true;
            await Refresh();
        }
      
        // seed the days of the week into the database upon startup
        public async Task CreateDOW()
        {
            // get dows
           var alldows = await dataDow.GetDOWsAsync();
                if (alldows.Count() > 0)
                    return;
                else if (alldows.Count() == 0)
                {

                    var DowSunday = new DOW
                    {
                        Name = "Sunday",                        
                        IsSelected = false,                       
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowSunday);

                    var DowMonday = new DOW
                    {
                        Name = "Monday",                      
                        IsSelected = false,                       
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowMonday);

                    var DowTuesday = new DOW
                    {
                        Name = "Tuesday",                       
                        IsSelected = false,                      
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowTuesday);

                    var DowWednesday = new DOW
                    {
                        Name = "Wednesday",                       
                        IsSelected = false,                     
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowWednesday);


                    var DowThursday = new DOW
                    {
                        Name = "Thursday",                       
                        IsSelected = false,                      
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowThursday);

                    var DowFriday = new DOW
                    {
                        Name = "Friday",                       
                        IsSelected = false,                       
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowFriday);

                    var DowSaturday = new DOW
                    {
                        Name = "Saturday",                       
                        IsSelected = false,                       
                    };
                    // create the Dow Object
                    await dataDow.AddDOWAsync(DowSaturday);                    
                }
            }     
        async Task gotoWeekstats()
        {
            var route = $"{nameof(WeeklyTask)}?weekId={weekId}";
            await Shell.Current.GoToAsync(route);
        }        
        async Task OnaddTask()
        {
            var route = $"{nameof(AddPlannedTask)}?{nameof(addTaskViewModel.WeekId)}={WeekId}";
            await Shell.Current.GoToAsync(route);
        }
        async Task SendTaskId(GoalTask goalTask)
        {
           
            //
            //// get the day for the task
            //var day = await dataDow.GetDOWAsync(goalTask.DowId);
            //// get the week for the day
            //var week = await dataWeek.GetWeekAsync(day.WeekId);
            ////check if they are subtasks inside the goaltask
            //var subtasks = await dataSubTask.GetSubTasksAsync(goalTask.Id);
            //if (DateTime.Today > goalTask.CreatedOn)
            //{
            //    // check if the task has subtasks
            //    if(subtasks.Count() > 0)
            //    {
            //        var route = $"{nameof(subTaskView)}?SubtaskId={goalTask.Id}";
            //        await Shell.Current.GoToAsync(route);
            //    }
            //    else if(subtasks.Count() == 0)
            //    {
            //        await Application.Current.MainPage.DisplayAlert("Alert!", "You are unable to access the subtask page as this task has expired without any subtasks.", "OK");
            //        return;
            //   }

            //}
            //else
            //{
                //if (week.Active || subtasks.Count() > 0)
                //{
                //    var route = $"{nameof(subTaskView)}?SubtaskId={goalTask.Id}";
                //    await Shell.Current.GoToAsync(route);
                //}
                //else
                //{
                //    await Application.Current.MainPage.DisplayAlert("Alert!", "There are no subtasks associated with this task.", "Ok");
                //    return;
                //}
            

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
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Task!", "All Subtasks within this Task will also be deleted. Continue?", "Yes", "No");
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
                Datatoast.toast("Task Deleted");
            }
            else if (!ans)
                return;

        }
        //method for dow buttons
        public async Task sunButton()
        {
            //// get week having the weekid
            //var week = await dataWeek.GetWeekAsync(weekId);
            //// get all dows in the database
            //var dows = await dataDow.GetDOWsAsync(weekId);          
            //// loop through the dows
            //foreach (var dow in dows)
            //{
            //    if (dow.Name == "Sunday")
            //    {

            //        // get the Id
            //        dowId = dow.DOWId;
            //        // set is selectio to true
            //        dow.IsSelected = true;
            //        daySelected = dowId;
            //    }
                    
            //    else
            //        dow.IsSelected = false;
            //    //update dow
            //    await dataDow.UpdateDOWAsync(dow);
            //}
            //await Refresh();            
        }
        public async Task monButton()
        {
            //// get week having the weekid
            //var week = await dataWeek.GetWeekAsync(weekId);
            //var dows = await dataDow.GetDOWsAsync(weekId);
            //// loop through the dows
            //foreach (var dow in dows)
            //{
            //    if (dow.Name == "Monday")
            //    {
            //        // get the Id
            //        dowId = dow.DOWId;
            //        // set is selectio to true
            //        dow.IsSelected = true;
            //        daySelected = dowId;
            //    }

            //    else
            //        dow.IsSelected = false;
            //    //update dow
            //    await dataDow.UpdateDOWAsync(dow);
            //}
            //await Refresh();

        }
        public async Task tueButton()
        {
            //// get week having the weekid
            //var week = await dataWeek.GetWeekAsync(weekId);
            //var dows = await dataDow.GetDOWsAsync(weekId);
            //// loop through the dows
            //foreach (var dow in dows)
            //{
            //    if (dow.Name == "Tuesday")
            //    {
            //        // get the Id
            //        dowId = dow.DOWId;
            //        // set is selectio to true
            //        dow.IsSelected = true;
            //        daySelected = dowId;
            //    }

            //    else
            //        dow.IsSelected = false;
            //    //update dow
            //    await dataDow.UpdateDOWAsync(dow);
            //}
            //await Refresh();


        }
        public async Task wedButton()
        {
           // // get week having the weekid
           // var week = await dataWeek.GetWeekAsync(weekId);
           // var dows = await dataDow.GetDOWsAsync(weekId);
           // // loop through the dows
           // foreach (var dow in dows)
           // {
           //     if (dow.Name == "Wednesday")
           //     {
           //         // get the Id
           //         dowId = dow.DOWId;
           //         // set is selectio to true
           //         dow.IsSelected = true;
           //         daySelected = dowId;
           //     }

           //     else
           //         dow.IsSelected = false;
           //     //update dow
           //     await dataDow.UpdateDOWAsync(dow);
           // }
           // // get all tasks having sundayId
           //// var tasks = await dataSubtask.GetTasksAsync(goalId, week.Id);
           // await Refresh();

        }
       public async Task thuButton()
       {
       //     // get week having the weekid
       //     var week = await dataWeek.GetWeekAsync(weekId);
       //     var dows = await dataDow.GetDOWsAsync(weekId);
       //     // loop through the dows
       //     foreach (var dow in dows)
       //     {
       //         if (dow.Name == "Thursday")
       //         {
       //             // get the Id
       //             dowId = dow.DOWId;
       //             // set is selectio to true
       //             dow.IsSelected = true;
       //             daySelected = dowId;
       //         }

       //         else
       //             dow.IsSelected = false;
       //         //update dow
       //         await dataDow.UpdateDOWAsync(dow);
       //     }
       //     // get all tasks having sundayId
       //     //var tasks = await dataSubtask.GetTasksAsync(goalId, week.Id);
       //     await Refresh();

        }
       public async Task friButton()
       {

            //// get week having the weekid
            //var week = await dataWeek.GetWeekAsync(weekId);
            //var dows = await dataDow.GetDOWsAsync(weekId);
            //// loop through the dows
            //foreach (var dow in dows)
            //{
            //    if (dow.Name == "Friday")
            //    {
            //        // get the Id
            //        dowId = dow.DOWId;
            //        // set is selectio to true
            //        dow.IsSelected = true;
            //        daySelected = dowId;
            //    }

            //    else
            //        dow.IsSelected = false;
            //    //update dow
            //    await dataDow.UpdateDOWAsync(dow);
            //}
            //// get all tasks having sundayId
            ////var tasks = await dataSubtask.GetTasksAsync(goalId, week.Id);
            //await Refresh();
        }
       public async Task satButton()
        {
            //// get week having the weekid
            //var week = await dataWeek.GetWeekAsync(weekId);
            //var dows = await dataDow.GetDOWsAsync(weekId);
            //// loop through the dows
            //foreach (var dow in dows)
            //{
            //    if (dow.Name == "Saturday")
            //    {
            //        // get the Id
            //        dowId = dow.DOWId;
            //        // set is selection to true
            //        dow.IsSelected = true;
            //        daySelected = dowId;
            //    }

            //    else
            //        dow.IsSelected = false;
            //    //update dow
            //    await dataDow.UpdateDOWAsync(dow);
            //}
            //// get all tasks having sundayId
            ////var tasks = await dataSubtask.GetTasksAsync(goalId, week.Id);        
            //await Refresh();
        }
        async Task GotoHelpPage()
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Am here", "Ok");
        }
        async Task GotoHelpweekPage()
        {
            //var route = $"{nameof(HelpWeekPage)}";
            //await Shell.Current.GoToAsync(route);
        }
        public async Task CompleteTask(int TaskId, bool IsComplete)
        {
           var todayDow = DateTime.Today ;
            // get the task having the same id as taskId
            var task = await dataTask.GetTaskAsync(TaskId);
            var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
            var taskDays = await dataTaskDay.GetTaskdaysAsync();
            // get the task that has todays dowId
            var todayTask = taskDays.Where(t => t.Taskid == TaskId && t.DowId == dowId).FirstOrDefault();
            // check if today is more than or equal task start date
            if(todayDow < task.StartTask)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", "You cannot mark this task as complete. The day allocated for the task has either already passed or has not yet been reached.", "Ok");
                await Refresh();
                return;
            }
            else if(todayDow >= task.StartTask)
            {
                // check if today is equal to dow name
                // get the dow having the dowId
                var dows = await dataDow.GetDOWsAsync();
                var dow = dows.Where(d => d.DOWId == dowId).FirstOrDefault();
                if(todayDow.DayOfWeek.ToString() != dow.Name)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "You cannot mark this task as complete. The day allocated for the task has either already passed or has not yet been reached.", "Ok");
                    return;
                }
                else if(todayDow.DayOfWeek.ToString() == dow.Name)
                {
                    if (todayTask == null)
                        return;
                    else
                    {
                        // check if todaytask is completed
                        if (task.IsCompleted)
                            return;
                        else if (!task.IsCompleted)
                        {
                            // get the day having the dowId
                            // get week
                            var week = await dataWeek.GetWeekAsync(task.WeekId);
                            // complete taskday
                            todayTask.Iscomplete = true;
                            await dataTaskDay.UpdateTaskdayAsync(todayTask);

                            task.IsCompleted = IsComplete;
                            await dataTask.UpdateTaskAsync(task);
                            await SetStatus();
                            await CalculateTotalWeekPercentage(week);
                        }
                    }
                }             
            }
           
         
            //// check if the incoming object 
            //if (task.IsCompleted)
            //    return;
            //else if (!task.IsCompleted)
            //{
            //    // get all task_days
            //    var task_days = await dataTaskDay.GetTaskdaysAsync();
            //    // get the object having the taskiD
            //    var validTasks = task_days.Where(T => T.Taskid == task.Id).ToList();
            //    if(validTasks.Count() == 1)
            //    {
            //        // get the object
            //        var getTask = validTasks.FirstOrDefault();
            //        // get the dow having the dowId
            //        var dow = await dataDow.GetDOWAsync(getTask.DowId);
            //        // check if the dowId is similar to today's dowId
            //        if(DateTime.Today.DayOfWeek.ToString() == dow.Name)
            //        {
            //            if (subtasks.Count() > 0)
            //                return;
            //            //check if it has no subtask
            //            else if (subtasks.Count() == 0)
            //            {
            //                task.IsCompleted = IsComplete;
            //                await dataTask.UpdateTaskAsync(task);
            //                await SetStatus();
            //                await CalculateTotalWeekPercentage(week);
            //            }
            //        }
            //        else
            //        {
            //            await Application.Current.MainPage.DisplayAlert("Error!", "You cannot mark this task as complete. The day allocated for the task has either already passed or has not yet been reached.", "Ok");
            //            await Refresh();
            //            return;
            //        }

            //    }
            //    else if(validTasks.Count()>1)
            //    {
            //        bool found = false;
            //        // loop through the valid taskday item
            //        foreach (var taskday in validTasks)
            //        {
            //            // get the dow having the dowId
            //            var dow = await dataDow.GetDOWAsync(taskday.DowId);
            //            if (DateTime.Today.DayOfWeek.ToString() == dow.Name)
            //            {
            //                found = true;
            //                if (subtasks.Count() > 0)
            //                    return;
            //                //check if it has no subtask
            //                else if (subtasks.Count() == 0)
            //                {
            //                    task.IsCompleted = IsComplete;
            //                    await dataTask.UpdateTaskAsync(task);
            //                    await SetStatus();
            //                    await CalculateTotalWeekPercentage(week);
            //                }
            //            }                        
            //        }
            //        if(!found)
            //        {

            //            await Application.Current.MainPage.DisplayAlert("Error!", "You cannot mark this task as complete. The day allocated for the task has either already passed or has not yet been reached.", "Ok");
            //            await Refresh();
            //            return;
            //        }
            //        return;
            //    }
            //        // loop thorugh the
            //        ////get the day the task is assigned to
            //        //var day = await dataDow.GetDOWAsync(task.DowId);
            //        //// check if the day name is less than or equal to the day of today                
            //        //// get the week the day is assigned to
            //        //var week = await dataWeek.GetWeekAsync(day.WeekId);

            //        //if (week.Active)
            //        //{
            //        //    if(DateTime.Today.Date == day.Date.Date)
            //        //    {
            //        //        if (subtasks.Count() > 0)
            //        //            return;
            //        //        //check if it has no subtask
            //        //        else if (subtasks.Count() == 0)
            //        //        {
            //        //            task.IsCompleted = IsComplete;
            //        //            await dataTask.UpdateTaskAsync(task);
            //        //            await SetStatus();
            //        //            await CalculateTotalWeekPercentage(week);
            //        //        }
            //        //    }                 
            //        //    else
            //        //    {
            //        //        await Application.Current.MainPage.DisplayAlert("Error!", "You cannot mark this task as complete. The day allocated for the task has either already passed or has not yet been reached.", "Ok");
            //        //        await Refresh();
            //        //        return;
            //        //    }                 
            //        //}
            //        //else
            //        //{
            //        //    //await Refresh();
            //        //    await Application.Current.MainPage.DisplayAlert("Error!", "You cannot mark this task as complete. The task has already expired.", "OK");
            //        //    await Refresh();
            //        //    return;
            //        //}


            //    }           
            //return;
        }
        public async Task CompleteWeeklyTask(int TaskId, bool IsComplete)
        {
            // get the task having the taskId
            var allTasks = await dataTask.GetTasksAsync(goalId, weekId);
            var task = allTasks.Where(t => t.Id == TaskId).FirstOrDefault();
            // get the week having the weekid
            var weeks = await dataWeek.GetWeeksAsync(goalId);
            var week = weeks.Where(w => w.Id == weekId).FirstOrDefault();

            if(week.status  == "Not Started" || week.status == "Expired")
            {
                if (week.status == "Not Started")
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", $"The start date for this week, has not been reached yet", "Ok");
                    return;
                    await Refresh();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", $" This week has expired", "Ok");
                    return;
                    await Refresh();
                }               
            }
            else
            {
                // get todays day of week
                var todayDow = DateTime.Today;
                // get  the dow having the dowId
                var dows = await dataDow.GetDOWsAsync();
                var dow = dows.Where(d => d.DOWId == dowId).FirstOrDefault();

                if (dow.Name != todayDow.DayOfWeek.ToString())
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", $"Only {todayDow.DayOfWeek.ToString()} tasks can be completed today!", "Ok");                   
                    return;
                    await Refresh();
                }
                else if (dow.Name == todayDow.DayOfWeek.ToString())
                {
                    // get all task days
                    var taskdays = await dataTaskDay.GetTaskdaysAsync();
                    // get the taskdday that have both the to be completed task and today's dowId
                    var taskDow = taskdays.Where(t => t.Taskid == task.Id && t.DowId == dowId).FirstOrDefault();
                    // check if it is completed
                    if (taskDow.Iscomplete)
                        return;
                    else if(!taskDow.Iscomplete)
                    {
                        taskDow.Iscomplete = true;
                        task.PendingPercentage += taskDow.Percentage;
                        task.IsCompleted = true;

                        await dataTask.UpdateTaskAsync(task);
                        await dataTaskDay.UpdateTaskdayAsync(taskDow);
                        await SetStatus();
                        await CalculateTotalWeekPercentage(week);                    

                    }
                }
            }        
        }
        public async Task UncompleteTask(int TaskId, bool IsComplete)
        {
            // get the task having the taskId
            var allTasks = await dataTask.GetTasksAsync(goalId, weekId);
            var task = allTasks.Where(t => t.Id == TaskId).FirstOrDefault();
            // get the week having the weekid
            var weeks = await dataWeek.GetWeeksAsync(goalId);
            var week = weeks.Where(w => w.Id == weekId).FirstOrDefault();

            if (week.status == "Not Started" || week.status == "Expired")
            {
                if (week.status == "Not Started")
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", $"The start date for this week, has not been reached yet", "Ok");
                    return;
                    await Refresh();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", $" This week has expired", "Ok");
                    return;
                    await Refresh();
                }

            }
            else
            {
                // get todays day of week
                var todayDow = DateTime.Today;
                // get  the dow having the dowId
                var dows = await dataDow.GetDOWsAsync();
                var dow = dows.Where(d => d.DOWId == dowId).FirstOrDefault();

                if (dow.Name != todayDow.DayOfWeek.ToString())
                {
                    await Application.Current.MainPage.DisplayAlert("Alert!", $"Only {todayDow.DayOfWeek.ToString()} tasks can be  uncompleted today!", "Ok");                   
                    return;
                    await Refresh();
                }
                else if (dow.Name == todayDow.DayOfWeek.ToString())
                {
                    // get all task days
                    var taskdays = await dataTaskDay.GetTaskdaysAsync();
                    // get the taskdday that have both the to be completed task and today's dowId
                    var taskDow = taskdays.Where(t => t.Taskid == task.Id && t.DowId == dowId).FirstOrDefault();
                    // check if it is completed
                    if (!taskDow.Iscomplete)
                        return;
                    else if (taskDow.Iscomplete)
                    {
                        taskDow.Iscomplete = false;
                        task.PendingPercentage -= taskDow.Percentage;
                        task.IsCompleted = false;

                        await dataTask.UpdateTaskAsync(task);
                        await dataTaskDay.UpdateTaskdayAsync(taskDow);
                        await SetStatus();
                        await CalculateTotalWeekPercentage(week);                                      
                    }
                }
            }
        }
        async Task CalculateSubtaskPercentage()
        {
            // get all week having the goal id
            var weeks = await dataWeek.GetWeeksAsync(goalId);
            // loop through the week and get the days id
            foreach(var week in weeks)
            {
                // get all tasks having the week id
                var weekTasks = await dataTask.GetTasksAsync(goalId,week.Id);
                if (weekTasks.Count() > 0)
                {
                    foreach (var task in weekTasks)
                    {
                        // get all subtasks having the tasks id
                        var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
                        //check if there are subtasks having this task's id
                        if (subtasks.Count() == 0)
                        {
                            task.IsEnabled = true;

                        }
                        else
                        {
                            // loop through the subtasks
                            foreach (var subtask in subtasks)
                            {
                                // check if they are completed
                                if (subtask.IsCompleted)
                                {
                                    roundedtask += subtask.Percentage;
                                }
                            }
                        }
                        task.PendingPercentage = 0;
                        task.PendingPercentage = Math.Round(roundedtask, 1);
                        task.Progress = task.PendingPercentage / task.Percentage;
                        await dataTask.UpdateTaskAsync(task);
                        await SetStatus();
                        roundedtask = 0;
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
       public async Task CalculateTotalWeekPercentage(Week week)
        {
            var getweeks = await dataWeek.GetWeeksAsync(GoalId);            
            double TaskPercentage = 0;
            double subtaskpercentage = 0;
            double Accumulated = 0;


            // get all tasks having the week id
            var weekTasks = await dataTask.GetTasksAsync(GoalId, weekId);
            if (weekTasks.Count() == 0)
                return;
            else
            {
                // loop through the tasks
                foreach (var task in weekTasks)
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
            }
          //  check if todays date is less than the weeks end date and update status accordingly
            if (DateTime.Today < week.EndDate)
            {
                if ( DateTime.Today.Date < week.StartDate.Date)
                    week.status = "Not Started";
                else if ( week.StartDate.Date >= DateTime.Today.Date && DateTime.Today.Date <= week.EndDate)
                    week.status = "In Progress";
                else if (week.AccumulatedPercentage == week.TargetPercentage)
                    week.status = "Completed";
            }
            else if (DateTime.Today > week.EndDate)
                week.status = "Expired";
            await dataWeek.UpdateWeekAsync(week);                    
                
                        //reset the below variables
                TaskPercentage = 0;
                subtaskpercentage = 0;
                Accumulated = 0;                            
            
        }       
        public async Task Refresh()
        {           
            IsBusy = true;          
            await CreateDOW();
            await CalculateSubtaskPercentage();
            // get week having the week id
            var currentWeek = await dataWeek.GetWeekAsync(weekId);
            await CalculateTotalWeekPercentage(currentWeek);
                     // dows.Clear();
            dowTasks.Clear();
            goalTaskslist.Clear();
            // get all dows
            var dbtaskdays = await dataTaskDay.GetTaskdaysAsync();
            if(dbtaskdays.Count() > 0)
            {                
                // loop through the task days
                foreach (var taskday in dbtaskdays)
                {
                    // get the tasks having this weeks id and  has dowid similar to dowid of 'this' dowid
                    if (taskday.DowId == DowId)
                    {    
                        // get task task
                        var task = await dataTask.GetTaskAsync(taskday.Taskid);
                        if (task != null)
                        {
                            // check if it is completed
                            if(taskday.Iscomplete)
                            {
                                task.IsCompleted = true;
                                task.Status = "Completed";
                                await dataTask.UpdateTaskAsync(task);
                            }
                           else if(!taskday.Iscomplete)
                           {
                                task.IsCompleted = false;
                                task.Status = "Not started";
                                await dataTask.UpdateTaskAsync(task);
                           }
                            // check if the task Id belongs to the designated week
                            if (task.WeekId == weekId)
                                goalTaskslist.Add(task);
                        }                                                  
                                                                 
                    }
                }
                if (goalTaskslist.Count() > 0)
                {
                    // get tasks having this week's Id
                    var weeklyTask = goalTaskslist.Where(T => T.WeekId == weekId).ToList();
                    if(weeklyTask.Count() == 0)
                    {
                        Datatoast.toast("Create Tasks for this week!");
                    }
                    else
                          dowTasks.AddRange(weeklyTask);
                }                  
                else
                    Datatoast.toast("No tasks!");
            }
            else return;
           
            //// get all tasks having the goal id
            //var tasks = await dataTask.GetTasksAsync(GoalId,weekId);
            //if(tasks.Count() >0)
            //{
            //    foreach (var task in tasks)
            //    {
            //        /// calculate number of sbtasks in the tasks
            //        var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
            //        task.SubtaskNumber = subtasks.Count();
            //        // update task
            //        await dataTask.UpdateTaskAsync(task);
            //    }
            //}

            //var dayTasks = tasks.Where(t => t.DowId == daySelected).ToList();          
            //dayName = null;
            //if (all)
            //    // retrieve the categories back
            //    dowTasks.AddRange(dayTasks);
            ////filter goals
            //else if (notstarted)
            //{
            //    var notstartedtasks = dayTasks.Where(g => g.Status == "Not Started").ToList();
            //    if(notstartedtasks.Count == 0)
            //        Datatoast.toast("No tasks!");
            //    else
            //    dowTasks.AddRange(notstartedtasks);
            //}
            //else if (completed)
            //{
            //   var completedtasks = dayTasks.Where(g => g.IsCompleted).ToList();
            //    if (completedtasks.Count == 0)
            //        Datatoast.toast("No tasks!");
            //    else
            //    dowTasks.AddRange(completedtasks);
            //}
            //else if (inprogress)
            //{
            //    var inprogressTasks = dayTasks.Where(g => g.PendingPercentage > 0 && g.Status != "Expired").ToList();
            //    if(inprogressTasks.Count == 0)
            //        Datatoast.toast("No tasks!");
            //    else
            //    dowTasks.AddRange(inprogressTasks);
            //}

            //else if (withSubtasks)
            //{
            //    List<GoalTask> tasklist = new List<GoalTask>();
            //    //loop through the tasks
            //    foreach (var Task in dayTasks)
            //    {
            //        // get tasks that have subtasks
            //        var subtasks = await dataSubTask.GetSubTasksAsync(Task.Id);
            //        if (subtasks.Count() > 0)
            //        {
            //            tasklist.Add(Task);
            //        }
            //    }
            //    if(tasklist.Count == 0)
            //        Datatoast.toast("No tasks!");
            //    else
            //    dowTasks.AddRange(tasklist);
            //}
            IsBusy = false;          
        }                  
    }
}
