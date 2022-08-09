using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class Category
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public String Name { get; set; }        
        public DateTime CreatedOn { get; set; }
        public int goalNumber { get; set; }
        public bool IsVisible { get; set; }


    }
}
