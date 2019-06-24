using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalParamLibrary;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using NLog;

namespace ATSLibrary
{
    #region CRC
    public class CRCITU
    {
        /// <summary>  
                /// 计算给定长度数据的16位CRC  
                /// </summary>  
                /// <param name="data">要计算CRC的字节数组</param>  
                /// <returns>CRC值</returns>  
        public static UInt16 GetCrc16(Byte[] data)
        {   // 初始化  
            Int32 High = 0xFF;  // 高字节  
            Int32 Low = 0xFF;   // 低字节  
            if (data != null)
            {
                foreach (Byte b in data)
                {
                    Int32 Index = Low ^ b;
                    Low = High ^ CRC16TABLE_LO[Index];
                    High = CRC16TABLE_HI[Index];
                }
            }

            return (UInt16)(~((High << 8) + Low));    // 取反  
        }
        /// <summary>  
        /// 计算给定长度数据的32位CRC  
        /// </summary>  
        /// <param name="data">要计算CRC的字节数组</param>  
        /// <returns>CRC值</returns>  
        public static UInt32 GetCrc32(Byte[] data)
        {
            UInt32 crcValue = 0xFFFFFFFF;
            if (data != null)
            {
                foreach (Byte b in data)
                {
                    crcValue = (crcValue >> 8) ^ CRC32TABLE[(crcValue ^ b) & 0xFF];
                }
            }

            return ~crcValue;
        }

