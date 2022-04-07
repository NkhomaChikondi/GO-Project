using SQLite;
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
        public string Status { get; set; }
        public double progress { get; set; }

        [Indexed]
        public int CategoryId { get; set; }


    }
}
