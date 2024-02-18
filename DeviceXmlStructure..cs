using System.Collections.Generic;
using System.Xml.Serialization;

namespace DeviceManager
{
    [XmlRoot("Devices")]
    public class DeviceList
    {
        [XmlElement("Dev")]
        public List<Device> Devices { get; set; }
    }

    public class Device
    {
        [XmlAttribute("SrNo")]
        public string SerialNumber { get; set; }

        public string Address { get; set; }
        public string DevName { get; set; }
        public string ModelName { get; set; }
        public string Type { get; set; }
        public CommunicationSettings CommSetting { get; set; }
    }

    public class CommunicationSettings
    {
        public string PortNo { get; set; }
        public string UseSSL { get; set; }
        public string Password { get; set; }
    }
}
