using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Classes
{
    public class CheckCollection : ObservableCollection<Check>
    {
        public bool Contains(Guid id)
        {
            bool result = false;

            foreach (Check check in Items)
            {
                if (check.Id.Equals(id))
                {
                    result = true;

                    break;
                }
            }

            return result;
        }
        
    }
}
