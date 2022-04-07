using GO.Models;
using GO.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(GoDataService))]
namespace GO.Services
{
    public class GoDataService : IDataStore<Category>, IDataGoal<Goal>, IDataTask<GoalTask>, IDataSubtask<Subtask>,IDowGoal<DOWGoal>
    {
        static SQLiteAsyncConnection db;
        // database connection class
        public static async Task Init()
        {
            if (db != null)
                return;
            //get path of the database file
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyGoData.db");

            db = new SQLiteAsyncConnection(databasePath);
            //creating database tables
            await db.CreateTableAsync<Category>();
            await db.CreateTableAsync<Goal>();
            await db.CreateTableAsync<GoalTask>();
            await db.CreateTableAsync<Subtask>();
            await db.CreateTableAsync<DOW>();
            await db.CreateTableAsync<DOWGoal>();

        }
        public async Task<bool> AddItemAsync(Category item)
        {
            // wait for the database to be created
            await Init();
            // create a new category instance

            var category = new Category
            {
                Id = item.Id,
                Name = item.Name,
                //Description = item.Description
            };
            // insert the values into the database
            await db.InsertAsync(category);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            await Init();
            // Remove the selected category item from the database
            var deleteCategory = await db.DeleteAsync<Category>(id);
            return await Task.FromResult(true);
        }
        public async Task<Category> GetItemAsync(int id)
        {
            await Init();
            // get the selected item 
            var categoryid = await db.Table<Category>().Where(d => d.Id == id).FirstOrDefaultAsync();
            return categoryid;
        }
        public async Task<IEnumerable<Category>> GetItemsAsync(bool forceRefresh = false)
        {
            await Init();
            // get all the categories in the database
            var allCategories = await db.Table<Category>().ToListAsync();
            return allCategories;
        }
        public async Task<bool> UpdateItemAsync(Category item)
        {

            // modifying category item in the database
            await Init();
            var updateCategory = await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // implementation for goal table
        public async Task<bool> AddGoalAsync(Goal item)
        {

            // wait for the database to be created
            await Init();
            // create a new goal instance

            var goal = new Goal
            {
                Id = item.Id,
                Name = item.Name,
                Start = item.Start,
                End = item.End,
                CreatedOn = item.CreatedOn,
                CategoryId = item.CategoryId,
                Time = item.Time,
                progress = item.progress,
                Percentage = item.Percentage,

                

            };
            // insert the values into the database
            await db.InsertAsync(goal);
            return await Task.FromResult(true);
        }
        public async Task<bool> DeleteGoalAsync(int id)
        {
            await Init();
            // Remove the selected goal item from the database
            var deleteCategory = await db.DeleteAsync<Goal>(id);
            return await Task.FromResult(true);
        }
        public async Task<Goal> GetGoalAsync(string name)
        {
            await Init();
            // get the selected item 
            var goalName = await db.Table<Goal>().Where(g => g.Name == name).FirstOrDefaultAsync();
            return goalName;
        }

        public async Task<Goal> GetGoalAsync(int id)
        {
            await Init();
            // get the selected item 
            var goalid = await db.Table<Goal>().Where(d => d.Id == id).FirstOrDefaultAsync();
            return goalid;
        }
        public async Task<IEnumerable<Goal>> GetGoalsAsync(int Id, bool forceRefresh = false)
        {
            await Init();
            // get all goals in the database
            var allGoals = await db.Table<Goal>().Where(g => g.CategoryId == Id).ToListAsync();
            return allGoals;
        }
        public async Task<bool> UpdateGoalAsync(Goal item)
        {
            // modifying category item in the database
            await Init();
            var updateGoal = await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> AddTaskAsync(GoalTask item)
        {
            // wait for the database to be created
            await Init();
            // create a new Task object

            var goaltask = new GoalTask
            {
                Id = item.Id,
                taskName = item.taskName,
                StartTask = item.StartTask,
                Description = item.Description,
                EndTask = item.EndTask,
                RemainingDays = item.RemainingDays,
                Percentage = item.Percentage,
                PendingPercentage = item.PendingPercentage,
                Progress = item.Progress,
                Status = item.Status,
                IsCompleted = item.IsCompleted,
                SubtaskNumber = item.SubtaskNumber,
                CompletedSubtask = item.CompletedSubtask,
                CreatedOn = item.CreatedOn,
                GoalId = item.GoalId

            };
            // insert the values into the database
            await db.InsertAsync(goaltask);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateTaskAsync(GoalTask item)
        {

            // modifying task item in the database
            await Init();
            await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            await Init();
            //get the item from the database with the incoming id
            var deleteTask = db.DeleteAsync<GoalTask>(id);

            return await Task.FromResult(true);
        }

        public async Task<GoalTask> GetTaskAsync(int id)
        {

            await Init();
            // get the selected item 
            var taskId = await db.Table<GoalTask>().Where(d => d.Id == id).FirstOrDefaultAsync();
            return taskId;
        }

        public async Task<IEnumerable<GoalTask>> GetTasksAsync(int Id, bool forceRefresh = false)
        {
            await Init();
            // get all goals in the database
            var allTasks = await db.Table<GoalTask>().Where(g => g.GoalId == Id).ToListAsync();
            return allTasks;
        }

        public async Task<bool> AddSubTaskAsync(Subtask item)
        {
            // wait for the database to be created
            await Init();
            // create a new Task object

            var subtask = new Subtask
            {
                Id = item.Id,
                SubName = item.SubName,
                SubStart = item.SubStart,
                SubEnd = item.SubEnd,
                CreatedOn = item.CreatedOn,
                RemainingDays = item.RemainingDays,
                Percentage = item.Percentage,
                IsCompleted = item.IsCompleted,
                Status= item.Status,
                TaskId = item.TaskId
            };
            // insert the values into the database
            await db.InsertAsync(subtask);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateSubTaskAsync(Subtask item)
        {
            // modifying subtask item in the database
            await Init();
            await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteSubTaskAsync(int id)
        {
            await Init();
            // Remove the selected subtask item from the database
            var deleteSubTask = await db.DeleteAsync<Subtask>(id);
            return await Task.FromResult(true);
        }

        public async Task<Subtask> GetSubTaskAsync(int id)
        {
            await Init();
            // get the selected item 
            var subtask = await db.Table<Subtask>().Where(d => d.Id == id).FirstOrDefaultAsync();
            return subtask;
        }

        public async Task<IEnumerable<Subtask>> GetSubTasksAsync(int Id, bool forceRefresh = false)
        {

            await Init();
            // get all subtasks in the database
            var allSubTasks = await db.Table<Subtask>().Where(g => g.TaskId == Id).ToListAsync();
            return allSubTasks;
        }

        public async Task<bool> AddDowGoalAsync(DOWGoal item)
        {
            await Init();
            var DowGoal = new DOWGoal
            {
                DOGid = item.DOGid,
                GoalId = item.GoalId,
                DowId = item.DowId
            };
            // insert the values into the database
            await db.InsertAsync(DowGoal);
            return await Task.FromResult(true);
        }

        public Task<bool> UpdateDowGoalAsync(DOWGoal item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteDowGoalAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<DOWGoal> GetDowGoalAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<DOWGoal>> GetDowGoalsAsync(int Id, bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

       
    }
}
