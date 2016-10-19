using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Controls.Forms.Commands
{
    public class FormNavigationCommands
    {
        public static bool HandleBackNavigation()
        {
            
            bool result = false;
            /*
            if (NavigationIndex == (int)FormNavigationIndex.Check)
            {
                FormManager.Instance.AutoSave.Save(FormManager.Instance.SelectedForm);

                FormManager.Instance.NavigationViewModel.NavigationIndex = (int)FormNavigationIndex.Form;
            }
            else if (NavigationIndex == (int)FormNavigationIndex.Image)
            {
                try
                {
                    ImageOperationCommands.SaveImage();
                }
                catch (Exception) { }

                Model.Model.Instance.SelectedImage = null;

                Model.Model.Instance.NavigationViewModel.NavigationIndex = (int)NavigationIndex.Check;
            }
 * */
            return result;
            
        }

    }
}
