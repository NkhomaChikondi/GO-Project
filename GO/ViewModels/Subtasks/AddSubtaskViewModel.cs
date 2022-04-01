using GO.Models;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.Subtasks
{
    [QueryProperty(nameof(GetTaskId), (nameof(GetTaskId)))]
    public class AddSubtaskViewModel : BaseViewmodel
    {
        private string name;
        private DateTime startDate = DateTime.Today;
        private DateTime endDate = DateTime.Today;
        private double duration;
        private double percentage;
        private int getTaskId;

        public ObservableRangeCollection<Subtask> subTasks { get; }
        public AsyncCommand subTaskAddCommand { get; }
        public AddSubtaskViewModel()
        {
            subTasks = new ObservableRangeCollection<Subtask>();
            subTaskAddCommand = new AsyncCommand(AddSubtask);
        }

        public string Name { get => name; set => name = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public DateTime EndDate { get => endDate; set => endDate = value; }
        public double Duration { get => duration; set => duration = value; }
        public int GetTaskId { get { return getTaskId; } set => getTaskId = value; }
        public double Percentage { get => percentage; set => percentage = value; }

        async Task AddSubtask()
        {
            // check if the app is busy
            if (IsBusy == true)
                return;
            try
            {
                // create a new goal object and save
                var newSubtask = new Subtask
                {
                    SubName = name,
                    CreatedOn = DateTime.Now,
                    SubStart = startDate,
                    SubEnd = endDate,
                    TaskId = getTaskId,
                    Percentage = percentage


                };
                // change the first letter of the Task name to upercase
                var UppercasedName = char.ToUpper(newSubtask.SubName[0]) + newSubtask.SubName.Substring(1);
                //check if the new task already exist in the database
                if (subTasks.Any(T => T.SubName == UppercasedName))
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", "Task Name already exist! Change. ", "OK");
                    return;
                }
                // get goal from the goal table through the given Id
                var subTaskInTask = await dataTask.GetTaskAsync(GetTaskId);
                // verify if the Start date and end date are within the duration of its selected goal
                if (newSubtask.SubStart >= subTaskInTask.StartTask && newSubtask.SubEnd <= subTaskInTask.EndTask)
                {
                    TimeSpan ts = newSubtask.SubEnd - newSubtask.SubStart;
                    Duration = ts.TotalDays;
                }

                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", " make sure the Start date and end date are within the duration of its selected Task", "OK");
                    return;
                }
                // pass the uppercased name to the category object
                var newestSubtask = new Subtask
                {

                    Id = newSubtask.Id,
                    SubName = UppercasedName,
                    SubStart = newSubtask.SubStart,
                    SubEnd = newSubtask.SubEnd,
                    Duration = duration,
                    CreatedOn = newSubtask.CreatedOn,
                    Percentage = newSubtask.Percentage,
                    IsCompleted = false,
                    TaskId = newSubtask.TaskId
                };
                await dataSubTask.AddSubTaskAsync(newestSubtask);
                await totalSubtask(getTaskId);
                AddSubTaskPercent();
                setVisibility();
                // go back to the previous page
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
        async void AddSubTaskPercent()
        {
            // get all subtasks having the specified goal id
            var AllSubtask = await dataSubTask.GetSubTasksAsync(getTaskId);
            // get the task having the gettaskid
            var Task = await dataTask.GetTaskAsync(getTaskId);
            // get the total number of tasks 
            var AllSubTaskCount = AllSubtask.Count();
            // make sure the task count is not zero
            if (AllSubTaskCount == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error!", "make sure a subtask is created", "OK");
                return;
            }

            //loop through the task and add the percentage
            foreach (var task in AllSubtask)
            {
                var SubTaskpercentage = Task.Percentage / AllSubTaskCount;
                task.Percentage = SubTaskpercentage;
                await dataSubTask.UpdateSubTaskAsync(task);
            }


        }
        async void setVisibility()
        {
            // get all subtask having "this" task id
            var subtasks = await dataSubTask.GetSubTasksAsync(getTaskId);
            // get the task equivalent to this viewmodel task id
            var task = await dataTask.GetTaskAsync(getTaskId);
            // update the number of subtask in task
            task.SubtaskNumber = subtasks.Count();
            await dataTask.UpdateTaskAsync(task);

        }
        public async Task totalSubtask(int taskid)
        {
            // get all subtasks whose percentages are equal to tasks
            var allSubtask = await dataSubTask.GetSubTasksAsync(taskid);
            // get the Task having Taskid id
            var Taskid = await dataTask.GetTaskAsync(taskid);
            var subtaskcount = allSubtask.Where(t => t.IsCompleted == true).ToList();
            var totalcount = subtaskcount.Count();
            Taskid.CompletedSubtask = totalcount;
            // send the count to task
            await dataTask.UpdateTaskAsync(Taskid);

        }
    }
}
