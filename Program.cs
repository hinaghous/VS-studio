using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            CollectFIDUData();
        }

        private static void CollectFIDUData()
        {
            DateTime day = DateTime.UtcNow;
            var CSVFilesPath = Path.GetDirectoryName(@"C:\Users\2941003\Desktop\AOITester\");
            var OutputPath = @"C:\Users\2941003\Desktop\output\";


            var CSVfiles = Directory.GetFiles(CSVFilesPath, "*.csv");

            string[] allLines;
            int num = 0;

            foreach (var file in CSVfiles)
            {
                allLines = File.ReadAllLines(file)[1].Split(',').ToArray();
                File.AppendAllLines(OutputPath + "OutputFile_" + num + ".txt", allLines[2].Split());
                num++;
            }

            num = 0;

            foreach (var file in CSVfiles)
            {
                allLines = File.ReadAllLines(file).Skip(4).ToArray();
                File.AppendAllLines(OutputPath + "OutputFile_" + num + ".txt", allLines);
                num++;
            }

            num = 0;

            var TemptxtFilesPath = Path.GetDirectoryName(OutputPath);
            var TemptxtFiles = Directory.GetFiles(TemptxtFilesPath, "*.txt");

            foreach (var txtfile in TemptxtFiles)
            {
                XElement xElement = new XElement("AOIMeasurment",
                new XElement("Barcode",
                new XAttribute("Value", File.ReadAllLines(txtfile).Take(1).First()),
                new XElement("Measurments",
                from str in File.ReadAllLines(txtfile).Skip(1)
                select new XElement("Measurment", new XAttribute("CRD", str.Split(',')[0]), new XAttribute("ArrayID", str.Split(',')[1]),
             new XAttribute("OffesetX", str.Split(',')[3]), new XAttribute("OffesetY", str.Split(',')[4]))))
            );

                xElement.Save(OutputPath + "xmlout" + num + ".xml");
                num++;
            }

            var one = XElement.Load(OutputPath + "xmlout0.xml");
            var two = XElement.Load(OutputPath + "xmlout1.xml");
            var three = XElement.Load(OutputPath + "xmlout2.xml");

            one.Add(two.FirstNode);
            one.Save(OutputPath + "xmlout3.xml");

            var four = XElement.Load(OutputPath + "xmlout3.xml");
            four.Add(three.FirstNode);
            var path = (OutputPath + "Barcode" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xml");
            four.Save(path);

            foreach (var txtfile in TemptxtFiles)
            {
                File.Delete(txtfile);
            }

            File.Delete(OutputPath + "xmlout0.xml");
            File.Delete(OutputPath + "xmlout1.xml");
            File.Delete(OutputPath + "xmlout2.xml");
            File.Delete(OutputPath + "xmlout3.xml");

            
            //var OutputPath = @"C:\Users\2941003\Desktop\output\";
            // To convert an XML node contained in string xml into a JSON string   
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            string jsonText = JsonConvert.SerializeXmlNode(doc.FirstChild.NextSibling);
            var myCleanJson = jsonText.Replace('@', ' ');
            var myCleanJsonObject = JObject.Parse(myCleanJson);

            //write a json into text file
            //open file stream
            using (StreamWriter file = File.CreateText(OutputPath + "Barcode" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt"))
            {
                JsonSerializer serializer = new JsonSerializer();
                //serialize object directly into file stream
                serializer.Serialize(file, myCleanJsonObject);


            }

        }


    }
}
    

