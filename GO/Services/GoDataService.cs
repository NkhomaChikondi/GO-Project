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
    public class GoDataService : IDataStore<Category>, IDataGoal<Goal>, IDataTask<GoalTask>, IDataSubtask<Subtask>, IDataWeek<Week>, IGoalWeek<GoalWeek>, IDataDow<DOW>, IDateNotification<Notification>,ITaskday<Task_Day>
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
            //  creating database tables
            await db.CreateTableAsync<Category>();
            await db.CreateTableAsync<Goal>();
            await db.CreateTableAsync<GoalTask>();
            await db.CreateTableAsync<Subtask>();
            await db.CreateTableAsync<DOW>();
            await db.CreateTableAsync<Week>();
            await db.CreateTableAsync<GoalWeek>();
            await db.CreateTableAsync<Notification>();
            await db.CreateTableAsync<Task_Day>();

        
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
                Description = item.Description,
                CreatedOn = item.CreatedOn,    
                goalNumber = item.goalNumber
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
                Description= item.Description,
                Time = item.Time,
                Progress = item.Progress,
                Percentage = item.Percentage,
                Status = item.Status,
                HasWeek = item.HasWeek,
                ExpectedPercentage = item.ExpectedPercentage,
                NumberOfWeeks = item.NumberOfWeeks,
                enddatetostring = item.enddatetostring,
                Noweek = item.Noweek,
                DaysLeft = item.DaysLeft

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
        public async Task<IEnumerable<Goal>> GetGoalsAsync(bool forceRefresh = false)
        {
            await Init();
            // get all goals in the database
            var allGoals = await db.Table<Goal>().ToListAsync();
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
                CompletedSubtask = item.CompletedSubtask,
                CreatedOn = item.CreatedOn,
                IsEnabled = item.IsEnabled,
                IsVisible = item.IsVisible,
                IsNotVisible = item.IsNotVisible,
                enddatetostring = item.enddatetostring,
                GoalId = item.GoalId,                
                Isrepeated = item.Isrepeated,
                SubtaskNumber = item.SubtaskNumber,
                WeekId = item.WeekId,
               
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
        public async Task<IEnumerable<GoalTask>> GetTasksAsync(int Id, int weekid, bool forceRefresh = false)
        {
            await Init();
            // get all goals in the database
            var allTasks = await db.Table<GoalTask>().Where(g => g.GoalId == Id).Where(t => t.WeekId == weekid).ToListAsync();
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
                Status = item.Status,
                Due_On = item.Due_On,
                enddatetostring = item.enddatetostring,
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
    

        //public Task<bool> UpdateSelectedWrapperAsync(SelectedItemWrapper<DOW> item)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> DeleteSelectedWrapperAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<SelectedItemWrapper<DOW>> GetSelectedWrapperAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<SelectedItemWrapper<DOW>>> GetSelectedWrapperAsync(int Id, bool forceRefresh = false)
        //{
        //    throw new NotImplementedException();
        //}
   

        public async Task<bool> AddDOWAsync(DOW item)
        {
            await Init();
            DOW dOW = new DOW()
            {
                DOWId = item.DOWId,
                Name = item.Name,              
                IsSelected = item.IsSelected              
            };
            await db.InsertAsync(dOW);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateDOWAsync(DOW item)
        {
            await Init();
            await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteDOWAsync(int id)
        {
            await Init();
            // Remove the selected week item from the database
            var deleteDow = await db.DeleteAsync<DOW>(id);
            return await Task.FromResult(true);
        }

        public async Task<DOW> GetDOWAsync(int id)
        {
            await Init();
            // get the selected item 
            var dow = await db.Table<DOW>().Where(d => d.DOWId == id).FirstOrDefaultAsync();
            return dow;
        }

       
        public async Task<IEnumerable<DOW>> GetDOWsAsync(bool forceRefresh = false)
        {
            await Init();
            // get all subtasks in the database
            var allDows = await db.Table<DOW>().ToListAsync();
            return allDows;
        }

        public async Task<bool> AddWeekAsync(Week item)
        {
            await Init();
            var week = new Week
            {
                Id = item.Id,
                WeekNumber = item.WeekNumber,
                TargetPercentage = item.TargetPercentage,
                AccumulatedPercentage = item.AccumulatedPercentage,                
                StartDate = item.StartDate,
                EndDate = item.EndDate,
                Progress = item.Progress,              
                GoalId = item.GoalId,
                status = item.status,
                totalnumberOfcompletedtask = item.totalnumberOfcompletedtask,
                totalnumberOftask = item.totalnumberOftask
            };
            await db.InsertAsync(week);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateWeekAsync(Week item)
        {
            await Init();
            await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteWeekAsync(int id)
        {
            await Init();
            // Remove the selected week item from the database
            var deleteweek = await db.DeleteAsync<Week>(id);
            return await Task.FromResult(true);
        }

        public async Task<Week> GetWeekAsync(int id)
        {
            await Init();
            // get the selected item 
            var Week = await db.Table<Week>().Where(d => d.Id == id).FirstOrDefaultAsync();
            return Week;
        }

        public async Task<IEnumerable<Week>> GetWeeksAsync(int Id, bool forceRefresh = false)
        {
            await Init();
            // get all subtasks in the database
            var allweeks = await db.Table<Week>().Where(g => g.GoalId== Id).ToListAsync();
            return allweeks;
        }

        public async Task<bool> AddGoalWeekAsync(GoalWeek item)
        {
            await Init();
            // create a new Goalweek object
            var newGoalWeek = new GoalWeek
            {
                GoalId = item.GoalId
            };
            await db.InsertAsync(newGoalWeek);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateGoalWeekAsync(GoalWeek item)
        {
            await Init();
            await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }
   
        public async Task<IEnumerable<GoalWeek>> GetGoalWeeksAsync(bool forceRefresh = false)
        {
            await Init();
            // get all the categories in the database
            var allgoalweek= await db.Table<GoalWeek>().ToListAsync();
            return allgoalweek;
        }

        public async Task<bool> AddNotificationAsync(Notification item)
        {
            await Init();
            var notification = new Notification
            {
                Title = item.Title,
                Descrption = item.Descrption,
                NotifyingDate = item.NotifyingDate,
                GoalId = item.GoalId,
                TaskId = item.TaskId,
                SubtaskId = item.SubtaskId
            };
            // insert the values into the database
            await db.InsertAsync(notification);
            return await Task.FromResult(true);

        }

        public async Task<bool> UpdateNotificationAsync(Notification item)
        {
            await Init();
            await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            await Init();
            // Remove the selected subtask item from the database
            var deleteNotification = await db.DeleteAsync<Notification>(id);
            return await Task.FromResult(true);
        }

        public async Task<Notification> GetNotificationAsync(int id)
        {
            await Init();
            // get the selected item 
            var notification = await db.Table<Notification>().Where(d => d.Id == id).FirstOrDefaultAsync();
            return notification;
        }

        public async Task<IEnumerable<Notification>> GetNotificationGoalAsync(int GoalId, bool forceRefresh = false)
        {
            await Init();
            // get all goals in the database
            var GoalNotifications = await db.Table<Notification>().Where(g => g.GoalId == GoalId).ToListAsync();
            return GoalNotifications;
        }

        public async Task<IEnumerable<Notification>> GetNotificationTaskAsync(int TaskId, bool forceRefresh = false)
        {
            await Init();
            // get all goals in the database
            var TaskNotifications = await db.Table<Notification>().Where(g => g.TaskId == TaskId).ToListAsync();
            return TaskNotifications;
        }

        public async Task<IEnumerable<Notification>> GetNotificationSubtaskAsync(int SubtaskId, bool forceRefresh = false)
        {
            await Init();
            // get all goals in the database
            var subtaskNotifications = await db.Table<Notification>().Where(g => g.SubtaskId == SubtaskId).ToListAsync();
            return subtaskNotifications;
        }

        public async Task<bool> AddTaskdayAsync(Task_Day item)
        {

            // wait for the database to be created
            await Init();
            // create a new Task object

            var taskday = new Task_Day
            {               
                Taskid = item.Taskid,
                DowId = item.DowId
            };
            // insert the values into the database
            await db.InsertAsync(taskday);
            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateTaskdayAsync(Task_Day item)
        {

            await Init();
            await db.UpdateAsync(item);
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteTaskdayAsync(int id)
        {

            await Init();
            // Remove the selected subtask item from the database
            var deleteTaskday = await db.DeleteAsync<Task_Day>(id);
            return await Task.FromResult(true);
        }

        public async Task<Task_Day> GetTaskdayAsync(int id)
        {

            await Init();
            // get all goals in the database
            var task_day = await db.Table<Task_Day>().Where(g => g.Id == id).FirstOrDefaultAsync();
            return task_day;
        }

        public async Task<IEnumerable<Task_Day>> GetTaskdaysAsync(bool forceRefresh = false)
        {
            await Init();
            // get all subtasks in the database
            var allTaskdays = await db.Table<Task_Day>().ToListAsync();
            return allTaskdays;
        }
    }
}
