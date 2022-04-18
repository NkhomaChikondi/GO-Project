using GO.ViewModels.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.Category
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryView : ContentPage
    {
        public CategoryView()
        {
            InitializeComponent();
            BindingContext = new CategoryViewModel();
            
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is CategoryViewModel cvm)
            {
                await cvm.OnPageAppearance();
                await cvm.Refresh();
            }
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = BindingContext as CategoryViewModel;
            var item = e.Item as Models.Category;
            vm.HideOrShowCategory(item);
             
        }
    }
}