using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class SelectedItemWrapper<T>
    {
        [PrimaryKey,AutoIncrement]
        public bool IsSelected { get; set; }
        public T Item { get; set; }
        [Indexed]
        public int goalId { get; set; }
    }
}

