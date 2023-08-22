using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class GoalTask //: ObservableObject
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string taskName { get; set; }
        public string Description { get; set; }
        public DateTime StartTask { get; set; }
        public DateTime EndTask { get; set; }
        public int RemainingDays { get; set; }
        public double Percentage { get; set; }
        public int Accumulatedpercentage { get; set; }     
        public bool IsCompleted { get; set; }       
        public double Progress { get; set; }
        public double PendingPercentage { get; set; }
        public bool IsEnabled { get; set; }
        public string Status { get; set; }
        public bool IsVisible { get; set; }
        public string enddatetostring { get; set; }
        public bool IsNotVisible { get; set; }
        public int CompletedSubtask { get; set; }
        public int SubtaskNumber { get; set; }
        public bool Isrepeated { get; set; }
        public DateTime CreatedOn { get; set; }
       

        [Indexed]
        public int GoalId { get; set; }
        public int WeekId { get; set; }
       
    }
}
