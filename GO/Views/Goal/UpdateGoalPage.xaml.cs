﻿using GO.Models;
using GO.Services;
using GO.ViewModels.Goals;
using GO.ViewModels.TaskInGoals;
using GO.Views.GoalTask;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.Goal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    [QueryProperty(nameof(goalId), nameof(goalId))]
    public partial class UpdateGoalPage : ContentPage
    {

        public string goalId { get; set; }
        public int CategoryId { get; set; }
        public int GoalId { get; set; }
        Models.Goal Goal = new Models.Goal();
        public IDataGoal<Models.Goal> dataGoal { get; }
        public IDataTask<Models.GoalTask> dataTask { get; }
        public IDataSubtask<Models.Subtask> dataSubtask { get; }
        public IDataWeek<Week> dataWeek { get; }
        public IDataDow<DOW> dataDow { get; }
        public IToast GetToast { get; }
        public UpdateGoalPage()
        {
            InitializeComponent();
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            dataWeek = DependencyService.Get<IDataWeek<Week>>();
            dataSubtask = DependencyService.Get<IDataSubtask<Subtask>>();
            dataDow = DependencyService.Get<IDataDow<DOW>>();
            GetToast = DependencyService.Get<IToast>();
            BindingContext = new GoalViewModel();
            //detaillabel.TranslateTo(100, 0, 3000, Easing.Linear);

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            int.TryParse(goalId, out var result);
            // get the goal having the id
            var goal = await dataGoal.GetGoalAsync(result);
            Goal = goal;
            CategoryId = goal.CategoryId;
            GoalId = goal.Id;
            Nameeditor.Text = goal.Name;
            Desclbl.Text = goal.Description;
            Createdlbl.Text = goal.CreatedOn.ToString();
            Statuslbl.Text = goal.Status;
            Startdatepicker.Date = goal.Start;
            enddatepicker.Date = goal.End;
            weeknumber.Text = goal.NumberOfWeeks.ToString();
            // get all tasks in this goal
            var tasks = await dataTask.GetTasksAsync(GoalId);
            AllTask.Text = tasks.Count().ToString();
            // get all completed tasks
            var completedTasks = tasks.Where(C => C.IsCompleted).ToList();
            CompletedTasks.Text = completedTasks.Count().ToString();
            // get all uncompletedTasks
            var uncompletedTasks = tasks.Where(U => !U.IsCompleted).ToList();
            UncompletedTasks.Text = uncompletedTasks.Count().ToString();
            // get all expiredtask
            var expiredtasks = tasks.Where(E => E.Status == "Expired").ToList();
            Expiredtasks.Text = expiredtasks.Count().ToString();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            // check if the app is busy
            if (IsBusy == true)
                return;
            try
            {
                
                // create a new goal object and save
                var newGoal = new Models.Goal
                {
                    Name = Nameeditor.Text,
                    Description = Desclbl.Text,                    
                    Start = Startdatepicker.Date,
                    End = enddatepicker.Date,                             
                    CategoryId = Goal.CategoryId
                };
                // get all tasks in GoalId
                var allGoals = await dataGoal.GetGoalsAsync(CategoryId);
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newGoal.Name[0]) + newGoal.Name.Substring(1);
                //check if the new task already exist in the database
               
                 if (newGoal.Description == null)
                     newGoal.Description = $"No Description for \" {UppercasedName}\" ";
                double weeksNumber = Goal.NumberOfWeeks;
                if(Goal.Noweek)
                {
                    if (newGoal.Start != Goal.Start)
                    {
                        // check that start date is not more than end date
                        if (newGoal.Start > newGoal.End)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error!", "Failed to update. Start Date of a goal cannot be more than the end date of the goal.", "Ok");
                            return;
                        }
                        // check that a start date of a goal is not equal to its end date
                        if (newGoal.Start == newGoal.End)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error!", "Failed to update. Start Date of a goal cannot be equal to the goal's end date.", "Ok");
                            return;
                        }
                        // first check if a goal has tasks
                        var goaltasks = await dataTask.GetTasksAsync(Goal.Id);
                        if (goaltasks.Count() > 0)
                        {
                            // make sure today's date is more than the newgoal start date
                            if (DateTime.Today > newGoal.Start)
                            {
                                await App.Current.MainPage.DisplayAlert("Alert!", "Failed to update goal, start date can not be less than today's date", "OK");
                                return;
                            }
                            else if (DateTime.Today <= newGoal.Start)
                            {
                                if (goaltasks.Any(t => t.StartTask < newGoal.Start))
                                {
                                    //check if there is any goaltask that has reached its due date and get all of them
                                    var endedtasks = goaltasks.Where(t => t.EndTask < newGoal.Start).ToList();
                                    // get all goals tasks whose end date if with the duration of the goal
                                    var validTasks = goaltasks.Where(t => t.EndTask > newGoal.Start && t.EndTask < Goal.End).ToList();
                                    if (endedtasks.Count() > 0 || validTasks.Count() > 0)
                                    {
                                        var Result = await Application.Current.MainPage.DisplayAlert("Alert!", "All tasks in this goal that have ended, will be deleted. All tasks whose start date is before the new goal's start date, if completed, will be uncompleted and then they will automatically be moved to another date. Continue?", "Yes", "No");
                                        if (Result)
                                        {
                                            if (endedtasks.Count() > 0 )
                                            {
                                                // loop through each task nand delete it
                                                foreach (var task in endedtasks)
                                                {
                                                    await deleteTask(task);
                                                }
                                                //check if it has valid tasks
                                                if(validTasks.Count() > 0)
                                                {
                                                    // if the tasks where completed, they will be uncompleted
                                                    foreach (var task in validTasks)
                                                    {
                                                        if (task.IsCompleted)
                                                        {
                                                            task.IsCompleted = false;
                                                        }
                                                        // give the task the start date of the goal's start date
                                                        task.StartTask = newGoal.Start;
                                                        //task.EndTask = newGoal.End;
                                                        await dataTask.UpdateTaskAsync(task);
                                                    }
                                                }

                                            }

                                            else if (validTasks.Count() > 0)
                                            {
                                                // if the tasks where completed, they will be uncompleted
                                                foreach (var task in validTasks)
                                                {
                                                    if (task.IsCompleted)
                                                    {
                                                        task.IsCompleted = false;
                                                    }
                                                        
                                                    // give the task the start date of the goal's start date
                                                    task.StartTask = newGoal.Start;
                                                    //task.EndTask = newGoal.End;
                                                    await dataTask.UpdateTaskAsync(task);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // check if the changed end date is below the date of today
                    if (newGoal.End < DateTime.Today)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "An updated End Date of a goal, cannot be below the date of today", "Ok");
                        return;
                    }
                    // make sure you cannot expand the end date of a task that has expired whilst it was completed
                    if (Goal.Percentage == Goal.ExpectedPercentage && Goal.Status == "Expired")
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Failed to change the end date. Cannot change the end date of a goal that has expired whilst completed", "Ok");
                        return;
                    }
                    // check if the incoming end date is not equal to the end date from the database
                    if (newGoal.End != Goal.End)
                    {
                        // check if the goal is completed
                        if (Goal.Percentage == Goal.ExpectedPercentage)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error", "Failed to change the end date. Cannot change the end date of a goal that has already been completed, unless, you add new tasks to it", "Ok");
                            return;
                        }
                        // check if todays date is more than goal.end date
                        if (DateTime.Today > Goal.End && newGoal.End > Goal.End)
                        {
                            // make sure the updated end date is more than todays date
                            if (newGoal.End < DateTime.Today)
                            {
                                await Application.Current.MainPage.DisplayAlert("Error", "End Date of a task cannot be below the date of today", "Ok");
                                return;
                            }
                            var result = await Application.Current.MainPage.DisplayAlert("Alert", "You are adjusting the end date of a goal that has expired. Continue?", "Yes", "No");
                            if (result)
                            {
                                if (Goal.Status == "Expired")
                                    Goal.Status = "In Progress";
                            }
                            else if (!result)
                                return;
                        }

                        //// recalculate number of weeks in goal
                        //var duration = newGoal.End - newGoal.Start;
                        //// divide the duration by 7
                        //double doubleduration = duration.TotalDays;
                        //weeksNumber = doubleduration / 7;
                        //// get the remainder if any from the above division
                        //var remainder = weeksNumber % 7;
                        //if (remainder != 0)
                        //{
                        //    // add 1 to weeknumber
                        //    weeksNumber = weeksNumber + 1;
                        //}
                    }
                    else if (newGoal.End < Goal.End)
                    {
                        // check if they are no tasks whose end date surpasses the goals end date
                        // get tasks having the goals id
                        var tasks = await dataTask.GetTasksAsync(Goal.Id);
                        // loop through the tasks
                        var counter = 0;
                        foreach (var task in tasks)
                        {
                            if (task.EndTask > Goal.End)
                                counter += 1;

                        }
                        if (counter > 0)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update goal, they are task's in it, whose end date is more than the goal's selected end date. Go to task page, find those tasks and modify their end dates", "OK");
                            return;
                        }
                    }
                    // create a new goal object
                    var newestGoal = new Models.Goal
                    {
                        Id = GoalId,
                        Name = UppercasedName,
                        Description = Desclbl.Text,
                        End = enddatepicker.Date,
                        CreatedOn = Convert.ToDateTime(Createdlbl.Text),
                        Start = Startdatepicker.Date,
                        CategoryId = Goal.CategoryId,
                        HasWeek = Goal.HasWeek,
                        Noweek = Goal.Noweek,
                        NumberOfWeeks = 0,
                        Percentage = Goal.Percentage,
                        ExpectedPercentage = Goal.ExpectedPercentage,
                        Progress = Goal.Progress,
                        Status = Goal.Status,
                        Time = Goal.Time,
                        enddatetostring = newGoal.End.ToLongDateString(),
                    };
                    await dataGoal.UpdateGoalAsync(newestGoal);
                    // check if updated goal's end date is more than dbgoal end date
                    if (newestGoal.End > Goal.End)
                    {
                        LocalNotificationCenter.Current.Cancel(newestGoal.Id);
                        // check if it has weeks
                        if (Goal.HasWeek)
                        {
                            await RecalculateWeekpercentage(newestGoal.NumberOfWeeks);
                        }
                    }
                    // cancel its notification
                    await SendNotification();
                    await Shell.Current.GoToAsync("..");
                    GetToast.toast("Goal updated");
                }
                else if(Goal.HasWeek)
                {
                    // get the week number
                    // recalculate number of weeks in goal
                    var duration = newGoal.End - newGoal.Start;
                    // divide the duration by 7
                    double doubleduration = duration.TotalDays;
                    weeksNumber = doubleduration / 7;
                    // get the remainder if any from the above division
                    var remainder = weeksNumber % 7;
                    if (remainder != 0)
                    {
                        // add 1 to weeknumber
                        weeksNumber = weeksNumber + 1;
                    }
                    // get all weeks in this goal
                    var goalWeeks = await dataWeek.GetWeeksAsync(Goal.Id);
                    if(newGoal.Start > Goal.Start)
                    {
                        // create a dow variable
                        DOW dOW = null ;
                        // get all weeks whose end date is below the new start date
                        var allWeeks = goalWeeks.Where(w => w.EndDate.Date < newGoal.Start.Date).ToList();
                        var leftWeeks = goalWeeks.Where(w => w.EndDate.Date > newGoal.Start.Date).ToList();
                        if (allWeeks.Count() > 0)
                        {
                            // delete all weeks whose end date is below 
                            foreach (var week in allWeeks)
                            {
                                await deleteweeklyTask(week);
                            }
                        }
                        // loop through all the weeks to get the day whose date is equal to weeks start date
                        foreach (var week in leftWeeks)
                        {
                            // get all days in the week
                            var days = await dataDow.GetDOWsAsync(week.Id);
                            // loop through the days and get the one having the date
                            foreach (var day in days)
                            {
                                if(day.Date == newGoal.Start.Date)
                                {
                                    dOW = day;
                                    return;
                                }
                            }                           
                        }
                        // get get the week of the day
                        var dayWeek = leftWeeks.Where(W => W.Id == dOW.WeekId).FirstOrDefault();
                        
                    }

                }
               // await goalrules(newGoal);
                //// check if the changed end date is below the date of today
                //if (newGoal.End < DateTime.Today)
                //{
                //    await Application.Current.MainPage.DisplayAlert("Error!", "An updated End Date of a goal, cannot be below the date of today", "Ok");
                //    return;
                //}
                //// make sure you cannot expand the end date of a task that has expired whilst it was completed
                //if (Goal.Percentage == Goal.ExpectedPercentage && Goal.Status == "Expired")
                //{
                //    await Application.Current.MainPage.DisplayAlert("Error", "Failed to change the end date. Cannot change the end date of a goal that has expired whilst completed", "Ok");
                //    return;
                //}
                //// check if the incoming end date is not equal to the end date from the database
                //if (newGoal.End != Goal.End)
                //{      
                //    // check if the goal is completed
                //    if(Goal.Percentage == Goal.ExpectedPercentage)
                //    {
                //        await Application.Current.MainPage.DisplayAlert("Error", "Failed to change the end date. Cannot change the end date of a goal that has already been completed, unless, you add new tasks to it", "Ok");
                //        return;
                //    }
                
                //        // check if todays date is more than goal.end date
                //    if (DateTime.Today > Goal.End && newGoal.End > Goal.End)
                //    {
                //        // make sure the updated end date is more than todays date
                //        if (newGoal.End < DateTime.Today)
                //        {
                //            await Application.Current.MainPage.DisplayAlert("Error!", "End Date of a task cannot be below the date of today", "Ok");
                //            return;
                //        }
                //            var result = await Application.Current.MainPage.DisplayAlert("Alert", "You are adjusting the end date of a goal that has expired. Continue?", "Yes", "No");
                //        if (result)
                //        {
                //            if (Goal.Status == "Expired")
                //                Goal.Status = "In Progress";
                //        }                                        
                //        else if (!result)
                //            return;
                //    }             
                
                //    // recalculate number of weeks in goal
                //    var duration = newGoal.End - newGoal.Start;
                //    // divide the duration by 7
                //    double doubleduration = duration.TotalDays;
                //    weeksNumber = doubleduration / 7;
                //    // get the remainder if any from the above division

                //    var remainder = weeksNumber % 7;
                //    if (remainder != 0)
                //    {

                //        // add 1 to weeknumber
                //        weeksNumber = weeksNumber + 1;
                //    }
                //}
                //else if(newGoal.End < Goal.End)
                //{
                //    // check if they are no tasks whose end date surpasses the goals end date
                //    // get tasks having the goals id
                //    var tasks = await dataTask.GetTasksAsync(Goal.Id);
                //    // loop through the tasks
                //    var counter = 0;
                //    foreach (var task in tasks)
                //    {
                //        if(task.EndTask > Goal.End)                        
                //            counter += 1;       
                        
                //    }
                //    if(counter > 0)
                //    {
                //        await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update goal, they are task's in it, whose end date is more than the goal's selected end date. Go to task page, find those tasks and modify their end dates","OK");
                //        return;
                //    }
                //}
                //// create a new goal object
                //var newestGoal = new Models.Goal
                //{
                //    Id = GoalId,
                //    Name = UppercasedName,
                //    Description = Desclbl.Text,
                //    End = enddatepicker.Date,
                //    CreatedOn = Convert.ToDateTime(Createdlbl.Text),
                //    Start = Startdatepicker.Date,
                //    CategoryId = Goal.CategoryId,
                //    HasWeek = Goal.HasWeek,
                //    Noweek = Goal.Noweek,
                //    NumberOfWeeks = (int)weeksNumber,
                //    Percentage = Goal.Percentage,
                //    ExpectedPercentage = Goal.ExpectedPercentage,
                //    Progress = Goal.Progress,
                //    Status = Goal.Status,
                //    Time = Goal.Time,
                //    enddatetostring = newGoal.End.ToLongDateString(),
                //};
                //await dataGoal.UpdateGoalAsync(newestGoal);
                //// check if updated goal's end date is more than dbgoal end date
                //if (newestGoal.End > Goal.End )
                //{
                //    LocalNotificationCenter.Current.Cancel(newestGoal.Id);
                //    // check if it has weeks
                //    if(Goal.HasWeek)
                //    {
                //        await RecalculateWeekpercentage(newestGoal.NumberOfWeeks);
                //    }
                //}              
                //// cancel its notification
                // await SendNotification();                
                //await Shell.Current.GoToAsync("..");
                //GetToast.toast("Goal updated");
            }

            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", $"Failed to update goal: {ex.Message}", "OK");
            }

            finally
            {
                IsBusy = false;
            }
            async Task SendNotification()
            {
                // get goal form the database
                var dbgoal = await dataGoal.GetGoalAsync(GoalId);
                
                var notification = new NotificationRequest
                {
                    BadgeNumber = 1,
                    Description = $"Goal '{dbgoal.Name}' is Due today!",
                    Title = "Due-Date!",
                    NotificationId = dbgoal.Id,
                    Schedule =
                    {
                        NotifyTime = dbgoal.End,
                    }
                };
                await LocalNotificationCenter.Current.Show(notification);

            };
        }
        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            if (Goal.HasWeek )
            {
                // get the weekid of the last inserted week
                // get all weeks having the goal id
                var weeks = await dataWeek.GetWeeksAsync(Goal.Id);
                var lastweek = weeks.ToList().LastOrDefault();
                var route = $"{nameof(WeeklyTask)}?weekId={lastweek.Id}";
                await Shell.Current.GoToAsync(route);
            }
            else if (Goal.Noweek )
            {
                var route = $"{nameof(GoalTaskPage)}?goalId={Goal.Id}";
                await Shell.Current.GoToAsync(route);
            }
        }
        async Task RecalculateWeekpercentage(int num)
        {
            // get all weeks having the goal Id
            var weeks = await dataWeek.GetWeeksAsync(GoalId);
            // calculate the new percentage
            var newweekPercentage = 100 / num;
            foreach (var week in weeks)
            {
                // assign the new percentage to the week percentage
                week.TargetPercentage = newweekPercentage;
                // get tasks having the week's Id
                var tasks = await dataTask.GetTasksAsync(GoalId, week.Id);
                if(tasks.Count() > 0)
                {
                    // loop through the tasks inside the tasks and recalculate their percentage
                    foreach (var task in tasks)
                    {
                        task.Percentage = newweekPercentage / tasks.Count();                       

                        // get subtasks having the tasks Id
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);
                        task.PendingPercentage = 0;
                        if (subtasks.Count() > 0)
                        {
                            // loop through the subtasks to recalculate their percentage
                            foreach (var subtask in subtasks)
                            {
                                subtask.Percentage = task.Percentage / subtasks.Count();
                              
                                task.PendingPercentage += subtask.Percentage;
                                await dataSubtask.UpdateSubTaskAsync(subtask);
                            }
                        }
                        await dataTask.UpdateTaskAsync(task);
                    }
                }
                await dataWeek.UpdateWeekAsync(week);
            }
        }
        async Task goalrules(Models.Goal newGoal)
        {
            if (newGoal.Start != Goal.Start)
            {
                // check that start date is not more than end date
                if (newGoal.Start > newGoal.End)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Failed to update. Start Date of a goal cannot be more than the end date of the goal.", "Ok");
                    return;
                }
                // check that a start date of a goal is not equal to its end date
                if (newGoal.Start == newGoal.End)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Failed to update. Start Date of a goal cannot be equal to the goal's end date.", "Ok");
                    return;
                }
                // first check if a goal has tasks
                var goaltasks = await dataTask.GetTasksAsync(Goal.Id);
                if (goaltasks.Count() > 0)
                {
                    // make sure today's date is more than the newgoal start date
                    if(DateTime.Today > newGoal.Start)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert!", "Failed to update goal, start date can not be less than today's date", "OK");
                        return;
                    }
                    else if(DateTime.Today <= newGoal.Start)
                    {                        
                        if (goaltasks.Any(t => t.StartTask < newGoal.Start))
                        {
                            //check if there is any goaltask that has reached its due date and get all of them
                            var endedtasks = goaltasks.Where(t => t.EndTask < newGoal.Start).ToList();
                            if(endedtasks.Count() > 0)
                            {
                                var Result = await Application.Current.MainPage.DisplayAlert("Alert!", "By updating this goal's start date, all tasks that have ended will be deleted. Continue?","Yes", "No");
                                if (Result)
                                {
                                    // loop through each task nand delete it
                                    foreach (var task in endedtasks)
                                    {
                                        await deleteTask(task);
                                    }
                                 //   GetToast.toast($"{endedtasks.Count()} tasks with their subtasks have been deleted");
                                }
                                else
                                    return;                               
                            }
                            var validTasks = goaltasks.Where(t => t.EndTask > newGoal.Start && t.EndTask < Goal.End).ToList();
                            if (validTasks.Count() > 0)
                            {
                                // get all goals tasks whose end date if with the duration of the goal
                               
                                if(validTasks.Count() > 0)
                                {
                                    var result = await App.Current.MainPage.DisplayAlert("Alert!", "By changing the start date of this goal, all tasks whose start date has already pass, can either be deleted or be automatically moved to another date. Continue?", "Yes", "No");
                                    if(result)
                                    {
                                        var result1 = await App.Current.MainPage.DisplayAlert("Alert!", "Choose whether to delete or move tasks", "Delete", "Move");
                                        if(result1)
                                        {
                                            var result11 = await App.Current.MainPage.DisplayAlert("Alert!", "You have chosen to delete the tasks. Continue?", "Yes?", "No");
                                            if (result11)
                                            {
                                                foreach (var task in validTasks)
                                                {
                                                    await deleteTask(task);
                                                }
                                               // GetToast.toast($"{validTasks.Count()} tasks with their subtasks have been deleted");
                                            }
                                            else
                                                return;
                                        }
                                        else if(!result1)
                                        {
                                            var result11 = await App.Current.MainPage.DisplayAlert("Alert!", "You have chosen to move the tasks. The start and end date of the tasks will be automatically adjusted. Continue?", "Yes?", "No");
                                            if (result11)
                                            {
                                                await App.Current.MainPage.DisplayAlert("Alert!", "By choosing to move tasks to a new date, all completed tasks will be uncompleted.", "Ok");
                                                // if the tasks where completed, they will be uncompleted
                                                foreach (var task in validTasks)
                                                {
                                                    if (task.IsCompleted)
                                                        task.IsCompleted = false;
                                                    // give the task the start date of the goal's start date
                                                    task.StartTask = newGoal.Start;
                                                    task.EndTask = newGoal.End;
                                                    await dataTask.UpdateTaskAsync(task);
                                                }
                                               // GetToast.toast($"{validTasks.Count()} tasks have been updated!");
                                            }
                                           
                                        }
                                    }
                                    
                               }
                            }                           
                           
                            // get tasks whose start date
                            await Application.Current.MainPage.DisplayAlert("Error!", "You cannot change the start date of this goal, they are some tasks in it, whose start date is before the goals start date.", "Ok");
                            return;
                        }
                        //else if (goaltasks.Any(t => t.StartTask >= newGoal.Start))
                        //{
                        //    if (goaltasks.Any(t => t.IsCompleted))
                        //    {
                        //        // send an alert informing the user that if they wish to change their start day their completed tasks will be made uncomplete
                        //        var result = await Application.Current.MainPage.DisplayAlert("Alert", "Changing the start date of this goal, will uncomplete all the completed tasks in it. Continue?", "Yes", "No");
                        //        if (result)
                        //        {
                        //            // check if there is any task that is completed
                        //            foreach (var task in goaltasks)
                        //            {
                        //                if (task.IsCompleted)
                        //                    task.IsCompleted = false;
                        //                await dataTask.UpdateTaskAsync(task);
                        //            }
                        //        }
                        //        else if (!result)
                        //            return;
                        //    }
                        //}
                    }
                    
                }

            }
        }
        async Task deleteTask (Models.GoalTask goalTask)
        {
            // get all subtasks if any and delete them
            var subtasks = await dataSubtask.GetSubTasksAsync(goalTask.Id);
            if (subtasks.Count() > 0)
            {
                foreach (var subtask in subtasks)
                {
                    await dataSubtask.DeleteSubTaskAsync(subtask.Id);
                }
            }
            await dataTask.DeleteTaskAsync(goalTask.Id);             
        }
        async Task deleteweeklyTask(Models.Week week)
        {
            //get the days having the week id
            var days = await dataDow.GetDOWsAsync(Goal.Id);
            //loop through the days and get the tasks in them
            foreach (var day in days)
            {
                // get tasks having the the days id
                var tasks = await dataTask.GetTasksAsync(day.DOWId);
                if (tasks.Count() > 0)
                {
                    // loop through the tasks to get their subtasks
                    foreach (var task in tasks)
                    {
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);
                        // delete task
                        await dataTask.DeleteTaskAsync(task.Id);
                        if(subtasks.Count() > 0)
                        {
                            // loop through the tasks and delete all of them
                            foreach (var subtask in subtasks)
                            {
                                // delete subtast
                                await dataSubtask.DeleteSubTaskAsync(subtask.Id);
                            }
                        }
                    }                    
                }
                // delete day
                await dataDow.DeleteDOWAsync(day.DOWId);
            }
            // delete week
            await dataWeek.DeleteWeekAsync(week.Id);           
        }
    }
}
