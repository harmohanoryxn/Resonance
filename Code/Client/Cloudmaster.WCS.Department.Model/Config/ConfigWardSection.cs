using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Department
{
	public class ConfigWardSection : ConfigurationSection
	{
		private static ConfigurationPropertyCollection _properties;
		private static ConfigurationProperty _name;
		private static ConfigurationProperty _wards;

		static ConfigWardSection()
		{
			_name = new ConfigurationProperty("name", typeof(string), null, ConfigurationPropertyOptions.IsRequired);

			_wards = new ConfigurationProperty("", typeof(ConfigWardCollection), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsDefaultCollection);

			_properties = new ConfigurationPropertyCollection();

			_properties.Add(_name);
			_properties.Add(_wards);
		}

		public string Name
		{
			get { return (string)base[_name]; }
			set { base[_name] = value; }
		}

		public ConfigWardCollection Wards
		{
			get { return (ConfigWardCollection)base[_wards]; }
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return _properties;
			}
		}
	}
}
