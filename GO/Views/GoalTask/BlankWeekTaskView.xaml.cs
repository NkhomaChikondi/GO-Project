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
    [QueryProperty(nameof(goalId), nameof(goalId))]
    public partial class BlankWeekTaskView : ContentPage
    {
        public string goalId { get; set; }
        public IDataGoal<Models.Goal> datagoal { get; }

        public BlankWeekTaskView()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            BindingContext = new WeeklyTaskViewModel();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(goalId, out var result);

            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(result);


            if (BindingContext is WeeklyTaskViewModel TVM)
            {
                TVM.GoalId = goal.Id;


            }
        }
    }
}