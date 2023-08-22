using GO.Services;
using GO.ViewModels.Goals;
using Microcharts;
using Microcharts.Forms;
using SkiaSharp;
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
    [QueryProperty(nameof(GoalID), nameof(GoalID))]
    
    public partial class GoalStats : ContentPage
    {
        public string GoalID { get; set; }
        private static int January ;
        private static int February;
        private static int March;
        private static int April;
        private static int May;
        private static int June;
        private static int July;
        private static int August;
        private static int September;
        private static int October;
        private static int November;
        private static int December;

        private static int numTask;
        private static int duration;
        private static int numSubtask;

        public IDataGoal<Models.Goal> dataGoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataSubtask<Models.Subtask> datasubtask { get; }
        public GoalStats()
        {
            InitializeComponent();
            BindingContext = new GoalViewModel();            
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            DataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            datasubtask = DependencyService.Get<IDataSubtask<Models.Subtask>>();                  
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(GoalID, out var result);
            await goalProgress(result); 
            await goalComplexity(result);
        }  
        private async Task goalProgress(int goalid)
        {
            January = 0;
            February = 0;
            March = 0;
            April = 0;
            May = 0;
            June = 0;
            July = 0;
            August = 0;
            September = 0;
            October = 0;
            November = 0;
            December = 0;

            // get goal in the database that has goalid
            var goal = await dataGoal.GetGoalAsync(goalid);
            // get all tasks
            var Tasks = await DataTask.GetTasksAsync(goal.Id);
            // get all tasks in this goal from january
            var JanTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 01).ToList();
            January = JanTasks.Count();
            var FebTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 02).ToList();
            February = FebTasks.Count();
            var MarTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 03).ToList();
            March = MarTasks.Count();
            var AprTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 04).ToList();
            April = AprTasks.Count();
            var MayTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 05).ToList();
            May = MayTasks.Count();
            var JunTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 06).ToList();
            June = JunTasks.Count();
            var JulTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 07).ToList();
            July = JulTasks.Count();
            var AugTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 08).ToList();
            August = AugTasks.Count();
            var SeptTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 09).ToList();
            September = SeptTasks.Count();
            var OctTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 10).ToList();
            October = OctTasks.Count();
            var NovTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 11).ToList();
            November = NovTasks.Count();
            var DecTasks = Tasks.Where(t => t.IsCompleted && t.EndTask.Month == 12).ToList();
            December = DecTasks.Count();

            List<ChartEntry> entries = new List<ChartEntry>
             {

                new ChartEntry(January)
                {
                    Label = "Jan",
                    ValueLabel = January.ToString(),
                    Color = SKColor.Parse("#012a4a")
                },
               new ChartEntry(February)
                {
                    Label = "Feb",
                    ValueLabel = February.ToString(),
                    Color = SKColor.Parse("#012a4a")
                },
                new ChartEntry(March)
                {
                    Label = "Mar",
                    ValueLabel = March.ToString(),
                    Color = SKColor.Parse("#ff8600")
                },
                new ChartEntry(April)
                {
                    Label = "Apr",
                    ValueLabel = April.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
                new ChartEntry(May)
                {
                    Label = "May",
                    ValueLabel = May.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
                new ChartEntry(June)
                {
                    Label = "Jun",
                    ValueLabel = June.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
                new ChartEntry(July)
                {
                    Label = " Jul",
                    ValueLabel = July.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
                new ChartEntry(August)
                {
                    Label = " Aug",
                    ValueLabel = August.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
                new ChartEntry(September)
                {
                    Label = " Sept",
                    ValueLabel = September.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
                new ChartEntry(October)
                {
                    Label = "Oct",
                    ValueLabel = October.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
                new ChartEntry(November)
                {
                    Label = "Nov",
                    ValueLabel = November.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
                new ChartEntry(December)
                {
                    Label = "Dec",
                    ValueLabel = December.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },

             };
            TimeSpan ts = new TimeSpan(0, 0, 0, 5);
            chartview.Chart = new LineChart { Entries = entries, IsAnimated = true, AnimationDuration = ts, AnimationProgress = 5, LabelTextSize = 30 };
          
        }
        async Task goalComplexity(int goalId)
        {
            numTask = 0;
            duration = 0;
            numSubtask = 0;

            // get the goal having goalId
            var goal = await dataGoal.GetGoalAsync(goalId);
            // get all tasks having the goal id
            var tasks = await DataTask.GetTasksAsync(goal.Id);
            // assign to num task
            numTask = tasks.Count();
            // passing a value to duration
            TimeSpan goalDuration = goal.End - goal.Start;
            duration = (int)goalDuration.TotalDays;
           
            //passing a value to number of tasks
            int subtasks = 0;
            //loop through all the tasks and get their subtasks
            foreach(var task in tasks)
            {
                // get the number of subtasks
                var totalsubtasks = await datasubtask.GetSubTasksAsync(task.Id);
                subtasks += totalsubtasks.Count();
            }
            numSubtask = subtasks;
            subtasks = 0;
            // checking if the goal is complex or not
            if (duration <= 90 && numTask <= 10 || duration <= 90 && numSubtask <= 10 )
            {
                Complexitylevel.Text = "Am here"; Complexitylevel.TextColor = Color.Gray;
            }
            else if(duration <= 90 && numTask >= 10 && numTask <= 15|| duration <=90 && numSubtask >= 10 && numSubtask <= 15)
            {
                 Complexitylevel.Text = "Am here too"; Complexitylevel.TextColor = Color.Gray;
            }
            else if (duration <= 90 && numTask >= 15 && numTask <= 20 || numSubtask >= 10 && numSubtask <= 20)
            {
                Complexitylevel.Text = "Medium High"; Complexitylevel.TextColor = Color.Gray;
            }
            else if (duration <= 90 && numTask >= 15 && numTask <= 25 || numSubtask >= 15 && numSubtask <= 25)
            {
                Complexitylevel.Text = "High"; Complexitylevel.TextColor = Color.Gray;
            }

            if (duration > 90 && duration <= 180 && numTask <= 20 || numSubtask <= 25)
            {
                Complexitylevel.Text = "Low"; Complexitylevel.TextColor = Color.LightCoral;
            }
            else if (duration > 90 && duration <= 180 && numTask >= 25 || numSubtask <= 35)
            {
                Complexitylevel.Text = "Medium"; Complexitylevel.TextColor = Color.LightCoral;
            }
            else if (duration > 90 && duration <= 180 && numTask >= 35 || numSubtask <= 45)
            {
                Complexitylevel.Text = "Medium High"; Complexitylevel.TextColor = Color.LightCoral;
            }
            else if (duration > 90 && duration <= 180 && numTask >= 45 || numSubtask > 55)
            {
                Complexitylevel.Text = "High"; Complexitylevel.TextColor = Color.LightCoral;
            }


            if (duration > 180 && duration <= 270 && numTask <= 55 || numSubtask <= 65)
            {
                Complexitylevel.Text = "Low"; Complexitylevel.TextColor = Color.Orange;
            }
            else if (duration > 180 && duration <= 270 && numTask >= 65 || numSubtask <= 75)
            {
                Complexitylevel.Text = "Medium"; Complexitylevel.TextColor = Color.Orange;
            }
            else if (duration > 180 && duration <= 270 && numTask >= 75 || numSubtask <= 85)
            {
                Complexitylevel.Text = "Medium High"; Complexitylevel.TextColor = Color.Orange;
            }
            else if (duration > 180 && duration <= 270 && numTask >= 85 || numSubtask <= 95)
            {
                Complexitylevel.Text = "High"; Complexitylevel.TextColor = Color.Orange;
            }


            if (duration > 270 && numTask <= 85 || numSubtask <= 95)
            {
                Complexitylevel.Text = "Low"; Complexitylevel.TextColor = Color.OrangeRed;
            }
            else if (duration > 270 && numTask >= 95 || numSubtask <= 100)
            {
                Complexitylevel.Text = "High"; Complexitylevel.TextColor = Color.OrangeRed;
            }

            TimeSpan ts = new TimeSpan(0, 0, 0, 5);
            List<ChartEntry> Pieentries = new List<ChartEntry>
             {
               new ChartEntry(numTask)
                {
                    Label = "Number of tasks",
                    ValueLabel = numTask.ToString(),
                    Color = SKColor.Parse("#012a4a")
                },
                new ChartEntry(duration)
                {
                    Label = "Number of days",
                    ValueLabel = duration.ToString(),
                    Color = SKColor.Parse("#ff8600"),
                },
                new ChartEntry(numSubtask)
                {
                    Label = "Number of subtasks",
                    ValueLabel = numSubtask.ToString(),
                    Color = SKColor.Parse("#2a6f97")
                },
             };
            Piechartview.Chart = new BarChart { Entries = Pieentries, IsAnimated = true, AnimationDuration = ts, AnimationProgress = 5, LabelTextSize = 30 };
        }
    }
}