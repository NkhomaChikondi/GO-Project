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
    public partial class Weekly_Task : ContentPage
    {
        public Weekly_Task()
        {
            InitializeComponent();
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            // int.TryParse(weekId, out var result);           
            //calendarview.TodayDate = DateTime.Today;
           
        }
    }
}