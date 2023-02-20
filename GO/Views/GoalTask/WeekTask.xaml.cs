﻿using GO.Models;
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
    [QueryProperty(nameof(weekId), nameof(weekId))]
    public partial class WeekTask : ContentPage
    {
        public string weekId { get; set; }
        private int goalId;
        public IDataGoal<Models.Goal> datagoal { get; }
        public IDataTask<Models.GoalTask> DataTask { get; }
        public IDataSubtask<Subtask> datasubtask { get; }
        public IDataWeek<Models.Week> dataWeek { get; }
        public IDataDow<DOW> dataDow { get; }
        public WeekTask()
        {
            InitializeComponent();
            datagoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            dataWeek = DependencyService.Get<IDataWeek<Models.Week>>();
            DataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            datasubtask = DependencyService.Get<IDataSubtask<Subtask>>();
            dataDow = DependencyService.Get<IDataDow<DOW>>();
            BindingContext = new WeeklyTaskViewModel();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            int.TryParse(weekId, out var result);
            // get the  week having the week id
            var week = await dataWeek.GetWeekAsync(result);
            // get the goal having the goalid
            var goal = await datagoal.GetGoalAsync(week.GoalId);
            // pass the goal id to the private field
            goalId = goal.Id;
            if (BindingContext is WeeklyTaskViewModel wvm)
            {
                wvm.GoalId = goal.Id;
                //wvm.DowId = selectedDow.DOWId;
                wvm.WeekId = result;
                await wvm.Refresh();

            }
        }
            private async void switch_Toggled(object sender, ToggledEventArgs e)
        {
            Switch @switch = (Switch)sender;
            var task = (Models.GoalTask)@switch.BindingContext;
            var taskid = task.Id;
            // get the task having the same id as taskId
            var taskdb = await DataTask.GetTaskAsync(taskid);

            if (task.IsCompleted)
            {
                // check if the incoming object 
                if (taskdb.IsCompleted)
                    return;
                else
                {
                    if (BindingContext is WeeklyTaskViewModel viewModel)
                    {
                        await viewModel.CompleteTask(taskid, task.IsCompleted);
                    }
                }
            }
            else if (!task.IsCompleted)
            {
                // check if the incoming object 
                if (!taskdb.IsCompleted)
                    return;
                if (BindingContext is WeeklyTaskViewModel viewModel)
                    await viewModel.UncompleteTask(taskid, task.IsCompleted);
            }
            return;

        }

        //private async void bdall_Clicked(object sender, EventArgs e)
        //{
        //    bdnotstarted.BackgroundColor = Color.Transparent;
        //    bdall.BackgroundColor = Color.LightGray;
        //    bdcompleted.BackgroundColor = Color.Transparent;
        //    bdinprogress.BackgroundColor = Color.Transparent;
        //    bdwithsubtasks.BackgroundColor = Color.Transparent;
        //    if (BindingContext is WeeklyTaskViewModel bvm)
        //    {
        //        await bvm.AllGoals();
        //    }
        //}

        //private async void bdnotstarted_Clicked(object sender, EventArgs e)
        //{
        //    bdnotstarted.BackgroundColor = Color.LightGray;
        //    bdall.BackgroundColor = Color.Transparent;
        //    bdcompleted.BackgroundColor = Color.Transparent;
        //    bdinprogress.BackgroundColor = Color.Transparent;
        //    bdwithsubtasks.BackgroundColor = Color.Transparent;
        //    var tasks = await DataTask.GetTasksAsync(goalId, Idweek);
        //    var dayTasks = tasks.Where(T => T.DowId == selecteddowId).ToList();
        //    // get all subtasks not started
        //    var notstartedTasks = dayTasks.Where(s => s.Status == "Not Started").ToList();
        //    if (notstartedTasks.Count() == 0)
        //    {
        //        notasks.Text = " They are no tasks that have not Started!";
        //        if (BindingContext is WeeklyTaskViewModel bvm)
        //        {
        //            await bvm.NotstartedGoals();
        //        }
        //    }
        //    else
        //    {
        //        notasks.Text = "";
        //        if (BindingContext is WeeklyTaskViewModel bvm)
        //        {
        //            await bvm.NotstartedGoals();
        //        }
        //    }

        //}

        //private async void bdinprogress_Clicked(object sender, EventArgs e)
        //{
        //    bdnotstarted.BackgroundColor = Color.Transparent;
        //    bdall.BackgroundColor = Color.Transparent;
        //    bdcompleted.BackgroundColor = Color.Transparent;
        //    bdinprogress.BackgroundColor = Color.LightGray;
        //    bdwithsubtasks.BackgroundColor = Color.Transparent;
        //    var tasks = await DataTask.GetTasksAsync(goalId, Idweek);
        //    var dayTasks = tasks.Where(T => T.DowId == selecteddowId).ToList();
        //    // get all subtasks not started
        //    var progressTasks = dayTasks.Where(s => s.Status == "In Progress").ToList();
        //    if (progressTasks.Count() == 0)
        //    {
        //        notasks.Text = " They are no tasks In Progress!";
        //        if (BindingContext is WeeklyTaskViewModel bvm)
        //        {
        //            await bvm.InprogressGoals();
        //        }
        //    }
        //    else
        //    {
        //        notasks.Text = "";
        //        if (BindingContext is WeeklyTaskViewModel bvm)
        //        {
        //            await bvm.InprogressGoals();
        //        }
        //    }
        //}

        //private async void bdcompleted_Clicked(object sender, EventArgs e)
        //{

        //    bdnotstarted.BackgroundColor = Color.Transparent;
        //    bdall.BackgroundColor = Color.Transparent;
        //    bdcompleted.BackgroundColor = Color.LightGray;
        //    bdinprogress.BackgroundColor = Color.Transparent;
        //    bdwithsubtasks.BackgroundColor = Color.Transparent;
        //    var tasks = await DataTask.GetTasksAsync(goalId, Idweek);
        //    var dayTasks = tasks.Where(T => T.DowId == selecteddowId).ToList();
        //    // get all subtasks not started
        //    var completedTasks = dayTasks.Where(s => s.Status == "Completed").ToList();
        //    if (completedTasks.Count() == 0)
        //    {
        //        notasks.Text = " They are no completed tasks!";
        //        if (BindingContext is WeeklyTaskViewModel bvm)
        //        {
        //            await bvm.CompletedGoals();
        //        }
        //    }
        //    else
        //    {
        //        notasks.Text = "";
        //        if (BindingContext is WeeklyTaskViewModel bvm)
        //        {
        //            await bvm.CompletedGoals();
        //        }
        //    }
        //}

        //private async void bdwithsubtasks_Clicked(object sender, EventArgs e)
        //{
        //    bdnotstarted.BackgroundColor = Color.Transparent;
        //    bdall.BackgroundColor = Color.Transparent;
        //    bdcompleted.BackgroundColor = Color.Transparent;
        //    bdinprogress.BackgroundColor = Color.Transparent;
        //    bdwithsubtasks.BackgroundColor = Color.LightGray;
        //    var tasks = await DataTask.GetTasksAsync(goalId, Idweek);
        //    var dayTasks = tasks.Where(T => T.DowId == selecteddowId).ToList();
        //    // get all subtasks not started
        //    List<Models.GoalTask> tasklist = new List<Models.GoalTask>();
        //    //loop through the tasks
        //    foreach (var Task in dayTasks)
        //    {
        //        // get tasks that have subtasks
        //        var subtasks = await datasubtask.GetSubTasksAsync(Task.Id);
        //        if (subtasks.Count() > 0)
        //        {
        //            tasklist.Add(Task);
        //        }
        //    }
        //    if (tasklist.Count() == 0)
        //    {
        //        notasks.Text = " They are no subtasks in this tasks!";
        //        if (BindingContext is WeeklyTaskViewModel bvm)
        //        {
        //            await bvm.WithsubtasksTask();
        //        }
        //    }
        //    else
        //    {
        //        notasks.Text = "";
        //        if (BindingContext is WeeklyTaskViewModel bvm)
        //        {
        //            await bvm.WithsubtasksTask();
        //        }
        //    }
        //}
    }
}