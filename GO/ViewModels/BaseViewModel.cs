using GO.Models;
using GO.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace GO.ViewModels
{
    public class BaseViewmodel
    {
        public IDataStore<Category> datastore { get; }
        public IDataGoal<Goal>datagoal { get; }
        public IDataTask<GoalTask> dataTask { get; }
        public IDataSubtask<Subtask> dataSubTask { get; }
        public IDowGoal<DOWGoal> dataDowgoal { get; }
        public BaseViewmodel()
        {
            // exposing Godataservice to all view models
            datastore = DependencyService.Get<IDataStore<Category>>();
            datagoal = DependencyService.Get<IDataGoal<Goal>>();
            dataTask = DependencyService.Get<IDataTask<GoalTask>>();
            dataSubTask = DependencyService.Get<IDataSubtask<Subtask>>();
            dataDowgoal = DependencyService.Get<IDowGoal<DOWGoal>>();

        }
        bool isBusy;
        string Title;
        public string title
        {
            // get the title
            get => Title;
            // setting the Title with an incoming value
            set
            {
                if (Title == value)
                    return;
                Title = value;
                OnPropertyChange();
            }
        }
        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy == value)
                    return;
                isBusy = value;
                OnPropertyChange();
                OnPropertyChange(nameof(IsNotBusy));

            }
        }
        public bool IsNotBusy => !IsBusy;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChange([CallerMemberName] string Name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
    }
}
