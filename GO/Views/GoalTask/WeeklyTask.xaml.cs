using GO.Models;
using GO.Services;
using GO.ViewModels.TaskInGoals;
using GO.ViewModels.Weeks;
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
    public partial class WeeklyTask : ContentPage
    {
        public string weekId { get; set; }
       
        // properties for Dows Id
        private int SunId;
        private int MonId;
        private int TueId;
        private int WedId;
        private int ThurId;
        private int FriId;
        private int SatId;
        private bool Isvalid = false;
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataWeek<Week> dataWeek { get; }
        public IDataDow<DOW> dataDow { get; }
        public IDataSubtask<Models.Subtask> dataSubtask { get; }
       

        // a property for calculation if number of times an image button has been clicked
        private int Counter = 0;
      
        private int newWeekNumber;
        private double sundayPercentage = 0;
        private double sundayPendingpercentage = 0;
        private double SuntotalPercetage = 0;

        private double monPercentage = 0;
        private double monPendingpercentage = 0;
        private double montotalPercetage = 0;

        private double tuePercentage = 0;
        private double tuePendingpercentage = 0;
        private double tuetotalPercetage = 0;

        private double wedPercentage = 0;
        private double wedPendingpercentage = 0;
        private double wedtotalPercetage = 0;

        private double thuPercentage = 0;
        private double thuPendingpercentage = 0;
        private double thutotalPercetage = 0;

        private double friPercentage = 0;
        private double friPendingpercentage = 0;
        private double fritotalPercetage = 0;


        private double satPercentage = 0;
        private double satPendingpercentage = 0;
        private double SattotalPercetage = 0;

        private Models.Week  GetWeek;
        public WeeklyTask()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataWeek = DependencyService.Get<IDataWeek<Models.Week>>();
            DataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            dataDow = DependencyService.Get<IDataDow<Models.DOW>>();
            dataSubtask = DependencyService.Get<IDataSubtask<Models.Subtask>>();
            BindingContext = new WeeklyTaskViewModel();           
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(weekId, out var result);
            // set newWeekVariable variable to 0;         
            //newWeekNumber = 0;
            if (BindingContext is WeeklyTaskViewModel vm)
            {
                // get week equal to week id
                var week = await dataWeek.GetWeekAsync(result);
               
                // get all weeks having the goal id
                var weeks = await dataWeek.GetWeeksAsync(week.GoalId);
                // loop through the weeks and check if any is equal to  vmweek id
                foreach(var weeek in weeks)
                {
                    //check if it is equal to vmweekid
                    if(weeek.Id == vm.WeekId)
                      Isvalid = true;               
                }

                if(!Isvalid)
                {                   
                    // pass the task object to getweek
                    GetWeek = week;
                    // assign to newweeknumber variable, getweek.weekNumber
                    newWeekNumber = GetWeek.WeekNumber;
                    // call the populate data method
                    PopulateData(GetWeek);
                    if (BindingContext is WeeklyTaskViewModel wvm)
                    {
                        wvm.GoalId = week.GoalId;
                        wvm.WeekId = week.Id;
                        await wvm.Refresh();
                    }
                }
                else 
                {
                    // get the week having the weekid
                    var wk = await dataWeek.GetWeekAsync(result);
                    PopulateData(wk);
                    if (BindingContext is WeeklyTaskViewModel wvm)
                    {
                        wvm.GoalId = week.GoalId;
                        //wvm.WeekId = week.Id;
                        await wvm.Refresh();
                    }
                }
            }
           
            
        }
        //private int newWeekNumber = 0;

        // for left chevron
        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            // increment counter by one
            Counter++;
            // get the week number of the latest and minus it with the counter number
             newWeekNumber -=  1;
            if (newWeekNumber <= 0)
            {
                newWeekNumber += 1;
                await Application.Current.MainPage.DisplayAlert("Alert", "No weeks left", "OK");
                return;               
            }
              
            else 
            {
                // loops through the weeks and get the week having the same weeknumber as newweeknumber
                var weeks = await dataWeek.GetWeeksAsync(GetWeek.GoalId);
                var week = weeks.Where(g => g.WeekNumber == newWeekNumber).FirstOrDefault();
                if (BindingContext is WeeklyTaskViewModel Wvm)
                    Wvm.WeekId = week.Id;
                PopulateData(week);           
                          
            }
        }
        // for right chevron
        private async void ImageButton_Clicked_1(object sender, EventArgs e)
        {          
            newWeekNumber +=  1;
            // get all weeks and get the one whose week number equals the newweeknumber
            var weeks = await dataWeek.GetWeeksAsync(GetWeek.GoalId);
            if(newWeekNumber > GetWeek.WeekNumber)
            {
                newWeekNumber -= 1;
                await Application.Current.MainPage.DisplayAlert("Alert", "No weeks left", "OK");
                return;
            }
            var week = weeks.Where(g => g.WeekNumber == newWeekNumber).FirstOrDefault();
            if (BindingContext is WeeklyTaskViewModel Wvm)
                Wvm.WeekId = week.Id;
             PopulateData(week);           

        }
        // a method for populating data to the view
       async void PopulateData( Week week)
       {
            // chec if the week is active
            if (week.Active)
            {
                isactive.IsVisible = true;
                isnotactive.IsVisible = false;
                Activename.Text = "Active Week";
                Activename.FontAttributes = FontAttributes.Bold;
            }
            else if (!week.Active)
            {
                isactive.IsVisible = false;
                isnotactive.IsVisible = true;
                Activename.Text = "Not Active";

            }            
            if(week.CreatedAutomatically)
            {
                ATW.IsVisible = true;
                Dowframe.IsVisible = false;
                weeksaccumulatedpercentage.Text = week.AccumulatedPercentage.ToString();
                startdate.Text = week.StartDate.ToShortDateString();
                enddate.Text = week.EndDate.ToShortDateString();
                Weekspercentage.Text = week.TargetPercentage.ToString();
                statusweek.Text = week.Status.ToString();
                statusweek.TextColor = Color.Black;
                //if (week.Status == "Not Started" && week.Status == "InProgress")
                //    statusweek.TextColor = Color.Accent;
                //else if (week.Status == "Completed")
                //    statusweek.TextColor = Color.GreenYellow;
                //else if (week.Status == "Expired")
                //    statusweek.TextColor = Color.Red;
                progress.Progress = week.Progress;

                // for frame 2
                weeknum.Text = week.WeekNumber.ToString();
            }
            else if(!week.CreatedAutomatically)
            {
                ATW.IsVisible = false;
                Dowframe.IsVisible = true;


                weeksaccumulatedpercentage.Text = week.AccumulatedPercentage.ToString();
                startdate.Text = week.StartDate.ToShortDateString();
                enddate.Text = week.EndDate.ToShortDateString();
                Weekspercentage.Text = week.TargetPercentage.ToString();
                statusweek.Text = week.Status.ToString();
                statusweek.TextColor = Color.Black;
                //if (week.Status == "Not Started" && week.Status == "InProgress")
                //    statusweek.TextColor = Color.Accent;
                //else if (week.Status == "Completed")
                //    statusweek.TextColor = Color.GreenYellow;
                //else if (week.Status == "Expired")
                //    statusweek.TextColor = Color.Red;
                progress.Progress = week.Progress;

                // for frame 2
                weeknum.Text = week.WeekNumber.ToString();

                // filter tasks to only sundays tasks

                // get all dows having the weeks id
                var weekDows = await dataDow.GetDOWsAsync(week.Id);
                //loop through the dows
                foreach (var dow in weekDows)
                {
                    if (dow.Name == "Sunday")
                        // get the id and assign it to sunId
                        SunId = dow.DOWId;
                    else if (dow.Name == "Monday")
                        MonId = dow.DOWId;
                    else if (dow.Name == "Tuesday")
                        TueId = dow.DOWId;
                    else if (dow.Name == "Wednesday")
                        WedId = dow.DOWId;
                    else if (dow.Name == "Thursday")
                        ThurId = dow.DOWId;
                    else if (dow.Name == "Friday")
                        FriId = dow.DOWId;
                    else if (dow.Name == "Saturday")
                        SatId = dow.DOWId;
                }
                var sundayTasks = await DataTask.GetTasksAsync(week.GoalId, SunId);
                if (sundayTasks.Count() == 0)
                {
                    if (week.Active)
                        btnSun.Text = "Create Tasks";
                    else if (!week.Active)
                        btnSun.Text = "No Tasks";
                }
                else
                    btnSun.Text = "View Tasks";
                SunNumTask.Text = sundayTasks.Count().ToString();
                // filter to completed tasks only
                foreach (var task in sundayTasks)
                {
                    //check if task is completed
                    if (task.IsCompleted)
                    {
                        sundayPercentage += task.Percentage;

                    }
                    else if (!task.IsCompleted)
                    {
                        // check task has subtasks
                        //get all subtasks having the tasks Id
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);

                        if (subtasks.Count() > 0)
                        {
                            // get the task's pending percentage
                            sundayPendingpercentage += task.PendingPercentage;
                        }
                    }
                }
                SuntotalPercetage += sundayPercentage + sundayPendingpercentage;
                SunTasktotalpercentage.Text = Math.Round(SuntotalPercetage, 1).ToString();
                sundayPendingpercentage = 0;
                sundayPercentage = 0;
                SuntotalPercetage = 0;
                SunId = 0;


                var mondayTasks = await DataTask.GetTasksAsync(week.GoalId, MonId);
                if (mondayTasks.Count() == 0)
                {
                    if (week.Active)
                        btnMon.Text = "Create Tasks";
                    else if (!week.Active)
                        btnMon.Text = "No Tasks";
                }
                else
                    btnMon.Text = "View Tasks";
                MonNumTask.Text = mondayTasks.Count().ToString();
                foreach (var task in mondayTasks)
                {
                    //check if task is completed
                    if (task.IsCompleted)
                    {
                        monPercentage += task.Percentage;

                    }
                    else if (!task.IsCompleted)
                    {
                        // check task has subtasks
                        //get all subtasks having the tasks Id
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);

                        if (subtasks.Count() > 0)
                        {
                            // get the task's pending percentage
                            monPendingpercentage += task.PendingPercentage;
                        }
                    }
                }
                montotalPercetage += monPercentage + monPendingpercentage;
                MontotalTaskpercentage.Text = Math.Round(montotalPercetage, 1).ToString();
                monPercentage = 0;
                monPendingpercentage = 0;
                montotalPercetage = 0;
                MonId = 0;


                var tuesdayTasks = await DataTask.GetTasksAsync(week.GoalId, TueId);
                if (tuesdayTasks.Count() == 0)
                {
                    if (week.Active)
                        btnTue.Text = "Create Tasks";
                    else if (!week.Active)
                        btnTue.Text = "No Tasks";
                }
                else
                    btnTue.Text = "View Tasks";
                TueNumTask.Text = tuesdayTasks.Count().ToString();
                foreach (var task in tuesdayTasks)
                {
                    //check if task is completed
                    if (task.IsCompleted)
                    {
                        tuePercentage += task.Percentage;

                    }
                    else if (!task.IsCompleted)
                    {
                        // check task has subtasks
                        //get all subtasks having the tasks Id
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);

                        if (subtasks.Count() > 0)
                        {
                            // get the task's pending percentage
                            tuePendingpercentage += task.PendingPercentage;
                        }
                    }
                }
                tuetotalPercetage += tuePercentage + tuePendingpercentage;
                TuetotalTaskpercentage.Text = Math.Round(tuetotalPercetage, 1).ToString();
                tuePercentage = 0;
                tuePendingpercentage = 0;
                tuetotalPercetage = 0;
                TueId = 0;


                var wednesdayTasks = await DataTask.GetTasksAsync(week.GoalId, WedId);
                if (wednesdayTasks.Count() == 0)
                {
                    if (week.Active)
                        btnWed.Text = "Create Tasks";
                    else if (!week.Active)
                        btnWed.Text = "No Tasks";
                }
                else
                    btnWed.Text = "View Tasks";
                WedNumTask.Text = wednesdayTasks.Count().ToString();
                foreach (var task in wednesdayTasks)
                {
                    //check if task is completed
                    if (task.IsCompleted)
                    {
                        wedPercentage += task.Percentage;

                    }
                    else if (!task.IsCompleted)
                    {
                        // check task has subtasks
                        //get all subtasks having the tasks Id
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);

                        if (subtasks.Count() > 0)
                        {
                            // get the task's pending percentage
                            wedPendingpercentage += task.PendingPercentage;
                        }
                    }
                }
                wedtotalPercetage += wedPercentage + wedPendingpercentage;
                wedtotalTaskpercentage.Text = Math.Round(wedtotalPercetage, 1).ToString();
                wedPendingpercentage = 0;
                wedPercentage = 0;
                wedtotalPercetage = 0;
                WedId = 0;


                var thursdayTasks = await DataTask.GetTasksAsync(week.GoalId, ThurId);
                if (thursdayTasks.Count() == 0)
                {
                    if (week.Active)
                        btnThu.Text = "Create Tasks";
                    else
                        btnThu.Text = "No Tasks";
                }
                else
                    btnThu.Text = "View Tasks";
                ThuNumTask.Text = thursdayTasks.Count().ToString();
                foreach (var task in thursdayTasks)
                {
                    //check if task is completed
                    if (task.IsCompleted)
                    {
                        thuPercentage += task.Percentage;

                    }
                    else if (!task.IsCompleted)
                    {
                        // check task has subtasks
                        //get all subtasks having the tasks Id
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);

                        if (subtasks.Count() > 0)
                        {
                            // get the task's pending percentage
                            thuPendingpercentage += task.PendingPercentage;
                        }
                    }
                }
                thutotalPercetage += thuPercentage + thuPendingpercentage;
                ThutotalTaskpercentage.Text = Math.Round(thutotalPercetage, 1).ToString();
                thuPendingpercentage = 0;
                thuPercentage = 0;
                thutotalPercetage = 0;
                ThurId = 0;

                var fridayTasks = await DataTask.GetTasksAsync(week.GoalId, FriId);
                if (fridayTasks.Count() == 0)
                {
                    if (week.Active)
                        btnFri.Text = "Create Tasks";
                    else if (!week.Active)
                        btnFri.Text = "No Tasks";
                }
                else
                    btnFri.Text = "View Tasks";
                friNumTask.Text = fridayTasks.Count().ToString();
                foreach (var task in fridayTasks)
                {
                    //check if task is completed
                    if (task.IsCompleted)
                    {
                        friPercentage += task.Percentage;

                    }
                    else if (!task.IsCompleted)
                    {
                        // check task has subtasks
                        //get all subtasks having the tasks Id
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);

                        if (subtasks.Count() > 0)
                        {
                            // get the task's pending percentage
                            friPendingpercentage += task.PendingPercentage;
                        }
                    }
                }
                fritotalPercetage += friPercentage + friPendingpercentage;
                fritotalTaskpercentage.Text = Math.Round(fritotalPercetage, 1).ToString();
                friPendingpercentage = 0;
                friPercentage = 0;
                fritotalPercetage = 0;
                FriId = 0;

                var saturdayTasks = await DataTask.GetTasksAsync(week.GoalId, SatId);
                if (saturdayTasks.Count() == 0)
                {
                    if (week.Active)
                        btnSat.Text = "Create Tasks";
                    else if (!week.Active)
                        btnSat.Text = "No Tasks";
                }
                else
                    btnSat.Text = "View Tasks";
                SatNumTask.Text = saturdayTasks.Count().ToString();
                foreach (var task in saturdayTasks)
                {
                    //check if task is completed
                    if (task.IsCompleted)
                    {
                        satPercentage += task.Percentage;

                    }
                    else if (!task.IsCompleted)
                    {
                        // check task has subtasks
                        //get all subtasks having the tasks Id
                        var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);

                        if (subtasks.Count() > 0)
                        {
                            // get the task's pending percentage
                            satPendingpercentage += task.PendingPercentage;
                        }
                    }
                }
                SattotalPercetage += satPercentage + satPendingpercentage;
                SattotalTaskpercentage.Text = Math.Round(SattotalPercetage, 1).ToString();
                satPendingpercentage = 0;
                satPercentage = 0;
                SattotalPercetage = 0;
                SatId = 0;

            }

        }     
    }
}