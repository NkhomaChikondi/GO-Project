using GO.Services;
using GO.ViewModels.Goals;
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
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]
    public partial class GoalView : ContentPage
    {
        public string CategoryId { get; set; }
        public IDataStore<Models.Category> dataCategory { get; }
        public IDataGoal<Models.Goal> dataGoal { get; }
        public GoalView()
        {
            InitializeComponent();
            dataCategory = DependencyService.Get<IDataStore<Models.Category>>();
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            BindingContext = new GoalViewModel();

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(CategoryId, out var result);
            // get all goals having the category id
            var goals = await dataGoal.GetGoalsAsync(result);
            if(goals.Count( ) == 0)
            {
                stackGoallist.IsVisible = false;
                StackGoalblank.IsVisible = true;
            }
            else 
            {
                stackGoallist.IsVisible = true;
                StackGoalblank.IsVisible = false;
            }
            if (BindingContext is GoalViewModel cvm)
            {
                cvm.CategoryId = result;
                await cvm.Refresh();
            }
        }
    }
}