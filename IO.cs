using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;


namespace intelligence_bot
{
    class IO
    {
        public static Dictionary<string, string> readINI(string filename)
        {
            string[] lines = File.ReadAllLines(filename);
            Dictionary<string, string> config = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                if(line.Length < 4)
                {
                    continue;
                }
                if(line.StartsWith("["))
                {
                    continue;
                }
                int index = line.IndexOf('=');
                config.Add(line.Substring(0, index), line.Substring(index + 1, line.Length - index - 1));
            }
            return config;
        }

        public static void saveXML<T>(string filename, T value)
        {
            try
            {
                XmlDocument xml = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(value.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, value);
                    stream.Position = 0;
                    xml.Load(stream);
                    xml.Save(filename);
                }
            } catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

        public static T readXML<T>(string filename, T defaultvalue, string defaultlog = "Returning default value.")
        {
            try
            {
                using (StreamReader stream = new StreamReader(filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    Console.WriteLine("Reading file: " + filename);
                    return (T)serializer.Deserialize(stream);
                }
            } catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.\n" + defaultlog);
                return defaultvalue;
            } catch (Exception e)
            {
                Console.Error.WriteLine(e);
                return defaultvalue;
            }
        }
    }
}
