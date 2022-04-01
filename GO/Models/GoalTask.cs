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
        public bool IsCompleted { get; set; }
        public double Progress { get; set; }
        public double PendingPercentage { get; set; }
        public string Status { get; set; }
        public int SubtaskNumber { get; set; }
        public int CompletedSubtask { get; set; }
        public DateTime CreatedOn { get; set; }
       [Indexed]
        public int GoalId { get; set; }
    }
}
