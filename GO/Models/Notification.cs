using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class Notification
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Descrption { get; set; }
        public DateTime NotifyingDate { get; set; }
        [Indexed]
        public int GoalId { get; set; }
        public int TaskId { get; set; }
        public int SubtaskId { get; set; }

    }
}
