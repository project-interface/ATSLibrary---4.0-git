using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalParamLibrary;
using System.Threading;
using System.Runtime.InteropServices;
using NLog;

namespace 深圳五号线信号卡斯柯
{
    #region 协议结构
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct sMsgFrame
    {
        public byte systemID;
        public short totalLength;
        public byte multiFlag;
    }
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct sPackageData
    {
        public short msg_len;
        public short msg_id;
        public byte Platform_cnt;
    }
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct sPackageFirstLastData
    {
        public short msg_len;
        public short msg_id;
        public short Platform_cnt;
    }
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct sFirstLastTrain
    {
        public byte station_id;
        public short platform_id;
        public short destination_id;
        public short train_service_number;
        public Int32 scheduled_arrival_time;
        public Int32 scheduled_departure_time;
        public byte flag;
    }
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct sTrainPackage
    {
        public sPackageData sPackage;
        public List<sTrain> trainList;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct sFirstLastTrainPackage
    {
        public sPackageFirstLastData sPackage;
        public List<sFirstLastTrain> trainList;
    }

    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct sTrain
    {
        public byte station_id;
        public short platform_id;
        public byte validityField1;
        public short train_unit_number1;
        public short train_service_number1;
        public short destination_id;
        public byte pre_arrival;
        public Int32 scheduled_arrival_time1;
        public Int32 scheduled_departure_time1;
        public byte arrival_status;
        public byte departure_status;
        public byte hold_status;
        public byte skip_status1;
        public byte out_of_service1;
        public byte last_train1;

        public byte validityField2;
        public short train_unit_number2;
        public short train_service_number2;
        public short destination_id2;
        public Int32 scheduled_arrival_time2;
        public Int32 scheduled_departure_time2;
        public byte skip_status2;
        public byte out_of_service2;
        public byte last_train2;

        public byte validityField3;
        public short train_unit_number3;
        public short train_service_number3;
        public short destination_id3;
        public Int32 scheduled_arrival_time3;
        public Int32 scheduled_departure_time3;
        public byte skip_status3;
        public byte out_of_service3;
        public byte last_train3;

        public byte validityField4;
        public short train_unit_number4;
        public short train_service_number4;
        public short destination_id4;
        public Int32 scheduled_arrival_time4;
        public Int32 scheduled_departure_time4;
        public byte skip_status4;
        public byte out_of_service4;
        public byte last_train4;
    }
    #endregion
    public class Logic : ILogic
    {
        private List<sATS_Platform> atsList;

        public static Logger logger = LogManager.GetLogger("LogicATS");

        private sFirstLastTrainPackage aFirstLastTrainPackage;
        private sPackageFirstLastData aPackageFirstLastData;

        private sPackageData aPackageData;
        private sTrainPackage aTrainPackage;

        public Logic(int stationPlatormCount)
        {
            aFirstLastTrainPackage = new sFirstLastTrainPackage();
            aFirstLastTrainPackage.trainList = new List<sFirstLastTrain>();
            aTrainPackage = new sTrainPackage();
            aTrainPackage.trainList = new List<sTrain>();
            atsList = new List<sATS_Platform>();

            if (atsList.Count == 0)
            {
                sATS_Platform atsPlatorm = new sATS_Platform();
                atsPlatorm.sTrain1 = new sATS_TIME();
                atsPlatorm.sTrain2 = new sATS_TIME();
                atsPlatorm.sTrain3 = new sATS_TIME();
                atsPlatorm.sTrain4 = new sATS_TIME();
                for (int i = 0; i < stationPlatormCount; i++)
                {
                    atsList.Add(atsPlatorm);
                }
            }

        }
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
        public List<sATS_Platform> process(byte[] dataArry, ref IConfig IConfigInterface)
        {
            #region 解析协议数据
            //lock (this)
            {
                // 打印原始日志
                string data = "ATS--PIS-";
                data += DateTime.Now.ToString() + "::-------";
                for (int i = 0; i < dataArry.Length; i++)
                {
                    data += "0x" + dataArry[i].ToString("x2") + ", ";
                }
                logger.Info(data);
                data = "";

                int UseLength = 0;
                string TimeNow;
                while (dataArry.Length - UseLength > 0)
                {
                    if (dataArry.Length - UseLength < 4)
                    {
                        logger.Info("数据长度有误,数据总长:" + dataArry.Length + "已使用长度:" + UseLength);
                    }
                    TimeNow = DateTime.Now.ToString();
                    byte[] Buf = new byte[dataArry.Length - UseLength];
                    Buffer.BlockCopy(dataArry, UseLength, Buf, 0, dataArry.Length - UseLength);

                    sMsgFrame aMsgFrame = new sMsgFrame();
                    byte[] msgBuf = new byte[Marshal.SizeOf(aMsgFrame)];
                    Buffer.BlockCopy(Buf, 0, msgBuf, 0, Marshal.SizeOf(aMsgFrame));
                    Array.Reverse(msgBuf, 1, 2);
                    aMsgFrame = (sMsgFrame)BytesToStuct(msgBuf, typeof(sMsgFrame));
                    if (Buf[0] == 0xFF)
                    {
                        if (Buf[7] == 33)
                        {
                            logger.Info(DateTime.Now.ToString() + "--" + "解析站台数据");
                            if (aMsgFrame.multiFlag == 1)
                            {
                                logger.Info("本包数据不是完整信息");
                                break;
                            }
                            UseLength = aMsgFrame.totalLength + UseLength + 3;

                            aTrainPackage.sPackage = aPackageData;
                            byte[] packBuf = new byte[Marshal.SizeOf(aPackageData)];
                            Buffer.BlockCopy(Buf, Marshal.SizeOf(aMsgFrame), packBuf, 0, Marshal.SizeOf(aPackageData));
                            Array.Reverse(packBuf, 0, 2);
                            Array.Reverse(packBuf, 2, 2);
                            aPackageData = (sPackageData)BytesToStuct(packBuf, typeof(sPackageData));

                            logger.Info(DateTime.Now.ToString() + "--" + "站台总数:" + aPackageData.Platform_cnt.ToString());
                            for (int i = 0; i < aPackageData.Platform_cnt; i++)
                            {
                                string stempNo1 = "第一列车";
                                string stempNo2 = "第二列车";
                                string stempNo3 = "第三列车";
                                string stempNo4 = "第四列车";
                                sTrain aTrain = new sTrain();
                                byte[] buf = new byte[Marshal.SizeOf(aTrain)];
                                Buffer.BlockCopy(Buf, Marshal.SizeOf(aMsgFrame) + Marshal.SizeOf(aPackageData) + (i * Marshal.SizeOf(aTrain)), buf, 0, Marshal.SizeOf(aTrain));
                                Array.Reverse(buf, 1, 2);
                                Array.Reverse(buf, 4, 2);
                                Array.Reverse(buf, 6, 2);
                                Array.Reverse(buf, 8, 2);
                                Array.Reverse(buf, 11, 4);
                                Array.Reverse(buf, 15, 4);

                                Array.Reverse(buf, 26, 2);
                                Array.Reverse(buf, 28, 2);
                                Array.Reverse(buf, 30, 2);
                                Array.Reverse(buf, 32, 4);
                                Array.Reverse(buf, 36, 4);

                                Array.Reverse(buf, 44, 2);
                                Array.Reverse(buf, 46, 2);
                                Array.Reverse(buf, 48, 2);
                                Array.Reverse(buf, 50, 4);
                                Array.Reverse(buf, 54, 4);

                                Array.Reverse(buf, 62, 2);
                                Array.Reverse(buf, 64, 2);
                                Array.Reverse(buf, 66, 2);
                                Array.Reverse(buf, 68, 4);
                                Array.Reverse(buf, 72, 4);
                                aTrain = (sTrain)BytesToStuct(buf, typeof(sTrain));

                                long seconds = (long)DateTime.UtcNow.Subtract(DateTime.Parse("1970-1-1")).TotalSeconds;
                                //                        aTrainPackage.trainList.Add(aTrain);
                                #region 打印日志
                                logger.Info(TimeNow + "当前站台:" + aTrain.station_id.ToString() + " 当前上下行: " + aTrain.platform_id.ToString());
                                stempNo1 += TimeNow + " 有效:" + aTrain.validityField1.ToString() +
                                 " 列车服务:" + aTrain.train_service_number1.ToString() +
                                 " 当前时间:" + seconds +
                                 " 到站时间:" + aTrain.scheduled_arrival_time1.ToString() +
                                 " 离站时间:" + aTrain.scheduled_departure_time1.ToString() +
                                 " 目的地号:" + aTrain.destination_id.ToString() +
                                 " 跳站标志:" + aTrain.skip_status1.ToString() +
                                 " 即将进站:" + aTrain.pre_arrival.ToString() +
                                 " 列车进站:" + aTrain.arrival_status.ToString();
                                logger.Info(stempNo1);
                                stempNo2 += TimeNow + " 有效:" + aTrain.validityField2.ToString() +
                                 " 列车服务:" + aTrain.train_service_number2.ToString() +
                                 " 当前时间:" + seconds +
                                 " 到站时间:" + aTrain.scheduled_arrival_time2.ToString() +
                                 " 离站时间:" + aTrain.scheduled_departure_time2.ToString() +
                                 " 目的地号:" + aTrain.destination_id2.ToString() +
                                 " 跳站标志:" + aTrain.skip_status2.ToString();
                                logger.Info(stempNo2);
                                stempNo3 += TimeNow + " 有效:" + aTrain.validityField3.ToString() +
                                 " 列车服务:" + aTrain.train_service_number3.ToString() +
                                 " 当前时间:" + seconds +
                                 " 到站时间:" + aTrain.scheduled_arrival_time3.ToString() +
                                 " 离站时间:" + aTrain.scheduled_departure_time3.ToString() +
                                 " 目的地号:" + aTrain.destination_id3.ToString() +
                                 " 跳站标志:" + aTrain.skip_status3.ToString();
                                logger.Info(stempNo3);
                                stempNo4 += TimeNow + " 有效:" + aTrain.validityField4.ToString() +
                                 " 列车服务:" + aTrain.train_service_number4.ToString() +
                                 " 当前时间:" + seconds +
                                 " 到站时间:" + aTrain.scheduled_arrival_time4.ToString() +
                                 " 离站时间:" + aTrain.scheduled_departure_time4.ToString() +
                                 " 目的地号:" + aTrain.destination_id4.ToString() +
                                 " 跳站标志:" + aTrain.skip_status4.ToString();
                                logger.Info(stempNo4);
                                #endregion
                                sATS_Platform aPlatorm = new sATS_Platform();
                                sATS_TIME train1 = new sATS_TIME();
                                sATS_TIME train2 = new sATS_TIME();
                                sATS_TIME train3 = new sATS_TIME();
                                sATS_TIME train4 = new sATS_TIME();

                                for (int j = 0; j < IConfigInterface.getConfigObj().confgStationList.Count; j++)
                                {
                                    for (int k = 0; k < IConfigInterface.getConfigObj().confgStationList[j].destCode.Count; k++)
                                    {
                                        if (IConfigInterface.getConfigObj().confgStationList[j].destCode[k] == aTrain.destination_id.ToString())
                                        {
                                            train1.strDst = IConfigInterface.getConfigObj().confgStationList[j].stationName[0];
                                            train1.strEnDst = IConfigInterface.getConfigObj().confgStationList[j].stationName[1];
                                        }
                                        if (IConfigInterface.getConfigObj().confgStationList[j].destCode[k] == aTrain.destination_id2.ToString())
                                        {
                                            train2.strDst = IConfigInterface.getConfigObj().confgStationList[j].stationName[0];
                                            train2.strEnDst = IConfigInterface.getConfigObj().confgStationList[j].stationName[1];
                                        }
                                        if (IConfigInterface.getConfigObj().confgStationList[j].destCode[k] == aTrain.destination_id3.ToString())
                                        {
                                            train3.strDst = IConfigInterface.getConfigObj().confgStationList[j].stationName[0];
                                            train3.strEnDst = IConfigInterface.getConfigObj().confgStationList[j].stationName[1];
                                        }
                                        if (IConfigInterface.getConfigObj().confgStationList[j].destCode[k] == aTrain.destination_id4.ToString())
                                        {
                                            train4.strDst = IConfigInterface.getConfigObj().confgStationList[j].stationName[0];
                                            train4.strEnDst = IConfigInterface.getConfigObj().confgStationList[j].stationName[1];
                                        }
                                    }
                                }

                                seconds = (long)DateTime.UtcNow.Subtract(DateTime.Parse("1970-1-1")).TotalSeconds;
                                long sec = aTrain.scheduled_arrival_time1 - seconds;
                                long lsec = aTrain.scheduled_departure_time1 - seconds;
                                long min = sec / 60;
                                if ((sec % 60) > 30)
                                {
                                    min = min + 1;
                                }
                                //无效
                                if (aTrain.validityField1 != 1 )
                                {
                                    train1.strTime = "";
                                    train1.strEnTime = "";
                                    train1.strDst = "";
                                    train1.strEnDst = "";
                                    logger.Info(TimeNow + "第一趟车无效:" + aTrain.validityField1);
                                }
                                //时间小于0
                                else if (lsec < 0 && aTrain.arrival_status == 0 && aTrain.pre_arrival == 0)
                                {
                                    train1.strTime = "";
                                    train1.strEnTime = "";
                                    logger.Info(TimeNow + "第一趟车时间错误:" + aTrain.scheduled_arrival_time1 + "当前时间" + seconds);
                                }
                                //列车跳站
                                else if (aTrain.skip_status1 == 1)
                                {
                                    train1.strTime = IConfigInterface.getConfigObj().skipStaionChValue;
                                    train1.strEnTime = IConfigInterface.getConfigObj().skipStationEngValue;
                                }
                                //列车到站
                                else if (sec <= IConfigInterface.getConfigObj().clearShow && IConfigInterface.getConfigObj().clearShow > 0)
                                {
                                    train1.strTime = "";
                                    train1.strEnTime = "";
                                }
                                else if (aTrain.arrival_status == 1 && IConfigInterface.getConfigObj().clearShow == 0)
                                {
                                    train1.strTime = IConfigInterface.getConfigObj().arriveChValue;
                                    train1.strEnTime = IConfigInterface.getConfigObj().arriveEngValue;
                                }
                                //列车即将到站
                                else if ((sec <= IConfigInterface.getConfigObj().arrivingShow && IConfigInterface.getConfigObj().arrivingShow > 0) ||
                                    (aTrain.pre_arrival == 1 && IConfigInterface.getConfigObj().arrivingShow == 0))
                                {
                                    train1.strTime = IConfigInterface.getConfigObj().arrivingComingChValue;
                                    train1.strEnTime = IConfigInterface.getConfigObj().arrivingComingEngValue;
                                }
                                //正常显示时间 
                                else
                                {
                                    train1.strEnTime = min.ToString() ;
                                    train1.strTime = min.ToString() ;
                                }

                                sec = aTrain.scheduled_arrival_time2 - seconds;
                                lsec = aTrain.scheduled_departure_time2 - seconds;
                                min = sec / 60;
                                if (aTrain.validityField2 != 1)
                                {
                                    train2.strTime = " ";
                                    train2.strEnTime = " ";
                                    train2.strDst = "";
                                    train2.strEnDst = "";
                                    logger.Info(TimeNow + "第二趟车无效:" + aTrain.validityField2);
                                }
                                else if (lsec < 0)
                                {
                                    train2.strTime = " ";
                                    train2.strEnTime = " ";
                                    logger.Info(TimeNow + "第二趟车时间错误:" + aTrain.scheduled_arrival_time2 + "当前时间" + seconds);
                                }
                                else
                                {
                                    train2.strTime = min.ToString();
                                    train2.strEnTime = min.ToString();
                                }
                                
                                sec = aTrain.scheduled_arrival_time3 - seconds;
                                lsec = aTrain.scheduled_departure_time3 - seconds;
                                min = sec / 60;
                                if (min < 0 || aTrain.validityField3 != 1)
                                {
                                    train3.strTime = " ";
                                    train3.strEnTime = " ";
                                    train3.strDst = "";
                                    train3.strEnDst = "";
                                    logger.Info(TimeNow + "第三趟车无效:" + aTrain.validityField3);
                                }
                                else if (lsec < 0)
                                {
                                    train3.strTime = " ";
                                    train3.strEnTime = " ";
                                    logger.Info(TimeNow + "第三趟车时间错误:" + aTrain.scheduled_arrival_time3 + "当前时间" + seconds);
                                }
                                else
                                {
                                    train3.strTime = min.ToString();
                                    train3.strEnTime = min.ToString();
                                }
                                
                                sec = aTrain.scheduled_arrival_time4 - seconds;
                                lsec = aTrain.scheduled_departure_time4 - seconds;
                                min = sec / 60;
                                if (min < 0 || aTrain.validityField4 != 1)
                                {
                                    train4.strTime = " ";
                                    train4.strEnTime = " ";
                                    train4.strDst = "";
                                    train4.strEnDst = "";
                                    logger.Info(TimeNow + "第四趟车无效:" + aTrain.validityField4);
                                }
                                else if (lsec < 0)
                                {
                                    train4.strTime = " ";
                                    train4.strEnTime = " ";
                                    logger.Info(TimeNow + "第四趟车时间错误:" + aTrain.scheduled_arrival_time4 + "当前时间" + seconds);
                                }
                                else
                                {
                                    train4.strTime = min.ToString();
                                    train4.strEnTime = min.ToString();
                                }
                                
                                
                                
                                if (aTrain.platform_id == 1 || aTrain.platform_id == 2)
                                {
                                    int Count = ((aTrain.station_id - 1) * 2) + (aTrain.platform_id - 1);
                                    if (Count < atsList.Count)
                                    {
                                        aPlatorm.sTrain1 = train1;
                                        aPlatorm.sTrain2 = train2;
                                        aPlatorm.sTrain3 = train3;
                                        aPlatorm.sTrain4 = train4;
                                        atsList[Count] = aPlatorm;
                                        #region 打印日志
                                        logger.Info(TimeNow + "第一趟车目的地:" + atsList[Count].sTrain1.strDst + " 到站时间: " + atsList[Count].sTrain1.strTime);
                                        logger.Info(TimeNow + "第二趟车目的地:" + atsList[Count].sTrain2.strDst + " 到站时间: " + atsList[Count].sTrain2.strTime);
                                        logger.Info(TimeNow + "第三趟车目的地:" + atsList[Count].sTrain3.strDst + " 到站时间: " + atsList[Count].sTrain3.strTime);
                                        logger.Info(TimeNow + "第四趟车目的地:" + atsList[Count].sTrain4.strDst + " 到站时间: " + atsList[Count].sTrain4.strTime);
                                        #endregion
                                    }
                                    else
                                    {
                                        logger.Info(TimeNow + "站号超出:" + aTrain.station_id + " 站台编号: " + aTrain.platform_id);
                                    }
                                }
                                else
                                {
                                    logger.Info(TimeNow + "车站号:" + aTrain.station_id + " 站台号超出: " + aTrain.platform_id);
                                }
                            }
                        }
                        else if (Buf[7] == 34)
                        {

                            logger.Info(TimeNow + "首末车数据");
                            UseLength = aMsgFrame.totalLength + UseLength + 3;

                            //aFirstLastTrainPackage.sPackage = aPackageFirstLastData;
                            //byte[] packBuf = new byte[Marshal.SizeOf(aPackageFirstLastData)];
                            //Buffer.BlockCopy(Buf, Marshal.SizeOf(aMsgFrame), packBuf, 0, Marshal.SizeOf(aPackageFirstLastData));
                            //Array.Reverse(packBuf, 0, 2);
                            //Array.Reverse(packBuf, 2, 2);
                            //Array.Reverse(packBuf, 4, 2);
                            //aPackageFirstLastData = (sPackageFirstLastData)BytesToStuct(packBuf, typeof(sPackageFirstLastData));
                            //aFirstLastTrainPackage.sPackage = aPackageFirstLastData;

                            //for (int i = 0; i < aPackageFirstLastData.Platform_cnt; i++)
                            //{
                            //    sFirstLastTrain aFirstLastTrain = new sFirstLastTrain();
                            //    byte[] buf = new byte[Marshal.SizeOf(aFirstLastTrain)];
                            //    Buffer.BlockCopy(Buf, Marshal.SizeOf(aMsgFrame) + Marshal.SizeOf(aPackageFirstLastData) + (i * Marshal.SizeOf(aFirstLastTrain)), buf, 0, Marshal.SizeOf(aFirstLastTrain));
                            //    Array.Reverse(buf, 1, 2);
                            //    Array.Reverse(buf, 3, 2);
                            //    Array.Reverse(buf, 5, 2);
                            //    Array.Reverse(buf, 7, 4);
                            //    Array.Reverse(buf, 11, 4);
                            //    aFirstLastTrain = (sFirstLastTrain)BytesToStuct(buf, typeof(sFirstLastTrain));
                            //    aFirstLastTrainPackage.trainList.Add(aFirstLastTrain);
                            //}
                        }
                    }
                    else
                    {
                        logger.Info(TimeNow + "数据包错误");
                        UseLength = dataArry.Length;
                    }
                }
            }
            return atsList;
            #endregion
        }

    }
}
