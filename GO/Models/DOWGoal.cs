using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    // a linking model class between Dows and goals
    public class DOWGoal
    {
        [PrimaryKey, AutoIncrement]
        public int DOGid { get; set; }
        [Indexed]
        public int GoalId { get; set; }
        [Indexed]
        public int DowId { get; set; }

    }
}
