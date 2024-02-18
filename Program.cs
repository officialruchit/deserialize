using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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

            // Validate XML format and parse devices
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

            // Convert the list of devices to a dictionary for easier access
            while (true)
            {
                Console.WriteLine("\nPlease select an option:");
                Console.WriteLine("[1] Show all devices");
                Console.WriteLine("[2] Search devices by serial number");
                Console.WriteLine("[3] Exit");

                string choice = Console.ReadLine().Trim();

                switch (choice)
                {
                    case "1":
                        ShowDevices(devicesDictionary);
                        break;
                    case "2":
                        Console.Write("Enter serial number of the device: ");
                        string serialNumber = Console.ReadLine().Trim();
                        SearchDevice(devicesDictionary, serialNumber);
                        break;
                    case "3":
                        Console.WriteLine("Program terminated.");
                        return;
                    default:
                        Console.WriteLine("Error: Invalid input. Please choose from the above options.");
                        break;
                }
            }
        }

        static Dictionary<string, Device> ParseXml(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(DeviceList));
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                DeviceList deviceList = (DeviceList)serializer.Deserialize(fileStream);
                Dictionary<string, Device> devicesDictionary = deviceList.Devices.ToDictionary(device => device.SerialNumber);

                // Validate devices
                int deviceIndex = 1;
                foreach (var device in deviceList.Devices)
                {
                    if (!IsValidDevice(device))
                    {
                        Console.WriteLine("Error: Invalid device information. Please refer below details.");
                        Console.WriteLine($"Device index: {deviceIndex}");
                        Console.WriteLine($"Serial Number: {device.SerialNumber}");
                        Console.WriteLine($"IP Address: {device.Address ?? "(Not present)"} {(IsValidIPAddress(device.Address) ? "" : "(Not supported format)")}");
                        Console.WriteLine($"Device Name: {device.DevName}");
                        Console.WriteLine($"Model Name: {device.ModelName}");
                        Console.WriteLine($"Type: {device.Type}");
                        Console.WriteLine($"Port Number: {device.CommSetting.PortNo}");
                        Console.WriteLine($"Use SSL: {device.CommSetting.UseSSL}");
                        Console.WriteLine($"Password: {device.CommSetting.Password}");
                        Console.WriteLine();
                    }

                    deviceIndex++;
                }

                return devicesDictionary;
            }
        }

        static bool IsValidDevice(Device device)
        {
            // Validate IP Address format
            if (string.IsNullOrWhiteSpace(device.Address) || !IsValidIPAddress(device.Address))
                return false;

            // Additional validation checks can be added here

            return true;
        }

        static bool IsValidIPAddress(string ipAddress)
        {
            // Basic validation for IPv4 address format
            return IPAddress.TryParse(ipAddress, out _);
        }

        static void ShowDevices(Dictionary<string, Device> devices)
        {
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("{0,-5} {1,-20} {2,-20} {3,-20} {4,-20} {5,-10} {6,-10} {7,-10}", "No", "Serial Number", "IP Address", "Device Name", "Model Name", "Type", "Port", "SSL", "Password");
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
            int i = 1;
            foreach (var device in devices.Values)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-20} {3,-20} {4,-20} {5,-10} {6,-10} {7,-10}", i++, device.SerialNumber, device.Address, device.DevName, device.ModelName, device.Type, device.CommSetting.PortNo, device.CommSetting.UseSSL, device.CommSetting.Password);
            }
            Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
        }

        static void SearchDevice(Dictionary<string, Device> devices, string serialNumber)
        {
            if (devices.ContainsKey(serialNumber))
            {
                Device device = devices[serialNumber];
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20} {4,-10} {5,-10} {6,-10}", "Serial Number", "IP Address", "Device Name", "Model Name", "Type", "Port", "SSL", "Password");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
                Console.WriteLine("{0,-20} {1,-20} {2,-20} {3,-20} {4,-10} {5,-10} {6,-10}", device.SerialNumber, device.Address, device.DevName, device.ModelName, device.Type, device.CommSetting.PortNo, device.CommSetting.UseSSL, device.CommSetting.Password);
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------------------------------------");
            }
            else
            {
                Console.WriteLine("Device not found.");
            }
        }
    }
}
