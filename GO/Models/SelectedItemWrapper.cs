using System;
using System.Collections.Generic;
using System.Text;

namespace GO.Models
{
    public class SelectedItemWrapper<T>
    {
        public bool IsSelected { get; set; }
        public T Item { get; set; }
    }
}

