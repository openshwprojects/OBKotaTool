using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PROTATool
{
    class OBKDevice
    {
        string adr;
        int webRequestTimeOut = 3000;
        public class Status
        {
            public int Module { get; set; }
            public string DeviceName { get; set; }
            public List<string> FriendlyName { get; set; }
            public string Topic { get; set; }
            public string ButtonTopic { get; set; }
            public int Power { get; set; }
            public int PowerOnState { get; set; }
            public int LedState { get; set; }
            public string LedMask { get; set; }
            public int SaveData { get; set; }
            public int SaveState { get; set; }
            public string SwitchTopic { get; set; }
            public List<int> SwitchMode { get; set; }
            public int ButtonRetain { get; set; }
            public int SwitchRetain { get; set; }
            public int SensorRetain { get; set; }
            public int PowerRetain { get; set; }
            public int InfoRetain { get; set; }
            public int StateRetain { get; set; }
        }

        public class StatusPRM
        {
            public int Baudrate { get; set; }
            public string SerialConfig { get; set; }
            public string GroupTopic { get; set; }
            public string OtaUrl { get; set; }
            public string RestartReason { get; set; }
            public int Uptime { get; set; }
            public string StartupUTC { get; set; }
            public int Sleep { get; set; }
            public int CfgHolder { get; set; }
            public int BootCount { get; set; }
            public string BCResetTime { get; set; }
            public int SaveCount { get; set; }
            public string SaveAddress { get; set; }
        }

        public class StatusFWR
        {
            public string Version { get; set; }
            public string BuildDateTime { get; set; }
            public int Boot { get; set; }
            public string Core { get; set; }
            public string SDK { get; set; }
            public int CpuFrequency { get; set; }
            public string Hardware { get; set; }
            public string CR { get; set; }
        }

        public class StatusLOG
        {
            public int SerialLog { get; set; }
            public int WebLog { get; set; }
            public int MqttLog { get; set; }
            public int SysLog { get; set; }
            public string LogHost { get; set; }
            public int LogPort { get; set; }
            public string SSId1 { get; set; }
            public string SSId2 { get; set; }
            public int TelePeriod { get; set; }
            public string Resolution { get; set; }
            public List<string> SetOption { get; set; }
        }

        public class StatusMEM
        {
            public int ProgramSize { get; set; }
            public int Free { get; set; }
            public int Heap { get; set; }
            public int ProgramFlashSize { get; set; }
            public int FlashSize { get; set; }
            public string FlashChipId { get; set; }
            public int FlashFrequency { get; set; }
            public int FlashMode { get; set; }
            public List<string> Features { get; set; }
            public string Drivers { get; set; }
            public string Sensors { get; set; }
        }

        public class StatusNET
        {
            public string Hostname { get; set; }
            public string IPAddress { get; set; }
            public string Gateway { get; set; }
            public string Subnetmask { get; set; }
            public string DNSServer1 { get; set; }
            public string DNSServer2 { get; set; }
            public string Mac { get; set; }
            public int Webserver { get; set; }
            public int HTTP_API { get; set; }
            public int WifiConfig { get; set; }
            public double WifiPower { get; set; }
        }

        public class StatusMQT
        {
            public string MqttHost { get; set; }
            public int MqttPort { get; set; }
            public string MqttClientMask { get; set; }
            public string MqttClient { get; set; }
            public string MqttUser { get; set; }
            public int MqttCount { get; set; }
            public int MAX_PACKET_SIZE { get; set; }
            public int KEEPALIVE { get; set; }
            public int SOCKET_TIMEOUT { get; set; }
        }

        public class StatusTIM
        {
            public string UTC { get; set; }
            public string Local { get; set; }
            public string StartDST { get; set; }
            public string EndDST { get; set; }
            public string Timezone { get; set; }
            public string Sunrise { get; set; }
            public string Sunset { get; set; }
        }

        public class Wifi
        {
            public int AP { get; set; }
            public string SSId { get; set; }
            public string BSSId { get; set; }
            public int Channel { get; set; }
            public string Mode { get; set; }
            public int RSSI { get; set; }
            public int Signal { get; set; }
            public int LinkCount { get; set; }
            public string Downtime { get; set; }
        }

        public class StatusSTS
        {
            public string Time { get; set; }
            public string Uptime { get; set; }
            public int UptimeSec { get; set; }
            public int Heap { get; set; }
            public string SleepMode { get; set; }
            public int Sleep { get; set; }
            public int LoadAvg { get; set; }
            public int MqttCount { get; set; }
            public string POWER1 { get; set; }
            public string POWER12 { get; set; }
            public string POWER13 { get; set; }
            public string POWER14 { get; set; }
            public string POWER15 { get; set; }
            public string POWER16 { get; set; }
            public string POWER17 { get; set; }
            public string POWER18 { get; set; }
            public Wifi Wifi { get; set; }
        }

        public class Root
        {
            public Status Status { get; set; }
            public StatusPRM StatusPRM { get; set; }
            public StatusFWR StatusFWR { get; set; }
            public StatusLOG StatusLOG { get; set; }
            public StatusMEM StatusMEM { get; set; }
            public StatusNET StatusNET { get; set; }
            public StatusMQT StatusMQT { get; set; }
            public StatusTIM StatusTIM { get; set; }
            public StatusSTS StatusSTS { get; set; }
        }

        // Helper method to read the response stream fully into a byte array (async version)
        static async Task<byte[]> ReadFullyAsync(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await memoryStream.WriteAsync(buffer, 0, bytesRead);
                }

                return memoryStream.ToArray();
            }
        }

        public async Task<Root> GetStatusAsync()
        {
            return await SendGetInternalJSONAsync<Root>("cm?cmnd=STATUS");
        }

        public async Task<OBKInfo> GetOBKInfoAsync()
        {
            return await SendGetInternalJSONAsync<OBKInfo>("api/info");
        }
        public async Task<string> GetHardwareAsync()
        {
            try
            {
                var root = await GetStatusAsync();
                if (root?.StatusFWR?.Hardware != null)
                {
                    return root.StatusFWR.Hardware;
                }
            }
            catch(Exception ex)
            {
            }
            try
            {
                var obkInfo = await GetOBKInfoAsync();
                if (obkInfo?.chipset != null)
                {
                    return obkInfo.chipset;
                }
            }
            catch (Exception)
            {
                // Optionally log the exception if needed
            }

            return "Unknown"; // Default value if both attempts fail
        }
        public async Task<T> SendGetInternalJSONAsync<T>(string path)
        {
            byte[] responseBytes = await SendGetInternalAsync(path);
            string s = responseBytes != null ? Encoding.ASCII.GetString(responseBytes) : null;
            return JsonSerializer.Deserialize<T>(s);
        }

        public async Task<string> SendGetInternalStringAsync(string path)
        {
            byte[] responseBytes = await SendGetInternalAsync(path);
            string s = responseBytes != null ? Encoding.ASCII.GetString(responseBytes) : null;
            Console.Write(s);
            return s;
        }
        public class OBKInfo
        {
            public int uptime_s { get; set; }
            public string build { get; set; }
            public string ip { get; set; }
            public string mac { get; set; }
            public string flags { get; set; }
            public string mqtthost { get; set; }
            public string mqtttopic { get; set; }
            public string chipset { get; set; }
            public string webapp { get; set; }
            public string shortName { get; set; }
            public string startcmd { get; set; }
            public int supportsSSDP { get; set; }
            public bool supportsClientDeviceDB { get; set; }
        }

        public async Task<byte[]> SendGetInternalAsync(string path)
        {
            try
            {
                string fullRequestText = "http://" + adr + "/" + path;
                WebRequest request = WebRequest.Create(fullRequestText);
                request.Timeout = webRequestTimeOut;

                if (!ToggleAllowUnsafeHeaderParsing(true))
                {
                    // Couldn't set flag. Log the fact, throw an exception or whatever.
                }

                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        return await ReadFullyAsync(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            return null;
        }

        // Enable/disable useUnsafeHeaderParsing.
        // See http://o2platform.wordpress.com/2010/10/20/dealing-with-the-server-committed-a-protocol-violation-sectionresponsestatusline/
        public static bool ToggleAllowUnsafeHeaderParsing(bool enable)
        {
            //Get the assembly that contains the internal class
            Assembly assembly = Assembly.GetAssembly(typeof(SettingsSection));
            if (assembly != null)
            {
                //Use the assembly in order to get the internal type for the internal class
                Type settingsSectionType = assembly.GetType("System.Net.Configuration.SettingsSectionInternal");
                if (settingsSectionType != null)
                {
                    //Use the internal static property to get an instance of the internal settings class.
                    //If the static instance isn't created already invoking the property will create it for us.
                    object anInstance = settingsSectionType.InvokeMember("Section",
                        BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.NonPublic, null, null, new object[] { });
                    if (anInstance != null)
                    {
                        //Locate the private bool field that tells the framework if unsafe header parsing is allowed
                        FieldInfo aUseUnsafeHeaderParsing = settingsSectionType.GetField("useUnsafeHeaderParsing", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (aUseUnsafeHeaderParsing != null)
                        {
                            aUseUnsafeHeaderParsing.SetValue(anInstance, enable);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal void setAddress(string v)
        {
            this.adr = v;
        }
    }
}
