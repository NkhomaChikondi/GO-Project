using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class Task_Day
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int Taskid { get; set; }
        public int DowId { get; set; }
        public bool Iscomplete { get; set; }
        public double Percentage { get; set; }
    }
}
