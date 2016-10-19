using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Cloudmaster.WCS.Classes
{
    public partial class TaskCollection : ObservableCollection<Task>
    {
        public bool Contains(string roomId)
        {
            bool result = false;

            foreach (Task task in Items)
            {
                if (task.Room.CompareTo(roomId) == 0)
                {
                    result = true;

                    break;
                }
            }

            return result;
        }
    }
}
