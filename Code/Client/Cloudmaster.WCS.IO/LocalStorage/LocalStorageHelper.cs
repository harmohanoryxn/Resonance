using System;
using System.IO;

namespace Cloudmaster.WCS.IO
{
    public class LocalStorageHelper
    {
        public static string GetLocalAppplicationFolderForTabletPCApplication()
        {
            string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string localApplicationFolder = string.Format(@"{0}\ClouldMaster\ISM\TabletPC", localApplicationData);

            if (!Directory.Exists(localApplicationFolder))
            {
                Directory.CreateDirectory(localApplicationFolder);
            }

            return localApplicationFolder;
        }

        public static string GetAbsoluteFilenameInLocalStorage(string relativeFilename)
        {
            string localApplicationFolder = GetLocalAppplicationFolderForTabletPCApplication();

            string localApplicationFilename = string.Format(@"{0}\{1}", localApplicationFolder, relativeFilename);

            return localApplicationFilename;
        }

        public static string GetAbsoluteFilenameInLocalStorage(string relativeFolder, string relativeFilename)
        {
            string localApplicationFolder = GetLocalAppplicationFolderForTabletPCApplication();

            string subFolder = string.Format(@"{0}\{1}", localApplicationFolder, relativeFolder);

            if (!Directory.Exists(subFolder))
            {
                Directory.CreateDirectory(subFolder);
            }

            string localApplicationFilename = string.Format(@"{0}\{1}", subFolder, relativeFilename);

            return localApplicationFilename;
        }
    }
}
