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
        private List<ChartEntry> entries = new List<ChartEntry>
        {
           new ChartEntry(212)
            {
                Label = "Sunday",
                ValueLabel = "112",
                Color = SKColor.Parse("#012a4a")
            },
            new ChartEntry(248)
            {
                Label = "Monday",
                ValueLabel = "648",
                Color = SKColor.Parse("#013a63")
            },
            new ChartEntry(248)
            {
                Label = "Tuesday",
                ValueLabel = "648",
                Color = SKColor.Parse("#2a6f97")
            },
            new ChartEntry(248)
            {
                Label = "wednesday",
                ValueLabel = "648",
                Color = SKColor.Parse("#ff8600")
            },
            new ChartEntry(128)
            {
                Label = "Thursday",
                ValueLabel = "428",
                Color = SKColor.Parse("#468faf")
            },
            new ChartEntry(514)
            {
                Label = "Friday",
                ValueLabel = "214",
                Color = SKColor.Parse("#89c2d9")
            },
            new ChartEntry(248)
            {
                Label = "Saturday",
                ValueLabel = "648",
                Color = SKColor.Parse("#a9d6e5")
                
            }
           


        };
    }
}