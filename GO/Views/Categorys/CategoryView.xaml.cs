using GO.Models;
using GO.Services;
using GO.ViewModels.Categorys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GO.Views.Categorys
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CategoryView : ContentPage
    {
        public IDataStore<Models.Category> dataCategory { get; }
        public CategoryView()
        {
            InitializeComponent();
            BindingContext = new CategoryViewModel();
            dataCategory = DependencyService.Get<IDataStore<Models.Category>>();
            List<CarouselImage> images = new List<CarouselImage>()
            {
                new CarouselImage {image = "blankpic.jpg"},
                new CarouselImage {image = "overlay1.jiff"},
                new CarouselImage {image = "overlay2.jiff"}
            };
            carousel.ItemsSource = images;
            Device.StartTimer(TimeSpan.FromSeconds(2), (Func<bool>)(() =>
            {
                carousel.Position = (carousel.Position + 1) % images.Count();
                return true;
            }));
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            // get all categories in the database
            var categories = await dataCategory.GetItemsAsync();

            if (categories.Count() == 0)
            {
                NoCategory.IsVisible = true;
                HasCategory.IsVisible = false;
            }

            else if (categories.Count() > 0)
            {
                HasCategory.IsVisible = true;
                NoCategory.IsVisible = false;
            }
            if (BindingContext is CategoryViewModel cvm)
            {
                await cvm.Refresh();
            }
        }

    }
}