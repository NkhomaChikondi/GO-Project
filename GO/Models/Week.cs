﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class Week
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int WeekNumber{ get; set; }      
        public double TargetPercentage { get; set; }
        public double AccumulatedPercentage { get; set; }
        public bool Active { get; set; }
        public double Progress { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        //for weeks that are created by the app (not the user)
        public bool CreatedAutomatically { get; set; }

        [Indexed]
        public int GoalId { get; set; }
    }
}