        private static readonly UInt32[] CRC32TABLE =
        {
        0x00000000,0x77073096,0xEE0E612C,0x990951BA,0x076DC419,0x706AF48F,0xE963A535,0x9E6495A3,
        0x0EDB8832,0x79DCB8A4,0xE0D5E91E,0x97D2D988,0x09B64C2B,0x7EB17CBD,0xE7B82D07,0x90BF1D91,
        0x1DB71064,0x6AB020F2,0xF3B97148,0x84BE41DE,0x1ADAD47D,0x6DDDE4EB,0xF4D4B551,0x83D385C7,
        0x136C9856,0x646BA8C0,0xFD62F97A,0x8A65C9EC,0x14015C4F,0x63066CD9,0xFA0F3D63,0x8D080DF5,
        0x3B6E20C8,0x4C69105E,0xD56041E4,0xA2677172,0x3C03E4D1,0x4B04D447,0xD20D85FD,0xA50AB56B,
        0x35B5A8FA,0x42B2986C,0xDBBBC9D6,0xACBCF940,0x32D86CE3,0x45DF5C75,0xDCD60DCF,0xABD13D59,
        0x26D930AC,0x51DE003A,0xC8D75180,0xBFD06116,0x21B4F4B5,0x56B3C423,0xCFBA9599,0xB8BDA50F,
        0x2802B89E,0x5F058808,0xC60CD9B2,0xB10BE924,0x2F6F7C87,0x58684C11,0xC1611DAB,0xB6662D3D,
        0x76DC4190,0x01DB7106,0x98D220BC,0xEFD5102A,0x71B18589,0x06B6B51F,0x9FBFE4A5,0xE8B8D433,
        0x7807C9A2,0x0F00F934,0x9609A88E,0xE10E9818,0x7F6A0DBB,0x086D3D2D,0x91646C97,0xE6635C01,
        0x6B6B51F4,0x1C6C6162,0x856530D8,0xF262004E,0x6C0695ED,0x1B01A57B,0x8208F4C1,0xF50FC457,
        0x65B0D9C6,0x12B7E950,0x8BBEB8EA,0xFCB9887C,0x62DD1DDF,0x15DA2D49,0x8CD37CF3,0xFBD44C65,
        0x4DB26158,0x3AB551CE,0xA3BC0074,0xD4BB30E2,0x4ADFA541,0x3DD895D7,0xA4D1C46D,0xD3D6F4FB,
        0x4369E96A,0x346ED9FC,0xAD678846,0xDA60B8D0,0x44042D73,0x33031DE5,0xAA0A4C5F,0xDD0D7CC9,
        0x5005713C,0x270241AA,0xBE0B1010,0xC90C2086,0x5768B525,0x206F85B3,0xB966D409,0xCE61E49F,
        0x5EDEF90E,0x29D9C998,0xB0D09822,0xC7D7A8B4,0x59B33D17,0x2EB40D81,0xB7BD5C3B,0xC0BA6CAD,
        0xEDB88320,0x9ABFB3B6,0x03B6E20C,0x74B1D29A,0xEAD54739,0x9DD277AF,0x04DB2615,0x73DC1683,
        0xE3630B12,0x94643B84,0x0D6D6A3E,0x7A6A5AA8,0xE40ECF0B,0x9309FF9D,0x0A00AE27,0x7D079EB1,
        0xF00F9344,0x8708A3D2,0x1E01F268,0x6906C2FE,0xF762575D,0x806567CB,0x196C3671,0x6E6B06E7,
        0xFED41B76,0x89D32BE0,0x10DA7A5A,0x67DD4ACC,0xF9B9DF6F,0x8EBEEFF9,0x17B7BE43,0x60B08ED5,
        0xD6D6A3E8,0xA1D1937E,0x38D8C2C4,0x4FDFF252,0xD1BB67F1,0xA6BC5767,0x3FB506DD,0x48B2364B,
        0xD80D2BDA,0xAF0A1B4C,0x36034AF6,0x41047A60,0xDF60EFC3,0xA867DF55,0x316E8EEF,0x4669BE79,
        0xCB61B38C,0xBC66831A,0x256FD2A0,0x5268E236,0xCC0C7795,0xBB0B4703,0x220216B9,0x5505262F,
        0xC5BA3BBE,0xB2BD0B28,0x2BB45A92,0x5CB36A04,0xC2D7FFA7,0xB5D0CF31,0x2CD99E8B,0x5BDEAE1D,
        0x9B64C2B0,0xEC63F226,0x756AA39C,0x026D930A,0x9C0906A9,0xEB0E363F,0x72076785,0x05005713,
        0x95BF4A82,0xE2B87A14,0x7BB12BAE,0x0CB61B38,0x92D28E9B,0xE5D5BE0D,0x7CDCEFB7,0x0BDBDF21,
        0x86D3D2D4,0xF1D4E242,0x68DDB3F8,0x1FDA836E,0x81BE16CD,0xF6B9265B,0x6FB077E1,0x18B74777,
        0x88085AE6,0xFF0F6A70,0x66063BCA,0x11010B5C,0x8F659EFF,0xF862AE69,0x616BFFD3,0x166CCF45,
        0xA00AE278,0xD70DD2EE,0x4E048354,0x3903B3C2,0xA7672661,0xD06016F7,0x4969474D,0x3E6E77DB,
        0xAED16A4A,0xD9D65ADC,0x40DF0B66,0x37D83BF0,0xA9BCAE53,0xDEBB9EC5,0x47B2CF7F,0x30B5FFE9,
        0xBDBDF21C,0xCABAC28A,0x53B39330,0x24B4A3A6,0xBAD03605,0xCDD70693,0x54DE5729,0x23D967BF,
        0xB3667A2E,0xC4614AB8,0x5D681B02,0x2A6F2B94,0xB40BBE37,0xC30C8EA1,0x5A05DF1B,0x2D02EF8D
        };
        /// <summary>  
                /// 检查给定长度数据的16位CRC是否正确  
                /// </summary>  
                /// <param name="data">要校验的字节数组</param>  
                /// <returns>  
                ///     true：正确  
                ///     false：错误  
                /// </returns>  
                /// <reamrks>  
                /// 字节数组最后2个字节为校验码，且低字节在前面，高字节在后面  
                /// </reamrks>  
        public static Boolean IsCrc16Good(Byte[] data)
        {
            // 初始化  
            Int32 High = 0xFF;
            Int32 Low = 0xFF;
            if (data != null)
            {
                foreach (Byte b in data)
                {
                    Int32 Index = Low ^ b;
                    Low = High ^ CRC16TABLE_LO[Index];
                    High = CRC16TABLE_HI[Index];
                }
            }

            return (High == 0xF0 && Low == 0xB8);
        }

