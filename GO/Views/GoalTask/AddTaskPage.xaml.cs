using GO.Models;
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
    public partial class AddTaskPage : ContentPage
    {
        public AddTaskPage()
        {
            InitializeComponent();
            BindingContext = new addTaskViewModel();
        }
        //protected  async override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    if (BindingContext is addTaskViewModel cvm)
        //    {
        //         await cvm.GetDows();

        //    }
        //}
    }
}