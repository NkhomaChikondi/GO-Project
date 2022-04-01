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
    public partial class AddGoalview : ContentPage
    {
        public AddGoalview()
        {
            InitializeComponent();
            BindingContext = new AddGoalViewModel();
        }



        private void btnday_Clicked(object sender, EventArgs e)
        {

            specifygridLayout.IsVisible = false;
            datelayout.IsVisible = false;
            dateEndLayeout.IsVisible = false;
            btFrame.IsVisible = true;
            TimeLabel.Text = "Notify Time:    ";
            labelduration.IsVisible = false;
            DurationLabel.IsVisible = true;
            DurationFrame.IsVisible = false;
            timeNotifier.IsVisible = false;
        }

        private void btnSpecify_Clicked(object sender, EventArgs e)
        {
            TimeLabel.Text = "Notify Time:   ";
            specifygridLayout.IsVisible = true;
            datelayout.IsVisible = true;
            dateEndLayeout.IsVisible = true;
            DurationFrame.IsVisible = true;
            btFrame.IsVisible = false;
            DurationLabel.IsVisible = true;
            labelduration.IsVisible = true;
            timeNotifier.IsVisible = true;




        }


    }
}
