using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalParamLibrary;
using Spire.Xls;

namespace ConfigLibrary
{
    public class Config : IConfig
    {
        private sConfig aConfig;

        public Config()
        {
            aConfig = new sConfig ();
            LoadDataFromExcel("ATS.xlsx");
        }
        public sConfig getConfigObj()
        {
            return aConfig;
        }

        private bool LoadDataFromExcel(string filePath)
        {
            try
            {
                aConfig.confgStationList = new List<sConfig_Station>();

                string strDir = System.Environment.CurrentDirectory;
                string strDirName = strDir + "\\" + filePath;

                Workbook workbook = new Workbook();
                workbook.LoadFromFile(@strDirName);

                int nIndex = 1;
                Worksheet sheetConfig = workbook.Worksheets["软件配置"];
                // ats IP PORT
                CellRange atsIP1 = sheetConfig[2, nIndex++];
                aConfig.atsServerIP1 = atsIP1.DisplayedText;
                CellRange atsPort1 = sheetConfig[2, nIndex++];
                aConfig.atsServerPort1 = int.Parse(atsPort1.DisplayedText);
                CellRange atsIP2 = sheetConfig[2, nIndex++];
                aConfig.atsServerIP2 = atsIP2.DisplayedText;
                CellRange atsPort2 = sheetConfig[2, nIndex++];
                aConfig.atsServerPort2 = int.Parse(atsPort2.DisplayedText);
                // pis IP PORT
                CellRange pisIP1 = sheetConfig[2, nIndex++];
                aConfig.pisServerIP1 = pisIP1.DisplayedText;
                CellRange pisPort1 = sheetConfig[2, nIndex++];
                aConfig.pisServerPort1 = int.Parse(pisPort1.DisplayedText);
                CellRange pisIP2 = sheetConfig[2, nIndex++];
                aConfig.pisServerIP2 = pisIP2.DisplayedText;
                CellRange pisPort2 = sheetConfig[2, nIndex++];
                aConfig.pisServerPort2 = int.Parse(pisPort2.DisplayedText);
                // multicast IP PORT
                CellRange multicastIP = sheetConfig[2, nIndex++];
                aConfig.sendIP = multicastIP.DisplayedText;
                CellRange multicastPort = sheetConfig[2, nIndex++];
                aConfig.sendPort = int.Parse(multicastPort.DisplayedText);
                // pcc 预留
                CellRange pccIP1 = sheetConfig[2, nIndex++];
                aConfig.pccServerIP1 = pccIP1.DisplayedText;
                CellRange pccPort1 = sheetConfig[2, nIndex++];
                aConfig.pccServerPort1 = int.Parse(pccPort1.DisplayedText);
                CellRange pccIP2 = sheetConfig[2, nIndex++];
                aConfig.pccServerIP2 = pccIP2.DisplayedText;
                CellRange pccPort2 = sheetConfig[2, nIndex++];
                aConfig.pccServerPort2 = int.Parse(pccPort2.DisplayedText);
                // 中英文切换时间（单位秒）
                CellRange switchTime = sheetConfig[2, nIndex++];
                aConfig.switchBetweenChineseAndEnglis = int.Parse(switchTime.DisplayedText);
                // 即将到站显示条件（单位秒）
                CellRange arriveTime = sheetConfig[2, nIndex++];
                aConfig.arrivingShow = int.Parse(arriveTime.DisplayedText);
                // 即将到站英文内容
                CellRange arriveComingEng = sheetConfig[2, nIndex++];
                aConfig.arrivingComingEngValue= arriveComingEng.DisplayedText;
                // 即将到站中文内容
                CellRange arriveComingCh = sheetConfig[2, nIndex++];
                aConfig.arrivingComingChValue = arriveComingCh.DisplayedText;
                // 到站英文内容
                CellRange arriveEng = sheetConfig[2, nIndex++];
                aConfig.arriveEngValue = arriveEng.DisplayedText;
                // 到站中文内容
                CellRange arriveCh = sheetConfig[2, nIndex++];
                aConfig.arriveChValue = arriveCh.DisplayedText;
                // 列车跳站英文内容
                CellRange skipStationEng = sheetConfig[2, nIndex++];
                aConfig.skipStationEngValue = skipStationEng.DisplayedText;
                // 列车跳站中文内容
                CellRange skipStationCh = sheetConfig[2, nIndex++];
                aConfig.skipStaionChValue = skipStationCh.DisplayedText;
                // 列车到站清空标志
                CellRange clearTime = sheetConfig[2, nIndex++];
                aConfig.clearShow = int.Parse(clearTime.DisplayedText);


                sheetConfig = workbook.Worksheets["站点编码表"];

                for (int i=1;i<sheetConfig.Rows.Length;i++)
                {
                    sConfig_Station aStation = new sConfig_Station();
                    aStation.stationName = new List<string>();
                    aStation.destCode = new List<string>();
                    CellRange cellData = sheetConfig[i + 1, 2];
                    aStation.stationCode = cellData.DisplayedText;
                    cellData = sheetConfig[i + 1, 3];
                    aStation.stationName.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 4];
                    aStation.stationName.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 5];
                    aStation.stationName.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 6];
                    aStation.stationName.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 7];
                    aStation.stationName.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 8];
                    aStation.platformName = cellData.DisplayedText;
                    cellData = sheetConfig[i + 1, 9];
                    aStation.platformCode = int.Parse(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 10];
                    aStation.destCode.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 11];
                    aStation.destCode.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 12];
                    aStation.destCode.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 13];
                    aStation.destCode.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 14];
                    aStation.destCode.Add(cellData.DisplayedText);
                    cellData = sheetConfig[i + 1, 15];
                    aStation.destCode.Add(cellData.DisplayedText);

                    aConfig.confgStationList.Add(aStation);
                }
                return true;
            }
            catch (Exception err)
            {

                Console.WriteLine("数据绑定Excel失败!:" + err.ToString());
                return false;
            }
        }
    }
}
