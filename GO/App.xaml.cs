using GO.Models;
using GO.Services;
using Plugin.LocalNotification;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO
{
    public partial class App : Application
    {
        IDataGoal<Goal> dataGoal { get; }
        IDataWeek<Week> dataWeek { get; }
        IDataStore<Category> dataStore { get; }
        public App()
        {
            InitializeComponent();
          
            MainPage = new AppShell();
            dataGoal = DependencyService.Get<IDataGoal<Goal>>();
            dataWeek = DependencyService.Get<IDataWeek<Week>>();
            dataStore = DependencyService.Get<IDataStore<Category>>();
        }

        protected override void OnStart()
        {
            base.OnStart();
           //CreateNewWeek();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        async Task CreateNewWeek()
        {
            // get all goals in the database
            var categories = await dataStore.GetItemsAsync();
            // loop through the categories to get their goal
            foreach(var category in categories)
            {
                // get all goals having the category id
                var goals = await dataGoal.GetGoalsAsync(category.Id);
                // get goal thas has weeks
                var weekgoals = goals.Where(g => g.HasWeek).ToList();
                // loop through the week goals to get the weeks
                foreach( var weekgoal in weekgoals)
                {
                    if(DateTime.Today < weekgoal.End)
                    {
                        //get the number of days left for the goal to end
                        var Daysleft = weekgoal.End - DateTime.Today;
                        TimeSpan daynumber = new TimeSpan(7, 0, 0, 0);


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
                       

                            // get all weeks having the week goal id
                            var allWeeks = await dataWeek.GetWeeksAsync(weekgoal.Id);
                            // get the last inserted week
                            var lastgoalweek = allWeeks.ToList().LastOrDefault();

                            if (DateTime.Today > lastgoalweek.EndDate)
                            {
                              try
                              {
                                //check if lastgoalweek is active
                                if (lastgoalweek.Active)
                                {
                                    lastgoalweek.Active = false;
                                    await dataWeek.UpdateWeekAsync(lastgoalweek);
                                }
                                // create a new week
                                //* check the number of days that are left in the week*/
                                //* finding the targeted percentage*
                                // get the total number of weeks from goal
                                var totalWeeks = weekgoal.NumberOfWeeks;
                                // calculate the percentage for every week
                                var weekPercentage = 100 / totalWeeks;

                               
                                if (Daysleft > daynumber)
                                {
                                    if (startDay == "Saturday")
                                    {
                                        dayValue = 6;

                                        
                                    }
                                    // how to find the end date
                                    var enddate = DateTime.Today.AddDays(dayValue);

                                    // *create a new week object*

                                    var newWeek = new Week
                                    {
                                        WeekNumber = lastgoalweek.WeekNumber + 1,
                                        TargetPercentage = weekPercentage,
                                        AccumulatedPercentage = 0,
                                        Active = true,
                                        StartDate = DateTime.Today,
                                        EndDate = enddate,
                                        GoalId = weekgoal.Id
                                    };
                                    // save the newly created week to the database
                                    await dataWeek.AddWeekAsync(newWeek);
                                    // get all weeks having GoalId
                                    var weeks = await dataWeek.GetWeeksAsync(newWeek.GoalId);
                                    // get the last goal
                                    var week = weeks.ToList().LastOrDefault();
                                    // get goal 
                                    var goal = await dataGoal.GetGoalAsync(newWeek.GoalId);
                                    TimeSpan duration = week.EndDate - week.StartDate;
                                    var date = (double)duration.TotalDays;
                                    var notification = new NotificationRequest
                                    {
                                        BadgeNumber = 1,
                                        Description = $"Week {newWeek.WeekNumber} of goal '{goal.Name}' is Due today!",
                                        Title = "Due-Date!",
                                        NotificationId = week.Id,
                                        Schedule =
                                        {
                                            NotifyTime = DateTime.Now.AddDays(date),
                                        }
                                    };
                                    await LocalNotificationCenter.Current.Show(notification);
                                }
                                else if (Daysleft < daynumber)
                                {
                                    var newWeek = new Week
                                    {
                                        WeekNumber = lastgoalweek.WeekNumber + 1,
                                        TargetPercentage = weekPercentage,
                                        AccumulatedPercentage = 0,
                                        Active = true,
                                        StartDate = DateTime.Today,
                                        EndDate = weekgoal.End,
                                        GoalId = weekgoal.Id
                                    };
                                    // save the newly created week to the database
                                    await dataWeek.AddWeekAsync(newWeek);
                                    // get all weeks having GoalId
                                    var weeks = await dataWeek.GetWeeksAsync(newWeek.GoalId);
                                    // get the last goal
                                    var week = weeks.ToList().LastOrDefault();
                                    // get goal 
                                    var goal = await dataGoal.GetGoalAsync(newWeek.GoalId);
                                    TimeSpan duration = week.EndDate - week.StartDate;
                                    var date = (double)duration.TotalDays;
                                    var notification = new NotificationRequest
                                    {
                                        BadgeNumber = 1,
                                        Description = $" Week {newWeek.WeekNumber} of goal '{goal.Name}' is Due today!",
                                        Title = "Due-Date!",
                                        NotificationId = week.Id,
                                        Schedule =
                                            {
                                                NotifyTime = DateTime.Now.AddDays(date),
                                            }
                                    };
                                    await LocalNotificationCenter.Current.Show(notification);
                                }

                              }
                                 catch (Exception ex)
                                {
                                    Debug.WriteLine($"Failed to add new goal: {ex.Message}");
                                    await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
                               }


                            }


                    }
                }    
            }
            return;
        }
    }
}
