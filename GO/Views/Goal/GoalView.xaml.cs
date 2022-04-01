using GO.ViewModels.Goals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.Goal
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GoalView : ContentPage
    {
        public GoalView()
        {
            InitializeComponent();
            BindingContext = new GoalViewModel();

        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is GoalViewModel cvm)
            {
                await cvm.Refresh();
            }
        }
    }
}