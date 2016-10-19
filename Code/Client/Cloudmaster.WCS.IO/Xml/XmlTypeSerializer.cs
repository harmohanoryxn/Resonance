using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Cloudmaster.WCS.IO
{
    public class XmlTypeSerializer<T>
    {
        public static void SerializeAndOverwriteFile(T obj, string filename)
        {
            using (FileStream fileStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                SerializeAndOverwriteFile(obj, fileStream);
            }
        }

        public static void SerializeAndOverwriteFile(T obj, Stream stream)
        {
            using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.Namespaces = true;

                XmlSerializer serializer = new XmlSerializer(typeof(T));

                serializer.Serialize(writer, obj);

                writer.Close();
                stream.Close();
            }
        }

        public static T Deserialize(string filename)
        {
            T result;

            using (XmlTextReader reader = new XmlTextReader(filename))
            {
                result = Deserialize(reader);
            }

            return result;
        }

        public static T Deserialize(Stream stream)
        {
            T result;

            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                result = Deserialize(reader);
            }

            return result;
        }

        public static T Deserialize(XmlTextReader reader)
        {
            T result;

            reader.WhitespaceHandling = WhitespaceHandling.None;

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            result = (T)serializer.Deserialize(reader);

            reader.Close();

            return result;
        }

        public static void TryParseFile(string file, out T result)
        {
            result = default(T);

            try
            {
                result = XmlTypeSerializer<T>.Deserialize(file);
            }
            catch { }
        }
    }
}
