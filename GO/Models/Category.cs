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
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }


    }
}
