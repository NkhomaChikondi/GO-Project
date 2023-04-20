using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace GO.Models
{
    // a class defining days of the week
    public class DOW
    {
        [PrimaryKey,AutoIncrement]
        public int DOWId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool IsSelected { get; set; }
        [Indexed]
        public int  WeekId { get; set; }


    }
}
