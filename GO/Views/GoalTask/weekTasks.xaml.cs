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
    public partial class weekTasks : ContentPage
    {

        public string weekId { get; set; }
        private int goalId;
        private int selecteddowId;
        private int Idweek;
     
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataSubtask<Subtask> datasubtask { get; }
        public IDataWeek<Models.Week> dataWeek { get; }
        public IDataDow<DOW> dataDow { get; }
        public weekTasks()
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
            // pass result to idWeek
            Idweek = result;
            // get the  week having the week id
            var week = await dataWeek.GetWeekAsync(result);
            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(week.GoalId);
            // pass the goal id to the private field
            goalId = goal.Id;
            notasks.Text = " ";
            // get all tasks having goal id
            var tasks = await DataTask.GetTasksAsync(goal.Id);
            // get the selected dow
           var dows = await dataDow.GetDOWsAsync(result);
            var selectedDow = dows.Where(D => D.IsSelected == true).FirstOrDefault();
            taskName.Text = selectedDow.Name;
            selecteddowId = selectedDow.DOWId;
            // get all tasks having the selectedDow id
            var dowTasks = tasks.Where(T => T.DowId == selectedDow.DOWId).ToList();
            if (dowTasks.Count() == 0)
            {
                WeekStackTasklist.IsVisible = false;
                weekstacklayout.IsVisible = true;
                weektasktoprow.IsVisible = false;
                tasknameandlockedstack.IsVisible = false;
            }
            else
            {
                WeekStackTasklist.IsVisible = true;
                weekstacklayout.IsVisible = false;
                weektasktoprow.IsVisible = true;
                tasknameandlockedstack.IsVisible = true;
            }
            // check if week in active or not
            if (week.Active)
            {
                // check if the day is valid
               if(selectedDow.Name.Equals("Sunday"))
               {
                    // check the day for today
                    if(DateTime.Today.DayOfWeek == DayOfWeek.Sunday)
                    {
                        // get all dows from the database having the weekid
                        var weekdows = await dataDow.GetDOWsAsync(week.Id);
                        //loop through the dows
                        foreach(var dow in weekdows)
                        {
                            if (dow.Name.Equals("Sunday"))                            
                                dow.ValidDay = true;                                               
                            else
                                dow.ValidDay = false;
                            await dataDow.UpdateDOWAsync(dow);
                        }                       
                        statusname.Text = "Unlocked";
                        statusname.TextColor = Color.DarkGreen;
                    }
                    else
                    {
                        statusname.Text = "Locked";
                        statusname.TextColor = Color.OrangeRed;
                        addbtn.IsEnabled = false;
                        addbtn.IsVisible = false;
                    }
               }
               else if(selectedDow.Name.Equals("Monday"))
               {
                    // check the day for today
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Monday)
                    {
                        // get all dows from the database having the weekid
                        var weekdows = await dataDow.GetDOWsAsync(week.Id);
                        //loop through the dows
                        foreach (var dow in weekdows)
                        {
                            if (dow.Name.Equals("Monday"))                            
                                dow.ValidDay = true;                            
                            else
                                dow.ValidDay = false;
                            await dataDow.UpdateDOWAsync(dow);
                        }
                        statusname.Text = "Unlocked";
                        statusname.TextColor = Color.DarkGreen;

                    }
                    else
                    {
                        statusname.Text = "Locked";
                        statusname.TextColor = Color.OrangeRed;
                        addbtn.IsEnabled = false;
                        addbtn.IsVisible = false;
                    }
                }
                else if (selectedDow.Name.Equals("Tuesday"))
                {
                    // check the day for today
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Tuesday)
                    {
                        statusname.Text = "Unlocked";
                        statusname.TextColor = Color.DarkGreen;
                        // get all dows from the database having the weekid
                        var weekdows = await dataDow.GetDOWsAsync(week.Id);
                        //loop through the dows
                        foreach (var dow in weekdows)
                        {
                            if (dow.Name.Equals("Tuesday"))                            
                                dow.ValidDay = true;                            
                            else
                                dow.ValidDay = false;
                            await dataDow.UpdateDOWAsync(dow);
                        }
                       
                    }
                    else
                    {
                        statusname.Text = "Locked";
                        statusname.TextColor = Color.OrangeRed;
                        addbtn.IsEnabled = false;
                        addbtn.IsVisible = false;
                    }
                }
                else if (selectedDow.Name.Equals("Wednesday"))
                {
                    // check the day for today
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Wednesday)
                    {
                        // get all dows from the database having the weekid
                        var weekdows = await dataDow.GetDOWsAsync(week.Id);
                        //loop through the dows
                        foreach (var dow in weekdows)
                        {
                            if (dow.Name.Equals("Wednesday"))                            
                                dow.ValidDay = true;                            
                            else
                                dow.ValidDay = false;
                            await dataDow.UpdateDOWAsync(dow);
                        }
                        statusname.Text = "Unlocked";
                        statusname.TextColor = Color.DarkGreen;
                    }
                    else
                    {        
                        statusname.Text = "Locked";
                        statusname.TextColor = Color.OrangeRed;
                        addbtn.IsEnabled = false;
                        addbtn.IsVisible = false;
                    }
                }
                else if (selectedDow.Name.Equals("Thursday"))
                {
                    // check the day for today
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Thursday)
                    {
                        // get all dows from the database having the weekid
                        var weekdows = await dataDow.GetDOWsAsync(week.Id);
                        //loop through the dows
                        foreach (var dow in weekdows)
                        {
                            if (dow.Name.Equals("Thursday"))                            
                                dow.ValidDay = true;                            
                            else
                                dow.ValidDay = false;
                            await dataDow.UpdateDOWAsync(dow);
                        }
                        statusname.Text = "Unlocked";
                        statusname.TextColor = Color.DarkGreen;
                    }
                    else
                    {
                        statusname.Text = "Locked";
                        statusname.TextColor = Color.OrangeRed;
                        addbtn.IsEnabled = false;
                        addbtn.IsVisible = false;
                    }
                }
                else if (selectedDow.Name.Equals("Friday"))
                {
                    // check the day for today
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Friday)
                    {
                        // get all dows from the database having the weekid
                        var weekdows = await dataDow.GetDOWsAsync(week.Id);
                        //loop through the dows
                        foreach (var dow in weekdows)
                        {
                            if (dow.Name.Equals("Friday"))                            
                                dow.ValidDay = true;
                            else
                                dow.ValidDay = false;
                            await dataDow.UpdateDOWAsync(dow);
                        }
                        statusname.Text = "Unlocked";
                        statusname.TextColor = Color.DarkGreen;
                    }
                    else
                    {
                        statusname.Text = "Locked";
                        statusname.TextColor = Color.OrangeRed;
                        addbtn.IsEnabled = false;
                        addbtn.IsVisible = false;
                    }
                }
                else if (selectedDow.Name.Equals("Saturday"))
                {
                    // check the day for today
                    if (DateTime.Today.DayOfWeek <= DayOfWeek.Saturday)
                    {
                        // get all dows from the database having the weekid
                        var weekdows = await dataDow.GetDOWsAsync(week.Id);
                        //loop through the dows
                        foreach (var dow in weekdows)
                        {
                            if (dow.Name.Equals("Saturday"))                            
                                dow.ValidDay = true;                       
                            else
                                dow.ValidDay = false;
                            await dataDow.UpdateDOWAsync(dow);
                        }
                        statusname.Text = "Unlocked";
                        statusname.TextColor = Color.DarkGreen;
                    }
                    else
                    {
                        statusname.Text = "Locked";
                        statusname.TextColor = Color.OrangeRed;
                        addbtn.IsEnabled = false;
                        addbtn.IsVisible = false;
                    }
                }

            }
            else if(!week.Active)
            {
                statusname.Text = "Locked";
                statusname.TextColor = Color.OrangeRed;
                addbtn.IsEnabled = false;
                addbtn.IsVisible = false;
               
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
                        await viewModel.CompleteTask(taskid, task.IsCompleted);
                    }                       
                }              
            }
            else if (!task.IsCompleted)
            {
                // check if the incoming object 
                if (!taskdb.IsCompleted)
                    return;
                if (BindingContext is WeeklyTaskViewModel viewModel)
                    await viewModel.UncompleteTask(taskid, task.IsCompleted);
            }
            return;

        }

      

    }
    
}