using GO.Models;
using GO.ViewModels.TaskInGoals;
using GO.Views.Goal;
using GO.Views.GoalTask;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.Goals
{
    [QueryProperty(nameof(CategoryId), nameof(CategoryId))]
    public class GoalViewModel : BaseViewmodel
    {
        // create a private property that will receive the incoming id 
        private int categoryId;
        private bool run;
        private string labeltext;
        public ObservableRangeCollection<Goal> goals { get; }
        public AsyncCommand<Goal> AddgoalCommand { get; }
        public AsyncCommand<Goal> GetgoalCommand { get; set; }
        public AsyncCommand<Goal> DeleteCommand { get; }
        public AsyncCommand<Goal> UpdateCommand { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<Goal> ItemSelectedCommand { get; }
        // get only those withna categoryId

        public int CategoryId
        {
            get
            { return categoryId; }
            set => categoryId = value;

        }

        public bool Run
        {
            get => run;
            set
            {
                run = value;
                OnPropertyChange();


            }
        }

        public string Labeltext
        {
            get => labeltext;
            set
            {
                labeltext = value;
                OnPropertyChange();

            }
        }

        public GoalViewModel()
        {

            goals = new ObservableRangeCollection<Goal>();
            AddgoalCommand = new AsyncCommand<Goal>(OnaddGoal);
            DeleteCommand = new AsyncCommand<Goal>(deleteGoal);
         //   UpdateCommand = new AsyncCommand<Goal>(OnUpdateGoal);
            RefreshCommand = new AsyncCommand(Refresh);
            ItemSelectedCommand = new AsyncCommand<Goal>(selectGoalItem);

        }

        async Task OnaddGoal(Object obj)
        {
            var route = $"{nameof(AddGoalview)}?{nameof(AddGoalViewModel.CategoryId)}={categoryId}";
            await Shell.Current.GoToAsync(route);


        }
        async Task selectGoalItem(Goal goal)
        {

            var route = $"{nameof(GoalTaskPage)}?{nameof(GoalTaskViewModel.GoalTaskId)}={goal.Id}";
            await Shell.Current.GoToAsync(route);
        }
        //async Task OnUpdateGoal(Goal goal)
        //{
        //    var route = $"{nameof(EditGoal)}?{nameof(EditGoalViewModel.GoalId)}={goal.Id}";
        //    await Shell.Current.GoToAsync(route);


        //}
        async Task getAllGoals()
        {
            // list down all categories in the database
            // check if the app is busy
            if (IsBusy)
                return;
            // otherwise
            try
            {
                IsBusy = true;
                var CategoryGoal = await datagoal.GetGoalsAsync(categoryId);
                goals.ReplaceRange(CategoryGoal);

            }
            catch (Exception ex)
            {
                // error message

                Debug.WriteLine($"Failed to add Category: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");

            }
            finally
            {
                IsBusy = false;
            }

        }
        async Task updateGoal(Goal goal)
        {

            if (goal == null)
                return;
            try
            {

                var updateGoal = new Goal
                {
                    Id = goal.Id,
                    Name = goal.Name,
                    Description = goal.Description
                };

                await datagoal.UpdateGoalAsync(updateGoal);
                await Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update Goal: {ex.Message}");
            }

        }
        async Task deleteGoal(Goal goal)
        {
            if (goal == null)
                return;
            await datagoal.DeleteGoalAsync(goal.Id);
            await Refresh();
        }
        public async Task Running(bool isRunning)
        {
            if (isRunning == true)
            {
                await Application.Current.MainPage.DisplayAlert("Alert!", "Completed!!", "OK");
            }

        }
        public async Task Running(double percentage)
        {

            Labeltext = $"{percentage}%";

        }
        public async Task Refresh()
        {

            // set "IsBusy" to true
            IsBusy = true;
            // make the refreshing process load for 2 seconds
          //  await Task.Delay(2000);
            // clear categories on the page
            goals.Clear();
            // get all categories
            var goal = await datagoal.GetGoalsAsync(categoryId);
            // retrieve the categories back
            goals.AddRange(goal);
            // set "isBusy" to false
            IsBusy = false;

        }
    }
}
