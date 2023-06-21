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
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataSubtask<Subtask> datasubtask { get; }
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
            startdate.Text = week.StartDate.ToString("d MMM yyyy");
            enddate.Text = week.EndDate.ToString("d MMM yyyy");
            weeknumber.Text = week.WeekNumber.ToString();
            //weeklygoalName.Text = goal.Name;    //var weekremaining = goal.NumberOfWeeks - week.WeekNumber;
            //goaldaysLeft.Text = weekremaining.ToString();
            //goaldaysLeft.Text = "7";
        
            if (week.status == "Not started")
            {
                // get the day of week of today
                var dowToday = DateTime.Today.DayOfWeek;
                status.Text = "Not Started";
                framestatus.BackgroundColor = Color.LightGray;
                dayOfTheWeekVisisbility(dowToday);
            }
            else if(week.status == "In Progress")
            {
                // get the day of week of today
                var dowToday = DateTime.Today.DayOfWeek;
                status.Text = "In Progress";
                framestatus.BackgroundColor = Color.OrangeRed;
              dayOfTheWeekVisisbility(dowToday);
            }
            else if (week.status == "Expired")
            {
                // get the day of week of today
                var dowToday = DateTime.Today.DayOfWeek;
                status.Text = "In Progress";            
                dayOfTheWeekVisisbility(dowToday);
            }

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
        //void setImagevisibility()
        //{
        //    sunImg.IsVisible = true;
        //    monImg.IsVisible = false;
        //    tueImg.IsVisible = false;
        //    wedImg.IsVisible = false;
        //    thuImg.IsVisible = false;
        //    friImg.IsVisible = false;
        //    satImg.IsVisible = false;
        //}

        private async void TapGestureRecognizersun_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var sundaydow = dOWs.Where(s => s.Name == "Sunday").FirstOrDefault();
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = sundaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Sunday";
            sunImg.BackgroundColor = Color.Pink;
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
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = mondaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Monday";
            sunImg.BackgroundColor = Color.White;
            monImg.BackgroundColor = Color.Pink;
            tueImg.BackgroundColor = Color.White;
            wedImg.BackgroundColor = Color.White;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizertue_Tapped(object sender, EventArgs e)
        {
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
            tueImg.BackgroundColor = Color.Pink;
            wedImg.BackgroundColor = Color.White;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizerwed_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var wednesdaydow = dOWs.Where(s => s.Name == "Wednesday").FirstOrDefault();
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DowId = wednesdaydow.DOWId;
                await wvm.Refresh();
            }
            dayTask.Text = "Wednesday";
            sunImg.BackgroundColor = Color.White;
            monImg.BackgroundColor = Color.White;
            tueImg.BackgroundColor = Color.White;
            wedImg.BackgroundColor = Color.Pink;
            thuImg.BackgroundColor = Color.White;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizerthu_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var thursdaydow = dOWs.Where(s => s.Name == "Thursday").FirstOrDefault();
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
            thuImg.BackgroundColor = Color.Pink;
            friImg.BackgroundColor = Color.White;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizerfri_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var fridaydow = dOWs.Where(s => s.Name == "Friday").FirstOrDefault();
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
            friImg.BackgroundColor = Color.Pink;
            satImg.BackgroundColor = Color.White;
        }
        private async void TapGestureRecognizersat_Tapped(object sender, EventArgs e)
        {
            // get the dow whose name is equal to sunday
            var saturdaydow = dOWs.Where(s => s.Name == "Saturday").FirstOrDefault();
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
            satImg.BackgroundColor = Color.Pink;
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
                    if (BindingContext is GoalTaskViewModel viewModel)
                    {
                        await viewModel.CompleteTask(taskid, task.IsCompleted);
                    }
                }
            }
            else if (!task.IsCompleted)
            {
                // check if the incoming object 
                if (!taskdb.IsCompleted)
                    return;
                if (BindingContext is GoalTaskViewModel viewModel)
                    await viewModel.UncompleteTask(taskid, task.IsCompleted);
            }
            return;

        }
        void dayOfTheWeekVisisbility(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Sunday)
            {
                sunImg.BackgroundColor = Color.Pink;
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
                monImg.BackgroundColor = Color.Pink;
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
                tueImg.BackgroundColor = Color.Pink;
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
                wedImg.BackgroundColor = Color.Pink;
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
                thuImg.BackgroundColor = Color.Pink;
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
                friImg.BackgroundColor = Color.Pink;
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
                satImg.BackgroundColor = Color.Pink;
            }
        }
    }
}