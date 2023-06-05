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
        private DOW DOW;
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
            startdate.Text = week.StartDate.ToString("d MMM yyyy"); 
            enddate.Text = week.EndDate.ToString("d MMM yyyy");
            weeknumber.Text = week.WeekNumber.ToString();
            if (week.status == "Not Started")
            {
                status.Text = "Not Started";
                status.TextColor = Color.LightGray;
            }
            else if(week.status == "In Progress")
            {
                status.Text = "In Progress";
                status.TextColor = Color.Orange;
            }
            else if(week.status == "Completed")
            {
                status.Text = "Completed";
                status.TextColor = Color.Green;
            }
            else
            {
                status.Text = "Expired";
                status.TextColor = Color.Red;
            }
                
            // call set date method
            SetDate(week);
            setImagevisibility();
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.GoalId = goal.Id;
                wvm.WeekId = result;              
                // await showButtonclicked(0);
                await wvm.Refresh();
            }

        }
        void SetDate(Week week)
        {
            sunDate.Text = week.StartDate.ToString("dd");
            monDate.Text = week.StartDate.AddDays(1).ToString("dd");
            tueDate.Text = week.StartDate.AddDays(2).ToString("dd");
            wedDate.Text = week.StartDate.AddDays(3).ToString("dd");
            thuDate.Text = week.StartDate.AddDays(4).ToString("dd");
            friDate.Text = week.StartDate.AddDays(5).ToString("dd");
            satDate.Text = week.StartDate.AddDays(6).ToString("dd");
        }
        void setImagevisibility()
        {
            sunImg.IsVisible = true;
            monImg.IsVisible = false;
            tueImg.IsVisible = false;
            wedImg.IsVisible = false;
            thuImg.IsVisible = false;
            friImg.IsVisible = false;
            satImg.IsVisible = false;

        }

        private void TapGestureRecognizersun_Tapped(object sender, EventArgs e)
        {
            dayTask.Text = "Sunday";
            sunImg.IsVisible = true;
            monImg.IsVisible = false;
            tueImg.IsVisible = false;
            wedImg.IsVisible = false;
            thuImg.IsVisible = false;
            friImg.IsVisible = false;
            satImg.IsVisible = false;
        }
        private void TapGestureRecognizermon_Tapped(object sender, EventArgs e)
        {
            dayTask.Text = "Monday";
            sunImg.IsVisible = false;
            monImg.IsVisible = true;
            tueImg.IsVisible = false;
            wedImg.IsVisible = false;
            thuImg.IsVisible = false;
            friImg.IsVisible = false;
            satImg.IsVisible = false;
        }
        private void TapGestureRecognizertue_Tapped(object sender, EventArgs e)
        {
            dayTask.Text = "Tuesday";
            sunImg.IsVisible = false;
            monImg.IsVisible = false;
            tueImg.IsVisible = true;
            wedImg.IsVisible = false;
            thuImg.IsVisible = false;
            friImg.IsVisible = false;
            satImg.IsVisible = false;
        }
        private void TapGestureRecognizerwed_Tapped(object sender, EventArgs e)
        {
            dayTask.Text = "Wednesday"; 
            sunImg.IsVisible = false;
            monImg.IsVisible = false;
            tueImg.IsVisible = false;
            wedImg.IsVisible = true;
            thuImg.IsVisible = false;
            friImg.IsVisible = false;
            satImg.IsVisible = false;
        }
        private void TapGestureRecognizerthu_Tapped(object sender, EventArgs e)
        {
            dayTask.Text = "Thursday";
            sunImg.IsVisible = false;
            monImg.IsVisible = false;
            tueImg.IsVisible = false;
            wedImg.IsVisible = false;
            thuImg.IsVisible = true;
            friImg.IsVisible = false;
            satImg.IsVisible = false;
        }
        private void TapGestureRecognizerfri_Tapped(object sender, EventArgs e)
        {
            dayTask.Text = "Friday";
            sunImg.IsVisible = false;
            monImg.IsVisible = false;
            tueImg.IsVisible = false;
            wedImg.IsVisible = false;
            thuImg.IsVisible = false;
            friImg.IsVisible = true;
            satImg.IsVisible = false;
        }
        private void TapGestureRecognizersat_Tapped(object sender, EventArgs e)
        {
            dayTask.Text = "Saturday";
            sunImg.IsVisible = false;
            monImg.IsVisible = false;
            tueImg.IsVisible = false;
            wedImg.IsVisible = false;
            thuImg.IsVisible = false;
            friImg.IsVisible = false;
            satImg.IsVisible = true;
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
    }
}