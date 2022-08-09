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
    [QueryProperty(nameof(goalId), nameof(goalId))]
    public partial class WeeklyTask : ContentPage
    {
        public string goalId { get; set; }
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataWeek<Models.Week> dataWeek { get; }
        public WeeklyTask()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataWeek = DependencyService.Get<IDataWeek<Models.Week>>();
            DataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            BindingContext = new WeeklyTaskViewModel();
            
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(goalId, out var result);
            // get the last week having the goal id
            var weeks = await dataWeek.GetWeeksAsync(result);
            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(result);
            // get all tasks having the goal id
            var tasks = await DataTask.GetTasksAsync(goal.Id);
            
            if(tasks.Count() == 0)
            {
                Stacklist.IsVisible = false;
                StackBlank.IsVisible = true;
            }
            else 
            {
                Stacklist.IsVisible = true;
                StackBlank.IsVisible = false;
            }
            // get the last week
            var lastweek = weeks.ToList().LastOrDefault();
            double accumulatedPercentage = lastweek.AccumulatedPercentage; ;
            int targetedPercentage = lastweek.TargetPercentage;
            int NumberOfWeeks = goal.NumberOfWeeks;
            int weekNumber = lastweek.WeekNumber;
            
            AccumulatedPercent.Text = accumulatedPercentage.ToString();
            TargetPercentage.Text = targetedPercentage.ToString();
            numberofweeks.Text = NumberOfWeeks.ToString();
            WeekNumber.Text = weekNumber.ToString();                            
          
            if (BindingContext is WeeklyTaskViewModel cvm)
            {
                //  // get the last task
                if (tasks.Count() == 0)
                    return;
                else
                {
                    var Dowtask = tasks.Where(T => T.DowId > 0).ToList();
                    if (Dowtask.Count == 0)
                        return;
                    else if (Dowtask.Count > 0)
                    {
                        // get last task
                        var lastTask = Dowtask.LastOrDefault();
                        if (lastTask.DowId == 0)
                            return;
                        else 
                        {
                            cvm.DayNumber = lastTask.DowId;
                            if (lastTask.DowId == 1)
                                sundayMeth();
                            else if (lastTask.DowId == 2)
                                MondayMeth();
                            else if (lastTask.DowId == 3)
                                tuesdayMeth();
                            else if (lastTask.DowId == 4)
                                wednesdayMeth();
                            else if (lastTask.DowId == 5)
                                thursdayMeth();
                            else if (lastTask.DowId == 6)
                                fridayMeth();
                            else
                                saturdayMeth();
                        }
                       
                    }
                }
                cvm.GoalId = goal.Id;
                await cvm.Refresh();

            }
        }
        private async void switch_Toggled(object sender, ToggledEventArgs e)
        {
            Switch @switch = (Switch)sender;
            var task = (Models.GoalTask)@switch.BindingContext;
            var taskid = task.Id;
            if (task.IsCompleted)
            {
                if (BindingContext is WeeklyTaskViewModel viewModel)
                    await viewModel.CompleteTask(taskid, task.IsCompleted);
            }
            else if (!task.IsCompleted)
            {
                if (BindingContext is WeeklyTaskViewModel viewModel)
                    await viewModel.UncompleteTask(taskid, task.IsCompleted);
                
            }
            return;

        }
        private void SunButton_Clicked(object sender, EventArgs e)
        {
            sundayMeth(); 
                       
        }

        private void MonButton_Clicked(object sender, EventArgs e)
        {
            MondayMeth();
        }

        private void TueButton_Clicked(object sender, EventArgs e)
        {

            tuesdayMeth();
        }

        private void WedButton_Clicked(object sender, EventArgs e)
        {
            wednesdayMeth();
        }

        private void ThurButton_Clicked(object sender, EventArgs e)
        {
            thursdayMeth();
        }

        private void FriButton_Clicked(object sender, EventArgs e)
        {
            fridayMeth();
           
        }

        private void SatButton_Clicked(object sender, EventArgs e)
        {
            saturdayMeth();
        }

        // METHODS
        void sundayMeth()
        {
            Sunday.Background = Color.AntiqueWhite;
            Monday.Background = Color.LightGray;
            Tuesday.Background = Color.LightGray;
            Wednesday.Background = Color.LightGray;
            Thursday.Background = Color.LightGray;
            Friday.Background = Color.LightGray;
            Saturday.Background = Color.LightGray;
        }
        void MondayMeth()
        {
            Monday.Background = Color.AntiqueWhite;
            Sunday.Background = Color.LightGray;
            Tuesday.Background = Color.LightGray;
            Wednesday.Background = Color.LightGray;
            Thursday.Background = Color.LightGray;
            Friday.Background = Color.LightGray;
            Saturday.Background = Color.LightGray;
        }
        void tuesdayMeth()
        {
            Tuesday.Background = Color.AntiqueWhite;
            Monday.Background = Color.LightGray;
            Sunday.Background = Color.LightGray;
            Wednesday.Background = Color.LightGray;
            Thursday.Background = Color.LightGray;
            Friday.Background = Color.LightGray;
            Saturday.Background = Color.LightGray;
        }
        void wednesdayMeth()
        {
            Wednesday.Background = Color.AntiqueWhite;
            Tuesday.Background = Color.LightGray;
            Monday.Background = Color.LightGray;
            Sunday.Background = Color.LightGray;
            Thursday.Background = Color.LightGray;
            Friday.Background = Color.LightGray;
            Saturday.Background = Color.LightGray;
        }
        void thursdayMeth()
        {
            Thursday.Background = Color.AntiqueWhite;
            Wednesday.Background = Color.LightGray;
            Tuesday.Background = Color.LightGray;
            Monday.Background = Color.LightGray;
            Sunday.Background = Color.LightGray;
            Wednesday.Background = Color.LightGray;
            Friday.Background = Color.LightGray;
            Saturday.Background = Color.LightGray;
        }
        void fridayMeth()
        {
            Friday.Background = Color.AntiqueWhite;
            Thursday.Background = Color.LightGray;
            Wednesday.Background = Color.LightGray;
            Tuesday.Background = Color.LightGray;
            Monday.Background = Color.LightGray;
            Sunday.Background = Color.LightGray;
            Wednesday.Background = Color.LightGray;
            Saturday.Background = Color.LightGray;
        }
        void saturdayMeth()
        {
            Saturday.Background = Color.AntiqueWhite;
            Friday.Background = Color.LightGray;
            Thursday.Background = Color.LightGray;
            Wednesday.Background = Color.LightGray;
            Tuesday.Background = Color.LightGray;
            Monday.Background = Color.LightGray;
            Sunday.Background = Color.LightGray;
            Wednesday.Background = Color.LightGray;
        }
    }
}