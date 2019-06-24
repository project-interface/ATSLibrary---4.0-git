using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using GlobalParamLibrary;
using ConfigLibrary;
using CommLibrary;
using ATSLibrary;
using 深圳五号线信号卡斯柯;


namespace ManagerLibrary
{
    public class Manager : IManager
    {
        public event pushSourceChangedHandler   pushSourceChanged;
        public event pushLogicChangedHandler    pushLogicChanged;

        private static Manager                  _Instance;
        private static readonly object          Locker = new object();
        private List<sATS_Data>                 stationList = null;
        private sConfig                         aConfig;
        private byte[]                          dataCache;
        private Queue<byte[]>                   sourceDataCache;

        private IConfig                         configLib = null;
        private ILogic                          logicLib = null;
        private IComm                           communicationLib = null;
        private IAts                            atsLib = null;
        private System.Timers.Timer             connectTimer;
        private List<sATS_Platform>             atsDataList;

        private Manager()
        {
            stationList = new List<sATS_Data>();
            aConfig = new sConfig();
            sourceDataCache = new Queue<byte[]>();
            start();
        }
        public static Manager GetInstance()
        {
            if (_Instance == null)
            {
                lock (Locker)
                {
                    if (_Instance == null)
                    {
                        _Instance = new Manager();
                    }
                }

            }
            return _Instance;
        }

        public void start()
        {
            InitLib();
        }

        public void stop()
        {
            sourceDataCache.Clear();
        }

        private void InitLib()
        {
            configLib = new Config();
            aConfig = configLib.getConfigObj();

            atsLib = new ATSSend(aConfig.switchBetweenChineseAndEnglis);
            atsLib.initATSLib(aConfig.sendIP, aConfig.sendPort, 30, aConfig.pisServerIP1, aConfig.pisServerPort1);

            logicLib = new Logic(aConfig.confgStationList.Count);
            communicationLib = new communication();
            if(!communicationLib.StarSevice(aConfig.atsServerPort1, aConfig.atsServerIP1, 4096 * 2, 0))
            {
                // 如果备也连接失败开户定时器每隔2秒重连
                // 实例化Timer类，设置间隔时间为10000毫秒；
                if (connectTimer == null)
                {
                    connectTimer = new System.Timers.Timer(5000);
                    // 到达时间的时候执行事件 获取中央ATS-列车状态；
                    connectTimer.Elapsed += new System.Timers.ElapsedEventHandler(reConnectServer);
                    // 设置是执行一次（false）还是一直执行(true)；
                    connectTimer.AutoReset = true;
                    // 是否执行System.Timers.Timer.Elapsed事件；
                    connectTimer.Enabled = true;
                }
            }
            communicationLib.recvCommChanged += CommunicationLib_recvCommChanged;
            communicationLib.disConnectChanged += CommunicationLib_disConnectChanged;

            // 处理数据线程
            Thread dataThread = new Thread(new ParameterizedThreadStart(logicDataThread));
            dataThread.Start((Object)this);
        }

        private void CommunicationLib_disConnectChanged()
        {
            communicationLib.StopSevice();
            
           // 如果备也连接失败开户定时器每隔2秒重连
           // 实例化Timer类，设置间隔时间为10000毫秒；
           if(connectTimer == null)
           {
                connectTimer = new System.Timers.Timer(5000);
                if (!connectTimer.Enabled)
                {
                    // 到达时间的时候执行事件 获取中央ATS-列车状态；
                    connectTimer.Elapsed += new System.Timers.ElapsedEventHandler(reConnectServer);
                    // 设置是执行一次（false）还是一直执行(true)；
                    connectTimer.AutoReset = true;
                    // 是否执行System.Timers.Timer.Elapsed事件；
                    connectTimer.Enabled = true;
                }
            }
        }

        public void reConnectServer(object source, System.Timers.ElapsedEventArgs e)
        {
            lock(this)
            {
                if(!communicationLib.IsConnect())
                {
                    if (!communicationLib.StarSevice(aConfig.atsServerPort1, aConfig.atsServerIP1, 4096 * 2, 0))
                    {
                        communicationLib.StarSevice(aConfig.atsServerPort2, aConfig.atsServerIP2, 4096 * 2, 0);
                    }
                }
                
                return;
            }
            
        }

        private void CommunicationLib_recvCommChanged(byte[] pbArry, string data)
        {
            dataCache = pbArry;
            sourceDataCache.Enqueue(dataCache);
        }

        private static void logicProcessThread(object obj)
        {
            
            Manager managerObj = (Manager)obj;
            // 发送数据
            managerObj.atsLib.sendATS(managerObj.atsDataList);
        }

        private static void logicDataThread(object obj)
        {
            Manager managerObj = (Manager)obj;
            while (true)
            {
                if(managerObj.sourceDataCache.Count>0)
                {
                    byte[] data = managerObj.sourceDataCache.Dequeue();
                    managerObj.atsDataList = managerObj.logicLib.process(data, ref managerObj.configLib);
                    managerObj.atsLib.sendATS(managerObj.atsDataList);
                }
            }
        }
    }
}
