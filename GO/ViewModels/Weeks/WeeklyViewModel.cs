using GO.Models;
using GO.Services;
using GO.Views.GoalTask;
using MvvmHelpers.Commands;
using Shiny.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace GO.ViewModels.Weeks
{
    [QueryProperty(nameof(GoalId), nameof(GoalId))]
    public class WeeklyViewModel : BaseViewmodel
    {
        private int goalId;
        private int dayNumber;
        public int GoalId { get => goalId; set => goalId = value; }

        public AsyncCommand SunCommand { get; }
        public AsyncCommand MonCommand { get; }
        public AsyncCommand TueCommand { get; }
        public AsyncCommand WedCommand { get; }
        public AsyncCommand ThuCommand { get; }
        public AsyncCommand FriCommand { get; }
        public AsyncCommand SatCommand { get; }
        public int DayNumber { get => dayNumber; set => dayNumber = value; }

        public WeeklyViewModel()
        {
        }

        //method for dow buttons
        async Task sunButton()
        {
            DayNumber = 1;
            var route = $"{nameof(weekTasks)}?GoalId={goalId}";
            await Shell.Current.GoToAsync(route);
            var daynum = $"{nameof(weekTasks)}?daynumId={DayNumber}";
            await Shell.Current.GoToAsync(daynum);
        }
        async Task monButton()
        {
            DayNumber = 2;
            var route = $"{nameof(weekTasks)}?GoalId={goalId}";
            await Shell.Current.GoToAsync(route);
            var daynum = $"{nameof(weekTasks)}?daynumId={DayNumber}";
            await Shell.Current.GoToAsync(daynum);
        }
        async Task tueButton()
        {
            DayNumber = 3;
            var route = $"{nameof(weekTasks)}?GoalId={goalId}";
            await Shell.Current.GoToAsync(route);
            var daynum = $"{nameof(weekTasks)}?daynumId={DayNumber}";
            await Shell.Current.GoToAsync(daynum);
        }
        async Task wedButton()
        {
            DayNumber = 4;
            var route = $"{nameof(weekTasks)}?GoalId={goalId}";
            await Shell.Current.GoToAsync(route);
            var daynum = $"{nameof(weekTasks)}?daynumId={DayNumber}";
            await Shell.Current.GoToAsync(daynum);
        }
        async Task thuButton()
        {
            DayNumber = 5;
            var route = $"{nameof(weekTasks)}?GoalId={goalId}";
            await Shell.Current.GoToAsync(route);
            var daynum = $"{nameof(weekTasks)}?daynumId={DayNumber}";
            await Shell.Current.GoToAsync(daynum);
        }
        async Task friButton()
        {
            DayNumber = 6;
            var route = $"{nameof(weekTasks)}?GoalId={goalId}";
            await Shell.Current.GoToAsync(route);
            var daynum = $"{nameof(weekTasks)}?daynumId={DayNumber}";
            await Shell.Current.GoToAsync(daynum);
        }
        async Task satButton()
        {
            DayNumber = 7;
            var route = $"{nameof(weekTasks)}?GoalId={goalId}";
            await Shell.Current.GoToAsync(route);
            var daynum = $"{nameof(weekTasks)}?daynumId={DayNumber}";
            await Shell.Current.GoToAsync(daynum);
        }


    }
}
