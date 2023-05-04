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
        private int categoryId;
        public IDataStore<Models.Category> dataCategory { get; }
        public IDataGoal<Models.Goal> dataGoal { get; }
        public GoalView()
        {
            InitializeComponent();
            dataCategory = DependencyService.Get<IDataStore<Models.Category>>();
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            BindingContext = new GoalViewModel();

        }
        // set progress bar color
      
        protected async override void OnAppearing()
        {
            
            base.OnAppearing();
            int.TryParse(CategoryId, out var result);
            // get all goals having the category id
            var goals = await dataGoal.GetGoalsAsync(result);
            categoryId = result;
            if (goals.Count( ) == 0)
            {
                stackGoallist.IsVisible = false;
                topRow.IsVisible = false;
                StackGoalblank.IsVisible = true;               
            }
            else 
            {
                stackGoallist.IsVisible = true;
                StackGoalblank.IsVisible = false;
                topRow.IsVisible = true;
            }
            btnall.BackgroundColor = Color.LightGray;
            if (BindingContext is GoalViewModel cvm)
            {
                cvm.CategoryId = result;
                await cvm.Refresh();
            }
        }
        private async void btnnotstarted_Clicked(object sender, EventArgs e)
        {
            btnnotstarted.BackgroundColor = Color.LightGray;
            btnall.BackgroundColor = Color.Transparent;
            btncompleted.BackgroundColor = Color.Transparent;
            btninprogress.BackgroundColor = Color.Transparent;
            duesoon.BackgroundColor = Color.Transparent;
            var goals = await dataGoal.GetGoalsAsync(categoryId);
            // get all subtasks not started
            var notstartedGoals = goals.Where(s => s.Status == "Not Started").ToList();
            if (notstartedGoals.Count() != 0)
            {

                nogoals.Text = "";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.NotstartedGoals();
                }                
            }
            else
            {
                nogoals.Text = " They are no Started goals!";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.NotstartedGoals();
                }
            }
        }

        private async void btnall_Clicked(object sender, EventArgs e)
        {
            btnnotstarted.BackgroundColor = Color.Transparent;
            btnall.BackgroundColor = Color.LightGray;
            btncompleted.BackgroundColor = Color.Transparent;
            btninprogress.BackgroundColor = Color.Transparent;
            duesoon.BackgroundColor = Color.Transparent;
            expired.BackgroundColor = Color.Transparent;
            if (BindingContext is GoalViewModel bvm)
            {
                await bvm.AllGoals();
            }
        }

        private async void btninprogress_Clicked(object sender, EventArgs e)
        {
            btnnotstarted.BackgroundColor = Color.Transparent;
            btnall.BackgroundColor = Color.Transparent;
            btncompleted.BackgroundColor = Color.Transparent;
            btninprogress.BackgroundColor = Color.LightGray;
            duesoon.BackgroundColor = Color.Transparent;
            expired.BackgroundColor = Color.Transparent;
            var goals = await dataGoal.GetGoalsAsync(categoryId);
            // get all subtasks not started
            var inprogressGoals = goals.Where(s => s.Status == "InProgress").ToList();
            if (inprogressGoals.Count() != 0)
            {
                nogoals.Text = "";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.InprogressGoals();
                }               
            }
            else
            {
                nogoals.Text = " They are no goals in progress!";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.InprogressGoals();
                }
            }
        }

        private async void btncompleted_Clicked(object sender, EventArgs e)
        {
            btnnotstarted.BackgroundColor = Color.Transparent;
            btnall.BackgroundColor = Color.Transparent;
            btncompleted.BackgroundColor = Color.LightGray;
            btninprogress.BackgroundColor = Color.Transparent;
            duesoon.BackgroundColor = Color.Transparent;
            expired.BackgroundColor = Color.Transparent;
            var goals = await dataGoal.GetGoalsAsync(categoryId);
            // get all subtasks not started
            var completedGoals = goals.Where(s => s.Percentage==100).ToList();
            if (completedGoals.Count() != 0)
            {
                nogoals.Text = "";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.CompletedGoals();
                }
            }
            else
            {              
                nogoals.Text = " They are no goals that are Completed!";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.CompletedGoals();
                }
            }
        }

        private async void duesoon_Clicked(object sender, EventArgs e)
        {
            btnnotstarted.BackgroundColor = Color.Transparent;
            btnall.BackgroundColor = Color.Transparent;
            btncompleted.BackgroundColor = Color.Transparent;
            btninprogress.BackgroundColor = Color.Transparent;
            duesoon.BackgroundColor = Color.LightGray;
            expired.BackgroundColor = Color.Transparent;

            var goals = await dataGoal.GetGoalsAsync(categoryId);
            var Date10 = DateTime.Today.AddDays(10);
            var duesoongoals = goals.Where(g => g.End <= Date10 && g.Status != "Expired").ToList();
            if (duesoongoals.Count() != 0)
            {
                nogoals.Text = "";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.DuesoonGoals();
                }              
            }
            else
            {
                nogoals.Text = " They are no goals that are Due soon!";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.DuesoonGoals();
                }
            }
        }

        private async void expired_Clicked(object sender, EventArgs e)
        {
            btnnotstarted.BackgroundColor = Color.Transparent;
            btnall.BackgroundColor = Color.Transparent;
            btncompleted.BackgroundColor = Color.Transparent;
            btninprogress.BackgroundColor = Color.Transparent;
            duesoon.BackgroundColor = Color.Transparent;
            expired.BackgroundColor = Color.LightGray;
            var goals = await dataGoal.GetGoalsAsync(categoryId);
            // get all subtasks not started
            var expiredGoals = goals.Where(s => s.Status == "Expired").ToList();
            if (expiredGoals.Count() != 0)
            {
                nogoals.Text = "";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.ExpiredGoals();
                }               
            }
            else
            {
                nogoals.Text = " They are no goals that have Expired!";
                if (BindingContext is GoalViewModel bvm)
                {
                    await bvm.ExpiredGoals();
                }
            }
        }
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.DisplayAlert("INFO", "* All goals will be listed on this page.\n \n * Scroll through the horizontal tab to filter the goal list according to your preference." +
                "\n \n * Each listed goal will have \n  1. Name \n  2. A progress bar that shows the progress of the goal in percentage. \n  3. Status. \n  4. Due date \n\n *" +
                " A goal will only be completed through the tasks that are created in it. \n * Tap on the goal to go to task page. \n\n * Long press on a goal to edit or delete.", "OK");              
        }

        //private async void ImageButton_Clicked(object sender, EventArgs e)
        //{
        //    ImageButton imageButton = (ImageButton)sender;
        //    var goal = (Models.Goal)imageButton.BindingContext;
        //    var goalid = goal.Id;
        //    var action = await DisplayActionSheet("MENU", "Cancel","", "View Stats");
        //    if(action == "View Stats")
        //    {
        //        var route = $"{nameof(GoalStats)}?GoalID={goal.Id}"; 
        //        await Shell.Current.GoToAsync(route);
        //    }

        //}
    }
}