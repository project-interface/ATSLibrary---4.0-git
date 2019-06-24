using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalParamLibrary;
using NLog;

namespace CommLibrary
{
    public class communication : IComm
    {
        public event recvCommChangedHandler recvCommChanged;
        public event disConnectChangedHandler disConnectChanged;

        public static Logger logger = LogManager.GetLogger("SourceTcp");
        private SimpleTCP.SimpleTcpClient TcpClientLib;
        private string sAddressIP;
        private int nPort;
        private byte[] pbArry;
        private int nReceiveBufferSize;
        private int nReconnectCount;
        private bool bConnect;
        private System.Timers.Timer t;

        public communication()
        {
            TcpClientLib = new SimpleTCP.SimpleTcpClient();
            TcpClientLib.DataReceived += TcpClientLib_DataReceived;
            nReconnectCount = 0;
            t = new System.Timers.Timer(5000);
        }
        private void TcpClientLib_DataReceived(object sender, SimpleTCP.Message e)
        {
            if (recvCommChanged != null)
            {
                lock (this)
                {
                    // 接收打印
                    pbArry = new byte[e.Data.Length];
                    pbArry = e.Data;

                    string data = "ATS--PIS-";
                    //data += DateTime.Now.ToString() + "::-------";
                    //for (int i = 0; i < pbArry.Length; i++)
                    //{
                    //    data += "0x" + pbArry[i].ToString("x2") + ", ";
                    //}
                    //logger.Info(data);
                    //// 转换日志
                    //data = "转换日志";
                    //data += "System ID:" + pbArry[0] + " Total Length:" + int.Parse(pbArry[1].ToString() + pbArry[2].ToString()) + " Multi-flag" + pbArry[3];
                    //UInt64 nDestTime = UInt64.Parse(pbArry[24].ToString() + pbArry[25].ToString() + pbArry[26].ToString() + pbArry[27].ToString());
                    //DateTime dt = DateTime.Parse("01/01/1970");
                    //TimeSpan ts = DateTime.Now - dt;
                    //int sec = ts.Seconds;
                    //data += " destTime:" + sec.ToString();
                    //logger.Info(data);
                    recvCommChanged(e.Data, data);
                }
            }
            return;
        }
        public bool IsConnect()
        {
            return bConnect;
        }

        public void Reconnect()
        {
            try
            {
                nReconnectCount = 0;
                TcpClientLib = TcpClientLib.Connect(sAddressIP, nPort);
                bConnect = TcpClientLib.TcpClient.Connected;
                TcpClientLib.TcpClient.ReceiveBufferSize = nReceiveBufferSize;
            }
            catch (Exception e)
            {
                string str = e.Message;
                while (nReconnectCount < 3)
                {
                    try
                    {
                        TcpClientLib = TcpClientLib.Connect(sAddressIP, nPort);
                        bConnect = TcpClientLib.TcpClient.Connected;
                        TcpClientLib.TcpClient.ReceiveBufferSize = nReceiveBufferSize;
                        return;
                    }
                    catch (Exception)
                    {
                        nReconnectCount++;
                    }
                }
            }
        }

        public void sendData(byte[] data)
        {
            TcpClientLib.Write(data);
        }

        public bool StarSevice(int nPort, string sAddressIP, int nReceiveBufferSize, eCommType commType)
        {
            if(eCommType.E_TCP == commType)
            {
                this.nReceiveBufferSize = nReceiveBufferSize;
                this.sAddressIP = sAddressIP;
                this.nPort = nPort;
                try
                {
                    TcpClientLib = TcpClientLib.Connect(sAddressIP, nPort);
                    TcpClientLib.TcpClient.ReceiveBufferSize = nReceiveBufferSize;
                    bConnect = TcpClientLib.TcpClient.Connected;

                    // 连接成功每五秒发心跳
                    // 实例化Timer类，设置间隔时间为10000毫秒；
                    
                    if(!t.Enabled)
                    {
                        // 到达时间的时候执行事件 获取中央ATS-列车状态；
                        t.Elapsed += new System.Timers.ElapsedEventHandler(processTcp);
                        // 设置是执行一次（false）还是一直执行(true)；
                        t.AutoReset = true;
                        // 是否执行System.Timers.Timer.Elapsed事件；
                        t.Enabled = true;
                        return TcpClientLib.TcpClient.Connected;
                    }
                }
                catch (Exception e)
                {
                    string str = e.Message;
                    bConnect = TcpClientLib.TcpClient.Connected;
                    return bConnect;
                }
            }
            return true;
        }

        public void processTcp(object source, System.Timers.ElapsedEventArgs e)
        {
            // 检查是否断开
            Byte[] byHartCheck = { 0xff, 0, 5, 0, 0, 2, 0, 0x33 };
            try
            {
                if(TcpClientLib.TcpClient.Connected)
                {
                    TcpClientLib.Write(byHartCheck);
                    return;
                }
                bConnect = false;
                if (disConnectChanged != null)
                {
                    disConnectChanged();
                }
                return;
            }
            catch (Exception)
            {
                bConnect = false;
                if (disConnectChanged != null)
                {
                    disConnectChanged();
                }
                return;
            }
        }
        public void StopSevice()
        {
            if(TcpClientLib.TcpClient!=null)
            {
                if (TcpClientLib.TcpClient.Connected)
                {
                    TcpClientLib.Disconnect();
                }
            }
        }
    }
}
