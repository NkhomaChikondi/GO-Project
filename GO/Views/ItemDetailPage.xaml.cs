using GO.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace GO.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}