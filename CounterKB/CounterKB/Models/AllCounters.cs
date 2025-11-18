using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Maui.Storage;

namespace CounterKB.Models
{
    public class AllCounters
    {
        const string XmlFileName = "counters.xml";

        public ObservableCollection<Counter> Counters { get; set; } = new();

        public AllCounters()
        {
            LoadCounters();
        }

        string GetFilePath()
        {
            string appDataPath = FileSystem.AppDataDirectory;
            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);
            return Path.Combine(appDataPath, XmlFileName);
        }

        public void LoadCounters()
        {
            Counters.Clear();
            string path = GetFilePath();

            if (!File.Exists(path))
                return;

            XElement root = XElement.Load(path);
            var items = root.Elements("Counter")
                            .Select(x =>
                            {
                                var id = (string)x.Attribute("Id") ?? Guid.NewGuid().ToString();
                                var name = (string)x.Element("Nazwa") ?? "Unnamed";
                                var valueStr = (string)x.Element("Wartosc") ?? "0";
                                int value = 0;
                                int.TryParse(valueStr, out value);

                                return new Counter
                                {
                                    Filename = id,
                                    Name = name,
                                    Value = value
                                };
                            });

            foreach (var c in items)
                Counters.Add(c);
            
        }

        public Counter AddCounter(string name)
        {
            var counter = new Counter
            {
                Filename = Guid.NewGuid().ToString(),
                Name = name,
                Value = 0
            };

            Counters.Insert(0, counter);
            SaveAllCounters();
            return counter;
        }

        public void SaveAllCounters()
        {
            string path = GetFilePath();

            var root = new XElement("Counters",
                    Counters.Select(c =>
                        new XElement("Counter",
                            new XAttribute("Id", c.Filename ?? Guid.NewGuid().ToString()),
                            new XElement("Nazwa", c.Name ?? string.Empty),
                            new XElement("Wartosc", c.Value.ToString())
                        )
                    )
                );
            root.Save(path);

        }

        public void SaveCounter(Counter counter)
        {
            SaveAllCounters();
        }

       
    }
}
