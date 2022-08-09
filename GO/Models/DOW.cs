using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    // a class defining days of the week
    public class DOW
    {
        [PrimaryKey,AutoIncrement]
        public int DOWId { get; set; }
        public string Name { get; set; }

       
    }
}
