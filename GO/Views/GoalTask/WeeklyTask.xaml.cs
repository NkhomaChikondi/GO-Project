using GO.Models;
using GO.Services;
using GO.ViewModels.TaskInGoals;
using Microcharts;
using SkiaSharp;
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
        private int totalSubtaks = 0;
   
        private static int completedtasks = 0;
        private static int UncompletedTasks = 0;
        private static int completedSubtasks = 0;
        private static int UncompletedSubtasks = 0;        
       

        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataWeek<Week> dataWeek { get; }
        public IDataDow<DOW> dataDow { get; }
        public IDataSubtask<Models.Subtask> dataSubtask { get; }
       

      
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
            // get the week throgh the id
            var Week = await dataWeek.GetWeekAsync(result);
            var Weektasks = await DataTask.GetTasksAsync(Week.GoalId, Week.Id);
            // divide tasks count with 514
            var dividedTasks = 0;
            var dividedsubTasks = 0;
            // loop trhu the tasks to get subtasks
            foreach (var task in Weektasks)
            {
                //get completed tasks
                if (task.IsCompleted)
                    completedtasks += 1;
                else if (!task.IsCompleted)
                    UncompletedTasks += 1;
                //get subtasks
                var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);
                if (subtasks.Count() > 0)
                    dividedsubTasks = 514 / subtasks.Count();
                foreach(var subtask in subtasks)
                {
                    //get completed subtasks
                    if (subtask.IsCompleted)
                        completedtasks += 1;
                    else if (!task.IsCompleted)
                        UncompletedSubtasks += 1;
                }                
                totalSubtaks += subtasks.Count();            
                
            }
            if (Weektasks.Count() > 0)
            {
                dividedTasks = 514 / Weektasks.Count();
                completedtasks = completedtasks * dividedTasks;
                UncompletedTasks = UncompletedTasks * dividedTasks;
            }

            if (completedSubtasks >0 || UncompletedSubtasks>0)
            {               
                completedSubtasks = completedtasks * dividedTasks;
                UncompletedSubtasks = UncompletedTasks * dividedTasks;
            }


            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                await wvm.CalculateTotalWeekPercentage(Week);                           
            }
            progressring.Progress = Week.Progress;
            wkpercentage.Text = Week.AccumulatedPercentage.ToString();
            status.Text = Week.Status;           
            target.Text = Week.TargetPercentage.ToString();
            startdate.Text = Week.StartDate.ToShortDateString();
            enddate.Text = Week.EndDate.ToShortDateString();
            alltasks.Text = Weektasks.Count().ToString();
            allsubtasks.Text = totalSubtaks.ToString();
            
            List<ChartEntry> entries = new List<ChartEntry>
            {
           new ChartEntry(completedtasks)
            {
                Label = "CT",
                ValueLabel = "5",
                Color = SKColor.Parse("#012a4a")
            },
            new ChartEntry(UncompletedTasks)
            {
                Label = "UT",
                ValueLabel = "648",
                Color = SKColor.Parse("#ff8600")
            },
            new ChartEntry(completedSubtasks)
            {
                Label = "CS",
                ValueLabel = "648",
                Color = SKColor.Parse("#89c2d9")
            },
            new ChartEntry(UncompletedSubtasks)
            {
                Label = "US",
                ValueLabel = "648",
                Color = SKColor.Parse("#ffb5a7")
            },
        };


            TimeSpan ts = new TimeSpan(0, 0, 0, 5);
            chartView.Chart = new BarChart { Entries = entries, IsAnimated = true, AnimationDuration = ts, AnimationProgress = 5, LabelTextSize = 30 };
            completedSubtasks = 0;
            completedtasks = 0;
            UncompletedSubtasks = 0;
            UncompletedTasks = 0;
        }
       
        
    }
}