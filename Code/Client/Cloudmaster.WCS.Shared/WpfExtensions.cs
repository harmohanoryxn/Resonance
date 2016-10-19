 

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Markup;
using System.Xml;

namespace WCS.Shared
{
	public static class WpfExtensions<T>
	{
		//public static T Clone(T from)
		//{
		//    using (MemoryStream s = new MemoryStream())
		//    {
		//        BinaryFormatter f = new BinaryFormatter();
		//        f.Serialize(s, from);
		//        s.Position = 0;
		//        object clone = f.Deserialize(s);

		//        return (T)clone;
		//    }
		//}
	//}

	//public static class WpfExtensions<T>
	//{
		public static T Clone(T from)
		{
			string gridXaml = XamlWriter.Save(from); 
			StringReader stringReader = new StringReader(gridXaml);
			XmlReader xmlReader = XmlReader.Create(stringReader);
			T newObj = (T)XamlReader.Load(xmlReader);
			return newObj;
		}
	}
}
