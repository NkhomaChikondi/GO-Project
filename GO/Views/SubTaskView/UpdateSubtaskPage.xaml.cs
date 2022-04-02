using GO.Models;
using GO.Services;
using GO.ViewModels.Subtasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.SubTaskView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [QueryProperty(nameof(SubtaskId), nameof(SubtaskId))]
    public partial class UpdateSubtaskPage : ContentPage
    {
        public string SubtaskId { get; set; }
        public IDataSubtask<Subtask> dataTask { get; }
        public UpdateSubtaskPage()
        {

            InitializeComponent();
             dataTask = DependencyService.Get<IDataSubtask<Subtask>>();
            BindingContext = new AddSubtaskViewModel();

        }
        protected async override void OnAppearing()
        {


            base.OnAppearing();
            int.TryParse(SubtaskId, out var result);
            BindingContext = await dataTask.GetSubTaskAsync(result);


        }
    }
}