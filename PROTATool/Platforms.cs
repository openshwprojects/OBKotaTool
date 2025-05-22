using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace PROTATool
{
    class Platform
    {
        public string name;
        public string extension;
    }
    internal class Platforms
    {
        public static Platform[] list = CreatePlatformsArray(); 
        internal static Platform[] CreatePlatformsArray()
        {
            return new Platform[]
            {
            new Platform { name = "OpenBK7231N", extension = ".rbl" },
            new Platform { name = "OpenBK7231T", extension = ".rbl" },
            new Platform { name = "OpenBK7238", extension = ".rbl" },
            //new Platform { name = "OpenBL602", extension = ".bin.xz" },
            new Platform { name = "OpenBL602", extension = ".bin.xz.ota" },
           // new Platform { name = "OpenESP32C2", extension = ".factory.bin" },
            new Platform { name = "OpenESP32C2", extension = ".img" },
           // new Platform { name = "OpenESP32C3", extension = ".factory.bin" },
            new Platform { name = "OpenESP32C3", extension = ".img" },
            //new Platform { name = "OpenESP32C6", extension = ".factory.bin" },
            new Platform { name = "OpenESP32C6", extension = ".img" },
            //new Platform { name = "OpenESP32S2", extension = ".factory.bin" },
            new Platform { name = "OpenESP32S2", extension = ".img" },
          //  new Platform { name = "OpenESP32S3", extension = ".factory.bin" },
            new Platform { name = "OpenESP32S3", extension = ".img" },
           // new Platform { name = "OpenESP32", extension = ".factory.bin" },
            new Platform { name = "OpenESP32", extension = ".img" },
            new Platform { name = "OpenLN882H", extension = "_OTA.bin" },
            new Platform { name = "OpenTR6260", extension = ".bin" },
            new Platform { name = "OpenW600", extension = ".img" },
            new Platform { name = "OpenW800", extension = ".img" },
            new Platform { name = "OpenRTL87X0C", extension = ".img" },
            new Platform { name = "OpenRTL8710B", extension = ".img" }
            };
        }

        internal static Platform findForChip(string hardware)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].name.ToLower() == (hardware.ToLower()))
                {
                    return list[i];
                }
                if (list[i].name.ToLower() == (("Open" + hardware).ToLower()))
                {
                    return list[i];
                }
            }
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i].name.ToLower().Contains(hardware.ToLower()))
                {
                    return list[i];
                }
            }
            return null;
        }
    }
}