        /// <summary>  
                /// CRC16查找表高字节  
                /// </summary>  
        private static readonly Byte[] CRC16TABLE_HI =
                {
                0x00,0x11,0x23,0x32,0x46,0x57,0x65,0x74,0x8C,0x9D,0xAF,0xBE,0xCA,0xDB,0xE9,0xF8,
                0x10,0x01,0x33,0x22,0x56,0x47,0x75,0x64,0x9C,0x8D,0xBF,0xAE,0xDA,0xCB,0xF9,0xE8,
                0x21,0x30,0x02,0x13,0x67,0x76,0x44,0x55,0xAD,0xBC,0x8E,0x9F,0xEB,0xFA,0xC8,0xD9,
                0x31,0x20,0x12,0x03,0x77,0x66,0x54,0x45,0xBD,0xAC,0x9E,0x8F,0xFB,0xEA,0xD8,0xC9,
                0x42,0x53,0x61,0x70,0x04,0x15,0x27,0x36,0xCE,0xDF,0xED,0xFC,0x88,0x99,0xAB,0xBA,
                0x52,0x43,0x71,0x60,0x14,0x05,0x37,0x26,0xDE,0xCF,0xFD,0xEC,0x98,0x89,0xBB,0xAA,
                0x63,0x72,0x40,0x51,0x25,0x34,0x06,0x17,0xEF,0xFE,0xCC,0xDD,0xA9,0xB8,0x8A,0x9B,
                0x73,0x62,0x50,0x41,0x35,0x24,0x16,0x07,0xFF,0xEE,0xDC,0xCD,0xB9,0xA8,0x9A,0x8B,
                0x84,0x95,0xA7,0xB6,0xC2,0xD3,0xE1,0xF0,0x08,0x19,0x2B,0x3A,0x4E,0x5F,0x6D,0x7C,
                0x94,0x85,0xB7,0xA6,0xD2,0xC3,0xF1,0xE0,0x18,0x09,0x3B,0x2A,0x5E,0x4F,0x7D,0x6C,
                0xA5,0xB4,0x86,0x97,0xE3,0xF2,0xC0,0xD1,0x29,0x38,0x0A,0x1B,0x6F,0x7E,0x4C,0x5D,
                0xB5,0xA4,0x96,0x87,0xF3,0xE2,0xD0,0xC1,0x39,0x28,0x1A,0x0B,0x7F,0x6E,0x5C,0x4D,
                0xC6,0xD7,0xE5,0xF4,0x80,0x91,0xA3,0xB2,0x4A,0x5B,0x69,0x78,0x0C,0x1D,0x2F,0x3E,
                0xD6,0xC7,0xF5,0xE4,0x90,0x81,0xB3,0xA2,0x5A,0x4B,0x79,0x68,0x1C,0x0D,0x3F,0x2E,
                0xE7,0xF6,0xC4,0xD5,0xA1,0xB0,0x82,0x93,0x6B,0x7A,0x48,0x59,0x2D,0x3C,0x0E,0x1F,
                0xF7,0xE6,0xD4,0xC5,0xB1,0xA0,0x92,0x83,0x7B,0x6A,0x58,0x49,0x3D,0x2C,0x1E,0x0F
                };

        /// <summary>  
                /// CRC16查找表低字节  
                /// </summary>  
        private static readonly Byte[] CRC16TABLE_LO =
                        {
                        0x00,0x89,0x12,0x9B,0x24,0xAD,0x36,0xBF,0x48,0xC1,0x5A,0xD3,0x6C,0xE5,0x7E,0xF7,
                        0x81,0x08,0x93,0x1A,0xA5,0x2C,0xB7,0x3E,0xC9,0x40,0xDB,0x52,0xED,0x64,0xFF,0x76,
                        0x02,0x8B,0x10,0x99,0x26,0xAF,0x34,0xBD,0x4A,0xC3,0x58,0xD1,0x6E,0xE7,0x7C,0xF5,
                        0x83,0x0A,0x91,0x18,0xA7,0x2E,0xB5,0x3C,0xCB,0x42,0xD9,0x50,0xEF,0x66,0xFD,0x74,
                        0x04,0x8D,0x16,0x9F,0x20,0xA9,0x32,0xBB,0x4C,0xC5,0x5E,0xD7,0x68,0xE1,0x7A,0xF3,
                        0x85,0x0C,0x97,0x1E,0xA1,0x28,0xB3,0x3A,0xCD,0x44,0xDF,0x56,0xE9,0x60,0xFB,0x72,
                        0x06,0x8F,0x14,0x9D,0x22,0xAB,0x30,0xB9,0x4E,0xC7,0x5C,0xD5,0x6A,0xE3,0x78,0xF1,
                        0x87,0x0E,0x95,0x1C,0xA3,0x2A,0xB1,0x38,0xCF,0x46,0xDD,0x54,0xEB,0x62,0xF9,0x70,
                        0x08,0x81,0x1A,0x93,0x2C,0xA5,0x3E,0xB7,0x40,0xC9,0x52,0xDB,0x64,0xED,0x76,0xFF,
                        0x89,0x00,0x9B,0x12,0xAD,0x24,0xBF,0x36,0xC1,0x48,0xD3,0x5A,0xE5,0x6C,0xF7,0x7E,
                        0x0A,0x83,0x18,0x91,0x2E,0xA7,0x3C,0xB5,0x42,0xCB,0x50,0xD9,0x66,0xEF,0x74,0xFD,
                        0x8B,0x02,0x99,0x10,0xAF,0x26,0xBD,0x34,0xC3,0x4A,0xD1,0x58,0xE7,0x6E,0xF5,0x7C,
                        0x0C,0x85,0x1E,0x97,0x28,0xA1,0x3A,0xB3,0x44,0xCD,0x56,0xDF,0x60,0xE9,0x72,0xFB,
                        0x8D,0x04,0x9F,0x16,0xA9,0x20,0xBB,0x32,0xC5,0x4C,0xD7,0x5E,0xE1,0x68,0xF3,0x7A,
                        0x0E,0x87,0x1C,0x95,0x2A,0xA3,0x38,0xB1,0x46,0xCF,0x54,0xDD,0x62,0xEB,0x70,0xF9,
                        0x8F,0x06,0x9D,0x14,0xAB,0x22,0xB9,0x30,0xC7,0x4E,0xD5,0x5C,0xE3,0x6A,0xF1,0x78
                        };
    }

