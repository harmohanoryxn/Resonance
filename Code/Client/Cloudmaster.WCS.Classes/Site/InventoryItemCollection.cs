using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Classes
{
    public class InventoryItemCollection : ObservableCollection<InventoryItem>
    {
        public bool Contains(string type)
        {
            bool result = false;

            foreach (InventoryItem item in Items)
            {
                if (item.Type.Equals(type))
                {
                    result = true;

                    break;
                }
            }

            return result;
        }
        
    }
}
