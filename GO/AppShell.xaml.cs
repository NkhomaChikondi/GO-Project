
using GO.Views.Goal;
using GO.Views.GoalTask;
using GO.Views.SubTaskView;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GO
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(GoalView), typeof(GoalView));
            Routing.RegisterRoute(nameof(AddGoalview), typeof(AddGoalview));            
            Routing.RegisterRoute(nameof(GoalTaskPage), typeof(GoalTaskPage));
            Routing.RegisterRoute(nameof(AddTaskPage), typeof(AddTaskPage));
            Routing.RegisterRoute(nameof(UpdateTaskPage), typeof(UpdateTaskPage));
            Routing.RegisterRoute(nameof(subTaskView), typeof(subTaskView));
            Routing.RegisterRoute(nameof(AddSubtask), typeof(AddSubtask));
            Routing.RegisterRoute(nameof(AddPlannedSubtask), typeof(AddPlannedSubtask));
            Routing.RegisterRoute(nameof(UpdateSubtaskPage), typeof(UpdateSubtaskPage));
            Routing.RegisterRoute(nameof(UpdateGoalPage), typeof(UpdateGoalPage));
            Routing.RegisterRoute(nameof(WeeklyTask), typeof(WeeklyTask));
            Routing.RegisterRoute(nameof(AddPlannedTask), typeof(AddPlannedTask));
            Routing.RegisterRoute(nameof(UpdateWeekTask), typeof(UpdateWeekTask));
            Routing.RegisterRoute(nameof(UpdateWeekSubtask), typeof(UpdateWeekSubtask));
           // Routing.RegisterRoute(nameof(BlankTaskView), typeof(BlankTaskView));
            //Routing.RegisterRoute(nameof(BlankGoalView), typeof(BlankGoalView));
            //Routing.RegisterRoute(nameof(BlankSubtaskView), typeof(BlankSubtaskView));
            //Routing.RegisterRoute(nameof(BlankWeekTaskView), typeof(BlankWeekTaskView));
            //Routing.RegisterRoute(nameof(BlankWeekSubtaskView), typeof(BlankWeekSubtaskView));









        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