    #endregion
    public class ATSSend : IAts
    {
        #region 私有成员
        private static IPAddress GroupAddress = IPAddress.Parse("224.4.0.9");
        private IPEndPoint pisEP;
        private UdpClient sendClient;
        private IPEndPoint groupEP;
        private static Logger logger = LogManager.GetLogger("ATSLibrary");
        private byte chineseEnglishSwitchTag;
        private int timeTag;
        private System.Timers.Timer chEngTimer;
        #endregion
        #region 辅助函数
        /// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="structObj">要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }

        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param name="bytes">byte数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }


        #endregion
        public ATSSend(int chineseEnglishSwitchTime)
        {
            timeTag = chineseEnglishSwitchTime;
            chineseEnglishSwitchTag = 0;
            if (timeTag>0)
            {
                if (chEngTimer == null)
                {
                    chEngTimer = new System.Timers.Timer(1000* timeTag);
                    // 到达时间的时候执行事件 获取中央ATS-列车状态；
                    chEngTimer.Elapsed += new System.Timers.ElapsedEventHandler(chOrEngTimer);
                    // 设置是执行一次（false）还是一直执行(true)；
                    chEngTimer.AutoReset = true;
                    // 是否执行System.Timers.Timer.Elapsed事件；
                    chEngTimer.Enabled = true;
                }
            }
        }

