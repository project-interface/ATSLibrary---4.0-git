using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GlobalParamLibrary
{
    #region 结构
    public struct sATS_Language
    {
        public string countdownNo1;
        public string destNo1;
        public string countdownNo2;
        public string destNo2;
        public string countdownNo3;
        public string destNo3;
        public string countdownNo4;
        public string destNo4;
    }
    public struct sATS_Data
    {
        public List<sATS_Language> atsDataList;
        public string              stationName;
        public int                 platform;
    }
    public struct sConfig_Station
    {
        public string       stationCode;
        public string       platformName;
        public int          platformCode;
        public List<string> stationName;
        public List<string> destCode;
    }
    public struct sConfig
    {
        public string atsServerIP1;
        public int    atsServerPort1;
        public string atsServerIP2;
        public int    atsServerPort2;
        public string pisServerIP1;
        public int    pisServerPort1;
        public string pisServerIP2;
        public int    pisServerPort2;
        public string pccServerIP1;
        public int    pccServerPort1;
        public string pccServerIP2;
        public int    pccServerPort2;
        public string sendIP;
        public int    sendPort;
        public int    switchBetweenChineseAndEnglis;    // 中英文切换时间
        public int    arrivingShow;                     // 即将到达显示条件时间（单位秒）
        public string arrivingComingChValue;            // 即将到站中文
        public string arrivingComingEngValue;           // 即将到站英文
        public string arriveChValue;                    // 列车到站中文
        public string arriveEngValue;                   // 列车到站英文
        public string skipStaionChValue;                // 跳站中文
        public string skipStationEngValue;              // 跳站英文
        public int    clearShow;                        // 列车到站清空条件时间（单位秒）
        public List<sConfig_Station> confgStationList;
    }
    public struct sATS_TIME
    {
        public string strTime;
        public string strDst;
        public string strEnTime;
        public string strEnDst;
    };
    public struct sATS_Platform
    {
        public sATS_TIME sTrain1;
        public sATS_TIME sTrain2;
        public sATS_TIME sTrain3;
        public sATS_TIME sTrain4;
    }
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ARM_ATS_INFO
    {
        public Int16 wCRC;           // 以下数据的CRC_CCITT（0x1021）校验，包括后面的ATS XML数据
        public Int32 iPlatformNum;   // 站台数量
        public byte buShowMode;      // 显示模式
        public Int32 iDataSize;	     // 整个数据包的数据大小，包括后面的ATS XML数据
    }
    #endregion

    #region 枚举
    public enum eCommType
    {
        E_TCP = 0,
        E_UDP = 1,
        E_SERIAL =2 
    }
    #endregion
    #region 委托
    public delegate void pushSourceChangedHandler(ref byte[] dataLog);
    public delegate void pushLogicChangedHandler(ref List<sATS_Data> stationATS);
    public delegate void recvCommChangedHandler(byte[] pbArry, string data);
    public delegate void disConnectChangedHandler();
    #endregion
    public interface IManager
    {
        event pushSourceChangedHandler  pushSourceChanged;
        event pushLogicChangedHandler   pushLogicChanged;
        void start();
        void stop();
    }

    public interface IConfig
    {
        sConfig getConfigObj();
    }
    public interface ILogic
    {
        List<sATS_Platform> process(byte[] dataArry,ref IConfig IConfigInterface);
    }
 
    public interface IComm
    {
        event recvCommChangedHandler recvCommChanged;
        event disConnectChangedHandler disConnectChanged;
        bool StarSevice(int nPort, string sAddressIP, int nReceiveBufferSize, eCommType commType);
        void StopSevice();
        void Reconnect();
        bool IsConnect();
        void sendData(byte[] data);
    }

    public interface IAts
    {
        /// <summary>
        /// initATSLib
        /// 初始化库，当调用本库时，首先运行
        /// </summary>
        /// <param name="groupAddr">组播地址</param>
        /// <param name="port">组播端口</param>
        /// <param name="TTL">TTL</param>
        /// <param name="localAddr">本机PIS地址</param>
        /// <param name="localPort">本机PIS端口</param>
        /// <returns>true:成功 false:失败 失败原因参看ATSLibrary目录日志</returns>
        bool initATSLib(string groupAddr, int port, int TTL, string localAddr, int localPort);
        /// <summary>
        /// sendATS
        /// 发送ATS数据
        /// </summary>
        /// <param name="List">车站ATS数据，由外部调用方提供</param>
        void sendATS(List<sATS_Platform> List);
        /// <summary>
        /// closeATSLib()
        /// 释放ATSLibrary 当程序退出时需要释放库
        /// </summary>
        void closeATSLib();
    }
    public class GlobalParam
    {
    }
}
