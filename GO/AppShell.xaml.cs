
using GO.Views.Category;
using GO.Views.Categorys;
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
            Routing.RegisterRoute(nameof(AddCategory), typeof(AddCategory));
            Routing.RegisterRoute(nameof(CategoryView), typeof(CategoryView));
            Routing.RegisterRoute(nameof(weekTasks), typeof(weekTasks));
            Routing.RegisterRoute(nameof(Helpgoalpage), typeof(Helpgoalpage));
            Routing.RegisterRoute(nameof(Helpaddgoalpage), typeof(Helpaddgoalpage));
            Routing.RegisterRoute(nameof(helptaskPage), typeof(helptaskPage));
            Routing.RegisterRoute(nameof(Helpaddtaskpage), typeof(Helpaddtaskpage));
            Routing.RegisterRoute(nameof(HelpSubtaskpage), typeof(HelpSubtaskpage));
            Routing.RegisterRoute(nameof(Helpaddsubtaskspage), typeof(Helpaddsubtaskspage));
            Routing.RegisterRoute(nameof(HelpweeklyTaskspage), typeof(HelpweeklyTaskspage));
            Routing.RegisterRoute(nameof(HelpWeekPage), typeof(HelpWeekPage ));
            Routing.RegisterRoute(nameof(WeekTask), typeof(WeeklyTask));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