        public void chOrEngTimer(object source, System.Timers.ElapsedEventArgs e)
        {
            lock (this)
            {
                if(chineseEnglishSwitchTag == 0)
                {
                    chineseEnglishSwitchTag = 1;
                }
                else if(chineseEnglishSwitchTag == 1)
                {
                    chineseEnglishSwitchTag = 0;
                }
            }
        }
        /// <summary>
        /// SendATS
        /// 向控制器发送ATS数据，通过组播。
        /// 包结构：包头+数据内容
        /// </summary>
        /// <param name="List">ATS数据链表，通过传递sATS_Train链表，生成xml数据格式</param>
        public void sendATS(List<sATS_Platform> List)
        {
            //logger.Info("ATS_Platform_Count: " + List.Count);
            // 生成ATS-XML数据
            string xmlAtsData = CreateATSXmlData(List, List.Count, 4);
            // 记录日志
            //logger.Info(xmlAtsData);
            // ARM结构包头
            ARM_ATS_INFO atsInfo = new ARM_ATS_INFO();
            byte[] byDataHead = new byte[Marshal.SizeOf(atsInfo)];
            atsInfo.buShowMode = chineseEnglishSwitchTag;
            atsInfo.iPlatformNum = System.Net.IPAddress.HostToNetworkOrder(List.Count);
            atsInfo.iDataSize = System.Net.IPAddress.HostToNetworkOrder(Marshal.SizeOf(atsInfo) + xmlAtsData.Length);
            atsInfo.wCRC = 0;
            byDataHead = StructToBytes(atsInfo);
            // 拼接车站ATS数据
            // 将文档内容设成utf-8
            byte[] bufMessage = System.Text.Encoding.UTF8.GetBytes(xmlAtsData);
            List<byte> byteSource = new List<byte>();
            byteSource.AddRange(byDataHead);
            byteSource.AddRange(bufMessage);
            byte[] byteSend = byteSource.ToArray();
            // 获取CRC校验（不计算byte数组前两位）
            byte[] crcBuf = new byte[bufMessage.Length + 9];
            Buffer.BlockCopy(byteSend, 2, crcBuf, 0, byteSend.Length - 2);
            UInt16 crc = CRCITU.GetCrc16(crcBuf);
            byte[] crcSend = BitConverter.GetBytes(crc);
            Buffer.BlockCopy(crcSend, 0, byteSend, 0, crcSend.Length);

            try
            {
                //logger.Info("Crc值： " + crc.ToString());
                int nSend = sendClient.Send(byteSend, byteSend.Length, groupEP);//使用UdpClient发送字节数据
                //logger.Info("向地址:" + groupEP.Address.ToString() + "端口:" + groupEP.Port.ToString() + "发送ATS数据--包长度：" + nSend.ToString());
                xmlAtsData = "";
                string temp = "";
                for (int i=0;i<byteSend.Length;i++)
                {
                    temp += " " + byteSend[i].ToString();
                }
                //logger.Info(temp);
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }
        /// <summary>
        /// initATSLib
        /// 初始化ATSLib库
        /// </summary>
        /// <param name="groupAddr">组播地址</param>
        /// <param name="port">组播端口号</param>
        /// <param name="TTL">交换机TTL跨点</param>
        /// <param name="localAddr">本机PIS地址</param>
        /// <param name="localPort">本机PIS端口</param>
        /// <returns>true:初始化成功 false:初始化失败</returns>
        public bool initATSLib(string groupAddr, int port, int TTL, string localAddr, int localPort)
        {
            pisEP = new IPEndPoint(IPAddress.Parse(localAddr), localPort);
            // 建立发送端的UdpClient实例
            sendClient = new UdpClient(pisEP);
            sendClient.EnableBroadcast = false;
            // ATS组播下发地址与端口号
            groupEP = new IPEndPoint(IPAddress.Parse(groupAddr), port);
            try
            {
                sendClient.JoinMulticastGroup(IPAddress.Parse(groupAddr), TTL);
                return true;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return false;
            }
        }
        /// <summary>
        /// closeATSLib
        /// 退出组播，并关闭连接
        /// </summary>
        public void closeATSLib()
        {
            if (sendClient.Client.Connected)
            {
                sendClient.DropMulticastGroup(GroupAddress);
                sendClient.Close();
            }
        }

        /// <summary>
        /// CreateATSXmlData
        /// 创建ATS数据，按照接口协议生成xml格式
        /// </summary>
        /// <param name="List">车站列表</param>
        /// <param name="nStationCount">站数量</param>
        /// <param name="nTrain">车数量</param>
        /// <returns></returns>
        private string CreateATSXmlData(List<sATS_Platform> List, int nStationCount, int nTrain)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes");
            xmlDoc.AppendChild(node);
            //创建根节点
            XmlNode root = xmlDoc.CreateElement("ATS");
            for (int i = 1; i <= nStationCount; i++)
            {
                XmlElement xmP = xmlDoc.CreateElement("P" + i.ToString());

                XmlElement xmT = xmlDoc.CreateElement("T1");
                xmP.AppendChild(xmT);
                xmT.SetAttribute("Time", List[i - 1].sTrain1.strTime);
                xmT.SetAttribute("EnTime", List[i - 1].sTrain1.strEnTime);
                xmT.SetAttribute("Dst", List[i - 1].sTrain1.strDst);
                xmT.SetAttribute("EnDst", List[i - 1].sTrain1.strEnDst);
                root.AppendChild(xmP);

                XmlElement xmT2 = xmlDoc.CreateElement("T2");
                xmP.AppendChild(xmT2);
                xmT2.SetAttribute("Time", List[i - 1].sTrain2.strTime);
                xmT2.SetAttribute("EnTime", List[i - 1].sTrain2.strEnTime);
                xmT2.SetAttribute("Dst", List[i - 1].sTrain2.strDst);
                xmT2.SetAttribute("EnDst", List[i - 1].sTrain2.strEnDst);
                root.AppendChild(xmP);

                XmlElement xmT3 = xmlDoc.CreateElement("T3");
                xmP.AppendChild(xmT3);
                xmT3.SetAttribute("Time", List[i - 1].sTrain3.strTime);
                xmT3.SetAttribute("EnTime", List[i - 1].sTrain3.strEnTime);
                xmT3.SetAttribute("Dst", List[i - 1].sTrain3.strDst);
                xmT3.SetAttribute("EnDst", List[i - 1].sTrain3.strEnDst);
                root.AppendChild(xmP);

                XmlElement xmT4 = xmlDoc.CreateElement("T4");
                xmP.AppendChild(xmT4);
                xmT4.SetAttribute("Time", List[i - 1].sTrain4.strTime);
                xmT4.SetAttribute("EnTime", List[i - 1].sTrain4.strEnTime);
                xmT4.SetAttribute("Dst", List[i - 1].sTrain4.strDst);
                xmT4.SetAttribute("EnDst", List[i - 1].sTrain4.strEnDst);
                root.AppendChild(xmP);
            }
            xmlDoc.AppendChild(root);
            return xmlDoc.InnerXml;
        }
    }
}
