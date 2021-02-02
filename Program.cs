using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace trying
{
    class Program
    {
        private class Machine
        {
            public string MachineID_Value { get; set; }
            public string JobFile_Value { get; set; }
            public string Barcode_Value { get; set; }
            public DateTime StartTime_Value { get; set; }
            public DateTime EndTime_Value { get; set; }
            public List<MachineRows> MachineRowData { get; set; }
        }
        private static List<Machine> MachineData = new List<Machine>();

        private class MachineRows
        {
            public string CRD_Value { get; set; }
            public short ArrayID_Value { get; set; }
            public double OffsetX_Value { get; set; }
            public double OffsetY_Value { get; set; }
        }
        private static List<MachineRows> MachineRowData = new List<MachineRows>();

        static void Main(string[] args)
        {
           

            {
                DateTime day = DateTime.UtcNow;
                Console.WriteLine("Collecting FIDU data for: " + day.ToString("yyyy-MM-dd") + "...");
            }
            string line;
            string[] split;
            bool isHeader1Row = true;
            bool isHeader2Row = false;
            bool isOffsetData = false;

            int machineID_ColIndex = -1;
            int jobFile_ColIndex = -1;
            int barcode_ColIndex = -1;
            int startTime_ColIndex = -1;
            int endTime_ColIndex = -1;
            int cRD_ColIndex = -1;
            int arrayID_ColIndex = -1;
            int offsetX_ColIndex = -1;
            int offsetY_ColIndex = -1;

            var CSVFilesPath = Path.GetDirectoryName(@"C:\Users\2941003\Desktop\AOITester\");
            var CSVfiles = Directory.GetFiles(CSVFilesPath, "*.csv");
            
            foreach (var file in CSVfiles)
            {
              isHeader1Row = true;
                isHeader2Row = false;
                isOffsetData = false;

                machineID_ColIndex = -1;
                jobFile_ColIndex = -1;
                barcode_ColIndex = -1;
                startTime_ColIndex = -1;
                endTime_ColIndex = -1;
                cRD_ColIndex = -1;
                arrayID_ColIndex = -1;
                offsetX_ColIndex = -1;
                offsetY_ColIndex = -1;

                try
                {
                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (var reader = new StreamReader(fileStream))
                        {
                            while ((line = reader.ReadLine()) != null)
                            {
                                split = line.Split(',');

                                if (isHeader1Row)
                                {
                                    if (string.IsNullOrWhiteSpace(line))
                                    {
                                        continue;
                                    }

                                    for (int i = 0; i < split.Length; i++)
                                    {
                                        switch (split[i].Trim().ToUpper())
                                        {
                                            case "MACHINEID":
                                                machineID_ColIndex = i;
                                                break;
                                            case "JOBFILE":
                                                jobFile_ColIndex = i;
                                                break;
                                            case "BARCODE":
                                                barcode_ColIndex = i;
                                                break;
                                            case "STARTTIME":
                                                startTime_ColIndex = i;
                                                break;
                                            case "ENDTIME":
                                                endTime_ColIndex = i;
                                                break;
                                        }
                                    }

                                    if (machineID_ColIndex == -1)
                                    {
                                        throw new Exception("Invalid AOILog, MachineID is missing!");
                                    }
                                    if (jobFile_ColIndex == -1)
                                    {
                                        throw new Exception("Invalid AOILog, JobFile is missing!");
                                    }
                                    if (barcode_ColIndex == -1)
                                    {
                                        throw new Exception("Invalid AOILog, Barcode is missing!");
                                    }
                                    if (startTime_ColIndex == -1)
                                    {
                                        throw new Exception("Invalid AOILog, StartTime is missing!");
                                    }
                                    if (endTime_ColIndex == -1)
                                    {
                                        throw new Exception("Invalid AOILog, EndTime is missing!");
                                    }

                                    isHeader1Row = false;
                                }
                                else if (!isHeader2Row)
                                {
                                    if (string.IsNullOrWhiteSpace(line))
                                    {
                                        isHeader2Row = true;
                                        continue;
                                    }


                                    MachineData.Add(new Machine
                                    {
                                        MachineID_Value = split[machineID_ColIndex].Trim(),
                                        JobFile_Value = split[jobFile_ColIndex].Trim(),
                                        Barcode_Value = split[barcode_ColIndex].Trim(),
                                        StartTime_Value = Convert.ToDateTime(split[startTime_ColIndex].Trim()),
                                        EndTime_Value = Convert.ToDateTime(split[endTime_ColIndex].Trim()),
                                        MachineRowData = new List<MachineRows>()
                                    });
                                }
                                else if (isHeader2Row)
                                {
                                    if (string.IsNullOrWhiteSpace(line))
                                    {
                                        continue;
                                    }
                                    else if (!isOffsetData)
                                    {
                                        for (int i = 0; i < split.Length; i++)
                                        {
                                            switch (split[i].Trim().ToUpper())
                                            {
                                                case "CRD":
                                                    cRD_ColIndex = i;
                                                    break;
                                                case "ARRAYID":
                                                    arrayID_ColIndex = i;
                                                    break;
                                                case "OFFSETX":
                                                    offsetX_ColIndex = i;
                                                    break;
                                                case "OFFSETY":
                                                    offsetY_ColIndex = i;
                                                    break;
                                            }
                                        }

                                        isOffsetData = true;
                                        continue;
                                    }
                                    else if (isOffsetData)
                                    {
                                        if (string.IsNullOrWhiteSpace(line))
                                        {
                                            continue;
                                        }
                                        if (cRD_ColIndex == -1)
                                        {
                                            throw new Exception("Invalid AOILog, CRD is missing!");
                                        }
                                        if (arrayID_ColIndex == -1)
                                        {
                                            throw new Exception("Invalid AOILog, ArrayID is missing!");
                                        }
                                        if (offsetX_ColIndex == -1)
                                        {
                                            throw new Exception("Invalid AOILog, OffsetX is missing!");
                                        }
                                        if (offsetY_ColIndex == -1)
                                        {
                                            throw new Exception("Invalid AOILog, OffsetY is missing!");
                                        }
                                        MachineData[MachineData.Count - 1].MachineRowData.Add(new MachineRows
                                        {
                                            CRD_Value = split[cRD_ColIndex].Trim(),
                                            ArrayID_Value = Convert.ToInt16(split[arrayID_ColIndex].Trim()),
                                            OffsetX_Value = Convert.ToDouble(split[offsetX_ColIndex].Trim(), CultureInfo.InvariantCulture),
                                            OffsetY_Value = Convert.ToDouble(split[offsetY_ColIndex].Trim(), CultureInfo.InvariantCulture)
                                        });



                                    }
                                }
                            }
                           
                            //XML generation
                            var OutputPath = @"C:\Users\2941003\Desktop\output\";
                            var hi = (OutputPath + "Barcode" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xml");
                            foreach (var m in MachineData)
                            {
                                XElement xElement = new XElement("AOIMeasurment",
                                new XElement("Barcode",
                                new XAttribute("Value", m.Barcode_Value),
                                new XElement("Measurments",
                                from rows in m.MachineRowData
                                select new XElement("Measurment", new XAttribute("CRD", rows.CRD_Value), new XAttribute("ArrayID", rows.ArrayID_Value),
                                new XAttribute("OffesetX", rows.OffsetX_Value), new XAttribute("OffesetY", rows.OffsetY_Value))))
                                );
                               
                                xElement.Save(hi);
                            }


                            
                            // To convert an XML node contained in string xml into a JSON string   
                            XmlDocument doc = new XmlDocument();
                            doc.Load(hi);
                            string jsonText = JsonConvert.SerializeXmlNode(doc.FirstChild.NextSibling);
                            var myCleanJson = jsonText.Replace('@', ' ');
                            var myCleanJsonObject = JObject.Parse(myCleanJson);

                            //write a json into text file
                            //open file stream
                            using (StreamWriter f = File.CreateText(OutputPath + "Barcode" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt"))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                //serialize object directly into file stream
                                serializer.Serialize(f, myCleanJsonObject);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}

    

