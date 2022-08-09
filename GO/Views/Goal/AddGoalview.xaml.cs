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

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (Hasweek.IsChecked)        
            noWeek.IsChecked = false;                
        }

        private void noWeek_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (noWeek.IsChecked)            
            Hasweek.IsChecked = false;         
        }
    }
}
