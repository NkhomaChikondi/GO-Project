using GO.Models;
using GO.ViewModels.TaskInGoals;
using GO.Views.Goal;
using GO.Views.GoalTask;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        public GoalViewModel()
        {
            goals = new ObservableRangeCollection<Goal>();
            AddgoalCommand = new AsyncCommand<Goal>(OnaddGoal);
            DeleteCommand = new AsyncCommand<Goal>(deleteGoal);
            UpdateCommand = new AsyncCommand<Goal>(OnUpdateGoal);
            RefreshCommand = new AsyncCommand(Refresh);
            ItemSelectedCommand = new AsyncCommand<Goal>(selectGoalItem);
        }
        async Task OnaddGoal(Object obj)
        {
            var route = $"{nameof(AddGoalview)}?{nameof(AddGoalViewModel.CategoryId)}={categoryId}";
            await Shell.Current.GoToAsync(route)  ;   
        }
        async Task selectGoalItem(Goal goal)        
        {
            // get the tasks having the goal id
            var tasks = await dataTask.GetTasksAsync(goal.Id);
            // check if the HAS WEEK in goal is == true
            if (goal.HasWeek && !goal.Noweek)
            {               
               
                    var route = $"{nameof(WeeklyTask)}?goalId={goal.Id}";
                    await Shell.Current.GoToAsync(route);
                           
            }
            else if(!goal.HasWeek && goal.Noweek)
            {
                //if (tasks.Count().Equals(0))
                //{
                //    var route1 = $"{nameof(BlankTaskView)}?goalId={goal.Id}";
                //    await Shell.Current.GoToAsync(route1);
                //}
               
                    var route = $"{nameof(GoalTaskPage)}?goalId={goal.Id}";
                    await Shell.Current.GoToAsync(route);
                            
            }           
        }
        async Task addIdToWeektaskviewmodel(int id)
        {
            var route = $"{nameof(WeeklyTask)}?{nameof(WeeklyTaskViewModel)}={id}";
            await Shell.Current.GoToAsync(route);
        }
        async Task OnUpdateGoal(Goal goal)
        {
            var route = $"{nameof(UpdateGoalPage)}?goalId={goal.Id}" ;
            await Shell.Current.GoToAsync(route);
        }
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
    
        async Task deleteGoal(Goal goal)
        {
            if (goal == null)
                return;
          var ans=  await Application.Current.MainPage.DisplayAlert("Delete Goal", "All Tasks in this Goal will be deleted. Continue?", "Yes", "No");
            if (ans)
            {
                await datagoal.DeleteGoalAsync(goal.Id);
                // get all tasks having the goal id
                var tasks = await dataTask.GetTasksAsync(goal.Id);
                // loop through the tasks
                foreach (var task in tasks)
                {
                    // delete the task
                    await dataTask.DeleteTaskAsync(task.Id);
                    //get subtasks having the task id
                    var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
                    // loop through the subtasks
                    foreach (var subtask in subtasks)
                    {
                        await dataSubTask.DeleteSubTaskAsync(subtask.Id);
                    }
                }
                await Refresh();
            }
            else if (!ans)
                return;
           
        }
        public async Task calculateGoalPercentage()
        {
            double TaskPercentage = 0;
            double subtaskpercentage = 0;
            double goalRoundedPercentage = 0;
            // get all goals having the category id
            var goals = await datagoal.GetGoalsAsync(categoryId);
            //// get all goes that has weeks
            //var WeekGoals = goals.Where(g => g.HasWeek).ToList();
            //// get all goals that has no week
            //var NoweekGoals = goals.Where(g => g.Noweek);
     
            // loop through the goals and get their tasks
            foreach (var goal in goals)
            {
                if(goal.HasWeek)
                {
                    double Accumulated = 0;
                    //get the goal's tasks 
                    var tasks = await dataTask.GetTasksAsync(goal.Id);
                    // get week
                    var getweeks = await dataWeek.GetWeeksAsync(goal.Id);
                    {
                        // loop through the weeks
                        foreach (var week in getweeks)
                        {
                            // get all tasks having the weeks id
                            var alltasks = tasks.Where(t => t.WeekId == week.Id).ToList();
                            // loop through the tasks
                            foreach(var task in alltasks)
                            {
                                //check if task is completed
                                if (task.IsCompleted)
                                {
                                    TaskPercentage += task.Percentage;

                                }
                                else if (!task.IsCompleted)
                                {
                                    // check task has subtasks
                                    //get all subtasks having the tasks Id
                                    var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);

                                    if (subtasks.Count() > 0)
                                    {
                                        // get the task's pending percentage
                                        subtaskpercentage += task.PendingPercentage;
                                    }
                                }
                            }
                               
                            
                            Accumulated += TaskPercentage + subtaskpercentage;
                            week.AccumulatedPercentage = Math.Round(Accumulated, MidpointRounding.AwayFromZero);
                            await dataWeek.UpdateWeekAsync(week);
                        }
                        goal.Percentage = Math.Round(Accumulated, MidpointRounding.AwayFromZero);
                        goal.Progress = goal.Percentage / goal.ExpectedPercentage;
                        await datagoal.UpdateGoalAsync(goal);
                        //reset the below variables
                        TaskPercentage = 0;
                        subtaskpercentage = 0;
                        Accumulated = 0;
                        // set status
                        await setStatus();
                    }
                }
                else if(goal.Noweek)
                {
                    //get the goal's tasks 
                    var tasks = await dataTask.GetTasksAsync(goal.Id);
                    // loop through the tasks to filter completed from uncompleted tasks
                    foreach (var task in tasks)
                    {
                        //check if task is completed
                        if (task.IsCompleted)
                        {
                            TaskPercentage += task.Percentage;

                        }
                        else if (!task.IsCompleted)
                        {
                            // check task has subtasks
                            //get all subtasks having the tasks Id
                            var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);

                            if (subtasks.Count() > 0)
                            {
                                // get the task's pending percentage
                                subtaskpercentage += task.PendingPercentage;
                            }
                        }
                    }
                    goalRoundedPercentage = TaskPercentage + subtaskpercentage;
                    goal.Percentage = Math.Round(goalRoundedPercentage, MidpointRounding.AwayFromZero);
                    goal.Progress = goal.Percentage / goal.ExpectedPercentage;
                    await datagoal.UpdateGoalAsync(goal);
                    //reset the below variables
                    TaskPercentage = 0;
                    subtaskpercentage = 0;
                    // set status
                    await setStatus();
                }
                
            }
            return; 
        }
        async Task setStatus()
        {
            // get all goals having the category id
            var goals = await datagoal.GetGoalsAsync(categoryId);
            // loop through them
            foreach (var goal in goals)
            {
                if (goal.Percentage == 0)
                    goal.Status = "Not Started";
                else if (goal.Percentage < goal.ExpectedPercentage)
                    goal.Status = "In Progress";
                else if (goal.Percentage == goal.ExpectedPercentage)
                    goal.Status = "Completed";
                else if (DateTime.Now > goal.End)
                    goal.Status = " Expired";
                await datagoal.UpdateGoalAsync(goal);
            }
        }

        public async Task Refresh()
        {
            // set "IsBusy" to true
            IsBusy = true;
            // clear categories on the page
            goals.Clear();
            await calculateGoalPercentage();
            // get all categories
            var goal = await datagoal.GetGoalsAsync(categoryId);
            // retrieve the categories back
            goals.AddRange(goal);
           // if (goals.Count() == 0)
           // {
           //     var route1 = $"{nameof(BlankGoalView)}?{nameof(GoalViewModel.CategoryId)}={CategoryId}";
           //     await Shell.Current.GoToAsync(route1);
           // }
           //// call set status

           // set "isBusy" to false
           IsBusy = false;

        }
    }
}
