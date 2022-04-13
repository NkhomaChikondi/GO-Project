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

        private void Button_Clicked(object sender, EventArgs e)
        {
            lblDescription.IsVisible = false;
            layoutStart.IsVisible = false;
            layoutDate.IsVisible = false;
            frameDow.IsVisible = true;
            buttonlong1.IsVisible = false;
            buttonlong2.IsVisible = false;
            buttondaily1.IsVisible = true;
            buttondaily2.IsVisible = true;
            frametime.IsVisible = true;
            frametime2.IsVisible = false;
            addbtn1.IsVisible = true;
            addbtn2.IsVisible = false;
           
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {

            lblDescription.IsVisible = true;
            layoutStart.IsVisible = true;
            layoutDate.IsVisible = true;
            frameDow.IsVisible = false;
            buttondaily1.IsVisible = false;
            buttondaily2.IsVisible = false;
            buttonlong1.IsVisible = true;
            buttonlong2.IsVisible = true;
            frametime.IsVisible = false;
            frametime2.IsVisible = true;
            addbtn1.IsVisible = false;
            addbtn2.IsVisible = true;
           
        }

        private void SwitchTime_Toggled(object sender, ToggledEventArgs e)
        {
            if(SwitchTime.IsToggled.Equals(true))
            {
                
                lblans.IsVisible = true;
                imgclock.IsVisible = true;
                layoutTime.IsVisible = true;
                frametime.Margin = new Thickness(20, 0, 20, -20);
                addbtn1.Margin = new Thickness (0,-26,0,34);
                              
                

            }
            else
            {
                lblans.IsVisible = false;
                imgclock.IsVisible = false;
                layoutTime.IsVisible = false;
                frametime.Margin = new Thickness(20, 0, 20, 65);
                addbtn1.Margin = new Thickness(0, -92, 0, 104);
              

            }

        }

        private void SwitchTime2_Toggled(object sender, ToggledEventArgs e)
        {
            if(SwitchTime2.IsToggled.Equals(true))
            {
                lblans1.IsVisible = true;
                imgclock1.IsVisible = true;
                layoutTime1.IsVisible = true;
                frametime2.Margin = new Thickness(20, 12, 20, -20);
                addbtn2.Margin = new Thickness(0, -26, 0, 34);
            }
            else 
            {
                lblans1.IsVisible = false;
                imgclock1.IsVisible = false;
                layoutTime1.IsVisible = false;
                frametime2.Margin = new Thickness(20, 12, 20, 65);
                addbtn2.Margin = new Thickness(0, -92, 0, 104);

            }
          
        }
    }
}
