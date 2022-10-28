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
        public ObservableRangeCollection<Goal> goals { get; }
        public AsyncCommand<Goal> AddgoalCommand { get; }
        public AsyncCommand<Goal> GetgoalCommand { get; set; }
        public AsyncCommand<Goal> DeleteCommand { get; }
        public AsyncCommand<Goal> UpdateCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand HelpCommand { get; }
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
        async Task OnaddGoal(Object obj)
        {
            var route = $"{nameof(AddGoalview)}?{nameof(AddGoalViewModel.CategoryId)}={categoryId}";
            await Shell.Current.GoToAsync(route);
        }
        async Task selectGoalItem(Goal goal)
        {
            // get the tasks having the goal id
            var tasks = await dataTask.GetTasksAsync(goal.Id);
            // check if the goal has expired with no tasks
            if (goal.Status == "Expired")
            {
                // check if it has tasks
                if (tasks.Count() == 0)
                    await Application.Current.MainPage.DisplayAlert("Alert", "Cannot view tasks for this goal! It expired with no tasks.", "Ok");
                return;
            }

            else
            {
                // check if the HAS WEEK in goal is == true
                if (goal.HasWeek && !goal.Noweek)
                {
                    await CreateWeeksInBulk(goal);
                    // get the last week's id in this goal
                    var weeks = await dataWeek.GetWeeksAsync(goal.Id);
                    var lastweek = weeks.ToList().LastOrDefault();

                    var route = $"{nameof(WeeklyTask)}?weekId={lastweek.Id}";
                    await Shell.Current.GoToAsync(route);

                }
                else if (!goal.HasWeek && goal.Noweek)
                {
                    var route = $"{nameof(GoalTaskPage)}?goalId={goal.Id}";
                    await Shell.Current.GoToAsync(route);
                }
            }

        }
        async Task GotoHelpPage()
        {
            var route = $"{nameof(Helpgoalpage)}";
            await Shell.Current.GoToAsync(route);
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
            }
            else if (!ans)
                return;

        }


        async Task calculateGoalPercentage()
        {
            double TaskPercentage = 0;
            double subtaskpercentage = 0;
            double goalRoundedPercentage = 0;

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
                            var tasks = await dataTask.GetTasksAsync(goal.Id, week.Id);
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
                            //weeks calculations
                        }
                    }
                    //goals calculation
                    goalRoundedPercentage = TaskPercentage + subtaskpercentage;
                    goal.Percentage = Math.Round(goalRoundedPercentage, 2);
                    goal.Progress = goal.Percentage / goal.ExpectedPercentage;
                    // update goal
                    await datagoal.UpdateGoalAsync(goal);
                    TaskPercentage = 0;
                    subtaskpercentage = 0;
                    goalRoundedPercentage = 0;
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
                    goal.Percentage = Math.Round(goalRoundedPercentage, 2);
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
        async Task CreateWeeksInBulk(Goal goal)
        {
            if (IsBusy.Equals(true))
                return;
            try
            {
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


                // get all weeks having the goalId
                var weeks = await dataWeek.GetWeeksAsync(goal.Id);
                // get the last created week
                var lastCreatedWeek = weeks.ToList().LastOrDefault();
                // create a mock value
                var yesToday = new DateTime(2022, 11, 13);
                // check the lastcreatedweek end date if has not been reached yet
                if (yesToday <= lastCreatedWeek.EndDate)//DateTime.Today <= lastCreatedWeek.EndDate)
                    return;
                else
                {
                    // find out what date it was on sunday(for this week)
                    DateTime sundayDate = yesToday;//DateTime.Today;
                    // to find the days between todays date and the last created week's end date
                    TimeSpan dd = new TimeSpan(6, 0, 0, 0);
                    //check if today is sunday
                    if (startDay == "Sunday")
                    {
                        sundayDate = yesToday; // DateTime.Today;
                    }
                    else if (startDay != "Sunday")
                    {
                        // subtract startday day value from sundays day value
                        var newValue = 6 - dayValue;
                        // create a timespan to show the days
                        TimeSpan dayDifference = new TimeSpan(newValue, 0, 0, 0);
                        // subtract today's date by the newValue to get sundays date
                        sundayDate = yesToday - dayDifference;// DateTime.Today - dayDifference;
                    }
                    // get the total number of weeks from goal
                    var totalWeeks = goal.NumberOfWeeks;
                    // calculate the percentage for every week
                    var weekPercentage = 100 / totalWeeks;

                    // check if adding 7 days to lastcreated week will be equal to sundayDate
                    if (lastCreatedWeek.EndDate.AddDays(8) == sundayDate)
                    {
                        // create a new week
                        var newWeek = new Week
                        {
                            WeekNumber = lastCreatedWeek.WeekNumber + 1,
                            TargetPercentage = weekPercentage,
                            AccumulatedPercentage = 0,
                            Active = true,
                            StartDate = sundayDate,
                            EndDate = sundayDate.AddDays(6),
                            Progress = 0,
                            CreatedAutomatically = false,
                            Status = "Not Started",
                            GoalId = goal.Id
                        };
                        // add the new week to database
                        await dataWeek.AddWeekAsync(newWeek);
                        var notification = new NotificationRequest
                        {
                            BadgeNumber = 1,
                            Description = $"Week {newWeek.WeekNumber} of goal '{goal.Name}' is Due today!",
                            Title = "Due-Date!",
                            NotificationId = newWeek.Id,
                            Schedule =
                                {
                                    NotifyTime = DateTime.Now.AddSeconds(20),
                                }
                        };
                        await LocalNotificationCenter.Current.Show(notification);
                    }
                    else if (lastCreatedWeek.EndDate.AddDays(8) < sundayDate)
                    {
                        // add 6 days to dateDifference
                        result = lastCreatedWeek.EndDate.AddDays(7);
                        // for week number
                        var WeekNum = lastCreatedWeek.WeekNumber;
                        while (result <= sundayDate)
                        {
                            // increment weeknum by 1 everytime it loops
                            WeekNum++;
                            if (result.AddDays(1) == sundayDate)
                            {
                                // create a new week
                                var newWeek = new Week
                                {
                                    WeekNumber = WeekNum,
                                    TargetPercentage = weekPercentage,
                                    AccumulatedPercentage = 0,
                                    Active = true,
                                    StartDate = result - dd,
                                    EndDate = result,
                                    Progress = 0,
                                    CreatedAutomatically = false,
                                    Status = "Not Started",
                                    GoalId = goal.Id
                                };
                                // add the new week to database
                                await dataWeek.AddWeekAsync(newWeek);
                                var notification = new NotificationRequest
                                {
                                    BadgeNumber = 1,
                                    Description = $"Week {newWeek.WeekNumber} of goal '{goal.Name}' is Due today!",
                                    Title = "Due-Date!",
                                    NotificationId = newWeek.Id,
                                    Schedule =
                                {
                                    NotifyTime = DateTime.Now.AddSeconds(20),
                                }
                                };
                                await LocalNotificationCenter.Current.Show(notification);

                            }
                            else if (result < sundayDate)
                            {
                                // create a new week
                                var newWeek = new Week
                                {
                                    WeekNumber = WeekNum,
                                    TargetPercentage = weekPercentage,
                                    AccumulatedPercentage = 0,
                                    Active = false,
                                    StartDate = result - dd,
                                    EndDate = result,
                                    Progress = 0,
                                    CreatedAutomatically = true,
                                    Status = "Not Started",
                                    GoalId = goal.Id
                                };
                                // add the new week to database
                                await dataWeek.AddWeekAsync(newWeek);
                            }
                            // give a new date to result each time it loops
                            //result = result.AddDays(7);
                        }
                        return;

                    }
                    ////check how many days are there between sunday date and lastcreatedweek end date
                    //var dateDifference = sundayDate - lastCreatedWeek.EndDate;
                    //// calculate percentage that has to be given to each week

                    //// check if the date difference is more than 6 days
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to create a new week, restart your app: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }

            finally
            {
                IsBusy = false;
            }

        }
        public async Task Refresh()
        {
            // set "IsBusy" to true
            IsBusy = true;
            // clear categories on the page
            goals.Clear();
            await calculateGoalPercentage();
            //await Getremainingdays();
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
                var completedtasks = goals.Where(g => g.Status == "Completed").ToList();
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
