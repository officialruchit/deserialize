using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace DeviceManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlFilePath;

            if (args.Length == 1)
            {
                xmlFilePath = args[0];
            }
            else
            {
                Console.WriteLine("Error: Invalid input. Program usage is as below.");
                Console.WriteLine("[DeviceUtil.exe] [XML file path]");
                Console.WriteLine("DeviceUtil.exe : Name of the executable file");
                Console.WriteLine("If anyone changes the name of the EXE, then the executable file name in usage should change accordingly.");
                Console.WriteLine("Terminate program.");
                return;
            }

            // XML file validation
            if (!File.Exists(xmlFilePath))
            {
                Console.WriteLine("Error: File does not exist. Please provide a valid file path.");
                Console.WriteLine("Terminate program.");
                return;
            }

            if (Path.GetExtension(xmlFilePath).ToLower() != ".xml")
            {
                Console.WriteLine("Error: Given file is not an XML file. The file extension is wrong.");
                Console.WriteLine("Terminate program.");
                return;
            }

            // Validate XML against XSD
            if (!ValidateXml(xmlFilePath, "DeviceSchema.xsd"))
            {
                Console.WriteLine("XML validation failed. Terminate program.");
                return;
            }

            // Proceed with XML parsing and processing devices
            Dictionary<string, Device> devicesDictionary;
            try
            {
                devicesDictionary = ParseXml(xmlFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: An unexpected error occurred while parsing XML. " + ex.Message);
                Console.WriteLine("Terminate program.");
                return;
            }

            // Continue with your logic for processing devices...
        }

        static bool ValidateXml(string xmlFilePath, string xsdFilePath)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas.Add(null, xsdFilePath);

            bool isValid = true;
            settings.ValidationEventHandler += (sender, e) =>
            {
                isValid = false;
                Console.WriteLine($"Validation error: {e.Message}");
            };

            using (XmlReader reader = XmlReader.Create(xmlFilePath, settings))
            {
                try
                {
                    while (reader.Read()) ;
                    Console.WriteLine("XML is valid.");
                }
                catch (XmlException ex)
                {
                    isValid = false;
                    Console.WriteLine($"XML validation error: {ex.Message}");
                }
            }

            return isValid;
        }

        static Dictionary<string, Device> ParseXml(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceList));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                DeviceList deviceList = (DeviceList)serializer.Deserialize(fileStream);
                Dictionary<string, Device> devicesDictionary = deviceList.Devices.ToDictionary(device => device.SerialNumber);
                return devicesDictionary;
            }
        }

        // Define your Device and DeviceList classes here as per your XML structure
        // Define your Device validation logic here as per your requirements
        // Define your methods for displaying devices, searching devices, etc.
    }
}
