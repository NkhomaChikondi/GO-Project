using GO.Services;
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
    [QueryProperty(nameof(goalId), nameof(goalId))]
    public partial class UpdateGoalPage : ContentPage
    {

        public string goalId { get; set; }
        public IDataGoal<Models.Goal> dataGoal { get; }
        public UpdateGoalPage()
        {
            InitializeComponent();
            dataGoal = DependencyService.Get<IDataGoal<Models.Goal>>();
            BindingContext = new AddGoalViewModel();
            //detaillabel.TranslateTo(100, 0, 3000, Easing.Linear);

        }
        protected async override void OnAppearing()
        {


            base.OnAppearing();
            int.TryParse(goalId, out var result);
            BindingContext = await dataGoal.GetGoalAsync(result);
            //var vm = BindingContext as AddGoalViewModel;
            //vm.InitializeProperties(goal);


        }
    }
}