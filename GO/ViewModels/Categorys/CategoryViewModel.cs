using GO.Models;
using GO.ViewModels.Goals;
using GO.Views.Categorys;
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
        public AsyncCommand AddCommand { get; }
        public AsyncCommand<Category> DeleteCommand { get; }
        public AsyncCommand<Category> UpdateCommand { get; }
        public AsyncCommand<Category> ActionCommand { get; }
        public AsyncCommand<int> DetailCommand { get; }
        public AsyncCommand<Category> ItemSelectedCommand { get; }

     
        public bool Isvisible { get => isvisible; set => isvisible = value; }
        public string Oldname { get => oldname; set => oldname = value; }
        public bool HasCategory { get => hasCategory; set => hasCategory = value; }
        public bool Nocategory { get => nocategory; set => nocategory = value; }
        public Category SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                if (!selectedItem.Equals(null))
                    selectItem(value);

            }
        }

        public string GoalNumberText { get => goalNumberText; set => goalNumberText = value; }

        private string goalNumberText;
        private bool isvisible = false;
        private string oldname;
        private int categoryid;
        private bool hasCategory;
        private bool nocategory;
        private Category selectedItem;
       


        public CategoryViewModel()
        {
            categories = new ObservableRangeCollection<Category>();           
            title = "Category";
            RefreshCommand = new AsyncCommand(Refresh);
            AddCommand = new AsyncCommand(addCategory);
            DeleteCommand = new AsyncCommand<Category>(deleteCategory);
            UpdateCommand = new AsyncCommand<Category>(updateCategory);
            DetailCommand = new AsyncCommand<int>(getCategory);
            ItemSelectedCommand = new AsyncCommand<Category>(selectItem);
        }

     
        async Task addCategory()
        {
            var route = $"{nameof(AddCategory)}";
            await Shell.Current.GoToAsync(route);
        }
        async Task deleteCategory(Category category)
        {
            if (category == null)
                return;
           var ans = await Application.Current.MainPage.DisplayAlert("Delete Category", "All Goals in this category will be deleted. Continue?", "Yes", "No");
            if (ans)
            {
                await datastore.DeleteItemAsync(category.Id);
                // get all goals having the 
                var goals = await datagoal.GetGoalsAsync(category.Id);

                // loop through them and delete
                foreach (var goal in goals)
                {
                    // get tasks in this goal
                    var tasks = await dataTask.GetTasksAsync(goal.Id);

                    await datagoal.DeleteGoalAsync(goal.Id);

                    foreach (var task in tasks)
                    {
                        // get subtasks in this tasks
                        var subtasks = await dataSubTask.GetSubTasksAsync(task.Id);
                        await dataTask.DeleteTaskAsync(task.Id);
                        //check if thy're subtasks having the task id 

                        foreach (var subtask in subtasks)
                        {
                            await dataSubTask.DeleteSubTaskAsync(subtask.Id);
                        }
                    }
                }
                await Refresh();
                Datatoast.toast("Category deleted ");
            }
            else if (!ans)
                return;
           
        }
        async Task updateCategory(Category category)
        {
            if (IsBusy)
                return;

            if (category == null)
                return;               
            try
            {
                // assign a value to " IsBusy"
                IsBusy = true;
                // get the categoryfrom the database
                var categorydb = await datastore.GetItemAsync(category.Id);
                // add an item
                var name = await Application.Current.MainPage.DisplayPromptAsync("Edit Category", "Name", "OK", "Cancel",$"{categorydb.Name}");
                if (string.IsNullOrEmpty(name))
                {                   
                    return;
                }              
                // change the first letter of the category name to upercase
                var UppercasedName = char.ToUpper(name[0]) + name.Substring(1);
            
                //check if the new category already exist in the database
                if (categories.Any(C => C.Name == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Name already exist", "OK");
                    return;
                }
                categorydb.Name = UppercasedName;
                await datastore.UpdateItemAsync(categorydb);                
                await Refresh();
                Datatoast.toast(" Category updated");
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
        async Task getCategory(int id)
        {
            // get details of the selected category from the database
            var categoryDetail = await datastore.GetItemAsync(id);
            await Refresh();        }
        async Task selectItem(Category category)
        {
         
            // get goals having the category id
            var goals = await datagoal.GetGoalsAsync(category.Id);
                     
                var route = $"{nameof(GoalView)}?CategoryId={category.Id}";
                await Shell.Current.GoToAsync(route);
                     
        }

        // amethod to check the number of goals in the category
        async Task CategoryGoalNumber()
        {
            // get all categories in the database
            var categories = await datastore.GetItemsAsync();
            // loop through them
            foreach (var category in categories)
            {
                // check if it has goals
                var goals = await datagoal.GetGoalsAsync(category.Id);
                category.goalNumber = goals.Count();
                await datastore.UpdateItemAsync(category);

            }

        }
        public async Task Refresh()
        {
            // set "IsBusy" to true
                IsBusy = true;           
               await CategoryGoalNumber();
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
