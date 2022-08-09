using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
      public class GoalWeek
      {
        [PrimaryKey,AutoIncrement]
        public int ID { get; set; }
        public int GoalId { get; set; } 

      }
}
