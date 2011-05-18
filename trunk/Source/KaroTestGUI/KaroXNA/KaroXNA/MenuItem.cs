using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaroXNA
{
    public class MenuItem
    {
        private String menuName;

        public String MenuName
        {
            get { return menuName; }
            set { menuName = value; }
        }

        public MenuItem(String name)
        {
            menuName = name;
        }
    }
}
