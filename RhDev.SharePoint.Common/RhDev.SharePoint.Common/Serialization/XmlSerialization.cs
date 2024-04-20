using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RhDev.SharePoint.Common.Serialization
{
    public static class XmlSerialization
    {
        public static TType DeserializeString<TType>(string data) where TType : class
        {
            using (StringReader sr = new StringReader(data))
                return Deserialize<TType>(sr);
        }

        public static object DeserializeString(string data, Type type)
        {
            using (var stringReader = new StringReader(data))
                using (var reader = XmlReader.Create(stringReader)) 
                    return Deserialize(reader, type);
        }

        public static TType Deserialize<TType>(string sourceFilePath) where TType : class
        {
            using (FileStream fs = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
            {
                using (XmlTextReader reader = new XmlTextReader(fs))
                    return Deserialize<TType>(reader);
            }
        }

        public static TType Deserialize<TType>(XmlReader reader)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(TType));
                return (TType)serializer.Deserialize(reader);
            }
            catch (InvalidOperationException ex)
            {
                throw new XmlSerializationException("Error in deserialization", ex);
            }
        }

        public static object Deserialize(XmlReader reader, Type type)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                return serializer.Deserialize(reader);
            }
            catch (InvalidOperationException ex)
            {
                throw new XmlSerializationException("Error in deserialization", ex);
            }
        }

        public static TType Deserialize<TType>(TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TType));
            return (TType)serializer.Deserialize(reader);
        }

        public static TType Deserialize<TType>(StringReader source)
            where TType : class
        {
            using (XmlReader reader = new XmlTextReader(source))
                return Deserialize<TType>(reader);
        }

        public static void Serialize(object obj, string destFilePath)
        {
            using (TextWriter writer = new StreamWriter(destFilePath))
                Serialize(writer, obj);
        }

        public static void Serialize(object obj, string destFilePath, Encoding encoding)
        {
            using (TextWriter writer = new StreamWriter(destFilePath, false, encoding))
                Serialize(writer, obj);
        }

        public static StringWriter Serialize(object obj)
        {
            using (StringWriter writer = new StringWriter())
            {
                Serialize(writer, obj);
                return writer;
            }
        }

        public static void Serialize(TextWriter tw, object obj, Type objType)
        {
            XmlSerializer serializer = new XmlSerializer(objType);
            serializer.Serialize(tw, obj);
        }

        public static void Serialize(XmlWriter tw, object obj, Type objType)
        {
            XmlSerializer serializer = new XmlSerializer(objType);
            serializer.Serialize(tw, obj);
        }

        public static void Serialize<TDataObject>(TextWriter tw, TDataObject obj)
            where TDataObject : class
        {
            Serialize(tw, obj, typeof(TDataObject));
        }

        public static void Serialize<TDataObject>(XmlWriter tw, TDataObject obj)
            where TDataObject : class
        {
            Serialize(tw, obj, typeof(TDataObject));
        }

        public static void Serialize(TextWriter tw, object obj)
        {
            Serialize(tw, obj, obj.GetType());
        }

        public static void Serialize(XmlWriter tw, object obj)
        {
            Serialize(tw, obj, obj.GetType());
        }

        public static string SerializeToString(object obj)
        {
            using (StringWriter sw = Serialize(obj))
                return sw.ToString();
        }
    }
}
