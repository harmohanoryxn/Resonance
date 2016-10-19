using System.Configuration;

namespace WCS.Shared
{
    public class ConfigurationHelper
    {
        public static ConnectionStringsSection GetConnectionStringsAndEncryptIfNotProtected(string applicationName)
        {
            // Allows editting of connection strings which are encrypted the next time application is run

            Configuration config = ConfigurationManager.OpenExeConfiguration(applicationName);

            ConnectionStringsSection section = config.GetSection("connectionStrings") as ConnectionStringsSection;

            if (!section.SectionInformation.IsProtected)
            {
                section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");

                config.Save();
            }

            section.SectionInformation.UnprotectSection();

            return section;
        }
    }
}
