using GO.Models;
using GO.Services;
using GO.ViewModels.TaskInGoals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.GoalTask
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(weekId), nameof(weekId))]
    public partial class Weekly_Task : ContentPage
    {
        public string weekId { get; set; }
        private int goalId;
        private IEnumerable<DOW> dOWs;
        private Week selectedWeek;
     
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataSubtask<Subtask> datasubtask { get; }
        public ITaskday<Models.Task_Day> datataskDay { get; set; }
        public IDataWeek<Models.Week> dataWeek { get; }
        public IDataDow<DOW> dataDow { get; }
        public Weekly_Task()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataWeek = DependencyService.Get<IDataWeek<Models.Week>>();
            DataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();      
            datasubtask = DependencyService.Get<IDataSubtask<Subtask>>();
            dataDow = DependencyService.Get<IDataDow<DOW>>();
            BindingContext = new WeeklyTaskViewModel();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
             int.TryParse(weekId, out var result);
            // get the  week having the week id
            var week = await dataWeek.GetWeekAsync(result);
            //var task_day = await datataskDay.GetTaskdaysAsync();           
            selectedWeek = week;
            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(week.GoalId);
            // get all DOws
            var dows = await dataDow.GetDOWsAsync();
            if(dows.Count() == 0)
            {
               if(BindingContext is WeeklyTaskViewModel WVM)
               {
                   await WVM.CreateDOW();
                    dows = await dataDow.GetDOWsAsync();
               }
            }
            DOW todayDow = null;
            dOWs = dows;
            // loop through the days and get the day that is equal to the currrent date
            foreach (var day in dows)
            {
                if(day.Name == DateTime.Today.DayOfWeek.ToString())
                { 
                    todayDow = day;
                }
            }
            dayTask.Text = todayDow.Name;
            // set the field of Isselected of dows to false apart from this
            todayDow.IsSelected = true;           
            await dataDow.UpdateDOWAsync(todayDow);
            foreach (var dowitem in dows)
            {
                if(dowitem.DOWId != todayDow.DOWId)
                {
                    dowitem.IsSelected = false;
                    await dataDow.UpdateDOWAsync(dowitem);
                }
            }
            goalweeklypercentage.Text = week.AccumulatedPercentage.ToString();
            weeklygoalprogress.Progress = week.Progress;
            await assigningWeekValues(week);
            // call set date method
            SetDate(week);
            //setImagevisibility();
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.GoalId = goal.Id;
                wvm.WeekId = result;
                wvm.DowId = todayDow.DOWId;
                // await showButtonclicked(0);
               await wvm.Refresh();
            }
        }
        void SetDate(Week week)
        {
            // get the day of the week of weeks start date
            var weekDOW = week.StartDate.DayOfWeek;
            if(DateTime.Today.DayOfWeek.ToString() == "Sunday" && weekDOW.ToString() == "Sunday")
            {

                sunDate.Text = week.StartDate.ToString("dd");
                monDate.Text = week.StartDate.AddDays(1).ToString("dd");
                tueDate.Text = week.StartDate.AddDays(2).ToString("dd");
                wedDate.Text = week.StartDate.AddDays(3).ToString("dd");
                thuDate.Text = week.StartDate.AddDays(4).ToString("dd");
                friDate.Text = week.StartDate.AddDays(5).ToString("dd");
                satDate.Text = week.StartDate.AddDays(6).ToString("dd");
            }
            else
            {
                // get the date of the previous sunday
                int daystoRemove = (int)weekDOW;
                DateTime previousSunday = week.StartDate.AddDays(-daystoRemove);

                sunDate.Text = previousSunday.ToString("dd");
                monDate.Text = previousSunday.AddDays(1).ToString("dd");
                tueDate.Text = previousSunday.AddDays(2).ToString("dd");
                wedDate.Text = previousSunday.AddDays(3).ToString("dd");
                thuDate.Text = previousSunday.AddDays(4).ToString("dd");
                friDate.Text = previousSunday.AddDays(5).ToString("dd");
                satDate.Text = previousSunday.AddDays(6).ToString("dd");
            }

        }     

        private async void TapGestureRecognizersun_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var sundaydow = dOWs.Where(s => s.Name == "Sunday").FirstOrDefault();
            var tasks = await DataTask.GetTasksAsync(selectedWeek.GoalId, selectedWeek.Id);
            if (DateTime.Today.DayOfWeek.ToString() != "Sunday")
            {

                foreach (var taskitem in tasks)
                {
                   // check if the task is completed

                }
            }
            else if (DateTime.Today.DayOfWeek.ToString() == "Sunday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = true;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = sundaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Sunday";
            sunImg.BackgroundColor = Color.LightGray;
            monImg.BackgroundColor = Color.White;
            tueImg.BackgroundColor = Color.White;
            wedImg.BackgroundColor = Color.White;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizermon_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var mondaydow = dOWs.Where(s => s.Name == "Monday").FirstOrDefault();          
            // the all tasks having the weekid will be disabled
            var tasks = await DataTask.GetTasksAsync(selectedWeek.GoalId, selectedWeek.Id);
            if (DateTime.Today.DayOfWeek.ToString() != "Monday")    
            {
               
                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = false;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            else if (DateTime.Today.DayOfWeek.ToString() == "Monday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = true;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = mondaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Monday";
            sunImg.BackgroundColor = Color.White;
            monImg.BackgroundColor = Color.LightGray;
            tueImg.BackgroundColor = Color.White;
            wedImg.BackgroundColor = Color.White;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizertue_Tapped(object sender, EventArgs e)
        {
            // the all tasks having the weekid will be disabled
            var tasks = await DataTask.GetTasksAsync(selectedWeek.GoalId, selectedWeek.Id);
            if (DateTime.Today.DayOfWeek.ToString() != "Tuesday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = false;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            else if (DateTime.Today.DayOfWeek.ToString() == "Tuesday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = true;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            // get the dow whose name is equal to sunday
            var tuesdaydow = dOWs.Where(s => s.Name == "Tuesday").FirstOrDefault();
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = tuesdaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Tuesday";
            sunImg.BackgroundColor = Color.White;
            monImg.BackgroundColor = Color.White;
            tueImg.BackgroundColor = Color.LightGray;
            wedImg.BackgroundColor = Color.White;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizerwed_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var wednesdaydow = dOWs.Where(s => s.Name == "Wednesday").FirstOrDefault();
            var tasks = await DataTask.GetTasksAsync(selectedWeek.GoalId, selectedWeek.Id);
            //if (DateTime.Today.DayOfWeek.ToString() != "Wednesday")
            //{

            //    foreach (var taskitem in tasks)
            //    {
            //        taskitem.IsEnabled = false;
            //        await DataTask.UpdateTaskAsync(taskitem);
            //    }
            //}
            //else if (DateTime.Today.DayOfWeek.ToString() == "Wednesday")
            //{

            //    foreach (var taskitem in tasks)
            //    {
            //        taskitem.IsEnabled = true;
            //        await DataTask.UpdateTaskAsync(taskitem);
            //    }
            //}
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = wednesdaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Wednesday";
            sunImg.BackgroundColor = Color.White;
            monImg.BackgroundColor = Color.White;
            tueImg.BackgroundColor = Color.White;
            wedImg.BackgroundColor = Color.LightGray;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizerthu_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var thursdaydow = dOWs.Where(s => s.Name == "Thursday").FirstOrDefault();
            var tasks = await DataTask.GetTasksAsync(selectedWeek.GoalId, selectedWeek.Id);
            if (DateTime.Today.DayOfWeek.ToString() != "Thursday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = false;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            else if (DateTime.Today.DayOfWeek.ToString() == "Thursday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = true;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = thursdaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Thursday";
            sunImg.BackgroundColor = Color.White;
            monImg.BackgroundColor = Color.White;
            tueImg.BackgroundColor = Color.White;
            wedImg.BackgroundColor = Color.White;
            thuImg.BackgroundColor = Color.LightGray;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizerfri_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var fridaydow = dOWs.Where(s => s.Name == "Friday").FirstOrDefault();
            var tasks = await DataTask.GetTasksAsync(selectedWeek.GoalId, selectedWeek.Id);
            if (DateTime.Today.DayOfWeek.ToString() != "Friday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = false;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            else if (DateTime.Today.DayOfWeek.ToString() == "Friday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = true;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = fridaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Friday";
            sunImg.BackgroundColor = Color.White;
            monImg.BackgroundColor = Color.White;
            tueImg.BackgroundColor = Color.White;
            wedImg.BackgroundColor = Color.White;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.LightGray;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizersat_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var saturdaydow = dOWs.Where(s => s.Name == "Saturday").FirstOrDefault();
            var tasks = await DataTask.GetTasksAsync(selectedWeek.GoalId, selectedWeek.Id);
            if (DateTime.Today.DayOfWeek.ToString() != "Saturday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = false;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            else if (DateTime.Today.DayOfWeek.ToString() == "Saturday")
            {

                foreach (var taskitem in tasks)
                {
                    taskitem.IsEnabled = true;
                    await DataTask.UpdateTaskAsync(taskitem);
                }
            }
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = saturdaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Saturday";
            sunImg.BackgroundColor = Color.White;
            monImg.BackgroundColor = Color.White;
            tueImg.BackgroundColor = Color.White;
            wedImg.BackgroundColor = Color.White;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.LightGray;
        }
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {

            var action = await DisplayActionSheet("Sort tasks by:", "Cancel", "", "All", "Not Started", "In Progress", "Completed", "With subtasks");
            if (action == "All")
            {
                if (BindingContext is WeeklyTaskViewModel bvm)
                {
                    await bvm.AllTasks();
                }
            }

            else if (action == "Not Started")
            {
                if (BindingContext is WeeklyTaskViewModel bvm)
                {
                    await bvm.NotstartedTasks();
                }
            }

            else if (action == "In Progress")
            {
                if (BindingContext is WeeklyTaskViewModel bvm)
                {
                    await bvm.InprogressTasks();
                }
            }

            else if (action == "Completed")
            {
                if (BindingContext is WeeklyTaskViewModel bvm)
                {
                    await bvm.CompletedTasks();
                }
            }

            else if (action == "With subtasks")
            {
                if (BindingContext is WeeklyTaskViewModel bvm)
                {
                    await bvm.Withsubtasks();
                }
            }
        }
        private async void switch_Toggled(object sender, ToggledEventArgs e)
        {
            Switch @switch = (Switch)sender;
            var task = (Models.GoalTask)@switch.BindingContext;
            var taskid = task.Id;
            // get the task having the same id as taskId
            var taskdb = await DataTask.GetTaskAsync(taskid);

            if (task.IsCompleted)
            {
                // check if the incoming object 
                if (taskdb.IsCompleted)
                    return;
                else
                {
                    if (BindingContext is WeeklyTaskViewModel viewModel)
                    {
                        await viewModel.CompleteWeeklyTask(taskid, task.IsCompleted);
                        calculateweekPercentage(selectedWeek);
                    }
                }
            }
            else if (!task.IsCompleted)
            {
                // check if the incoming object 
                if (!taskdb.IsCompleted)
                    return;
                else
                {
                    if (BindingContext is WeeklyTaskViewModel viewModel)
                        await viewModel.UncompleteTask(taskid, task.IsCompleted);
                    calculateweekPercentage(selectedWeek);
                }               
            }
            return;
        }
        void dayOfTheWeekVisisbility(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Sunday)
            {
                sunImg.BackgroundColor = Color.LightGray;
                monImg.BackgroundColor = Color.White;
                tueImg.BackgroundColor = Color.White;
                wedImg.BackgroundColor = Color.White;
                thuImg.BackgroundColor = Color.White;
                friImg.BackgroundColor = Color.White;
                satImg.BackgroundColor = Color.White;
            }
            else if (dayOfWeek == DayOfWeek.Monday)
            {
                sunImg.BackgroundColor = Color.White;
                monImg.BackgroundColor = Color.LightGray;
                tueImg.BackgroundColor = Color.White;
                wedImg.BackgroundColor = Color.White;
                thuImg.BackgroundColor = Color.White;
                friImg.BackgroundColor = Color.White;
                satImg.BackgroundColor = Color.White;
            }
            else if (dayOfWeek == DayOfWeek.Tuesday)
            {
                sunImg.BackgroundColor = Color.White;
                monImg.BackgroundColor = Color.White;
                tueImg.BackgroundColor = Color.LightGray;
                wedImg.BackgroundColor = Color.White;
                thuImg.BackgroundColor = Color.White;
                friImg.BackgroundColor = Color.White;
                satImg.BackgroundColor = Color.White;
            }
            else if (dayOfWeek == DayOfWeek.Wednesday)
            {               
                sunImg.BackgroundColor = Color.White;
                monImg.BackgroundColor = Color.White;
                tueImg.BackgroundColor = Color.White;
                wedImg.BackgroundColor = Color.LightGray;
                thuImg.BackgroundColor = Color.White;
                friImg.BackgroundColor = Color.White;
                satImg.BackgroundColor = Color.White;
            }
            else if (dayOfWeek == DayOfWeek.Thursday)
            {
                sunImg.BackgroundColor = Color.White;
                monImg.BackgroundColor = Color.White;
                tueImg.BackgroundColor = Color.White;
                wedImg.BackgroundColor = Color.White;
                thuImg.BackgroundColor = Color.LightGray;
                friImg.BackgroundColor = Color.White;
                satImg.BackgroundColor = Color.White;
            }
            else if (dayOfWeek == DayOfWeek.Friday)
            {
                sunImg.BackgroundColor = Color.White;
                monImg.BackgroundColor = Color.White;
                tueImg.BackgroundColor = Color.White;
                wedImg.BackgroundColor = Color.White;
                thuImg.BackgroundColor = Color.White;
                friImg.BackgroundColor = Color.LightGray;
                satImg.BackgroundColor = Color.White;
            }
            else if (dayOfWeek == DayOfWeek.Saturday)
            {
                sunImg.BackgroundColor = Color.Pink;
                monImg.BackgroundColor = Color.White;
                tueImg.BackgroundColor = Color.White;
                wedImg.BackgroundColor = Color.White;
                thuImg.BackgroundColor = Color.White;
                friImg.BackgroundColor = Color.White;
                satImg.BackgroundColor = Color.LightGray;
            }
        }
        async Task assigningWeekValues( Week week)
        {
            //get all weeks for this goal
            var allWeeks = await dataWeek.GetWeeksAsync(week.GoalId);
            // get all weeks having the week id
            var allTasks = await DataTask.GetTasksAsync(week.GoalId,week.Id);
            if(allTasks.Count() == 0)
            {
                // get the previous week
                var previousWeek = allWeeks.Where(T => T.WeekNumber == week.WeekNumber - 1).FirstOrDefault();
                if(previousWeek != null)
                {
                    // get all tasks having the previousweek Id
                    var previousweekTasks = await DataTask.GetTasksAsync(previousWeek.GoalId, previousWeek.Id);
                    if(previousweekTasks.Count() > 0)
                    {
                        // get the total number of tasks and assign them tothe previous weelk
                        previousWeek.totalnumberOftask = previousweekTasks.Count();
                        // get all completed tasks
                        var completedTask = previousweekTasks.Where(C => C.IsCompleted).ToList();
                        previousWeek.totalnumberOfcompletedtask = completedTask.Count();
                        // update previous week
                        await dataWeek.UpdateWeekAsync(previousWeek);

                        // get only tasks whose isrepeated is true
                        var validTasks = previousweekTasks.Where(T => T.Isrepeated).ToList();
                        //reassign the week percentage to tasks
                        var newTaskPercentage = week.TargetPercentage / validTasks.Count();
                        // loop through the tasks and assign new values
                        foreach (var task in validTasks)
                        {
                            task.WeekId = week.Id;
                            task.IsCompleted = false;
                            task.PendingPercentage = 0;
                            task.Percentage = newTaskPercentage;
                            // check if it has subtask
                            // get all subtask
                            var subtasks = await datasubtask.GetSubTasksAsync(task.Id);
                            if(subtasks.Count() > 0)
                            {
                                // loop through the subtasks and assign new values
                                foreach (var subtask in subtasks)
                                {
                                    subtask.IsCompleted = false;
                                    await datasubtask.UpdateSubTaskAsync(subtask);
                                }
                            }
                            await DataTask.UpdateTaskAsync(task);
                        }
                        // get only tasks whose isrepeated is true
                        var invalidTasks = previousweekTasks.Where(T => T.Isrepeated == false).ToList();
                        // loop through the tasks and assign new values
                        foreach (var task in invalidTasks)
                        {
                            await DataTask.DeleteTaskAsync(task.Id);
                            // get all subtask
                            var subtasks = await datasubtask.GetSubTasksAsync(task.Id);
                            if (subtasks.Count() > 0)
                            {
                                // loop through the subtasks and assign new values
                                foreach (var subtask in subtasks)
                                {                                  
                                    await datasubtask.DeleteSubTaskAsync(subtask.Id);
                                }
                            }                           
                        }                      
                    }
                    else if(previousweekTasks.Count() == 0)
                    {
                        assigncurrentweekvalues(week);
                    }
                }
                else
                {
                    assigncurrentweekvalues(week);
                }              
            }
            else if(allTasks.Count() > 0)
            {
                assigncurrentweekvalues(week);
            }
        }
        async void assigncurrentweekvalues(Week week)
        {
            startdate.Text = week.StartDate.ToString("d MMM yyyy");
            enddate.Text = week.EndDate.ToString("d MMM yyyy");
            weeknumber.Text = week.WeekNumber.ToString();
            // get all tasks having the weekId
            var tasks = await DataTask.GetTasksAsync(week.GoalId,week.Id);
            tasksnumber.Text = tasks.Count().ToString();            
            if (week.status == "Not started")
            {
                // get the day of week of today
                var dowToday = DateTime.Today.DayOfWeek;
                status.Text = "Not Started";
                framestatus.BackgroundColor = Color.LightGray;
                dayOfTheWeekVisisbility(dowToday);
            }
            else if (week.status == "In Progress")
            {
                // get the day of week of today
                var dowToday = DateTime.Today.DayOfWeek;
                status.Text = "In Progress";
                framestatus.BackgroundColor = Color.OrangeRed;
                dayOfTheWeekVisisbility(dowToday);
            }
            else if (week.status == "Completed")
            {
                // get the day of week of today
                var dowToday = DateTime.Today.DayOfWeek;
                status.Text = "Completed";
                framestatus.BackgroundColor = Color.LightSeaGreen;
                dayOfTheWeekVisisbility(dowToday);
            }
            else if (week.status == "Expired")
            {
                // get the day of week of today
                var dowToday = DateTime.Today.DayOfWeek;
                status.Text = "In Progress";
                dayOfTheWeekVisisbility(dowToday);
            }
        }
        async void calculateweekPercentage(Week week)
        {
            double taskPercentage = 0;
            // get all tasks having the week id
            var weekTasks = await DataTask.GetTasksAsync(week.GoalId, week.Id);
            // loop through the tasks and get their task's pending percentage
            foreach (var task in weekTasks)
            {
                taskPercentage += task.PendingPercentage;
            }
            var weekpercentage = Math.Round(taskPercentage, 1);
            goalweeklypercentage.Text = weekpercentage.ToString();
            weeklygoalprogress.Progress = weekpercentage / week.TargetPercentage;
        }
    }
}