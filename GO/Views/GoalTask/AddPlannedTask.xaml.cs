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
    public partial class AddPlannedTask : ContentPage
    {
        private bool suntapped = false;
        private bool montapped = false;
        private bool tuetapped = false;
        private bool wedtapped = false;
        private bool thutapped = false;
        private bool fritapped = false;
        private bool sattapped = false;
       
        public AddPlannedTask()
        {
            InitializeComponent();
            BindingContext = new addTaskViewModel();
        }

        private void Sun_Tapped(object sender, EventArgs e)
        {
            if(!suntapped)
            {
                suntapped = true;
                framesun.BackgroundColor = Color.LightGray;
                Sun.Text = "Sunday";
                if(BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Add("Sunday");
                }
            }
            else if(suntapped)
            {
                suntapped = false;
                framesun.BackgroundColor = Color.White;
                Sun.Text = null;
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Remove("Sunday");
                }
            }
            
        }
        private void Mon_Tapped(object sender, EventArgs e)
        {
            if (!montapped)
            {
                montapped = true;
                framemon.BackgroundColor = Color.LightGray;
                Mon.Text = "Monday";
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Add("Monday");
                }
            }
            else if (montapped)
            {
                montapped = false;
                framemon.BackgroundColor = Color.White;
                Mon.Text = null;
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Remove("Monday");
                }
            }
        }
        private void Tue_Tapped(object sender, EventArgs e)
        {

            if (!tuetapped)
            {
                tuetapped = true;
                frametue.BackgroundColor = Color.LightGray;
                Tue.Text = "Tuesday";
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Add("Tuesday");
                }
            }
            else if (tuetapped)
            {
                tuetapped = false;
                frametue.BackgroundColor = Color.White;
                Tue.Text = null;
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Remove("Tuesday");
                }
            }
        }
        private void Wed_Tapped(object sender, EventArgs e)
        {
            if (!wedtapped)
            {
                wedtapped = true;
                framewed.BackgroundColor = Color.LightGray;
                Wed.Text = "Wednesday";
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Add("Wednesday");
                }
            }
            else if (wedtapped)
            {
                wedtapped = false;
                framewed.BackgroundColor = Color.White;
                Wed.Text = null;
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Remove("Wednesday");
                }
            }
        }
        private void Thu_Tapped(object sender, EventArgs e)
        {
            if (!thutapped)
            {
                thutapped = true;
                framethu.BackgroundColor = Color.LightGray;
                Thu.Text = "Thursday";
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Add("Thursday");
                }
            }
            else if (thutapped)
            {
                thutapped = false;
                framethu.BackgroundColor = Color.White;
                Thu.Text = null;
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Remove("Thursday");
                }
            }
        }
        private void Fri_Tapped(object sender, EventArgs e)
        {
            if (!fritapped)
            {
                fritapped = true;
                framefri.BackgroundColor = Color.LightGray;
                Fri.Text = "Friday";
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Add("Friday");
                }
            }
            else if (fritapped)
            {
                fritapped = false;
                framefri.BackgroundColor = Color.White;
                Fri.Text = null;
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Remove("Friday");
                }
            }
        }
        private void Sat_Tapped(object sender, EventArgs e)
        {
            if (!sattapped)
            {
                sattapped = true;
                framesat.BackgroundColor = Color.LightGray;
                Sat.Text = "Saturday";
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Add("Saturday");
                }
            }
            else if (sattapped)
            {
                sattapped = false;
                framesat.BackgroundColor = Color.White;
                Sat.Text = null;
                if (BindingContext is addTaskViewModel atv)
                {
                    atv.Day_names.Remove("Saturday");
                }
            }
        }

        //private async void Chooseday_clicked(object sender, EventArgs e)
        //{
        //    var action = await DisplayActionSheet("Options", "Cancel", "", "No Repeat", "Repeat");
        //    if (action == "Repeat")
        //    {
        //        if (BindingContext is addTaskViewModel bvm)
        //        {
        //            bvm.IsRepeated = true;
        //        }
        //    }
        //    else if (action == "No Repeat")
        //    {
        //        if (BindingContext is addTaskViewModel bvm)
        //        {
        //            bvm.IsRepeated = false;
        //        }
        //    }
        //}

        private void MyPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedOption = MyPicker.SelectedItem as string;

            if (selectedOption == "Repeat")
            {
                if (BindingContext is addTaskViewModel bvm)
                {
                    bvm.IsRepeated = true;
                }
            }
            else if (selectedOption == "No Repeat")
            {
                if (BindingContext is addTaskViewModel bvm)
                {
                    bvm.IsRepeated = false;
                }
            }
        }
    }
}