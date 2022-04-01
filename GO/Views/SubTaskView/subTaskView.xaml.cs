using GO.ViewModels.Subtasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.SubTaskView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class subTaskView : ContentPage
    {
        public subTaskView()
        {
            InitializeComponent();
            BindingContext = new SubtaskViewModel();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SubtaskViewModel cvm)
            {
                await cvm.Refresh();

            }
        }

        private async void switchTask_Toggled(object sender, ToggledEventArgs e)
        {
            Switch @switch = (Switch)sender;
            var Subtask = (Models.Subtask)@switch.BindingContext;
            var Subtaskid = Subtask.Id;
            var SubtaskIsComplete = Subtask.IsCompleted;

            if (@switch.IsToggled)
            {
                if (BindingContext is SubtaskViewModel viewModel)
                    await viewModel.AddSubTaskPercentage(Subtaskid, SubtaskIsComplete);
            }

            else if (SubtaskIsComplete == false || !@switch.IsToggled)
            {
                if (BindingContext is SubtaskViewModel viewModel)
                    await viewModel.AddSubTaskPercentage(Subtaskid, SubtaskIsComplete);
            }

        }
    }
}