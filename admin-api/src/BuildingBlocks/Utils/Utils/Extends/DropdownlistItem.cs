using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public class DropdownlistItem
    {
        public String Text { get; set; }
        public String Value { get; set; }

        public DropdownlistItem(String Text, String Value)
        {
            this.Text = Text;
            this.Value = Value;
        }
    }
}
