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
    [QueryProperty(nameof(taskId), nameof(taskId))]
    public partial class UpdateTaskPage : ContentPage
    {
        public string taskId { get; set; }
        public IDataTask<Models.GoalTask> dataTask { get; }
        public UpdateTaskPage()
        {
            InitializeComponent();
            dataTask = DependencyService.Get<IDataTask<Models.GoalTask>>();
            BindingContext = new addTaskViewModel();
            detaillabel.TranslateTo(100, 0,3000, Easing.Linear);




        }

        protected async override void OnAppearing()
        {


            base.OnAppearing();
            int.TryParse(taskId, out var result);
            BindingContext = await dataTask.GetTaskAsync(result);


        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            var task = (Models.GoalTask)button.BindingContext;

            if (button.IsPressed == false)
            {


                BindingContext = new addTaskViewModel();
                if (BindingContext is addTaskViewModel addTaskViewModel)
                {
                    IsEnabled = addTaskViewModel.IsNotBusy;
                    await addTaskViewModel.SendTaskId(task);

                }


            }

        }
    }
}