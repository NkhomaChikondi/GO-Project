using GO.Models;
using GO.ViewModels.Goals;
using GO.Views.Goal;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.Categorys
{
    public class CategoryViewModel : BaseViewmodel
    {
        // create an enumerable (observable) collection of categories
        public ObservableRangeCollection<Category> categories { get; }
        public AsyncCommand RefreshCommand { get; }
        public AsyncCommand<Category> AddCommand { get; }
        public AsyncCommand<Category> DeleteCommand { get; }
        public AsyncCommand<Category> UpdateCommand { get; }
        public AsyncCommand<Category> ActionCommand { get; }
        public AsyncCommand<int> DetailCommand { get; }
        public AsyncCommand<Category> ItemSelectedCommand { get; }


        public CategoryViewModel()
        {
            categories = new ObservableRangeCollection<Category>();
            title = "Category";
            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand<Category>(addCategory);
            DeleteCommand = new AsyncCommand<Category>(deleteCategory);
            UpdateCommand = new AsyncCommand<Category>(updateCategory);
            DetailCommand = new AsyncCommand<int>(getCategory);

            ItemSelectedCommand = new AsyncCommand<Category>(selectItem);

        }

        async Task addCategory(Category category)
        {
            // check if the app is busy
            if (IsBusy)
                return;
            try

            {

                // assign a value to " IsBusy"
                IsBusy = true;
                // add an item
                var name = await App.Current.MainPage.DisplayPromptAsync("Add New Category", "Name", "OK", "Cancel");


                var newCategory = new Category
                {
                    Name = name,

                    CreatedOn = DateTime.Now

                };
                // change the first letter of the category name to upercase
                var UppercasedName = char.ToUpper(newCategory.Name[0]) + newCategory.Name.Substring(1);
                // pass the uppercased name to the category object
                var newestCategory = new Category
                {
                    Name = UppercasedName,
                    Id = newCategory.Id
                };

                //check if the new category already exist in the database
                if (categories.Any(C => C.Name == newestCategory.Name))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Name already exist", "OK");
                    return;

                }


                // validate new category values
                if (string.IsNullOrWhiteSpace(newestCategory.Name))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Please write the Category Name", "OK");

                    return;
                }
                await datastore.AddItemAsync(newestCategory);
                await Refresh();

            }
            catch (Exception ex)
            {
                // error messages

                Debug.WriteLine($"Failed to add Category: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
        async Task deleteCategory(Category category)
        {
            if (category == null)
                return;
            await datastore.DeleteItemAsync(category.Id);
            await Refresh();
        }
        async Task updateCategory(Category category)
        {

            if (category == null)
                return;
            try
            {
                var editCategory = await Application.Current.MainPage.DisplayPromptAsync(" Edit Category", "Name", "Save", "Cancel", $"{category.Name}");

                // change the first letter of category name to uppercase if it is not
                var editedCategory = char.ToUpper(editCategory[0]) + editCategory.Substring(1);
                var updateCategory = new Category
                {
                    Id = category.Id,
                    Name = editedCategory
                };

                await datastore.UpdateItemAsync(updateCategory);
                await Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to update Category: {ex.Message}");
            }

        }
        async Task getCategory(int id)
        {
            // get details of the selected category from the database
            var categoryDetail = await datastore.GetItemAsync(id);



            await Refresh();
        }
        async Task selectItem(Category category)
        {
            // check if the goal item has been selected
            if (category == null)
                return;
            var route = $"{nameof(GoalView)}?{nameof(GoalViewModel.CategoryId)}={category.Id}";

            await Shell.Current.GoToAsync(route);
        }
        async Task getAllCategories()
        {
            // list down all categories in the database
            // check if the app is busy
            if (IsBusy)
                return;
            // otherwise
            try
            {
                IsBusy = true;
                var Allcategories = await datastore.GetItemsAsync();
                categories.ReplaceRange(Allcategories);

            }
            catch (Exception ex)
            {
                // error message

                Debug.WriteLine($"Failed to add Category: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");

            }
            finally
            {
                IsBusy = false;
            }

        }
        public async Task Refresh()
        {

            // set "IsBusy" to true
            IsBusy = true;
            // make the refreshing process load for 2 seconds
            await Task.Delay(2000);
            // clear categories on the page
            categories.Clear();
            // get all categories
            var category = await datastore.GetItemsAsync();
            // retrieve the categories back
            categories.AddRange(category);
            // set "isBusy" to true
            IsBusy = false;

        }


    }



}
