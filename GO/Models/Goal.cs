﻿using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class Goal
    {

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public String Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime Time { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public double Percentage { get; set; }
        public double ExpectedPercentage { get; set; }
        public string Status { get; set; }
        public double Progress { get; set; }
        public bool WithDuration { get; set; }
        public bool HasWeek { get; set; }
        public bool Noweek { get; set; }
        public int NumberOfWeeks { get; set; }

        [Indexed]
        public int CategoryId { get; set; }


    }
}
