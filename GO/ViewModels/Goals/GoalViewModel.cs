using GO.Models;
using GO.ViewModels.TaskInGoals;
using GO.Views.Goal;
using GO.Views.GoalTask;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.Goals
{
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]
    public class GoalViewModel : BaseViewmodel
    {
        // create a private property that will receive the incoming id 
        private int categoryId;
        private bool all = true;
        private bool notstarted;
        private bool inprogress;
        private bool completed;
        private bool duesoon;
        private bool expired;
        private DateTime result;
        private DateTime startdate;
        private DateTime endDate;
        public ObservableRangeCollection<Goal> goals { get; }
        public AsyncCommand<Goal> AddgoalCommand { get; }
        public AsyncCommand<Goal> GetgoalCommand { get; set; }
        public AsyncCommand<Goal> DeleteCommand { get; }
        public AsyncCommand<Goal> UpdateCommand { get; }
        public AsyncCommand RefreshCommand { get; }
       
        public AsyncCommand<Goal> ItemSelectedCommand { get; }
        // get only those withna categoryId

        public int CategoryId
        {
            get
            { return categoryId; }
            set => categoryId = value;
        }

        public bool All { get => all; set => all = value; }
        public bool Notstarted { get => notstarted; set => notstarted = value; }
        public bool Inprogress { get => inprogress; set => inprogress = value; }
        public bool Completed { get => completed; set => completed = value; }
        public bool Duesoon { get => duesoon; set => duesoon = value; }
        public bool Expired { get => expired; set => expired = value; }
        //public DateTime Result { get => result; set => result = value; }

        public GoalViewModel()
        {
            goals = new ObservableRangeCollection<Goal>();
            AddgoalCommand = new AsyncCommand<Goal>(OnaddGoal);
            DeleteCommand = new AsyncCommand<Goal>(deleteGoal);
            UpdateCommand = new AsyncCommand<Goal>(OnUpdateGoal);
            RefreshCommand = new AsyncCommand(Refresh);
            ItemSelectedCommand = new AsyncCommand<Goal>(selectGoalItem);
           
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
        async Task OnaddGoal(Object obj)
        {
            var route = $"{nameof(AddGoalview)}?{nameof(AddGoalViewModel.CategoryId)}={categoryId}";
            await Shell.Current.GoToAsync(route);
        }
        async Task selectGoalItem(Goal goal)
        {            
            var tasks = await dataTask.GetTasksAsync(goal.Id);
            // check if the HAS WEEK in goal is == true
            if (goal.HasWeek && !goal.Noweek)
            {
                // get all weeks having the goalId
                var allWeeks = await dataWeek.GetWeeksAsync(goal.Id);

                // get the dates for the current week
                DateTime today = DateTime.Today;
                int daysUntilLastSunday = ((int)today.DayOfWeek - (int)DayOfWeek.Sunday + 7) % 7;
                startdate = today.AddDays(-daysUntilLastSunday);
                endDate = startdate.AddDays(6);

                // loop through the weeks and assign their status
                foreach (var week in allWeeks)
                {
                    if(DateTime.Today >= week.StartDate && DateTime.Today <= week.EndDate)
                    {
                        // get all tasks having the week Id
                        var Task_Week = tasks.Where(T => T.WeekId == week.Id).ToList();
                        if(Task_Week.Count() > 0)
                        {
                            // check if all tasks having the week id have been completed
                            if (Task_Week.All(T => T.IsCompleted))
                            {
                                week.status = "Completed";
                            }
                            else
                                week.status = "In Progress";
                        }
                      
                    }
                    else if(DateTime.Today > week.StartDate)
                    {
                        week.status = "Expired";
                    }
                    //  update week
                    await dataWeek.UpdateWeekAsync(week);
                }

                if(goal.Status == "Expired")
                {
                    // get the last week of the goal
                    var lastweek = allWeeks.LastOrDefault();

                    var route = $"{nameof(Weekly_Task)}?weekId={lastweek.Id}";
                    await Shell.Current.GoToAsync(route);

                }
                // check if the goal has started
                else if(DateTime.Today <= goal.Start)
                {
                    // get the first week of the week
                    var first_Week = allWeeks.FirstOrDefault();
                    var route = $"{nameof(Weekly_Task)}?weekId={first_Week.Id}";
                    await Shell.Current.GoToAsync(route);
                }               
                else
                {                   
                    // get the week similar to dates of the current calendar week
                    var Activeweek = allWeeks.Where(W => W.StartDate >= startdate && W.EndDate <= endDate).FirstOrDefault();

                    var route = $"{nameof(Weekly_Task)}?weekId={Activeweek.Id}";
                    await Shell.Current.GoToAsync(route);
                }             
            }
            else if (!goal.HasWeek && goal.Noweek)
            {
                var route = $"{nameof(GoalTaskPage)}?goalId={goal.Id}";
                await Shell.Current.GoToAsync(route);
            }         
          
        }      
        async Task OnUpdateGoal(Goal goal)
        {
            var route = $"{nameof(UpdateGoalPage)}?goalId={goal.Id}";
            await Shell.Current.GoToAsync(route);
        }
        async Task deleteGoal(Goal goal)
        {
            if (goal == null)
                return;
            var ans = await Application.Current.MainPage.DisplayAlert("Delete Goal", "All Tasks in this Goal will be deleted. Continue?", "Yes", "No");
            if (ans)
            {
                await datagoal.DeleteGoalAsync(goal.Id);
                // get all tasks having the goal id
                var tasks = await dataTask.GetTasksAsync(goal.Id);
                // cancel its notification
                LocalNotificationCenter.Current.Cancel(goal.Id);
                // check if it has weeks
                if (goal.NumberOfWeeks > 0)
                {
                    // get all weeks having goal id
                    var weeks = await dataWeek.GetWeeksAsync(goal.Id);
                    //loop through the weeks
                    foreach (var week in weeks)
                    {
                        // cancel its notification
                        LocalNotificationCenter.Current.Cancel(week.Id);
                        await dataWeek.DeleteWeekAsync(week.Id);                        
                    }
                }
                // loop through the tasks
                foreach (var task in tasks)
                {
                    // delete the task
                    await dataTask.DeleteTaskAsync(task.Id);
                    // cancel notification
                    //get subtasks having the task id
                    LocalNotificationCenter.Current.Cancel(task.Id);
                    var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
                    // loop through the subtasks
                    foreach (var subtask in subtasks)
                    {
                        await dataSubTask.DeleteSubTaskAsync(subtask.Id);
                        //cancel subtask notification
                        LocalNotificationCenter.Current.Cancel(subtask.Id);
                    }
                }
                await Refresh();
                Datatoast.toast("Goal deleted ");
            }
            else if (!ans)
                return;

        }
        async Task calculateGoalPercentage()
        {
            double TaskPercentage = 0;
            double subtaskpercentage = 0;
            double goalRoundedPercentage = 0;
            Double totalWeekPercentage = 0;

            // get all the goals having the category id
            var goals = await datagoal.GetGoalsAsync(categoryId);
            // get goals that have weeks in them
            var weekgoals = goals.Where(g => g.HasWeek).ToList();
            if (weekgoals.Count() > 0)
            {
                // Loop through the week goals
                foreach (var goal in weekgoals)
                {
                    // get weeks having the goalid
                    var weeks = await dataWeek.GetWeeksAsync(goal.Id);
                    // check if weeks has more than zero weeks
                    if (weeks.Count() > 0)
                    {               
                        // loop through the weeks and get tasks having the weekid
                        foreach (var week in weeks)
                        {
                            // get all tasks having the goal id and week id
                            var weektasks = await dataTask.GetTasksAsync(goal.Id, week.Id);
                            // loop through the tasks 
                            foreach(var task in weektasks)
                            {                               
                                // check if task is complete
                                if (task.IsCompleted)
                                {
                                    TaskPercentage += task.Percentage;
                                }
                                else if(!task.IsCompleted)
                                {
                                    TaskPercentage += task.PendingPercentage;
                                }    
                            }
                            //totalWeekPercentage += week.AccumulatedPercentage;
                        }
                    }
                   
                    goal.Percentage = Math.Round(TaskPercentage);
                    goal.Progress = goal.Percentage / goal.ExpectedPercentage;
                    // update goal
                    await datagoal.UpdateGoalAsync(goal);
                    totalWeekPercentage = 0;
                    TaskPercentage = 0;
                }
            }
            var weeklessGoals = goals.Where(g => g.Noweek).ToList();
            if (weeklessGoals.Count() > 0)
            {
                //loop through the goals
                foreach (var goal in weeklessGoals)
                {
                    // get all tasks having the goal id
                    var tasks = await dataTask.GetTasksAsync(goal.Id);
                    // check if they are tasks having the week id
                    if (tasks.Count() > 0)
                    {
                        // loop through the tasks to get their percentage
                        foreach (var task in tasks)
                        {
                            // check if it is completed
                            if (task.IsCompleted)
                            {
                                TaskPercentage += task.Percentage;
                                TaskPercentage = Math.Round(TaskPercentage, 2);
                            }
                            else if (!task.IsCompleted)
                            {
                                // check if it has subtask
                                var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
                                if (subtasks.Count() > 0)
                                {
                                    // get only subtasks that are completed
                                    var completedsubtasks = subtasks.Where(s => s.IsCompleted).ToList();
                                    // loop through the completed subtasks
                                    foreach (var subtask in completedsubtasks)
                                    {
                                        subtaskpercentage += subtask.Percentage;
                                    }
                                }

                            }
                        }
                    }
                    //goals calculation
                    goalRoundedPercentage = TaskPercentage + subtaskpercentage;
                    goal.Percentage = Math.Round(goalRoundedPercentage);
                    goal.Progress = goal.Percentage / goal.ExpectedPercentage;
                    // update goal
                    await datagoal.UpdateGoalAsync(goal);
                    TaskPercentage = 0;
                    subtaskpercentage = 0;
                    goalRoundedPercentage = 0;
                }

            }

        }
        async Task setStatus()
        {
            // get all goals having the category id
            var goals = await datagoal.GetGoalsAsync(categoryId);
            // loop through them
            foreach (var goal in goals)
            {
                if (goal.Percentage == 0 && DateTime.Today <= goal.End)
                    goal.Status = "Not Started";
                else if (goal.Percentage > 0 && goal.Percentage < goal.ExpectedPercentage && DateTime.Today <= goal.End)
                    goal.Status = "In Progress";
                else if (goal.Percentage == goal.ExpectedPercentage && DateTime.Today <= goal.End)
                    goal.Status = "Completed";
                else if (DateTime.Today > goal.End)
                    goal.Status = "Expired";
                await datagoal.UpdateGoalAsync(goal);
            }
        }
        async Task Getremainingdays()
        {
            var goals = await datagoal.GetGoalsAsync(categoryId);
            foreach (var goal in goals)
            {
                if (DateTime.Today < goal.End)
                {
                    TimeSpan daysleft = goal.End - DateTime.Today;
                    goal.DaysLeft = (int)daysleft.TotalDays;
                }
                else if (DateTime.Today == goal.End)
                    goal.DaysLeft = 1;
                else
                    goal.DaysLeft = 0;
                await datagoal.UpdateGoalAsync(goal);
            }
        }       
        async Task CreateWeek(Goal goal)
        {
            if (IsBusy.Equals(true))
                return;
            try
            {
                int WeekNumber = 0;
                //get the number of days left for the goal to end
                var Daysleft = goal.End - DateTime.Today;
                TimeSpan daynumber = new TimeSpan(7, 0, 0, 0);

                // get the start day, day of the week
                var startDay = DateTime.Today.DayOfWeek.ToString();
                int dayValue = 0;
                if (startDay == "Sunday")
                    dayValue = 6;
                else if (startDay == "Monday")
                    dayValue = 5;
                else if (startDay == "Tuesday")
                    dayValue = 4;
                else if (startDay == "Wednesday")
                    dayValue = 3;
                else if (startDay == "Thursday")
                    dayValue = 2;
                else if (startDay == "Friday")
                    dayValue = 1;
                else if (startDay == "Saturday")
                    dayValue = 0;

                if (DateTime.Today == goal.End)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", "Failed to create a new week. The goal for this week is expiring today", "Ok");
                    return;
                }

                else if (DateTime.Today > goal.End)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", "Failed to create a new week. The goal for this week expired", "Ok");
                    return;
                }
                // check how many days are left till the goal is due
                var DaysTillTheGoalIsDue = goal.End - DateTime.Today;

                if (DaysTillTheGoalIsDue.TotalDays < 1)
                {
                    await Application.Current.MainPage.DisplayAlert("Alert", "Failed to create new week for this goal. It is expiring today.", "Ok");
                    return;
                }
                // get the weeks having the goal id
                var GoalWeeks = await dataWeek.GetWeeksAsync(goal.Id);
                // get the last created week
                var LastCreatedWeek = GoalWeeks.ToList().LastOrDefault();
                
                // check if today's date is more that last created week end date
                if(DateTime.Today > LastCreatedWeek.EndDate)
                {
                    //// check if it active
                    //if (LastCreatedWeek.Active)
                    //{
                    //    LastCreatedWeek.Active = false;
                    //    await dataWeek.UpdateWeekAsync(LastCreatedWeek);
                    //}
                    // get the number of weeks in a goal
                    var totalWeeks = goal.NumberOfWeeks;
                    // calculate the percentage for every week
                    var weekPercentage = 100 / totalWeeks;
                    // subtract goal's end date from the last created week end date
                    var result = goal.End - LastCreatedWeek.EndDate;

                    if (result.TotalDays <= 1)
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "failed to create a new week. The week's goal has 1 day left before it expires.", "Ok");
                        return;
                    }
                    else
                    {
                        DateTime endDate = new DateTime();
                        DateTime startDate = new DateTime();
                        // get the number of week days
                        var weeksLeft = result.TotalDays / 7;
                        // if there is a remainder after division, add 1 to weeknumber
                        if (weeksLeft % 7 != 0)
                            weeksLeft += 1;

                        if (weeksLeft == 1)
                        {
                            endDate = goal.End;
                        }
                        else if(weeksLeft > 1)
                        {
                           // check if today's day of the week is friday
                           if(dayValue.Equals(0))
                           {
                                var answer = await Application.Current.MainPage.DisplayAlert("Alert", "If you create a new week today (Saturday), the start date of the week will automatically be moved to Sunday. Do you wish to proceed", "Yes", "No");
                                if (!answer)
                                    return;
                                else if(answer)
                                {
                                    DateTime SundayDate;
                                    if(DateTime.Today.DayOfWeek.Equals("Friday"))
                                    {
                                        SundayDate = DateTime.Today.AddDays(2);
                                        startDate = SundayDate;
                                        endDate = SundayDate.AddDays(6);
                                    }
                                    else if(DateTime.Today.DayOfWeek.Equals("Saturday"))
                                    {
                                        SundayDate = DateTime.Today.AddDays(1);
                                        startDate = SundayDate;
                                        endDate = SundayDate.AddDays(6);
                                    }
                                         
                                }
                           }
                           else
                           {
                                // subtract dayvalue from 6(Sunday's day value                                
                                startDate = DateTime.Today;
                                endDate = startDate.AddDays(dayValue);
 
                           }
                            // calculation to find the weeks number
                            // get the date of the past saturdays day
                            var saturdayDate = DateTime.Today.AddDays(dayValue);
                            var totalDays = saturdayDate - LastCreatedWeek.EndDate;
                            var divideTotaldays = totalDays.TotalDays / 7;
                            WeekNumber = LastCreatedWeek.WeekNumber + (int) divideTotaldays;
                           
                           //create a new week
                            var Newweek = new Week
                            {
                                EndDate = endDate,
                                StartDate = startDate,
                                AccumulatedPercentage = 0,
                              
                                WeekNumber = WeekNumber,
                                TargetPercentage = weekPercentage,
                                Progress = 0,
                              
                                GoalId = goal.Id
                            };
                            // add the new week to the database
                            await dataWeek.AddWeekAsync(Newweek);
                            var notification = new NotificationRequest
                            {
                                BadgeNumber = 1,
                                Description = $" Week {Newweek.WeekNumber} of goal '{goal.Name}' is Due today!",
                                Title = "Due-Date!",
                                NotificationId = Newweek.Id,
                                Schedule =
                                            {
                                                NotifyTime = DateTime.Today, //Newweek.EndDate,
                                            }
                            };
                            await LocalNotificationCenter.Current.Show(notification);
                        }                     

                    }                   
                                       
                }
                else 
                {
                    return;                
                }            
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to create a new week {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }

            finally
            {
                IsBusy = false;
            }

        }           
        async Task createWeek(Goal goal)
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            // get the week of goals end date
            CultureInfo ci = CultureInfo.InvariantCulture;
            DateTimeFormatInfo dfi = DateTimeFormatInfo.GetInstance(ci);
            Calendar cal = ci.Calendar;

            int week1 = cal.GetWeekOfYear(goal.End, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            if(goal.Start.DayOfWeek.ToString() != "Sunday")
            {
                // get the date of next saturday
               var nextsaturday = goal.Start.AddDays(6 - (int)goal.Start.DayOfWeek);
                startDate = goal.Start;
            }
           else if (goal.Start.DayOfWeek.ToString() == "Sunday")
           {              
                startDate = goal.Start;
           }
            // loop through the number of weeks inside a goal
            for (int i = 0; i < goal.NumberOfWeeks; i++)
            {
                var newWeek = new Week
                {
                    StartDate = startDate,
                    
                };

            }
           
        }
        public async Task Refresh()
        {
            // set "IsBusy" to true
            IsBusy = true;
            // clear categories on the page
            goals.Clear();
            await calculateGoalPercentage();
           // await Getremainingdays();
            await setStatus();
            // get all categories
            var goal = await datagoal.GetGoalsAsync(categoryId);

            if (all)
                // retrieve the categories back
                goals.AddRange(goal);
            //filter goals
            else if (notstarted)
            {
                var notstartedtasks = goal.Where(g => g.Status == "Not Started").ToList();
                goals.AddRange(notstartedtasks);
            }
            else if (completed)
            {
                var completedtasks = goal.Where(g =>  g.Percentage == 100).ToList();
                goals.AddRange(completedtasks);
            }
            else if (inprogress)
            {
                var inprogressTasks = goal.Where(g => g.Status == "In Progress").ToList();
                goals.AddRange(inprogressTasks);
            }
            else if (duesoon)
            {
                var Date10 = DateTime.Today.AddDays(10);
                var duesoongoals = goal.Where(g => g.End <= Date10 && g.Status != "Expired").ToList();
                goals.AddRange(duesoongoals);
            }
            else if (expired)
            {
                var expiredTasks = goal.Where(g => g.Status == "Expired").ToList();
                goals.AddRange(expiredTasks);
            }

            // set "isBusy" to false
            IsBusy = false;

        }
    }
}
