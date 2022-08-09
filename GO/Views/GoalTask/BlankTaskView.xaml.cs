using GO.Services;
using GO.ViewModels.TaskInGoals;
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
    [QueryProperty(nameof(goalId), nameof(goalId))]
    public partial class BlankTaskView : ContentPage
    {
        public string goalId { get; set; }
        public IDataGoal<Models.Goal> datagoal { get; }
       
        public BlankTaskView()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();            
            BindingContext = new GoalTaskViewModel();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(goalId, out var result);
          
            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(result);
            

            if (BindingContext is GoalTaskViewModel TVM)
            {
                TVM.GoalId = goal.Id;
             

            }
        }
    }
}