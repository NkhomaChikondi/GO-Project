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
        private int dividedTasks = 0;
        private int dividedsubTasks = 0;
        private double progress = 0.0;

        private static int completedtasks = 0;
        private static int UncompletedTasks = 0;
        private static int completedSubtasks = 0;
        private static int UncompletedSubtasks = 0;

        int completedTaskscount = 0;
        int uncompletedTaskscount = 0;
        int completedSubtaskscount = 0;
        int uncompletedSubtaskscount = 0;

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
            if (Weektasks.Count() > 0)
            {
                // get the number of all completed tasks
                var ct = Weektasks.Where(T => T.IsCompleted).ToList();
                if(ct.Count> 0)
                {
                    completedtasks = ct.Count();
                    completedTaskscount = completedtasks;
                }
                
                var ut = Weektasks.Where(t => !t.IsCompleted).ToList();
                if (ut.Count > 0)
                {
                    UncompletedTasks = ut.Count();
                    uncompletedTaskscount = UncompletedTasks;
                }
               
                foreach (var task in Weektasks)
                {
                    // get subtasks in the task
                    var subtasks = await dataSubtask.GetSubTasksAsync(task.Id);
                    if (subtasks.Count() > 0)
                    {
                        totalSubtaks += subtasks.Count();
                        dividedsubTasks = 514 / subtasks.Count();
                        foreach (var subtask in subtasks)
                        {
                            //get completed subtasks
                            if (subtask.IsCompleted)
                                completedSubtasks += 1;
                            else if (!task.IsCompleted)
                                UncompletedSubtasks += 1;
                        }
                    }

                }
                //assign new values to the completed and uncompleted tasks and subtasks
                dividedTasks = 514 / Weektasks.Count();
                completedtasks = completedtasks * dividedTasks;
                UncompletedTasks = UncompletedTasks * dividedTasks;

                completedSubtaskscount = completedSubtasks;
                uncompletedSubtaskscount = UncompletedSubtasks;
                completedSubtasks = completedSubtasks * dividedsubTasks;
                UncompletedSubtasks = UncompletedSubtasks * dividedsubTasks;
            }

            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                await wvm.CalculateTotalWeekPercentage(Week);                           
            }
            wknum.Text = Week.WeekNumber.ToString();
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
                ValueLabel = completedTaskscount.ToString(),
                Color = SKColor.Parse("#012a4a")
            },
            new ChartEntry(UncompletedTasks)
            {
                Label = "UT",
                ValueLabel = uncompletedTaskscount.ToString(),
                Color = SKColor.Parse("#ff8600")
            },
            new ChartEntry(completedSubtasks)
            {
                Label = "CS",
                ValueLabel = completedSubtaskscount.ToString(),
                Color = SKColor.Parse("#89c2d9")
            },
            new ChartEntry(UncompletedSubtasks)
            {
                Label = "US",
                ValueLabel = uncompletedSubtaskscount.ToString(),
                Color = SKColor.Parse("#ffb5a7")
            },
        };


            TimeSpan ts = new TimeSpan(0, 0, 0, 5);
            chartView.Chart = new BarChart { Entries = entries, IsAnimated = true, AnimationDuration = ts, AnimationProgress = 5, LabelTextSize = 30 };
            await perfomanceRating(Week);

            completedSubtasks = 0;
            completedtasks = 0;
            UncompletedSubtasks = 0;
            UncompletedTasks = 0;
            uncompletedSubtaskscount = 0;
            completedSubtaskscount = 0;
            completedTaskscount = 0;
            uncompletedTaskscount = 0;
            dividedsubTasks = 0;
            dividedTasks = 0;
        }
        async Task perfomanceRating(Week week)
        {
            //get all tasks in the week
            var weekTasks = await DataTask.GetTasksAsync(week.GoalId, week.Id);
            //get completed tasks
            var completedTasks = weekTasks.Where(d => d.IsCompleted).ToList();
            // get tasks whose pending percentages are more than 0
            var pendingPercentageTasks = weekTasks.Where(p => p.PendingPercentage > 0 && !p.IsCompleted).ToList();
            if(weekTasks.Count() <= 4 && completedTasks.Count() == 0)
            {
                progress = 0;
                Weeklyprogresbar.Progress = progress;
            }
            else if(weekTasks.Count()> 0 && weekTasks.Count() <= 4)
            {
                if(completedTasks.Count > 0)
                {
                    progress = 1.25;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
                else if(pendingPercentageTasks.Count> 0)
                {
                    progress = 1.25;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
            }
            if(weekTasks.Count() >= 5 && weekTasks.Count() < 7) 
            {
                if(completedTasks.Count() > 0 && completedTasks.Count() >= (2 * (completedTasks.Count) / 4))
                {
                    progress = 5;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
                else if(completedTasks.Count > 0)
                {
                    progress = 2.5;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
                // here the subtasks will be calculated
                else if (pendingPercentageTasks.Count > 0)
                {
                    progress = 2.5;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
                else 
                {
                    progress = 0;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
            }
            if(weekTasks.Count() >= 7)
            {
                if (completedTasks.Count() > 0 && completedTasks.Count() >= (3 * (completedTasks.Count) / 4))
                {
                    progress = 7.5;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
                else if (completedTasks.Count > 0)
                {
                    progress = 3.75;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
                // here the subtasks will be calculated
                else if (pendingPercentageTasks.Count > 0)
                {
                    progress = 3.75;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
                else
                {
                    progress = 0;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }

            }
            if(weekTasks.Count() >= 7)
            {
                if(weekTasks.Count() == completedTasks.Count())
                {
                    progress = 10;
                    Weeklyprogresbar.Progress = progress / 10;
                    progress = 0;
                }
            }
        }
    }
}