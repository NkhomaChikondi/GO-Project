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

        private void noWeek_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (noWeek.IsChecked)
                Hasweek.IsChecked = false;
        }

        private void Hasweek_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (Hasweek.IsChecked)
                noWeek.IsChecked = false;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage.DisplayAlert("INFO", "\b ONCE-OFF \b \n * Select when you want your goal to have tasks that are not repeated (One-time occurrence). \n\n WEEKLY \n * Select when you want to have tasks that can be repeated weekly.", "OK");
        }
    }
}

