using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class Subtask
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string SubName { get; set; }
        public DateTime SubStart { get; set; }
        public DateTime SubEnd { get; set; }
        public string Due_On { get; set; }
        public double Percentage { get; set; }
        public int RemainingDays { get; set; }
        public DateTime CreatedOn { get; set; }
        public string enddatetostring { get; set; }
        public string Status { get; set; }
        public bool IsCompleted { get; set; }
        [Indexed]
        public int TaskId { get; set; }

    }
}
