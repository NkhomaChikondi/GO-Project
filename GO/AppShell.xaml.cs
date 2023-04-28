

using GO.Views.Categorys;
using GO.Views.Goal;
using GO.Views.GoalTask;
using GO.Views.SubTaskView;
using Plugin.Share;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
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
            Routing.RegisterRoute(nameof(HelpPage), typeof(HelpPage));  
            Routing.RegisterRoute(nameof(helptaskPage), typeof(helptaskPage));
            Routing.RegisterRoute(nameof(Helpaddtaskpage), typeof(Helpaddtaskpage));
            Routing.RegisterRoute(nameof(HelpSubtaskpage), typeof(HelpSubtaskpage));
            Routing.RegisterRoute(nameof(Helpaddsubtaskspage), typeof(Helpaddsubtaskspage));
            Routing.RegisterRoute(nameof(HelpweeklyTaskspage), typeof(HelpweeklyTaskspage));
            Routing.RegisterRoute(nameof(HelpWeekPage), typeof(HelpWeekPage ));
            Routing.RegisterRoute(nameof(WeekTask), typeof(WeekTask));
            Routing.RegisterRoute(nameof(GoalStats), typeof(GoalStats));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            string email = "chikondinkhoma51@gmail.com";
            string subject = "Feedback: GO App";
            await Email.ComposeAsync(subject, "", email);
           
        }

        //private async void MenuItem_Clicked(object sender, EventArgs e)
        //{
        //    await Share.RequestAsync(new ShareTextRequest
        //    {
        //       Text = "",
        //        Title = "Share Text"
        //    });
        //}              
    }
}
