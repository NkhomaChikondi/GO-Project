using GO.Models;
using GO.Services;
using GO.ViewModels.TaskInGoals;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        private bool Clicked = false;
        private int weekNumber = 0;
        private int clicks = 1;
        private int weektaskCount = 0;
        private bool newtaskAdded = false;
        // properties for day dates
        private DateTime sunDate = new DateTime();
        private DateTime monDate = new DateTime();
        private DateTime tueDate = new DateTime();
        private DateTime wedDate = new DateTime();
        private DateTime thuDate = new DateTime();
        private DateTime friDate = new DateTime();
        private DateTime satDate = new DateTime();

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

            if (!Clicked)
            {
                latestWeek = result;
                // get the  week having the week id
                var week = await dataWeek.GetWeekAsync(result);

                if (DateTime.Today > week.EndDate)
                {
                    week.Active = false;
                    await dataWeek.UpdateWeekAsync(week);
                    status.Text = "Expired Week";
                }
                else
                {
                    status.Text = "Active Week";
                }
                // get the goal having the goalid
                var goal = await datagoal.GetGoalAsync(week.GoalId);
                // pass the goal id to the private field
                goalId = goal.Id;

                //call calculate weekdays method
                await calculateWeekdays(latestWeek);

                addbtn.IsVisible = true;
                dateToday.Text = DateTime.Today.ToLongDateString().ToString();
                //get all tasks having the goal id for the latest week
                var weekTasks = await DataTask.GetTasksAsync(goalId, latestWeek);
                
                if(weekTasks.Count() > weektaskCount)
                {
                    newtaskAdded = true;
                  weektaskCount =  weekTasks.Count();
                }
               else if (weektaskCount == 0)
                {
                    newtaskAdded = false;
                }

                else if(weekTasks.Count() <= weektaskCount)                
                    newtaskAdded = false;
                               
                if (weekTasks.Count() == 0)
                {
                    listView.IsVisible = false;
                    notasks.IsVisible = true;
                    notasks.Text = "No tasks, Tap on the + button to add tasks";
                    Daytask.Text = DateTime.Today.DayOfWeek.ToString();
                    if (BindingContext is WeeklyTaskViewModel wvm)
                    {
                        wvm.GoalId = goal.Id;
                        wvm.WeekId = result;
                        taskNumber.Text = "0";
                        // await showButtonclicked(0);
                        await wvm.Refresh();
                    }
                }
                else if (weekTasks.Count() != 0)
                {
                    if (BindingContext is WeeklyTaskViewModel wvm)
                    {
                        wvm.GoalId = goal.Id;
                        listView.IsVisible = true;
                        notasks.IsVisible = false;                   
                                                
                            // get the last inserted task
                            var lastTask = weekTasks.ToList().LastOrDefault();
                        var daydate = lastTask.StartTask;
                        if(newtaskAdded)
                        {
                            wvm.DaySelected = lastTask.DowId;
                            await showButtonclicked(lastTask.DowId);
                            Daytask.Text = lastTask.StartTask.DayOfWeek.ToString();
                            newtaskAdded = false;
                        }
                        else
                        {
                            if (lastTask.StartTask > DateTime.Today)
                            {
                                // get a task whose start date is today
                                var task = weekTasks.Where(s => s.StartTask.Date == DateTime.Today.Date).FirstOrDefault();
                                if (task == null)
                                {
                                    wvm.DaySelected = lastTask.DowId;
                                    await showButtonclicked(lastTask.DowId);
                                    Daytask.Text = lastTask.StartTask.DayOfWeek.ToString();
                                }
                                else
                                {
                                    wvm.DaySelected = task.DowId;
                                    await showButtonclicked(task.DowId);
                                    Daytask.Text = task.StartTask.DayOfWeek.ToString();
                                }
                            }
                        }                                           
                        wvm.WeekId = result;
                        await wvm.Refresh();


                    }
                }
            }

            else if (Clicked)
            {
                //call calculate weekdays method
                await calculateWeekdays(weekNumber);
                // get the  week having the week id
                var week = await dataWeek.GetWeekAsync(weekNumber);
                if (DateTime.Today > week.EndDate)
                {
                    week.Active = false;
                    await dataWeek.UpdateWeekAsync(week);

                }

                // get the goal having the goalid
                var goal = await datagoal.GetGoalAsync(week.GoalId);
                // pass the goal id to the private field
                goalId = goal.Id;

                //get all tasks having the goal id for the latest week
                var Tasks = await DataTask.GetTasksAsync(goalId);
                // get tasks thats have the weeknmber id
                var weekTasks = Tasks.Where(d => d.WeekId == weekNumber).ToList();
                if (week.Active)
                {
                    if (weektaskCount == 0)
                    {
                        newtaskAdded = false;
                    }
                    else if (weekTasks.Count() > weektaskCount)
                    {
                        newtaskAdded = true;
                        weektaskCount = weekTasks.Count();
                    }
                    else if (weekTasks.Count() <= weektaskCount)
                        newtaskAdded = false;

                    weektaskCount = weekTasks.Count();

                    if (weekTasks.Count() == 0)
                    {
                        status.Text = "Active";
                        addbtn.IsVisible = true;
                        listView.IsVisible = false;
                        notasks.IsVisible = true;
                        notasks.Text = "They are no tasks for this week, Tap on the + button to add tasks";
                        //  Daytask.Text = DateTime.Today.DayOfWeek.ToString();
                        if (BindingContext is WeeklyTaskViewModel wvm)
                        {
                            wvm.GoalId = goal.Id;
                            wvm.WeekId = weekNumber;
                            taskNumber.Text = "0";
                            Daytask.Text = "   No";
                            framesun.BackgroundColor = Color.White;
                            framemon.BackgroundColor = Color.White;
                            frametue.BackgroundColor = Color.White;
                            framewed.BackgroundColor = Color.White;
                            framethu.BackgroundColor = Color.White;
                            framefri.BackgroundColor = Color.White;
                            framesat.BackgroundColor = Color.White;
                            await wvm.Refresh();
                        }
                    }
                    else if (weekTasks.Count() != 0)
                    {
                        if (BindingContext is WeeklyTaskViewModel wvm)
                        {
                            wvm.GoalId = goal.Id;
                            // get the last inserted task
                            var lastTask = weekTasks.ToList().LastOrDefault();
                            if (newtaskAdded)
                            {
                                wvm.DaySelected = lastTask.DowId;
                                await showButtonclicked(lastTask.DowId);
                                Daytask.Text = lastTask.StartTask.DayOfWeek.ToString();
                                newtaskAdded = false;
                            }
                            else
                            {
                                if (lastTask.StartTask > DateTime.Today)
                                {
                                    // get a task whose start date is today
                                    var task = weekTasks.Where(s => s.StartTask.Date == DateTime.Today.Date).FirstOrDefault();
                                    if (task == null)
                                    {
                                        wvm.DaySelected = lastTask.DowId;
                                        await showButtonclicked(lastTask.DowId);
                                        Daytask.Text = lastTask.StartTask.DayOfWeek.ToString();
                                    }
                                    else
                                    {
                                        wvm.DaySelected = task.DowId;
                                        await showButtonclicked(task.DowId);
                                        Daytask.Text = task.StartTask.DayOfWeek.ToString();
                                    }
                                }
                            }
                            wvm.WeekId = weekNumber;
                            await wvm.Refresh();
                        }
                    }
                }
                else if (!week.Active)
                {

                    if (weekTasks.Count() == 0)
                    {
                        status.Text = "Expired";
                        addbtn.IsVisible = false;
                        listView.IsVisible = false;
                        notasks.IsVisible = true;
                        notasks.Text = "They are no tasks for this week.";
                        //  Daytask.Text = DateTime.Today.DayOfWeek.ToString();
                        if (BindingContext is WeeklyTaskViewModel wvm)
                        {
                            wvm.GoalId = goal.Id;
                            wvm.WeekId = weekNumber;
                            taskNumber.Text = "0";
                            Daytask.Text = "   No";
                            framesun.BackgroundColor = Color.White;
                            framemon.BackgroundColor = Color.White;
                            frametue.BackgroundColor = Color.White;
                            framewed.BackgroundColor = Color.White;
                            framethu.BackgroundColor = Color.White;
                            framefri.BackgroundColor = Color.White;
                            framesat.BackgroundColor = Color.White;
                            await wvm.Refresh();
                        }
                    }
                    else if (weekTasks.Count() != 0)
                    {

                        status.Text = "Expired";
                        addbtn.IsVisible = false;
                        listView.IsVisible = true;
                        notasks.IsVisible = false;
                        if (BindingContext is WeeklyTaskViewModel wvm)
                        {
                            wvm.GoalId = goal.Id;
                            if (wvm.DaySelected == 0)
                            {
                                // get the first task
                                var firstTask = weekTasks.ToList().FirstOrDefault();
                                wvm.DaySelected = firstTask.DowId;
                                // Daytask.Text = firstTask.StartTask.DayOfWeek.ToString();
                                await showButtonclicked(firstTask.DowId);
                            }
                            else
                            {
                                // get the last inserted task
                                var lastTask = weekTasks.LastOrDefault();
                                wvm.DaySelected = lastTask.DowId;
                                // Daytask.Text = lastTask.StartTask.DayOfWeek.ToString();
                                await showButtonclicked(lastTask.DowId);
                            }
                            wvm.WeekId = weekNumber;
                            await wvm.Refresh();
                        }
                    }
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
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                Daytask.Text = "Sunday";
                await wvm.sunButton();
                //get dayselected
                await showButtonclicked(wvm.DaySelected);
            }

        }
        private void frameTapmon(object sender, EventArgs e)
        {
            mondayTap();
        }
        private async Task mondayTap()
        {
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                Daytask.Text = "Monday";
                await wvm.monButton();
                //get dayselected
                await showButtonclicked(wvm.DaySelected);
            }
        }
        private void frameTapTue(object sender, EventArgs e)
        {
            tuesdayTap();
        }
        private async Task tuesdayTap()
        {
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                Daytask.Text = "Tuesday";
                await wvm.tueButton();
                //get dayselected
                await showButtonclicked(wvm.DaySelected);
            }
        }
        private void frameTapWed(object sender, EventArgs e)
        {
            wednesdayTap();
        }
        private async Task wednesdayTap()
        {
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                Daytask.Text = "Wednesday";
                await wvm.wedButton();
                //get dayselected
                await showButtonclicked(wvm.DaySelected);
            }

        }
        private void frameTapThur(object sender, EventArgs e)
        {
            thursdayTap();
        }
        private async Task thursdayTap()
        {
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                Daytask.Text = "Thursday";
                await wvm.thuButton();
                //get dayselected
                await showButtonclicked(wvm.DaySelected);
            }
        }
        private void frameTapFri(object sender, EventArgs e)
        {
            fridayTap();
        }
        private async Task fridayTap()
        {
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                Daytask.Text = "Friday";
                //get dayselected
                await wvm.friButton();
                await showButtonclicked(wvm.DaySelected);               
            }

        }
        private void frameTapSat(object sender, EventArgs e)
        {
            saturdayTap();
        }
        private async Task saturdayTap()
        {
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                Daytask.Text = "Saturday";
                await wvm.satButton();
                //get dayselected
                await showButtonclicked(wvm.DaySelected);
            }

        }
        private async Task calculateWeekdays(int Id)
        {
            bool dayselected = false;
            // defualt date values
            Sunday.Text = "  N/A  ";
            Monday.Text = "  N/A  ";
            Tuesday.Text = " N/A  ";
            Wednesday.Text = "  N/A  ";
            Thursday.Text = "  N/A  ";
            Friday.Text = "  N/A  ";
            Saturday.Text = "  N/A  ";
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

            double counter = 0.0;

            // get the week having the same Id as the incoming Id
            var week = await dataWeek.GetWeekAsync(Id);
            weeknumber.Text = week.WeekNumber.ToString();
            weekstats.Text = weeknumber.Text;
            startDatetxt.Text = week.StartDate.ToLongDateString();
            endDatetxt.Text = week.EndDate.ToLongDateString();
            // loop through the list
            foreach (var day in weekDays)
            {
                //loop until day is equal to week's start day

                if (week.StartDate.DayOfWeek.ToString() == day)
                {
                    if (!dayselected)
                    {
                        if (day == "Sunday")
                        {
                            var date = week.StartDate.ToLongDateString();
                            sunDate = week.StartDate;
                            string month = date.Substring(0, 6);
                            Sunday.Text = month;
                        }
                        if (day == "Monday")
                        {
                            var date = week.StartDate.ToLongDateString();
                            monDate = week.StartDate;
                            string month = date.Substring(0, 6);
                            Monday.Text = month;
                        }

                        if (day == "Tuesday")
                        {
                            tueDate = week.StartDate;
                            var date = week.StartDate.ToLongDateString();
                            string month = date.Substring(0, 6);
                            Tuesday.Text = month;
                        }

                        if (day == "Wednesday")
                        {
                            wedDate = week.StartDate;
                            var date = week.StartDate.ToLongDateString();
                            string month = date.Substring(0, 6);
                            Wednesday.Text = month;
                        }
                        if (day == "Thursday")
                        {
                            thuDate = week.StartDate;
                            var date = week.StartDate.ToLongDateString();
                            string month = date.Substring(0, 6);
                            Thursday.Text = month;
                        }
                        if (day == "Friday")
                        {
                            friDate = week.StartDate;
                            var date = week.StartDate.ToLongDateString();
                            string month = date.Substring(0, 6);
                            Friday.Text = month;
                        }
                        if (day == "Saturday")
                        {
                            satDate = week.StartDate;
                            var date = week.StartDate.ToLongDateString();
                            string month = date.Substring(0, 6);
                            Saturday.Text = month;
                        }
                        dayselected = true;
                    }

                }
                else if (dayselected)
                {
                    counter++;

                    if (day == "Sunday")
                    {
                        DateTime startDate = week.StartDate.AddDays(counter);
                        sunDate = startDate;
                        string date = startDate.ToLongDateString();
                        Sunday.Text = date.Substring(0, 6);
                    }

                    if (day == "Monday")
                    {
                        DateTime startDate = week.StartDate.AddDays(counter);
                        monDate = startDate;
                        string date = startDate.ToLongDateString();
                        Monday.Text = date.Substring(0, 6);
                    }

                    if (day == "Tuesday")
                    {
                        DateTime startDate = week.StartDate.AddDays(counter);
                        tueDate = startDate;
                        string date = startDate.ToLongDateString();
                        Tuesday.Text = date.Substring(0, 6);
                    }

                    if (day == "Wednesday")
                    {
                        DateTime startDate = week.StartDate.AddDays(counter);
                        wedDate = startDate;
                        string date = startDate.ToLongDateString();
                        Wednesday.Text = date.Substring(0, 6);
                    }

                    if (day == "Thursday")
                    {
                        DateTime startDate = week.StartDate.AddDays(counter);
                        thuDate = startDate;
                        string date = startDate.ToLongDateString();
                        Thursday.Text = date.Substring(0, 6);
                    }

                    if (day == "Friday")
                    {
                        DateTime startDate = week.StartDate.AddDays(counter);
                        friDate = startDate;
                        string date = startDate.ToLongDateString();
                        Friday.Text = date.Substring(0, 6);
                    }

                    if (day == "Saturday")
                    {
                        DateTime startDate = week.StartDate.AddDays(counter);
                        satDate = startDate;
                        string date = startDate.ToLongDateString();
                        Saturday.Text = date.Substring(0, 6);
                    }

                }

            }
        }
        private void LeftClicked(object sender, EventArgs e)
        {
            LeftClickMethod();
        }
        async Task LeftClickMethod()
        {
            clicks++;
            // get weeks
            var weeks = await dataWeek.GetWeeksAsync(goalId);
            int count = weeks.Count();
            if (count - clicks < 0)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "No Weeks left", "Ok");
                clicks--;
                return;
            }
            else
            {
                // Week activeWeek = weeks.ElementAtOrDefault(count -clicks);
                Week activeWeek = weeks.ElementAt(count - clicks);
                if (!Clicked)
                {
                    // pass the number of weeks to weekNumber
                    weekNumber = activeWeek.Id;

                    // assign clicked to true
                    Clicked = true;
                    //call the onapperance method
                    OnAppearing();
                }
                else if (Clicked)
                {
                    // pass the number of weeks to weekNumber
                    weekNumber = activeWeek.Id;
                    //call the onapperance method
                    OnAppearing();
                }
            }
        }
        private void RightClicked(object sender, EventArgs e)
        {
            RightClickMethod();
        }
        async Task RightClickMethod()
        {
            clicks--;
            // get weeks
            var weeks = await dataWeek.GetWeeksAsync(goalId);
            int count = weeks.Count();
            if (clicks < 1)
            {
                await App.Current.MainPage.DisplayAlert("Alert", "No Weeks left", "Ok");
                clicks++;
                return;
            }
            else
            {
                // Week activeWeek = weeks.ElementAtOrDefault(count -clicks);
                Week activeWeek = weeks.ElementAt(count - clicks);
                if (!Clicked)
                {
                    // pass the number of weeks to weekNumber
                    weekNumber = activeWeek.Id;


                    // assign clicked to true
                    Clicked = true;
                    //call the onapperance method
                    OnAppearing();
                }
                else if (Clicked)
                {
                    // pass the number of weeks to weekNumber
                    weekNumber = activeWeek.Id;
                    // get the last week
                    var lastWeek = weeks.LastOrDefault();
                    if (activeWeek.Id == lastWeek.Id)
                    {
                        Clicked = false;
                    }
                    //call the onapperance method
                    OnAppearing();
                }
            }
        }
        async Task showButtonclicked(int id)
        {
            // get task through the id
            var dow = await dataDow.GetDOWAsync(id);
            // get all tasks havng the dowd
            var daytasks = await DataTask.GetTasksAsync(goalId);
            var dowtask = daytasks.Where(d => d.DowId == id).ToList();

            taskNumber.Text = dowtask.Count().ToString();
            var startday = dow.Name;
            if (startday == "Sunday")
            {
                if(dow.Date == new DateTime())
                {
                    dow.Date = sunDate;
                    await dataDow.UpdateDOWAsync(dow);
                }
                Daytask.Text = startday;
                framesun.BackgroundColor = Color.LightGray;
                framemon.BackgroundColor = Color.White;
                frametue.BackgroundColor = Color.White;
                framewed.BackgroundColor = Color.White;
                framethu.BackgroundColor = Color.White;
                framefri.BackgroundColor = Color.White;
                framesat.BackgroundColor = Color.White;

            }
            else if (startday == "Monday")
            {
                if (dow.Date == new DateTime())
                {
                    dow.Date = monDate;
                    await dataDow.UpdateDOWAsync(dow);
                }
                Daytask.Text = startday;
                framesun.BackgroundColor = Color.White;
                framemon.BackgroundColor = Color.LightGray;
                frametue.BackgroundColor = Color.White;
                framewed.BackgroundColor = Color.White;
                framethu.BackgroundColor = Color.White;
                framefri.BackgroundColor = Color.White;
                framesat.BackgroundColor = Color.White;

            }
            else if (startday == "Tuesday")
            {
                if (dow.Date == new DateTime())
                {
                    dow.Date = tueDate;
                    await dataDow.UpdateDOWAsync(dow);
                }
                Daytask.Text = startday;
                framesun.BackgroundColor = Color.White;
                framemon.BackgroundColor = Color.White;
                frametue.BackgroundColor = Color.LightGray;
                framewed.BackgroundColor = Color.White;
                framethu.BackgroundColor = Color.White;
                framefri.BackgroundColor = Color.White;
                framesat.BackgroundColor = Color.White;

            }
            else if (startday == "Wednesday")
            {
                if (dow.Date == new DateTime())
                {
                    dow.Date = wedDate;
                    await dataDow.UpdateDOWAsync(dow);
                }
                Daytask.Text = startday;
                framesun.BackgroundColor = Color.White;
                framemon.BackgroundColor = Color.White;
                frametue.BackgroundColor = Color.White;
                framewed.BackgroundColor = Color.LightGray;
                framethu.BackgroundColor = Color.White;
                framefri.BackgroundColor = Color.White;
                framesat.BackgroundColor = Color.White;

            }
            else if (startday == "Thursday")
            {
                if (dow.Date == new DateTime())
                {
                    dow.Date = thuDate;
                    await dataDow.UpdateDOWAsync(dow);
                }
                Daytask.Text = startday;
                framesun.BackgroundColor = Color.White;
                framemon.BackgroundColor = Color.White;
                frametue.BackgroundColor = Color.White;
                framewed.BackgroundColor = Color.White;
                framethu.BackgroundColor = Color.LightGray;
                framefri.BackgroundColor = Color.White;
                framesat.BackgroundColor = Color.White;

            }
            else if (startday == "Friday")
            {
                if (dow.Date == new DateTime())
                {
                    dow.Date = friDate;
                    await dataDow.UpdateDOWAsync(dow);
                }
                Daytask.Text = startday;
                framesun.BackgroundColor = Color.White;
                framemon.BackgroundColor = Color.White;
                frametue.BackgroundColor = Color.White;
                framewed.BackgroundColor = Color.White;
                framethu.BackgroundColor = Color.White;
                framefri.BackgroundColor = Color.LightGray;
                framesat.BackgroundColor = Color.White;

            }
            else if (startday == "Saturday")
            {
                if (dow.Date == new DateTime())
                {
                    dow.Date = satDate;
                    await dataDow.UpdateDOWAsync(dow);
                }
                Daytask.Text = startday;
                framesun.BackgroundColor = Color.White;
                framemon.BackgroundColor = Color.White;
                frametue.BackgroundColor = Color.White;
                framewed.BackgroundColor = Color.White;
                framethu.BackgroundColor = Color.White;
                framefri.BackgroundColor = Color.White;
                framesat.BackgroundColor = Color.LightGray;

            }
            else
            {
                framesun.BackgroundColor = Color.White;
                framemon.BackgroundColor = Color.White;
                frametue.BackgroundColor = Color.White;
                framewed.BackgroundColor = Color.White;
                framethu.BackgroundColor = Color.White;
                framefri.BackgroundColor = Color.White;
                framesat.BackgroundColor = Color.White;

            }
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
           
            var action = await DisplayActionSheet("Sort tasks by:", "Cancel", "", "All","Not Started","In Progress","Completed","With subtasks");
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
        private void ImageButton_Clicked_1(object sender, EventArgs e)
        {
            Application.Current.MainPage.DisplayAlert("INFO", "* The first week of a goal will be created upon creating your goal. \n " +
                "* The proceeding weeks in the goal, will be created automatically upon tapping on the goal after the end date of the previous week has surpassed. \n" +
                "* A week ends on saturday and a new week is created on Sunday or on any day you tap on the goal when the previous week has expired. ", "OK");
        }
        private void ImageButton_Clicked_2(object sender, EventArgs e)
        {
            Application.Current.MainPage.DisplayAlert("INFO", "* The scrollable tabs below are indicating the dates and days from the start of your week to the end date. \n " +
               "* Tapping on any day, will load tasks that are assigned to that day. \n * New tasks can be added to the day provided the date has not surpass. if you add a new task task on a day that has surpass, that new task will be added to the day whose date is equal to the date of that day.\n\n" +
               "NOTE \n * If a day is having N/A indicated on top, it means that day doesnt fall within the week's start or end day and it does nothing.", "OK");
        }
    }

}


    
