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
    public partial class WeekTask : ContentPage
    {
        public string weekId { get; set; }
        private int goalId;
        int latestWeek;
        List<Models.GoalTask> goalTasks = new List<Models.GoalTask>();
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataSubtask<Subtask> datasubtask { get; }
        public IDataWeek<Models.Week> dataWeek { get; }
        public IDataDow<DOW> dataDow { get; }
        public WeekTask()
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
            latestWeek = result;
            // get the  week having the week id
            var week = await dataWeek.GetWeekAsync(result);
            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(week.GoalId);
            // pass the goal id to the private field
            goalId = goal.Id;
            // get all tasks having goal id
            var tasks = await DataTask.GetTasksAsync(goal.Id);
            //call calculate weekdays method
            await calculateWeekdays(latestWeek);
            dateToday.Text = DateTime.Today.ToLongDateString().ToString();
            // assign the tasks to goalaTask list           
            foreach(var task in tasks)
            {
                goalTasks.Add(task);
            }
            
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.GoalId = goal.Id;
                //wvm.DowId = selectedDow.DOWId;
                wvm.WeekId = result;
               // await wvm.Refresh();

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

        private void frameTapSun(object sender, EventArgs e)
        {
            sundayTap();
          
        }
        private async Task sundayTap()
        {
            var dowsList = await dataDow.GetDOWsAsync(latestWeek);
            // get the id of dow sunday
            var sunday = dowsList.Where(D => D.Name == "Sunday").FirstOrDefault();
            // pass it to  the refresh method
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DaySelected = sunday.DOWId;
                wvm.Refresh();
            }
            
        }

        private void frameTapmon(object sender, EventArgs e)
        {
            mondayTap();
        }
        private async Task mondayTap()
        {
            var dowsList = await dataDow.GetDOWsAsync(latestWeek);
            // get the id of dow sunday
            var sunday = dowsList.Where(D => D.Name == "Monday").FirstOrDefault();
            // pass it to  the refresh method
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DaySelected = sunday.DOWId;
                wvm.Refresh();
            }

        }

        private void frameTapTue(object sender, EventArgs e)
        {
            tuesdayTap();
        }
        private async Task tuesdayTap()
        {
            var dowsList = await dataDow.GetDOWsAsync(latestWeek);
            // get the id of dow sunday
            var sunday = dowsList.Where(D => D.Name == "Tuesday").FirstOrDefault();
            // pass it to  the refresh method
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DaySelected = sunday.DOWId;
                wvm.Refresh();
            }

        }

        private void frameTapWed(object sender, EventArgs e)
        {
            wednesdayTap();
        }
        private async Task wednesdayTap()
        {
            var dowsList = await dataDow.GetDOWsAsync(latestWeek);
            // get the id of dow sunday
            var sunday = dowsList.Where(D => D.Name == "Wednesday").FirstOrDefault();
            // pass it to  the refresh method
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DaySelected = sunday.DOWId;
                wvm.Refresh();
            }

        }

        private void frameTapThur(object sender, EventArgs e)
        {
            thursdayTap();
        }


        private async Task thursdayTap()
        {
            var dowsList = await dataDow.GetDOWsAsync(latestWeek);
            // get the id of dow sunday
            var sunday = dowsList.Where(D => D.Name == "Thurday").FirstOrDefault();
            // pass it to  the refresh method
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DaySelected = sunday.DOWId;
                wvm.Refresh();
            }

        }

        private void frameTapFri(object sender, EventArgs e)
        {
            fridayTap();
        }
        private async Task fridayTap()
        {
            var dowsList = await dataDow.GetDOWsAsync(latestWeek);
            // get the id of dow sunday
            var sunday = dowsList.Where(D => D.Name == "Friday").FirstOrDefault();
            // pass it to  the refresh method
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DaySelected = sunday.DOWId;
                wvm.Refresh();
            }

        }

        private void frameTapSat(object sender, EventArgs e)
        {
            saturdayTap();
        }
        private async Task saturdayTap()
        {
            var dowsList = await dataDow.GetDOWsAsync(latestWeek);
            // get the id of dow sunday
            var sunday = dowsList.Where(D => D.Name == "Saturday").FirstOrDefault();
            // pass it to  the refresh method
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.DaySelected = sunday.DOWId;
                wvm.Refresh();
            }

        }
        private async Task calculateWeekdays(int Id)
        {
            bool dayselected = false; ;
            // create a list of days
            List<String> weekDays = new List<string>();
            //add items to list
            weekDays.Add("Sunday");
            weekDays.Add("Monday");
            weekDays.Add("Tuesday");
            weekDays.Add("Wednesday");
            weekDays.Add("Thursday");
            weekDays.Add("Friday");
            weekDays.Add("Saturday");

            // create anothe  list having the ui day dates
            List<string> daydates = new List<string>();
            daydates.Add(Sunday.Text);
            daydates.Add(Monday.Text);
            daydates.Add(Tuesday.Text);
            daydates.Add(Wednesday.Text);
            daydates.Add(Thursday.Text);
            daydates.Add(Friday.Text);
            daydates.Add(Saturday.Text);
            double counter = 0.0;

            // get the week having the same Id as the incoming Id
            var week = await dataWeek.GetWeekAsync(Id);
            // loop through the list
            foreach(var day in weekDays)
            {
                if (week.StartDate.DayOfWeek.ToString() == day)
                {
                    if (!dayselected)
                    {
                        if (day == "Sunday")
                            Sunday.Text = week.StartDate.Day.ToString();
                        if (day == "Monday")
                            Monday.Text = week.StartDate.Day.ToString();
                        if (day == "Tuesday")
                            Tuesday.Text = week.StartDate.Day.ToString();
                        if (day == "Wednesday")
                            Wednesday.Text = week.StartDate.Day.ToString();
                        if (day == "Thursday")
                            Thursday.Text = week.StartDate.Day.ToString();
                        if (day == "Friday")
                            Friday.Text = week.StartDate.Day.ToString();
                        if (day == "Saturday")
                            Saturday.Text = week.StartDate.Day.ToString();

                    }
                    dayselected = true;
                }
                else
                {
                    dayselected = true;
                    counter++;

                    if (day == "Sunday")
                        Sunday.Text = week.StartDate.AddDays(counter).Day.ToString();
                    if (day == "Monday")
                        Monday.Text = week.StartDate.AddDays(counter).Day.ToString();
                    if (day == "Tuesday")
                        Tuesday.Text = week.StartDate.AddDays(counter).Day.ToString();
                    if (day == "Wednesday")
                        Wednesday.Text = week.StartDate.AddDays(counter).Day.ToString();
                    if (day == "Thursday")
                        Thursday.Text = week.StartDate.AddDays(counter).Day.ToString();
                    if (day == "Friday")
                        Friday.Text = week.StartDate.AddDays(counter).Day.ToString();
                    if (day == "Saturday")
                        Saturday.Text = week.StartDate.AddDays(counter).Day.ToString();
                }                   
                
                //dayselected = false;
                //counter = 0.0;
            }
        }
    }
}
    
