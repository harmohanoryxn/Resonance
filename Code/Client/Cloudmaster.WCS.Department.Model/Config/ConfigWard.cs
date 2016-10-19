using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Department
{
	public class ConfigWard : ConfigurationElement
	{
		private static ConfigurationPropertyCollection _properties;
		private static ConfigurationProperty _name;
		private static ConfigurationProperty _code;
		private static ConfigurationProperty _sequence;
	
		static ConfigWard()
		{
			_name = new ConfigurationProperty("name",typeof(string),null,ConfigurationPropertyOptions.IsRequired);

			_code = new ConfigurationProperty("code",typeof(string),null,ConfigurationPropertyOptions.None);

			_sequence = new ConfigurationProperty("sequence",typeof(int),0,ConfigurationPropertyOptions.IsRequired);

			_properties = new ConfigurationPropertyCollection();

			_properties.Add(_name);
			_properties.Add(_code);
			_properties.Add(_sequence);
		}
	 
		public string Name
		{
			get { return (string)base[_name]; }
			set { base[_name] = value; }
		}

		public string Code
		{
			get { return (string)base[_code]; }
			set { base[_code] = value; }
		}

	 
		public int Sequence
		{
			get { return (int)base[_sequence]; }
			set { base[_sequence] = value; }
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
