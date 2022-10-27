using GO.Models;
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
  
    public class AddCategoryViewModel: BaseViewmodel
    {

        
        private string name;
        private string description;

      
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }

        public AsyncCommand AddcategoryCommand { get; set; }
        public AddCategoryViewModel()
        {
            AddcategoryCommand = new AsyncCommand(AddCategory);
        }
        async Task AddCategory()
        {
            if (IsBusy)
                return;
            try
            {
                // create a new category object
                var newCategory = new Category
                {
                    Name = name,
                    Description = description
                };
                var allCategories = await datastore.GetItemsAsync();
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newCategory.Name[0]) + newCategory.Name.Substring(1);
                //check if the new task already exist in the database
                if (allCategories.Any(G => G.Name == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "A Goal with that Name already exist! Change. ", "OK");
                    return;
                }
                if (newCategory.Description == null)
                    newCategory.Description = $"No Description for {newCategory.Name} ";
                // create newest category
                var newestCategory = new Category
                {
                    Name = UppercasedName,
                    Description = newCategory.Description,
                    CreatedOn = DateTime.Now,
                    goalNumber = 0                    
                };
                await datastore.AddItemAsync(newestCategory);
                await Application.Current.MainPage.DisplayAlert("Alert!", "Added Successfully ", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to add new goal: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
            }

            finally
            {
                IsBusy = false;
            }
        }
    }
}
