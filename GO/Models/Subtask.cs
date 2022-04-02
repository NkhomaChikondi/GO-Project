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
        public double Percentage { get; set; }
        public int RemainingDays { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsCompleted { get; set; }
        [Indexed]
        public int TaskId { get; set; }

    }
}
