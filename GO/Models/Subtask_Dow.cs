using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class Subtask_Dow
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Indexed]
        public int Subtataskid { get; set; }
        public int DowId { get; set; }
        public bool Iscomplete { get; set; }
    }
}
