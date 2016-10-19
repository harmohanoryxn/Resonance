using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Cloudmaster.WCS.Department
{
	public class ConfigWardCollection : ConfigurationElementCollection
	{
		public ConfigWardCollection()
		{
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}
		protected override string ElementName
		{
			get
			{
				return "ward";
			}
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get
			{
				return new ConfigurationPropertyCollection();
			}
		}

		public ConfigWard this[int index]
		{
			get
			{
				return (ConfigWard)base.BaseGet(index);
			}
			set
			{
				if (base.BaseGet(index) != null)
				{
					base.BaseRemoveAt(index);
				}
				base.BaseAdd(index, value);
			}
		}

		public ConfigWard this[string name]
		{
			get
			{
				return (ConfigWard)base.BaseGet(name);
			}
		}

		public void Add(ConfigWard item)
		{
			base.BaseAdd(item);
		}

		public void Remove(ConfigWard item)
		{
			base.BaseRemove(item);
		}

		public void RemoveAt(int index)
		{
			base.BaseRemoveAt(index);
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ConfigWard();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return (element as ConfigWard).Name;
		}
	}

}
