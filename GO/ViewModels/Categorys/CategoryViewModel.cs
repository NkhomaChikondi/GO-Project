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
        public AsyncCommand ItemSelectedCommand { get; }

     
        public bool Isvisible { get => isvisible; set => isvisible = value; }
        public string Oldname { get => oldname; set => oldname = value; }

        private bool isvisible = false;
        private string oldname;
        private int categoryid;
       


        public CategoryViewModel()
        {
            categories = new ObservableRangeCollection<Category>();
            title = "Category";
            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand<Category>(addCategory);
            DeleteCommand = new AsyncCommand<Category>(deleteCategory);
            UpdateCommand = new AsyncCommand<Category>(updateCategory);
            DetailCommand = new AsyncCommand<int>(getCategory);

            ItemSelectedCommand = new AsyncCommand(selectItem);

        }
        public async void HideOrShowCategory(Category item)
        {
            
            if(item.IsVisible == false)
            {
                item.IsVisible = true;
                oldname = item.Name;
                categoryid = item.Id;
                
                await datastore.UpdateItemAsync(item);
                await Refresh();
            }
            else if(item.IsVisible == true)
            {
               
                try
                {
                    item.IsVisible = false;
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        await Application.Current.MainPage.DisplayAlert("Error!", "Category Name cant be empty!", "OK");
                        // await Refresh();
                        return;
                    }
                    item.Name = char.ToUpper(item.Name[0]) + item.Name.Substring(1);
                    //get all categories in the database
                    var categories = await datastore.GetItemsAsync();

                    if (categories.Any(c => c.Name == item.Name))
                    {
                        if (item.Name == oldname)
                        {
                            await datastore.UpdateItemAsync(item);

                            await Refresh();
                        }
                        else if (item.Name != oldname)
                        {
                            await Application.Current.MainPage.DisplayAlert("Error!", "Name already exist, Change!", "OK");
                            // await Refresh();
                            return;
                        }
                    }

                    await datastore.UpdateItemAsync(item);

                    await Refresh();
                }
                catch (Exception ex)
                {

                    Debug.WriteLine($"Failed to update Category: {ex.Message}");
                    await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
                }
                finally
                {
                    IsBusy = false;
                }
             
            }
           
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
                    CreatedOn = newCategory.CreatedOn,
                    IsVisible = isvisible                
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
                // get all the categories in the database
                var allCategories = await datastore.GetItemsAsync();
                // change the first letter of the category name to upercase
                var UppercasedName = char.ToUpper(category.Name[0]) + category.Name.Substring(1);
                // pass the uppercased name to the category object
                category.Name = UppercasedName;

                //check if the new category already exist in the database
                if (categories.Any(C => C.Name == category.Name))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Name already exist", "OK");
                    return;

                }
                // check if the incoming object already exist in the database
                if (allCategories.Any( C => C.Id == category.Id))
                {
                    // update the category in the database
                    await datastore.UpdateItemAsync(category);
                }
                await Refresh();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to Update! Category does not exist!: {ex.Message}");
            }

        }
        async Task getCategory(int id)
        {
            // get details of the selected category from the database
            var categoryDetail = await datastore.GetItemAsync(id);



            await Refresh();
        }
        async Task selectItem()
        {
            // check if the goal item has been selected
            if (categoryid == null)
                return;
            var route = $"{nameof(GoalView)}?{nameof(GoalViewModel.CategoryId)}={categoryid}";

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
