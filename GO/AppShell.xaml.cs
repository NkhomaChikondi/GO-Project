
using GO.Views.Goal;
using GO.Views.GoalTask;
using GO.Views.SubTaskView;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace GO
{
    public partial class AppShell : Xamarin.Forms.Shell
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


        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
