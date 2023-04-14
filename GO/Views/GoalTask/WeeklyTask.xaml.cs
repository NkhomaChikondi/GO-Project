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
       
        // properties for Dows Id
        private Week GetWeek;
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
       

      
        public WeeklyTask()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataWeek = DependencyService.Get<IDataWeek<Models.Week>>();
            DataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            dataDow = DependencyService.Get<IDataDow<Models.DOW>>();
            dataSubtask = DependencyService.Get<IDataSubtask<Models.Subtask>>();
            BindingContext = new WeeklyTaskViewModel();
            TimeSpan ts = new TimeSpan(0, 0, 0, 5);
            chartView.Chart = new BarChart { Entries = entries, IsAnimated=true,AnimationDuration=ts,  AnimationProgress= 5,LabelTextSize = 30 };

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(weekId, out var result);
            // get the week throgh the id
            var Week = await dataWeek.GetWeekAsync(result);
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
        }
        private List<ChartEntry> entries = new List<ChartEntry>
        {
           new ChartEntry(514)
            {
                Label = "CT",
                ValueLabel = "5",
                Color = SKColor.Parse("#012a4a")
            },
            new ChartEntry(248)
            {
                Label = "UT",
                ValueLabel = "648",
                Color = SKColor.Parse("#ff8600")
            },
            new ChartEntry(248)
            {
                Label = "CS",
                ValueLabel = "648",
                Color = SKColor.Parse("#89c2d9")
            },
            new ChartEntry(248)
            {
                Label = "US",
                ValueLabel = "648",
                Color = SKColor.Parse("#ffb5a7")
            },      
        };
    }
}