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
    public partial class GoalTaskPage : ContentPage
    {
        public GoalTaskPage()
        {
            InitializeComponent();
            BindingContext = new GoalTaskViewModel();


        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is GoalTaskViewModel cvm)
            {
                await cvm.Refresh();

            }
        }


        private async void switch_Toggled(object sender, ToggledEventArgs e)
        {

            Switch @switch = (Switch)sender;
            var task = (Models.GoalTask)@switch.BindingContext;
            var taskid = task.Id;
            var taskIsComplete = task.IsCompleted;

            if (taskIsComplete == false || !@switch.IsToggled)
            {
                if (BindingContext is GoalTaskViewModel viewModel)
                    await viewModel.AddPercentage(taskid, taskIsComplete);
            }

            if (@switch.IsToggled)
            {
                if (BindingContext is GoalTaskViewModel viewModel)
                    await viewModel.AddPercentage(taskid, taskIsComplete);
            }

        }


    }

}
