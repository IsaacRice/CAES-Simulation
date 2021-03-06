﻿using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Reflection;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;
using NIS = CAES.ScenarioNode.NodeInScenario; //NIS Represent Node In Scenario
using WV = CAES.ScenarioNode.EachVictim; //WV Represent EachVictim In Scenario
using Scenario = CAES.ScenarioNode.Scenario; //Scenario Represent Scenario In Another Class
using ResultStorage = CAES.ScenarioNode.ResultStorage;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Thread = System.Threading.Thread;
namespace CAES
{
    internal class Starting
    {
        /*
        public static TreeClass.LinkedTree<string> RecursiveTestStart(TreeClass.LinkedTree<string> RootNode, string Target)
        {
            Queue<Tree<T>> queue = new Queue<Tree<T>>();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                Tree<T> tmpTree = queue.Dequeue();
                foreach (Tree<T> child in tmpTree.Children)
                    queue.Enqueue(child);
            }
        }
        */


        private static void Main(string[] args)
        {
            /*
            ///Test Start

            TreeClass.LinkedTree<string> Root = new TreeClass.LinkedTree<string>("Root");
            TreeClass.LinkedTree<string> RootClone = new TreeClass.LinkedTree<string>("Root");
            Root.Add(new TreeClass.LinkedTree<string>("Level 1 01"));
            Root.Add(new TreeClass.LinkedTree<string>("Level 1 02"));
            Root.Add(new TreeClass.LinkedTree<string>("Level 1 03"));
            TreeClass.FindSpecific<string>(Root, "Level 1 03").Value = "Level 1 04";
            int NowCount = 0;
            foreach (TreeClass.LinkedTree<string> n in Root.Children)
            {
                NowCount++;
                n.Add(new TreeClass.LinkedTree<string>("Level 2 0" + NowCount.ToString() + " 01"));
                n.Add(new TreeClass.LinkedTree<string>("Level 2 0" + NowCount.ToString() + " 02"));
                n.Add(new TreeClass.LinkedTree<string>("Level 2 0" + NowCount.ToString() + " 03"));
            }

            List<string> Path = TreeClass.BreadthFirst<string>(Root);
            RootClone = (TreeClass.LinkedTree<string>)Root.Clone();
            TreeClass.LinkedTree<string> TargetNode = TreeClass.FindSpecific<string>(Root, "Level 1 02");
            RootClone = TreeClass.AddSpecific<string>(TargetNode, RootClone, "Level 1 02");
            Root = TreeClass.RemoveSpecific<string>(Root, "Level 1 02");


            ///Test End
            */

            bool IfRecordScenario = false;//true //false
            bool IfOpenExcel = true;
            double Alpha = 0.6,
                   Beta = 0.3,
                   Gamma = 0.1;
            int SensorHeight = 3, SensorWidth = 3, //Sensor高與寬之數目
                VictimsAmount = 0, //總人數
                CorridorLimit = 3, //走廊限制人數
                //DistancePerCorridor = 1,//走廊單位距離
                //RunningTimes = 0,
                FireStartingNumber = 0,
                FireSpreadTime = 5;
            List<int> CAESWithHmDndSWithOldDL,
                      CAESWithSmSWithOldDL,
                      CAESWithSmDWithOldDL,
                      CAESWithHmDndSwSWithOldDL,
                      CAESWithSmSwSWithOldDL,
                      CAESWithSmDwSWithOldDL,
                      CAESWithHmDndS,
                      CAESWithSmS,
                      CAESWithSmD,
                      CAESWithHmDndSwS,
                      CAESWithSmSwS,
                      CAESWithSmDwS,
                      CAESWithHmDndSWithNewDL,
                      CAESWithSmSWithNewDL,
                      CAESWithSmDWithNewDL,
                      CAESWithHmDndSwSWithNewDL,
                      CAESWithSmSwSWithNewDL,
                      CAESWithSmDwSWithNewDL,
                      MSCG,
                      ESSPlus,
                      LEGS,
                      HEXInc,
                      ShortestPath,
                      CAESDeathWithHmDndSWithOldDL,
                      CAESDeathWithSmSWithOldDL,
                      CAESDeathWithSmDWithOldDL,
                      CAESDeathWithHmDndSwSWithOldDL,
                      CAESDeathWithSmSwSWithOldDL,
                      CAESDeathWithSmDwSWithOldDL,
                      CAESDeathWithHmDndS,
                      CAESDeathWithSmS,
                      CAESDeathWithSmD,
                      CAESDeathWithHmDndSwS,
                      CAESDeathWithSmSwS,
                      CAESDeathWithSmDwS,
                      CAESDeathWithHmDndSWithNewDL,
                      CAESDeathWithSmSWithNewDL,
                      CAESDeathWithSmDWithNewDL,
                      CAESDeathWithHmDndSwSWithNewDL,
                      CAESDeathWithSmSwSWithNewDL,
                      CAESDeathWithSmDwSWithNewDL,
                      MSCGDeath,
                      ESSPlusDeath,
                      LEGSDeath,
                      HEXIncDeath,
                      ShortestPathDeath;
            int[,] DefaultExitSensor, DefaultFireSensor; //出口節點設定

            for (int MapType = 1; MapType < 4; MapType++)
            {
                List<ResultStorage> StorageList = new List<ResultStorage>();
                /*
                Console.Write("輸入高有多少Sensor：");
                SensorHeight = Convert.ToInt32(Console.ReadLine());

                Console.Write("輸入寬有多少Sensor：");
                SensorWidth = Convert.ToInt32(Console.ReadLine());
                */
                //int LoopTimes = 1; //How many times to simulate evecuation

                //Console.Write("Input Total Round");
                //LoopTimes = Convert.ToInt32(Console.ReadLine());
                //LoopTimes = 100;
                //Console.Write("Input Total People");
                //VictimsAmount = Convert.ToInt32(Console.ReadLine());
                //VictimsAmount = 250;
                /*
                Console.Write("輸入一單位走廊限制人數：");
                CorridorLimit = Convert.ToInt32(Console.ReadLine());
                */

                //Console.WriteLine("Setting Exit");
                //Set the exit sensor
                //DefaultExitSensor = new int[,] {{ 0, 0, -1,0, 0 },
                //    { SensorWidth - 1, (SensorHeight - 1) / 2, -1, 0, 0 },
                //    { (SensorWidth - 1) / 2, SensorHeight - 1, -1,0, 0  } };

                //DefaultFireSensor = new int[,] { { (SensorWidth - 1) / 2, (SensorHeight - 1) / 2, FireStartingNumber } };

                //Create a basic scenario
                Scenario BasicScenario = new Scenario();
                string ResultFileName = "Result_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_tt")
                    + "_" + "MapType0" + MapType.ToString() + ".csv";
                /*
                Excel.Application _Excel = null;
                _Excel = new Excel.Application();

                if (IfRecordScenario == false)
                    _Excel.Visible = true;
                else
                    _Excel.Visible = false;

                _Excel.Visible = true;

                Excel.Workbook book = _Excel.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                Excel.Worksheet sheet = (Worksheet)book.Worksheets[1];
                Excel.Range range = null;

                string typeExcel = "Result_Scenario_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss_tt") + ".xlsx";
                string ResultFolder = System.Windows.Forms.Application.StartupPath + "\\Result\\";
                string strPath = ResultFolder + typeExcel;
                /*
                book = _Excel.Workbooks.Open(strPath);
                System.IO.FileInfo xlsAttribute = new FileInfo(strPath);
                xlsAttribute.Attributes = FileAttributes.Normal;
                */
                /*
                BasicScenario.ExcelProcess = new ScenarioNode.ExcelProcess();
                BasicScenario.ExcelProcess.ExcelApp = _Excel;
                BasicScenario.ExcelProcess.Book = book;
                BasicScenario.ExcelProcess.Sheet = sheet;
                BasicScenario.ExcelProcess.Range = range;
            
                if (IfOpenExcel == false)
                {
                    _Excel.DisplayAlerts = false;
                    if (sheet != null)
                    {
                        Marshal.FinalReleaseComObject(sheet);
                    }
                    if (book != null)
                    {
                        book.Close(false); //忽略尚未存檔內容，避免跳出提示卡住
                        Marshal.FinalReleaseComObject(book);

                    }
                    if (_Excel != null)
                    {
                        _Excel.Workbooks.Close();
                        _Excel.Application.Quit();
                        _Excel.Quit();
                        Marshal.FinalReleaseComObject(_Excel);
                    }
                    CloseComExcel();
                }
                */
                Console.WriteLine("Start Simulation…");

                StartWriteCSV(ResultFileName);

                for (SensorHeight = 6; SensorHeight <= 10; SensorHeight++)
                    for (CorridorLimit = 3; CorridorLimit <= 10; CorridorLimit++)
                        for (VictimsAmount = 100; VictimsAmount < 1501; VictimsAmount += 100)
                        //for (SensorWidth = 4; SensorWidth < 7; SensorWidth++)
                        {
                            if (VictimsAmount < 2000 && VictimsAmount > 1750)
                                VictimsAmount += 150;
                            else if (VictimsAmount >= 2000 && VictimsAmount < 3000)
                                VictimsAmount += 400;
                            else if (VictimsAmount > 3000)
                                VictimsAmount += 900;


                            SensorWidth = SensorHeight;

                            //Set the exit sensor
                            DefaultExitSensor = new int[,] {{ 0, 0, -1,0, 0 }
                            , { SensorWidth-1 , (SensorHeight - 1) / 2, -1, 0, 0 }
                            , { (SensorWidth - 1) / 2, SensorHeight-1 , -1,0, 0  }};

                            DefaultFireSensor = new int[,] { { (SensorWidth - 1) / 3, (SensorHeight - 1) / 3, FireStartingNumber } };
                            //DefaultFireSensor = new int[,] { { 1, 0, FireStartingNumber } };

                            CAESWithHmDndS = new List<int>();
                            CAESWithSmS = new List<int>();
                            CAESWithSmD = new List<int>();
                            CAESWithHmDndSwS = new List<int>();
                            CAESWithSmSwS = new List<int>();
                            CAESWithSmDwS = new List<int>();
                            CAESWithHmDndSWithOldDL = new List<int>();
                            CAESWithSmSWithOldDL = new List<int>();
                            CAESWithSmDWithOldDL = new List<int>();
                            CAESWithHmDndSwSWithOldDL = new List<int>();
                            CAESWithSmSwSWithOldDL = new List<int>();
                            CAESWithSmDwSWithOldDL = new List<int>();
                            CAESWithHmDndSWithNewDL = new List<int>();
                            CAESWithSmSWithNewDL = new List<int>();
                            CAESWithSmDWithNewDL = new List<int>();
                            CAESWithHmDndSwSWithNewDL = new List<int>();
                            CAESWithSmSwSWithNewDL = new List<int>();
                            CAESWithSmDwSWithNewDL = new List<int>();
                            MSCG = new List<int>();
                            ESSPlus = new List<int>();
                            LEGS = new List<int>();
                            HEXInc = new List<int>();
                            ShortestPath = new List<int>();

                            CAESDeathWithHmDndS = new List<int>();
                            CAESDeathWithSmS = new List<int>();
                            CAESDeathWithSmD = new List<int>();
                            CAESDeathWithHmDndSwS = new List<int>();
                            CAESDeathWithSmSwS = new List<int>();
                            CAESDeathWithSmDwS = new List<int>();
                            CAESDeathWithHmDndSWithOldDL = new List<int>();
                            CAESDeathWithSmSWithOldDL = new List<int>();
                            CAESDeathWithSmDWithOldDL = new List<int>();
                            CAESDeathWithHmDndSwSWithOldDL = new List<int>();
                            CAESDeathWithSmSwSWithOldDL = new List<int>();
                            CAESDeathWithSmDwSWithOldDL = new List<int>();
                            CAESDeathWithHmDndSWithNewDL = new List<int>();
                            CAESDeathWithSmSWithNewDL = new List<int>();
                            CAESDeathWithSmDWithNewDL = new List<int>();
                            CAESDeathWithHmDndSwSWithNewDL = new List<int>();
                            CAESDeathWithSmSwSWithNewDL = new List<int>();
                            CAESDeathWithSmDwSWithNewDL = new List<int>();
                            MSCGDeath = new List<int>();
                            ESSPlusDeath = new List<int>();
                            LEGSDeath = new List<int>();
                            HEXIncDeath = new List<int>();
                            ShortestPathDeath = new List<int>();

                            for (int i = 1; i < 51; i++)
                            {

                                Console.WriteLine(DateTime.Now.ToString(@"hh\:mm\:ss"));
                                Console.WriteLine("Starting " + i.ToString() + "Round Simultaion");
                                Console.WriteLine(
                                    SensorWidth.ToString()
                                    + "x" + SensorHeight.ToString() + "node, "
                                    + CorridorLimit.ToString() + "Corridor Limit, "
                                    + VictimsAmount.ToString() + "people");

                                Console.WriteLine("Setting New Scenario…");
                                /*
                                ///Having set exit and fire
                                BasicScenario = SettingParameter.SetBasicParameter(
                                    BasicScenario
                                    , SensorWidth
                                    , SensorHeight
                                    , CorridorLimit
                                    , VictimsAmount
                                    , Alpha
                                    , Beta
                                    , Gamma
                                    , 1 //Scenario Type
                                    , DefaultExitSensor
                                    , DefaultFireSensor
                                    , FireSpreadTime);
                                */

                                ///Randomly set exit and fire
                                BasicScenario = SettingParameter.SetBasicParameterRNGFireNExit(
                                    BasicScenario
                                    , SensorWidth
                                    , SensorHeight
                                    , CorridorLimit
                                    , VictimsAmount
                                    , Alpha
                                    , Beta
                                    , Gamma
                                    , MapType //Scenario Type
                                    , 3
                                    , 2
                                    , FireSpreadTime);


                                //BasicScenario.ExcelProcess.IfRecordXlsx = IfRecordScenario;

                                BasicScenario.LoopRound = i + 1;

                                Console.WriteLine("Copy Scenario");
                                Scenario LoopScenario = new Scenario();
                                LoopScenario = SettingParameter.CopySenario(BasicScenario);
                                LoopScenario = SettingParameter.DistributeVictims(BasicScenario, VictimsAmount);

                                //Console.WriteLine("複製場景供CAES MixDL運行…");
                                Scenario CAESScenario = new Scenario();
                                CAESScenario = SettingParameter.CopySenario(LoopScenario);
                                CAESScenario.AlgorithmType = "CAES";
                                CAESScenario.DLCalculation = "MixDL";

                                //Console.WriteLine("複製場景供CAES OldDL運行…");
                                Scenario CAESScenarioWithOldDL = new Scenario();
                                CAESScenarioWithOldDL = SettingParameter.CopySenario(CAESScenario);
                                CAESScenarioWithOldDL.DLCalculation = "OldDL";

                                //Console.WriteLine("複製場景供CAES NewDL運行…");
                                Scenario CAESScenarioWithNewDL = new Scenario();
                                CAESScenarioWithNewDL = SettingParameter.CopySenario(CAESScenario);
                                CAESScenarioWithNewDL.DLCalculation = "NewDL";

                                //Console.WriteLine("複製場景供CAES MixDL門檻最高減標準差與離散指數(包含自身節點)運行…");
                                Scenario CAESScenarioHmSTDnDIwS = new Scenario();
                                CAESScenarioHmSTDnDIwS = SettingParameter.CopySenario(CAESScenario);
                                CAESScenarioHmSTDnDIwS.ThresholdType = "HmSTDnDIwS";

                                //Console.WriteLine("複製場景供CAES MixDL門檻自身減標準差(包含自身節點)運行…");
                                Scenario CAESScenarioSmSTDwS = new Scenario();
                                CAESScenarioSmSTDwS = SettingParameter.CopySenario(CAESScenario);
                                CAESScenarioSmSTDwS.ThresholdType = "SmSTDwS";

                                //Console.WriteLine("複製場景供CAES MixDL門檻自身減離散指數(包含自身節點)運行…");
                                Scenario CAESScenarioSmDIwS = new Scenario();
                                CAESScenarioSmDIwS = SettingParameter.CopySenario(CAESScenario);
                                CAESScenarioSmDIwS.ThresholdType = "SmDIwS";

                                //Console.WriteLine("複製場景供CAES MixDL門檻最高減標準差與離散指數(不包含自身節點)運行…");
                                Scenario CAESScenarioHmSTDnDI = new Scenario();
                                CAESScenarioHmSTDnDI = SettingParameter.CopySenario(CAESScenario);
                                CAESScenarioHmSTDnDI.ThresholdType = "HmSTDnDIwoS";

                                //Console.WriteLine("複製場景供CAES MixDL門檻自身減標準差(不包含自身節點)運行…");
                                Scenario CAESScenarioSmSTD = new Scenario();
                                CAESScenarioSmSTD = SettingParameter.CopySenario(CAESScenario);
                                CAESScenarioSmSTD.ThresholdType = "SmSTDwoS";

                                //Console.WriteLine("複製場景供CAES MixDL門檻自身減離散指數(不包含自身節點)運行…");
                                Scenario CAESScenarioSmDI = new Scenario();
                                CAESScenarioSmDI = SettingParameter.CopySenario(CAESScenario);
                                CAESScenarioSmDI.ThresholdType = "SmDIwoS";

                                //Console.WriteLine("複製場景供CAES OldDL門檻最高減標準差與離散指數(包含自身節點)運行…");
                                Scenario CAESScenarioHmSTDnDIwSWithOldDL = new Scenario();
                                CAESScenarioHmSTDnDIwSWithOldDL = SettingParameter.CopySenario(CAESScenarioWithOldDL);
                                CAESScenarioHmSTDnDIwSWithOldDL.ThresholdType = "HmSTDnDIwS";

                                //Console.WriteLine("複製場景供CAES OldDL門檻自身減標準差(包含自身節點)運行…");
                                Scenario CAESScenarioSmSTDwSWithOldDL = new Scenario();
                                CAESScenarioSmSTDwSWithOldDL = SettingParameter.CopySenario(CAESScenarioWithOldDL);
                                CAESScenarioSmSTDwSWithOldDL.ThresholdType = "SmSTDwS";

                                //Console.WriteLine("複製場景供CAES OldDL門檻自身減離散指數(包含自身節點)運行…");
                                Scenario CAESScenarioSmDIwSWithOldDL = new Scenario();
                                CAESScenarioSmDIwSWithOldDL = SettingParameter.CopySenario(CAESScenarioWithOldDL);
                                CAESScenarioSmDIwSWithOldDL.ThresholdType = "SmDIwS";

                                //Console.WriteLine("複製場景供CAES OldDL門檻最高減標準差與離散指數(不包含自身節點)運行…");
                                Scenario CAESScenarioHmSTDnDIWithOldDL = new Scenario();
                                CAESScenarioHmSTDnDIWithOldDL = SettingParameter.CopySenario(CAESScenarioWithOldDL);
                                CAESScenarioHmSTDnDIWithOldDL.ThresholdType = "HmSTDnDIwoS";

                                //Console.WriteLine("複製場景供CAES OldDL門檻自身減標準差(不包含自身節點)運行…");
                                Scenario CAESScenarioSmSTDWithOldDL = new Scenario();
                                CAESScenarioSmSTDWithOldDL = SettingParameter.CopySenario(CAESScenarioWithOldDL);
                                CAESScenarioSmSTDWithOldDL.ThresholdType = "SmSTDwoS";

                                //Console.WriteLine("複製場景供CAES OldDL門檻自身減離散指數(不包含自身節點)運行…");
                                Scenario CAESScenarioSmDIWithOldDL = new Scenario();
                                CAESScenarioSmDIWithOldDL = SettingParameter.CopySenario(CAESScenarioWithOldDL);
                                CAESScenarioSmDIWithOldDL.ThresholdType = "SmDIwoS";

                                //Console.WriteLine("複製場景供CAES NewDL門檻最高減標準差與離散指數(包含自身節點)運行…");
                                Scenario CAESScenarioHmSTDnDIwSWithNewDL = new Scenario();
                                CAESScenarioHmSTDnDIwSWithNewDL = SettingParameter.CopySenario(CAESScenarioWithNewDL);
                                CAESScenarioHmSTDnDIwSWithNewDL.ThresholdType = "HmSTDnDIwS";

                                //Console.WriteLine("複製場景供CAES NewDL門檻自身減標準差(包含自身節點)運行…");
                                Scenario CAESScenarioSmSTDwSWithNewDL = new Scenario();
                                CAESScenarioSmSTDwSWithNewDL = SettingParameter.CopySenario(CAESScenarioWithNewDL);
                                CAESScenarioSmSTDwSWithNewDL.ThresholdType = "SmSTDwS";

                                //Console.WriteLine("複製場景供CAES NewDL門檻自身減離散指數(包含自身節點)運行…");
                                Scenario CAESScenarioSmDIwSWithNewDL = new Scenario();
                                CAESScenarioSmDIwSWithNewDL = SettingParameter.CopySenario(CAESScenarioWithNewDL);
                                CAESScenarioSmDIwSWithNewDL.ThresholdType = "SmDIwS";

                                //Console.WriteLine("複製場景供CAES NewDL門檻最高減標準差與離散指數(不包含自身節點)運行…");
                                Scenario CAESScenarioHmSTDnDIWithNewDL = new Scenario();
                                CAESScenarioHmSTDnDIWithNewDL = SettingParameter.CopySenario(CAESScenarioWithNewDL);
                                CAESScenarioHmSTDnDIWithNewDL.ThresholdType = "HmSTDnDIwoS";

                                //Console.WriteLine("複製場景供CAES NewDL門檻自身減標準差(不包含自身節點)運行…");
                                Scenario CAESScenarioSmSTDWithNewDL = new Scenario();
                                CAESScenarioSmSTDWithNewDL = SettingParameter.CopySenario(CAESScenarioWithNewDL);
                                CAESScenarioSmSTDWithNewDL.ThresholdType = "SmSTDwoS";

                                //Console.WriteLine("複製場景供CAES NewDL門檻自身減離散指數(不包含自身節點)運行…");
                                Scenario CAESScenarioSmDIWithNewDL = new Scenario();
                                CAESScenarioSmDIWithNewDL = SettingParameter.CopySenario(CAESScenarioWithNewDL);
                                CAESScenarioSmDIWithNewDL.ThresholdType = "SmDIwoS";

                                //Console.WriteLine("複製場景供MSCG運行…");
                                Scenario MSCGScenario = new Scenario();
                                MSCGScenario = SettingParameter.CopySenario(LoopScenario);
                                MSCGScenario.AlgorithmType = "MSCG";

                                //Console.WriteLine("複製場景供ESS+運行…");
                                Scenario ESSPlusScenario = new Scenario();
                                ESSPlusScenario = SettingParameter.CopySenario(LoopScenario);
                                ESSPlusScenario.AlgorithmType = "ESSPlus";

                                //Console.WriteLine("複製場景供HEXInc運行…");
                                Scenario HEXIncScenario = new Scenario();
                                HEXIncScenario = SettingParameter.CopySenario(LoopScenario);
                                HEXIncScenario.AlgorithmType = "HEXInc";

                                //Console.WriteLine("複製場景供LEGS運行…");
                                Scenario LEGSScenario = new Scenario();
                                LEGSScenario = SettingParameter.CopySenario(LoopScenario);
                                LEGSScenario.AlgorithmType = "LEGS";

                                //Console.WriteLine("複製場景供SP運行…");
                                Scenario SPScenario = new Scenario();
                                SPScenario = SettingParameter.CopySenario(LoopScenario);
                                SPScenario.AlgorithmType = "ShortestPath";

                                //Console.WriteLine("逃生模擬開始…");
                                /*
                                Console.WriteLine("LEGS運行中…");
                                LEGSScenario = Simulation.NromalSimulation(LEGSScenario);
                                LEGS.Add(LEGSScenario.TotalEvacuationRound);
                                LEGSDeath.Add(LEGSScenario.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES MixDL門檻最高減標準差與離散指數(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioHmSTDnDIwS = new BackgroundWorker();
                                BGCAESScenarioHmSTDnDIwS.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioHmSTDnDIwS.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioHmSTDnDIwS.RunWorkerAsync(CAESScenarioHmSTDnDIwS);
                                /*
                                CAESScenarioHmSTDnDIwS = Simulation.NromalSimulation(CAESScenarioHmSTDnDIwS);
                                CAESWithHmDndSwS.Add(CAESScenarioHmSTDnDIwS.TotalEvacuationRound);
                                CAESDeathWithHmDndSwS.Add(CAESScenarioHmSTDnDIwS.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES MixDL門檻自身減標準差(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmSTDwS = new BackgroundWorker();
                                BGCAESScenarioSmSTDwS.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmSTDwS.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmSTDwS.RunWorkerAsync(CAESScenarioSmSTDwS);
                                /*
                                CAESScenarioSmSTDwS = Simulation.NromalSimulation(CAESScenarioSmSTDwS);
                                CAESWithSmSwS.Add(CAESScenarioSmSTDwS.TotalEvacuationRound);
                                CAESDeathWithSmSwS.Add(CAESScenarioSmSTDwS.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES MixDL門檻自身減離散指數(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmDIwS = new BackgroundWorker();
                                BGCAESScenarioSmDIwS.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmDIwS.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmDIwS.RunWorkerAsync(CAESScenarioSmDIwS);
                                /*
                                CAESScenarioSmDIwS = Simulation.NromalSimulation(CAESScenarioSmDIwS);
                                CAESWithSmDwS.Add(CAESScenarioSmDIwS.TotalEvacuationRound);
                                CAESDeathWithSmDwS.Add(CAESScenarioSmDIwS.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES MixDL門檻最高減標準差與離散指數(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioHmSTDnDI = new BackgroundWorker();
                                BGCAESScenarioHmSTDnDI.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioHmSTDnDI.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioHmSTDnDI.RunWorkerAsync(CAESScenarioHmSTDnDI);
                                /*
                                CAESScenarioHmSTDnDI = Simulation.NromalSimulation(CAESScenarioHmSTDnDI);
                                CAESWithHmDndS.Add(CAESScenarioHmSTDnDI.TotalEvacuationRound);
                                CAESDeathWithHmDndS.Add(CAESScenarioHmSTDnDI.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES MixDL門檻自身減標準差(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmSTD = new BackgroundWorker();
                                BGCAESScenarioSmSTD.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmSTD.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmSTD.RunWorkerAsync(CAESScenarioSmSTD);
                                /*
                                CAESScenarioSmSTD = Simulation.NromalSimulation(CAESScenarioSmSTD);
                                CAESWithSmS.Add(CAESScenarioSmSTD.TotalEvacuationRound);
                                CAESDeathWithSmS.Add(CAESScenarioSmSTD.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES MixDL門檻自身減離散指數(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmDI = new BackgroundWorker();
                                BGCAESScenarioSmDI.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmDI.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmDI.RunWorkerAsync(CAESScenarioSmDI);
                                /*
                                CAESScenarioSmDI = Simulation.NromalSimulation(CAESScenarioSmDI);
                                CAESWithSmD.Add(CAESScenarioSmDI.TotalEvacuationRound);
                                CAESDeathWithSmD.Add(CAESScenarioSmDI.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES OldDL門檻最高減標準差與離散指數(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioHmSTDnDIwSWithOldDL = new BackgroundWorker();
                                BGCAESScenarioHmSTDnDIwSWithOldDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioHmSTDnDIwSWithOldDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioHmSTDnDIwSWithOldDL.RunWorkerAsync(CAESScenarioHmSTDnDIwSWithOldDL);
                                /*
                                CAESScenarioHmSTDnDIwSWithOldDL = Simulation.NromalSimulation(CAESScenarioHmSTDnDIwSWithOldDL);
                                CAESWithHmDndSwSWithOldDL.Add(CAESScenarioHmSTDnDIwSWithOldDL.TotalEvacuationRound);
                                CAESDeathWithHmDndSwSWithOldDL.Add(CAESScenarioHmSTDnDIwSWithOldDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES OldDL門檻自身減標準差(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmSTDwSWithOldDL = new BackgroundWorker();
                                BGCAESScenarioSmSTDwSWithOldDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmSTDwSWithOldDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmSTDwSWithOldDL.RunWorkerAsync(CAESScenarioSmSTDwSWithOldDL);
                                /*
                                CAESScenarioSmSTDwSWithOldDL = Simulation.NromalSimulation(CAESScenarioSmSTDwSWithOldDL);
                                CAESWithSmSwSWithOldDL.Add(CAESScenarioSmSTDwSWithOldDL.TotalEvacuationRound);
                                CAESDeathWithSmSwSWithOldDL.Add(CAESScenarioSmSTDwSWithOldDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES OldDL門檻自身減離散指數(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmDIwSWithOldDL = new BackgroundWorker();
                                BGCAESScenarioSmDIwSWithOldDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmDIwSWithOldDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmDIwSWithOldDL.RunWorkerAsync(CAESScenarioSmDIwSWithOldDL);
                                /*
                                CAESScenarioSmDIwSWithOldDL = Simulation.NromalSimulation(CAESScenarioSmDIwSWithOldDL);
                                CAESWithSmDwSWithOldDL.Add(CAESScenarioSmDIwSWithOldDL.TotalEvacuationRound);
                                CAESDeathWithSmDwSWithOldDL.Add(CAESScenarioSmDIwSWithOldDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES OldDL門檻最高減標準差與離散指數(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioHmSTDnDIWithOldDL = new BackgroundWorker();
                                BGCAESScenarioHmSTDnDIWithOldDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioHmSTDnDIWithOldDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioHmSTDnDIWithOldDL.RunWorkerAsync(CAESScenarioHmSTDnDIWithOldDL);
                                /*
                                CAESScenarioHmSTDnDIWithOldDL = Simulation.NromalSimulation(CAESScenarioHmSTDnDIWithOldDL);
                                CAESWithHmDndSWithOldDL.Add(CAESScenarioHmSTDnDIWithOldDL.TotalEvacuationRound);
                                CAESDeathWithHmDndSWithOldDL.Add(CAESScenarioHmSTDnDIWithOldDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES OldDL門檻自身減標準差(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmSTDWithOldDL = new BackgroundWorker();
                                BGCAESScenarioSmSTDWithOldDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmSTDWithOldDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmSTDWithOldDL.RunWorkerAsync(CAESScenarioSmSTDWithOldDL);
                                /*
                                CAESScenarioSmSTDWithOldDL = Simulation.NromalSimulation(CAESScenarioSmSTDWithOldDL);
                                CAESWithSmSWithOldDL.Add(CAESScenarioSmSTDWithOldDL.TotalEvacuationRound);
                                CAESDeathWithSmSWithOldDL.Add(CAESScenarioSmSTDWithOldDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES OldDL門檻自身減離散指數(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmDIWithOldDL = new BackgroundWorker();
                                BGCAESScenarioSmDIWithOldDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmDIWithOldDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmDIWithOldDL.RunWorkerAsync(CAESScenarioSmDIWithOldDL);
                                /*
                                CAESScenarioSmDIWithOldDL = Simulation.NromalSimulation(CAESScenarioSmDIWithOldDL);
                                CAESWithSmDWithOldDL.Add(CAESScenarioSmDIWithOldDL.TotalEvacuationRound);
                                CAESDeathWithSmDWithOldDL.Add(CAESScenarioSmDIWithOldDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES NewDL門檻最高減標準差與離散指數(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioHmSTDnDIwSWithNewDL = new BackgroundWorker();
                                BGCAESScenarioHmSTDnDIwSWithNewDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioHmSTDnDIwSWithNewDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioHmSTDnDIwSWithNewDL.RunWorkerAsync(CAESScenarioHmSTDnDIwSWithNewDL);
                                /*
                                CAESScenarioHmSTDnDIwSWithNewDL = Simulation.NromalSimulation(CAESScenarioHmSTDnDIwSWithNewDL);
                                CAESWithHmDndSwSWithNewDL.Add(CAESScenarioHmSTDnDIwSWithNewDL.TotalEvacuationRound);
                                CAESDeathWithHmDndSwSWithNewDL.Add(CAESScenarioHmSTDnDIwSWithNewDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES NewDL門檻自身減標準差(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmSTDwSWithNewDL = new BackgroundWorker();
                                BGCAESScenarioSmSTDwSWithNewDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmSTDwSWithNewDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmSTDwSWithNewDL.RunWorkerAsync(CAESScenarioSmSTDwSWithNewDL);
                                /*
                                CAESScenarioSmSTDwSWithNewDL = Simulation.NromalSimulation(CAESScenarioSmSTDwSWithNewDL);
                                CAESWithSmSwSWithNewDL.Add(CAESScenarioSmSTDwSWithNewDL.TotalEvacuationRound);
                                CAESDeathWithSmSwSWithNewDL.Add(CAESScenarioSmSTDwSWithNewDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES NewDL門檻自身減離散指數(包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmDIwSWithNewDL = new BackgroundWorker();
                                BGCAESScenarioSmDIwSWithNewDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmDIwSWithNewDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmDIwSWithNewDL.RunWorkerAsync(CAESScenarioSmDIwSWithNewDL);
                                /*
                                CAESScenarioSmDIwSWithNewDL = Simulation.NromalSimulation(CAESScenarioSmDIwSWithNewDL);
                                CAESWithSmDwSWithNewDL.Add(CAESScenarioSmDIwSWithNewDL.TotalEvacuationRound);
                                CAESDeathWithSmDwSWithNewDL.Add(CAESScenarioSmDIwSWithNewDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES NewDL門檻最高減標準差與離散指數(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioHmSTDnDIWithNewDL = new BackgroundWorker();
                                BGCAESScenarioHmSTDnDIWithNewDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioHmSTDnDIWithNewDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioHmSTDnDIWithNewDL.RunWorkerAsync(CAESScenarioHmSTDnDIWithNewDL);
                                /*
                                CAESScenarioHmSTDnDIWithNewDL = Simulation.NromalSimulation(CAESScenarioHmSTDnDIWithNewDL);
                                CAESWithHmDndSWithNewDL.Add(CAESScenarioHmSTDnDIWithNewDL.TotalEvacuationRound);
                                CAESDeathWithHmDndSWithNewDL.Add(CAESScenarioHmSTDnDIWithNewDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES NewDL門檻自身減標準差(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmSTDWithNewDL = new BackgroundWorker();
                                BGCAESScenarioSmSTDWithNewDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmSTDWithNewDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmSTDWithNewDL.RunWorkerAsync(CAESScenarioSmSTDWithNewDL);
                                /*
                                CAESScenarioSmSTDWithNewDL = Simulation.NromalSimulation(CAESScenarioSmSTDWithNewDL);
                                CAESWithSmSWithNewDL.Add(CAESScenarioSmSTDWithNewDL.TotalEvacuationRound);
                                CAESDeathWithSmSWithNewDL.Add(CAESScenarioSmSTDWithNewDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("CAES NewDL門檻自身減離散指數(不包含自身節點)運行中…");
                                BackgroundWorker BGCAESScenarioSmDIWithNewDL = new BackgroundWorker();
                                BGCAESScenarioSmDIWithNewDL.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGCAESScenarioSmDIWithNewDL.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGCAESScenarioSmDIWithNewDL.RunWorkerAsync(CAESScenarioSmDIWithNewDL);
                                /*
                                CAESScenarioSmDIWithNewDL = Simulation.NromalSimulation(CAESScenarioSmDIWithNewDL);
                                CAESWithSmDWithNewDL.Add(CAESScenarioSmDIWithNewDL.TotalEvacuationRound);
                                CAESDeathWithSmDWithNewDL.Add(CAESScenarioSmDIWithNewDL.DeathVictimsAmount);
                                */

                                //Console.WriteLine("MSCG運行中…");
                                BackgroundWorker BGMSCGScenario = new BackgroundWorker();
                                BGMSCGScenario.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGMSCGScenario.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGMSCGScenario.RunWorkerAsync(MSCGScenario);
                                /*
                                MSCGScenario = Simulation.NromalSimulation(MSCGScenario);
                                MSCG.Add(MSCGScenario.TotalEvacuationRound);
                                MSCGDeath.Add(MSCGScenario.DeathVictimsAmount);
                                */

                                //Console.WriteLine("ESS+運行中…");
                                BackgroundWorker BGESSPlusScenario = new BackgroundWorker();
                                BGESSPlusScenario.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGESSPlusScenario.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGESSPlusScenario.RunWorkerAsync(ESSPlusScenario);
                                /*
                                ESSPlusScenario = Simulation.NromalSimulation(ESSPlusScenario);
                                ESSPlus.Add(ESSPlusScenario.TotalEvacuationRound);
                                ESSPlusDeath.Add(ESSPlusScenario.DeathVictimsAmount);
                                 * */

                                //Console.WriteLine("LEGS運行中…");
                                BackgroundWorker BGLEGSScenario = new BackgroundWorker();
                                BGLEGSScenario.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGLEGSScenario.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGLEGSScenario.RunWorkerAsync(LEGSScenario);
                                /*
                                LEGSScenario = Simulation.NromalSimulation(LEGSScenario);
                                LEGS.Add(LEGSScenario.TotalEvacuationRound);
                                LEGSDeath.Add(LEGSScenario.DeathVictimsAmount);
                                 * */

                                //Console.WriteLine("HEXInc運行中…");
                                BackgroundWorker BGHEXIncScenario = new BackgroundWorker();
                                BGHEXIncScenario.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGHEXIncScenario.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGHEXIncScenario.RunWorkerAsync(HEXIncScenario);
                                /*
                                HEXIncScenario = Simulation.NromalSimulation(HEXIncScenario);
                                HEXInc.Add(HEXIncScenario.TotalEvacuationRound);
                                HEXIncDeath.Add(HEXIncScenario.DeathVictimsAmount);
                                */

                                //Console.WriteLine("SP運行中…");
                                BackgroundWorker BGSPScenario = new BackgroundWorker();
                                BGSPScenario.DoWork += new DoWorkEventHandler(BGSimulation_DoWork);
                                BGSPScenario.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGSimulation_RunWorkerCompleted);
                                //BGSPScenario.RunWorkerAsync(SPScenario);
                                /*
                                SPScenario = Simulation.NromalSimulation(SPScenario);
                                ShortestPath.Add(SPScenario.TotalEvacuationRound);
                                ShortestPathDeath.Add(SPScenario.DeathVictimsAmount);
                                */


                                BGCAESScenarioHmSTDnDIwS.RunWorkerAsync(CAESScenarioHmSTDnDIwS);
                                BGCAESScenarioSmSTDwS.RunWorkerAsync(CAESScenarioSmSTDwS);
                                BGCAESScenarioSmDIwS.RunWorkerAsync(CAESScenarioSmDIwS);
                                BGCAESScenarioHmSTDnDI.RunWorkerAsync(CAESScenarioHmSTDnDI);
                                BGCAESScenarioSmSTD.RunWorkerAsync(CAESScenarioSmSTD);
                                BGCAESScenarioSmDI.RunWorkerAsync(CAESScenarioSmDI);
                                BGCAESScenarioHmSTDnDIwSWithOldDL.RunWorkerAsync(CAESScenarioHmSTDnDIwSWithOldDL);
                                BGCAESScenarioSmSTDwSWithOldDL.RunWorkerAsync(CAESScenarioSmSTDwSWithOldDL);
                                BGCAESScenarioSmDIwSWithOldDL.RunWorkerAsync(CAESScenarioSmDIwSWithOldDL);
                                BGCAESScenarioHmSTDnDIWithOldDL.RunWorkerAsync(CAESScenarioHmSTDnDIWithOldDL);
                                BGCAESScenarioSmSTDWithOldDL.RunWorkerAsync(CAESScenarioSmSTDWithOldDL);
                                BGCAESScenarioSmDIWithOldDL.RunWorkerAsync(CAESScenarioSmDIWithOldDL);
                                BGCAESScenarioHmSTDnDIwSWithNewDL.RunWorkerAsync(CAESScenarioHmSTDnDIwSWithNewDL);
                                BGCAESScenarioSmSTDwSWithNewDL.RunWorkerAsync(CAESScenarioSmSTDwSWithNewDL);
                                BGCAESScenarioSmDIwSWithNewDL.RunWorkerAsync(CAESScenarioSmDIwSWithNewDL);
                                BGCAESScenarioHmSTDnDIWithNewDL.RunWorkerAsync(CAESScenarioHmSTDnDIWithNewDL);
                                BGCAESScenarioSmSTDWithNewDL.RunWorkerAsync(CAESScenarioSmSTDWithNewDL);
                                BGCAESScenarioSmDIWithNewDL.RunWorkerAsync(CAESScenarioSmDIWithNewDL);
                                BGMSCGScenario.RunWorkerAsync(MSCGScenario);
                                BGLEGSScenario.RunWorkerAsync(LEGSScenario);
                                BGHEXIncScenario.RunWorkerAsync(HEXIncScenario);
                                BGSPScenario.RunWorkerAsync(SPScenario);
                                BGESSPlusScenario.RunWorkerAsync(ESSPlusScenario);

                                bool CheckBGRunning = true;

                                //Thread.Sleep(1000);
                            /*
                            while (CheckBGRunning)
                            {
                                CheckBGRunning = false;

                                string NotYet = "";

                                if (BGCAESScenarioHmSTDnDIwS.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioHmSTDnDIwS) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDwS.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmSTDwS) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIwS.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmDIwS) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDI.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioHmSTDnDI) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTD.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmSTD) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDI.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmDI) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDIwSWithOldDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioHmSTDnDIwSWithOldDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDwSWithOldDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmSTDwSWithOldDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIwSWithOldDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmDIwSWithOldDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDIWithOldDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioHmSTDnDIWithOldDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDWithOldDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmSTDWithOldDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIWithOldDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmDIWithOldDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDIwSWithNewDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioHmSTDnDIwSWithNewDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDwSWithNewDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmSTDwSWithNewDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIwSWithNewDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmDIwSWithNewDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDIWithNewDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioHmSTDnDIWithNewDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDWithNewDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmSTDWithNewDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIWithNewDL.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(CAESScenarioSmDIWithNewDL) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGMSCGScenario.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(MSCGScenario) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGESSPlusScenario.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(ESSPlusScenario) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGLEGSScenario.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(LEGSScenario) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGHEXIncScenario.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(HEXIncScenario) + "\n";
                                    CheckBGRunning = true;
                                }
                                if (BGSPScenario.IsBusy)
                                {
                                    NotYet += CheckWhereStayCounting(SPScenario) + "\n";
                                    CheckBGRunning = true;
                                }

                                //Console.WriteLine("StillRunning:\n" + NotYet);

                                Thread.Sleep(2000);
                            }
                            */

                            StartCheck:
                                Thread.Sleep(150);
                                //await PutTaskDelay();
                                CheckBGRunning = false;

                                if (BGCAESScenarioHmSTDnDIwS.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDwS.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIwS.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDI.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTD.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDI.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDIwSWithOldDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDwSWithOldDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIwSWithOldDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDIWithOldDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDWithOldDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIWithOldDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDIwSWithNewDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDwSWithNewDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIwSWithNewDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioHmSTDnDIWithNewDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmSTDWithNewDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGCAESScenarioSmDIWithNewDL.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGMSCGScenario.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGESSPlusScenario.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGLEGSScenario.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGHEXIncScenario.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }
                                if (BGSPScenario.IsBusy)
                                {
                                    CheckBGRunning = true;
                                }

                                if (CheckBGRunning)
                                    goto StartCheck;

                                Thread.Sleep(50);
                                CAESWithHmDndSwS.Add(ReadThings(CAESScenarioHmSTDnDIwS)[0]);
                                CAESDeathWithHmDndSwS.Add(ReadThings(CAESScenarioHmSTDnDIwS)[1]);

                                CAESWithSmSwS.Add(ReadThings(CAESScenarioSmSTDwS)[0]);
                                CAESDeathWithSmSwS.Add(ReadThings(CAESScenarioSmSTDwS)[1]);

                                CAESWithSmDwS.Add(ReadThings(CAESScenarioSmDIwS)[0]);
                                CAESDeathWithSmDwS.Add(ReadThings(CAESScenarioSmDIwS)[1]);

                                CAESWithHmDndS.Add(ReadThings(CAESScenarioHmSTDnDI)[0]);
                                CAESDeathWithHmDndS.Add(ReadThings(CAESScenarioHmSTDnDI)[1]);

                                CAESWithSmS.Add(ReadThings(CAESScenarioSmSTD)[0]);
                                CAESDeathWithSmS.Add(ReadThings(CAESScenarioSmSTD)[1]);

                                CAESWithSmD.Add(ReadThings(CAESScenarioSmDI)[0]);
                                CAESDeathWithSmD.Add(ReadThings(CAESScenarioSmDI)[1]);

                                CAESWithHmDndSwSWithOldDL.Add(ReadThings(CAESScenarioHmSTDnDIwSWithOldDL)[0]);
                                CAESDeathWithHmDndSwSWithOldDL.Add(ReadThings(CAESScenarioHmSTDnDIwSWithOldDL)[1]);

                                CAESWithSmSwSWithOldDL.Add(ReadThings(CAESScenarioSmSTDwSWithOldDL)[0]);
                                CAESDeathWithSmSwSWithOldDL.Add(ReadThings(CAESScenarioSmSTDwSWithOldDL)[1]);

                                CAESWithSmDwSWithOldDL.Add(ReadThings(CAESScenarioSmDIwSWithOldDL)[0]);
                                CAESDeathWithSmDwSWithOldDL.Add(ReadThings(CAESScenarioSmDIwSWithOldDL)[1]);

                                CAESWithHmDndSWithOldDL.Add(ReadThings(CAESScenarioHmSTDnDIWithOldDL)[0]);
                                CAESDeathWithHmDndSWithOldDL.Add(ReadThings(CAESScenarioHmSTDnDIWithOldDL)[1]);

                                CAESWithSmSWithOldDL.Add(ReadThings(CAESScenarioSmSTDWithOldDL)[0]);
                                CAESDeathWithSmSWithOldDL.Add(ReadThings(CAESScenarioSmSTDWithOldDL)[1]);

                                CAESWithSmDWithOldDL.Add(ReadThings(CAESScenarioSmDIWithOldDL)[0]);
                                CAESDeathWithSmDWithOldDL.Add(ReadThings(CAESScenarioSmDIWithOldDL)[1]);

                                CAESWithHmDndSwSWithNewDL.Add(ReadThings(CAESScenarioHmSTDnDIwSWithNewDL)[0]);
                                CAESDeathWithHmDndSwSWithNewDL.Add(ReadThings(CAESScenarioHmSTDnDIwSWithNewDL)[1]);

                                CAESWithSmSwSWithNewDL.Add(ReadThings(CAESScenarioSmSTDwSWithNewDL)[0]);
                                CAESDeathWithSmSwSWithNewDL.Add(ReadThings(CAESScenarioSmSTDwSWithNewDL)[1]);

                                CAESWithSmDwSWithNewDL.Add(ReadThings(CAESScenarioSmDIwSWithNewDL)[0]);
                                CAESDeathWithSmDwSWithNewDL.Add(ReadThings(CAESScenarioSmDIwSWithNewDL)[1]);

                                CAESWithHmDndSWithNewDL.Add(ReadThings(CAESScenarioHmSTDnDIWithNewDL)[0]);
                                CAESDeathWithHmDndSWithNewDL.Add(ReadThings(CAESScenarioHmSTDnDIWithNewDL)[1]);

                                CAESWithSmSWithNewDL.Add(ReadThings(CAESScenarioSmSTDWithNewDL)[0]);
                                CAESDeathWithSmSWithNewDL.Add(ReadThings(CAESScenarioSmSTDWithNewDL)[1]);

                                CAESWithSmDWithNewDL.Add(ReadThings(CAESScenarioSmDIWithNewDL)[0]);
                                CAESDeathWithSmDWithNewDL.Add(ReadThings(CAESScenarioSmDIWithNewDL)[1]);

                                MSCG.Add(ReadThings(MSCGScenario)[0]);
                                MSCGDeath.Add(ReadThings(MSCGScenario)[1]);

                                ESSPlus.Add(ReadThings(ESSPlusScenario)[0]);
                                ESSPlusDeath.Add(ReadThings(ESSPlusScenario)[1]);

                                LEGS.Add(ReadThings(LEGSScenario)[0]);
                                LEGSDeath.Add(ReadThings(LEGSScenario)[1]);

                                HEXInc.Add(ReadThings(HEXIncScenario)[0]);
                                HEXIncDeath.Add(ReadThings(HEXIncScenario)[1]);

                                ShortestPath.Add(ReadThings(SPScenario)[0]);
                                ShortestPathDeath.Add(ReadThings(SPScenario)[1]);
                            }
                            /*
                            //Add result to result list
                            StorageList = AddNewResult(
                                StorageList
                                , CorridorLimit
                                , VictimsAmount
                                , SensorHeight
                                , SensorWidth
                                , CAESWithHmDndS
                                , CAESWithSmS
                                , CAESWithSmD
                                , CAESWithHmDndSwS
                                , CAESWithSmSwS
                                , CAESWithSmDwS
                                , CAESWithHmDndSWithOldDL
                                , CAESWithSmSWithOldDL
                                , CAESWithSmDWithOldDL
                                , CAESWithHmDndSwSWithOldDL
                                , CAESWithSmSwSWithOldDL
                                , CAESWithSmDwSWithOldDL
                                , CAESWithHmDndSWithNewDL
                                , CAESWithSmSWithNewDL
                                , CAESWithSmDWithNewDL
                                , CAESWithHmDndSwSWithNewDL
                                , CAESWithSmSwSWithNewDL
                                , CAESWithSmDwSWithNewDL
                                , MSCG
                                , ESSPlus
                                , LEGS
                                , HEXInc
                                , ShortestPath
                                , CAESDeathWithHmDndS
                                , CAESDeathWithSmS
                                , CAESDeathWithSmD
                                , CAESDeathWithHmDndSwS
                                , CAESDeathWithSmSwS
                                , CAESDeathWithSmDwS
                                , CAESDeathWithHmDndSWithOldDL
                                , CAESDeathWithSmSWithOldDL
                                , CAESDeathWithSmDWithOldDL
                                , CAESDeathWithHmDndSwSWithOldDL
                                , CAESDeathWithSmSwSWithOldDL
                                , CAESDeathWithSmDwSWithOldDL
                                , CAESDeathWithHmDndSWithNewDL
                                , CAESDeathWithSmSWithNewDL
                                , CAESDeathWithSmDWithNewDL
                                , CAESDeathWithHmDndSwSWithNewDL
                                , CAESDeathWithSmSwSWithNewDL
                                , CAESDeathWithSmDwSWithNewDL
                                , MSCGDeath
                                , ESSPlusDeath
                                , LEGSDeath
                                , HEXIncDeath
                                , ShortestPathDeath);
                            */
                            Thread.Sleep(100);
                            ContinueWriteCSV(FindResult50Round(
                                 StorageList
                                 , CorridorLimit
                                 , VictimsAmount
                                 , SensorHeight
                                 , SensorWidth
                                 , CAESWithHmDndS
                                 , CAESWithSmS
                                 , CAESWithSmD
                                 , CAESWithHmDndSwS
                                 , CAESWithSmSwS
                                 , CAESWithSmDwS
                                 , CAESWithHmDndSWithOldDL
                                 , CAESWithSmSWithOldDL
                                 , CAESWithSmDWithOldDL
                                 , CAESWithHmDndSwSWithOldDL
                                 , CAESWithSmSwSWithOldDL
                                 , CAESWithSmDwSWithOldDL
                                 , CAESWithHmDndSWithNewDL
                                 , CAESWithSmSWithNewDL
                                 , CAESWithSmDWithNewDL
                                 , CAESWithHmDndSwSWithNewDL
                                 , CAESWithSmSwSWithNewDL
                                 , CAESWithSmDwSWithNewDL
                                 , MSCG
                                 , ESSPlus
                                 , LEGS
                                 , HEXInc
                                 , ShortestPath
                                 , CAESDeathWithHmDndS
                                 , CAESDeathWithSmS
                                 , CAESDeathWithSmD
                                 , CAESDeathWithHmDndSwS
                                 , CAESDeathWithSmSwS
                                 , CAESDeathWithSmDwS
                                 , CAESDeathWithHmDndSWithOldDL
                                 , CAESDeathWithSmSWithOldDL
                                 , CAESDeathWithSmDWithOldDL
                                 , CAESDeathWithHmDndSwSWithOldDL
                                 , CAESDeathWithSmSwSWithOldDL
                                 , CAESDeathWithSmDwSWithOldDL
                                 , CAESDeathWithHmDndSWithNewDL
                                 , CAESDeathWithSmSWithNewDL
                                 , CAESDeathWithSmDWithNewDL
                                 , CAESDeathWithHmDndSwSWithNewDL
                                 , CAESDeathWithSmSwSWithNewDL
                                 , CAESDeathWithSmDwSWithNewDL
                                 , MSCGDeath
                                 , ESSPlusDeath
                                 , LEGSDeath
                                 , HEXIncDeath
                                 , ShortestPathDeath), ResultFileName);

                        }

            }

            //Thread.Sleep(100);
            //ContinueWriteCSV(StorageList, ResultFileName);
            //WriteCSV(StorageList, ResultFileName);

            //book.SaveAs("Result Scenario " + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss tt") + ".xlsx", Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            //WriteALotResultToCSV(StorageList);
            /*
            if (IfRecordScenario == true)
            {
                sheet = (Excel.Worksheet)_Excel.Application.Worksheets.Add();
                sheet = (Excel.Worksheet)book.Worksheets[1];
            }
            sheet.Name = "Total Result";

            AddNewResultToXlsx(_Excel, sheet, book, range, StorageList);
            //book.SaveCopyAs(strPath);

            if (!System.IO.File.Exists(ResultFolder))
            {
                System.IO.Directory.CreateDirectory(ResultFolder);
            }

            book.SaveAs(Filename: strPath, AccessMode: Excel.XlSaveAsAccessMode.xlNoChange);

            _Excel.DisplayAlerts = false;
            if (sheet != null)
            {
                Marshal.FinalReleaseComObject(sheet);
                sheet = null;
            }
            if (book != null)
            {
                book.Close(false); //忽略尚未存檔內容，避免跳出提示卡住
                Marshal.FinalReleaseComObject(book);
                book = null;
            }
            if (_Excel != null)
            {
                _Excel.Workbooks.Close();
                _Excel.Quit();
                Marshal.FinalReleaseComObject(_Excel);
                _Excel = null;
            }
            CloseComExcel();
            GC.Collect();
            */
        }

        async Task PutTaskDelay()
        {
            await Task.Delay(5000);
        }


        public static void StartWriteCSV(string FileName)
        {

            string ResultFolder = System.Windows.Forms.Application.StartupPath + "\\Result\\";

            if (!System.IO.File.Exists(ResultFolder))
            {
                System.IO.Directory.CreateDirectory(ResultFolder);
            }


            using (StreamWriter sw = new StreamWriter(ResultFolder + FileName))   //小寫TXT     
            {
                string FirstLineStr = ",,,,,,,,,,,,,woSMix,woSMix,woSMix,wSMix,wSMix,wSMix,woSOld,woSOld,woSOld,wSOld,wSOld,wSOld,woSNew,woSNew,woSNew,wSNew,wSNew,wSNew";
                string SecLineStr = ",LateralNodeAmount,PortraitNodeAmount,TotalVictims,CorridorLimit,SP,CAESMixDL,CAESOldDL,CAESNewDL,MSCG,ESSPlus,LEGS,HEXInc,HighestDL-DI-D,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI";
                sw.WriteLine(FirstLineStr);
                sw.WriteLine(SecLineStr);
            }
        }

        public static void ContinueWriteCSV(ResultStorage Result, string FileName)
        {
            string ResultFolder = System.Windows.Forms.Application.StartupPath + "\\Result\\";

            if (!System.IO.File.Exists(ResultFolder))
            {
                System.IO.Directory.CreateDirectory(ResultFolder);
            }


            using (StreamWriter sw = new StreamWriter(ResultFolder + FileName, true))   //小寫TXT     
            {
                double CAESMixDL = Math.Round((double)(
                    Result.CGAWithHighestMinusDIandSTDResultwithSelf
                    + Result.CGAWithSelfMinusDIResultwithSelf
                    + Result.CGAWithSelfMinusSTDResultwithSelf
                    + Result.CGAWithHighestMinusDIandSTDResult
                    + Result.CGAWithSelfMinusDIResult
                    + Result.CGAWithSelfMinusSTDResult) / (double)6, 15);
                double CAESOldDL = Math.Round((double)(
                    Result.CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL
                    + Result.CGAWithSelfMinusDIResultwithSelfWithOldDL
                    + Result.CGAWithSelfMinusSTDResultwithSelfWithOldDL
                    + Result.CGAWithHighestMinusDIandSTDResultWithOldDL
                    + Result.CGAWithSelfMinusDIResultWithOldDL
                    + Result.CGAWithSelfMinusSTDResultWithOldDL) / (double)6, 15);
                double CAESNewDL = Math.Round((double)(
                    Result.CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL
                    + Result.CGAWithSelfMinusDIResultwithSelfWithNewDL
                    + Result.CGAWithSelfMinusSTDResultwithSelfWithNewDL
                    + Result.CGAWithHighestMinusDIandSTDResultWithNewDL
                    + Result.CGAWithSelfMinusDIResultWithNewDL
                    + Result.CGAWithSelfMinusSTDResultWithNewDL) / (double)6, 15);
                string ResultLineStr = "Round," + Result.width.ToString()
                                           + "," + Result.height.ToString()
                                           + "," + Result.totalpeople.ToString()
                                           + "," + Result.corridorlimit.ToString()
                                           + "," + Result.ShortestPath.ToString()
                                           + "," + CAESMixDL.ToString()
                                           + "," + CAESOldDL.ToString()
                                           + "," + CAESNewDL.ToString()
                                           + "," + Result.MSCG.ToString()
                                           + "," + Result.ESSPlus.ToString()
                                           + "," + Result.LEGS.ToString()
                                           + "," + Result.HEXInc.ToString()
                                           + "," + Result.CGAWithHighestMinusDIandSTDResultwithSelf.ToString()
                                           + "," + Result.CGAWithSelfMinusDIResultwithSelf.ToString()
                                           + "," + Result.CGAWithSelfMinusSTDResultwithSelf.ToString()
                                           + "," + Result.CGAWithHighestMinusDIandSTDResult.ToString()
                                           + "," + Result.CGAWithSelfMinusDIResult.ToString()
                                           + "," + Result.CGAWithSelfMinusSTDResult.ToString()
                                           + "," + Result.CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL.ToString()
                                           + "," + Result.CGAWithSelfMinusDIResultwithSelfWithOldDL.ToString()
                                           + "," + Result.CGAWithSelfMinusSTDResultwithSelfWithOldDL.ToString()
                                           + "," + Result.CGAWithHighestMinusDIandSTDResultWithOldDL.ToString()
                                           + "," + Result.CGAWithSelfMinusDIResultWithOldDL.ToString()
                                           + "," + Result.CGAWithSelfMinusSTDResultWithOldDL.ToString()
                                           + "," + Result.CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL.ToString()
                                           + "," + Result.CGAWithSelfMinusDIResultwithSelfWithNewDL.ToString()
                                           + "," + Result.CGAWithSelfMinusSTDResultwithSelfWithNewDL.ToString()
                                           + "," + Result.CGAWithHighestMinusDIandSTDResultWithNewDL.ToString()
                                           + "," + Result.CGAWithSelfMinusDIResultWithNewDL.ToString()
                                           + "," + Result.CGAWithSelfMinusSTDResultWithNewDL.ToString();


                sw.WriteLine(ResultLineStr);

                double CAESMixDLDeath = Math.Round((double)(
                    Result.CGADeathWithHighestMinusDIandSTDResultwithSelf
                    + Result.CGADeathWithSelfMinusDIResultwithSelf
                    + Result.CGADeathWithSelfMinusSTDResultwithSelf
                    + Result.CGADeathWithHighestMinusDIandSTDResult
                    + Result.CGADeathWithSelfMinusDIResult
                    + Result.CGADeathWithSelfMinusSTDResult) / (double)6, 15);
                double CAESOldDLDeath = Math.Round((double)(
                    Result.CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL
                    + Result.CGADeathWithSelfMinusDIResultwithSelfWithOldDL
                    + Result.CGADeathWithSelfMinusSTDResultwithSelfWithOldDL
                    + Result.CGADeathWithHighestMinusDIandSTDResultWithOldDL
                    + Result.CGADeathWithSelfMinusDIResultWithOldDL
                    + Result.CGADeathWithSelfMinusSTDResultWithOldDL) / (double)6, 15);
                double CAESNewDLDeath = Math.Round((double)(
                    Result.CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL
                    + Result.CGADeathWithSelfMinusDIResultwithSelfWithNewDL
                    + Result.CGADeathWithSelfMinusSTDResultwithSelfWithNewDL
                    + Result.CGADeathWithHighestMinusDIandSTDResultWithNewDL
                    + Result.CGADeathWithSelfMinusDIResultWithNewDL
                    + Result.CGADeathWithSelfMinusSTDResultWithNewDL) / (double)6, 15);

                string DeathResultLineStr = "DeathAmount," + Result.width.ToString()
                                                + "," + Result.height.ToString()
                                                + "," + Result.totalpeople.ToString()
                                                + "," + Result.corridorlimit.ToString()
                                                + "," + Result.ShortestPathDeath.ToString()
                                                + "," + CAESMixDLDeath.ToString()
                                                + "," + CAESOldDLDeath.ToString()
                                                + "," + CAESNewDLDeath.ToString()
                                                + "," + Result.MSCGDeath.ToString()
                                                + "," + Result.ESSPlusDeath.ToString()
                                                + "," + Result.LEGSDeath.ToString()
                                                + "," + Result.HEXIncDeath.ToString()
                                                + "," + Result.CGADeathWithHighestMinusDIandSTDResultwithSelf.ToString()
                                                + "," + Result.CGADeathWithSelfMinusDIResultwithSelf.ToString()
                                                + "," + Result.CGADeathWithSelfMinusSTDResultwithSelf.ToString()
                                                + "," + Result.CGADeathWithHighestMinusDIandSTDResult.ToString()
                                                + "," + Result.CGADeathWithSelfMinusDIResult.ToString()
                                                + "," + Result.CGADeathWithSelfMinusSTDResult.ToString()
                                                + "," + Result.CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL.ToString()
                                                + "," + Result.CGADeathWithSelfMinusDIResultwithSelfWithOldDL.ToString()
                                                + "," + Result.CGADeathWithSelfMinusSTDResultwithSelfWithOldDL.ToString()
                                                + "," + Result.CGADeathWithHighestMinusDIandSTDResultWithOldDL.ToString()
                                                + "," + Result.CGADeathWithSelfMinusDIResultWithOldDL.ToString()
                                                + "," + Result.CGADeathWithSelfMinusSTDResultWithOldDL.ToString()
                                                + "," + Result.CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL.ToString()
                                                + "," + Result.CGADeathWithSelfMinusDIResultwithSelfWithNewDL.ToString()
                                                + "," + Result.CGADeathWithSelfMinusSTDResultwithSelfWithNewDL.ToString()
                                                + "," + Result.CGADeathWithHighestMinusDIandSTDResultWithNewDL.ToString()
                                                + "," + Result.CGADeathWithSelfMinusDIResultWithNewDL.ToString()
                                                + "," + Result.CGADeathWithSelfMinusSTDResultWithNewDL.ToString();

                sw.WriteLine(DeathResultLineStr);
            }
        }


        public static ResultStorage FindResult50Round(
            List<ResultStorage> OriginStorage
            , int CorridorLimit
            , int VictimsAmount
            , int SensorHeight
            , int SensorWidth
            , List<int> CAESWithHmDndS
            , List<int> CAESWithSmS
            , List<int> CAESWithSmD
            , List<int> CAESWithHmDndSwS
            , List<int> CAESWithSmSwS
            , List<int> CAESWithSmDwS
            , List<int> CAESWithHmDndSWithOldDL
            , List<int> CAESWithSmSWithOldDL
            , List<int> CAESWithSmDWithOldDL
            , List<int> CAESWithHmDndSwSWithOldDL
            , List<int> CAESWithSmSwSWithOldDL
            , List<int> CAESWithSmDwSWithOldDL
            , List<int> CAESWithHmDndSWithNewDL
            , List<int> CAESWithSmSWithNewDL
            , List<int> CAESWithSmDWithNewDL
            , List<int> CAESWithHmDndSwSWithNewDL
            , List<int> CAESWithSmSwSWithNewDL
            , List<int> CAESWithSmDwSWithNewDL
            , List<int> MSCG
            , List<int> ESSPlus
            , List<int> LEGS
            , List<int> HEXInc
            , List<int> ShortestPath
            , List<int> CAESDeathWithHmDndS
            , List<int> CAESDeathWithSmS
            , List<int> CAESDeathWithSmD
            , List<int> CAESDeathWithHmDndSwS
            , List<int> CAESDeathWithSmSwS
            , List<int> CAESDeathWithSmDwS
            , List<int> CAESDeathWithHmDndSWithOldDL
            , List<int> CAESDeathWithSmSWithOldDL
            , List<int> CAESDeathWithSmDWithOldDL
            , List<int> CAESDeathWithHmDndSwSWithOldDL
            , List<int> CAESDeathWithSmSwSWithOldDL
            , List<int> CAESDeathWithSmDwSWithOldDL
            , List<int> CAESDeathWithHmDndSWithNewDL
            , List<int> CAESDeathWithSmSWithNewDL
            , List<int> CAESDeathWithSmDWithNewDL
            , List<int> CAESDeathWithHmDndSwSWithNewDL
            , List<int> CAESDeathWithSmSwSWithNewDL
            , List<int> CAESDeathWithSmDwSWithNewDL
            , List<int> MSCGDeath
            , List<int> ESSPlusDeath
            , List<int> LEGSDeath
            , List<int> HEXIncDeath
            , List<int> ShortestPathDeath)
        {
            ResultStorage StorageTemp = new ResultStorage();
            StorageTemp.corridorlimit = CorridorLimit;
            StorageTemp.totalpeople = VictimsAmount;
            StorageTemp.height = SensorHeight;
            StorageTemp.width = SensorWidth;
            StorageTemp.CGAWithHighestMinusDIandSTDResult = MeanOfResult(CAESWithHmDndS);
            StorageTemp.CGAWithSelfMinusSTDResult = MeanOfResult(CAESWithSmS);
            StorageTemp.CGAWithSelfMinusDIResult = MeanOfResult(CAESWithSmD);
            StorageTemp.CGAWithHighestMinusDIandSTDResultwithSelf = MeanOfResult(CAESWithHmDndSwS);
            StorageTemp.CGAWithSelfMinusSTDResultwithSelf = MeanOfResult(CAESWithSmSwS);
            StorageTemp.CGAWithSelfMinusDIResultwithSelf = MeanOfResult(CAESWithSmDwS);
            StorageTemp.CGAWithHighestMinusDIandSTDResultWithOldDL = MeanOfResult(CAESWithHmDndSWithOldDL);
            StorageTemp.CGAWithSelfMinusSTDResultWithOldDL = MeanOfResult(CAESWithSmSWithOldDL);
            StorageTemp.CGAWithSelfMinusDIResultWithOldDL = MeanOfResult(CAESWithSmDWithOldDL);
            StorageTemp.CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL = MeanOfResult(CAESWithHmDndSwSWithOldDL);
            StorageTemp.CGAWithSelfMinusSTDResultwithSelfWithOldDL = MeanOfResult(CAESWithSmSwSWithOldDL);
            StorageTemp.CGAWithSelfMinusDIResultwithSelfWithOldDL = MeanOfResult(CAESWithSmDwSWithOldDL);
            StorageTemp.CGAWithHighestMinusDIandSTDResultWithNewDL = MeanOfResult(CAESWithHmDndSWithNewDL);
            StorageTemp.CGAWithSelfMinusSTDResultWithNewDL = MeanOfResult(CAESWithSmSWithNewDL);
            StorageTemp.CGAWithSelfMinusDIResultWithNewDL = MeanOfResult(CAESWithSmDWithNewDL);
            StorageTemp.CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL = MeanOfResult(CAESWithHmDndSwSWithNewDL);
            StorageTemp.CGAWithSelfMinusSTDResultwithSelfWithNewDL = MeanOfResult(CAESWithSmSwSWithNewDL);
            StorageTemp.CGAWithSelfMinusDIResultwithSelfWithNewDL = MeanOfResult(CAESWithSmDwSWithNewDL);
            StorageTemp.MSCG = MeanOfResult(MSCG);
            StorageTemp.ESSPlus = MeanOfResult(ESSPlus);
            StorageTemp.LEGS = MeanOfResult(LEGS);
            StorageTemp.HEXInc = MeanOfResult(HEXInc);
            StorageTemp.ShortestPath = MeanOfResult(ShortestPath);

            StorageTemp.CGADeathWithHighestMinusDIandSTDResult = MeanOfResult(CAESDeathWithHmDndS);
            StorageTemp.CGADeathWithSelfMinusSTDResult = MeanOfResult(CAESDeathWithSmS);
            StorageTemp.CGADeathWithSelfMinusDIResult = MeanOfResult(CAESDeathWithSmD);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultwithSelf = MeanOfResult(CAESDeathWithHmDndSwS);
            StorageTemp.CGADeathWithSelfMinusSTDResultwithSelf = MeanOfResult(CAESDeathWithSmSwS);
            StorageTemp.CGADeathWithSelfMinusDIResultwithSelf = MeanOfResult(CAESDeathWithSmDwS);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultWithOldDL = MeanOfResult(CAESDeathWithHmDndSWithOldDL);
            StorageTemp.CGADeathWithSelfMinusSTDResultWithOldDL = MeanOfResult(CAESDeathWithSmSWithOldDL);
            StorageTemp.CGADeathWithSelfMinusDIResultWithOldDL = MeanOfResult(CAESDeathWithSmDWithOldDL);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL = MeanOfResult(CAESDeathWithHmDndSwSWithOldDL);
            StorageTemp.CGADeathWithSelfMinusSTDResultwithSelfWithOldDL = MeanOfResult(CAESDeathWithSmSwSWithOldDL);
            StorageTemp.CGADeathWithSelfMinusDIResultwithSelfWithOldDL = MeanOfResult(CAESDeathWithSmDwSWithOldDL);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultWithNewDL = MeanOfResult(CAESDeathWithHmDndSWithNewDL);
            StorageTemp.CGADeathWithSelfMinusSTDResultWithNewDL = MeanOfResult(CAESDeathWithSmSWithNewDL);
            StorageTemp.CGADeathWithSelfMinusDIResultWithNewDL = MeanOfResult(CAESDeathWithSmDWithNewDL);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL = MeanOfResult(CAESDeathWithHmDndSwSWithNewDL);
            StorageTemp.CGADeathWithSelfMinusSTDResultwithSelfWithNewDL = MeanOfResult(CAESDeathWithSmSwSWithNewDL);
            StorageTemp.CGADeathWithSelfMinusDIResultwithSelfWithNewDL = MeanOfResult(CAESDeathWithSmDwSWithNewDL);
            StorageTemp.MSCGDeath = MeanOfResult(MSCGDeath);
            StorageTemp.ESSPlusDeath = MeanOfResult(ESSPlusDeath);
            StorageTemp.LEGSDeath = MeanOfResult(LEGSDeath);
            StorageTemp.HEXIncDeath = MeanOfResult(HEXIncDeath);
            StorageTemp.ShortestPathDeath = MeanOfResult(ShortestPathDeath);

            OriginStorage.Add(StorageTemp);

            return StorageTemp;
        }


        public static void WriteCSV(List<ResultStorage> Result, string FileName)
        {
            string ResultFolder = System.Windows.Forms.Application.StartupPath + "\\Result\\";

            if (!System.IO.File.Exists(ResultFolder))
            {
                System.IO.Directory.CreateDirectory(ResultFolder);
            }


            using (StreamWriter sw = new StreamWriter(ResultFolder + FileName))   //小寫TXT     
            {
                string FirstLineStr = ",,,,,,,,,,,,woSMix,woSMix,woSMix,wSMix,wSMix,wSMix,woSOld,woSOld,woSOld,wSOld,wSOld,wSOld,woSNew,woSNew,woSNew,wSNew,wSNew,wSNew";
                string SecLineStr = "LateralNodeAmount,PortraitNodeAmount,TotalVictims,CorridorLimit,SP,CAESMixDL,CAESOldDL,CAESNewDL,MSCG,ESSPlus,LEGS,HEXInc,HighestDL-DI-D,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI,HighestDL-DI-STD,SelfDL-STD,SelfDL-DI";
                sw.WriteLine(FirstLineStr);
                sw.WriteLine(SecLineStr);


                for (int i = 0; i < Result.Count; i++)
                {
                    double CAESMixDL = Math.Round((double)(
                        Result[i].CGAWithHighestMinusDIandSTDResultwithSelf
                        + Result[i].CGAWithSelfMinusDIResultwithSelf
                        + Result[i].CGAWithSelfMinusSTDResultwithSelf
                        + Result[i].CGAWithHighestMinusDIandSTDResult
                        + Result[i].CGAWithSelfMinusDIResult
                        + Result[i].CGAWithSelfMinusSTDResult) / (double)6, 15);
                    double CAESOldDL = Math.Round((double)(
                        Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL
                        + Result[i].CGAWithSelfMinusDIResultwithSelfWithOldDL
                        + Result[i].CGAWithSelfMinusSTDResultwithSelfWithOldDL
                        + Result[i].CGAWithHighestMinusDIandSTDResultWithOldDL
                        + Result[i].CGAWithSelfMinusDIResultWithOldDL
                        + Result[i].CGAWithSelfMinusSTDResultWithOldDL) / (double)6, 15);
                    double CAESNewDL = Math.Round((double)(
                        Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL
                        + Result[i].CGAWithSelfMinusDIResultwithSelfWithNewDL
                        + Result[i].CGAWithSelfMinusSTDResultwithSelfWithNewDL
                        + Result[i].CGAWithHighestMinusDIandSTDResultWithNewDL
                        + Result[i].CGAWithSelfMinusDIResultWithNewDL
                        + Result[i].CGAWithSelfMinusSTDResultWithNewDL) / (double)6, 15);
                    string ResultLineStr = Result[i].width.ToString()
                                               + "," + Result[i].height.ToString()
                                               + "," + Result[i].totalpeople.ToString()
                                               + "," + Result[i].corridorlimit.ToString()
                                               + "," + Result[i].ShortestPath.ToString()
                                               + "," + CAESMixDL.ToString()
                                               + "," + CAESOldDL.ToString()
                                               + "," + CAESNewDL.ToString()
                                               + "," + Result[i].MSCG.ToString()
                                               + "," + Result[i].ESSPlus.ToString()
                                               + "," + Result[i].LEGS.ToString()
                                               + "," + Result[i].HEXInc.ToString()
                                               + "," + Result[i].CGAWithHighestMinusDIandSTDResultwithSelf.ToString()
                                               + "," + Result[i].CGAWithSelfMinusDIResultwithSelf.ToString()
                                               + "," + Result[i].CGAWithSelfMinusSTDResultwithSelf.ToString()
                                               + "," + Result[i].CGAWithHighestMinusDIandSTDResult.ToString()
                                               + "," + Result[i].CGAWithSelfMinusDIResult.ToString()
                                               + "," + Result[i].CGAWithSelfMinusSTDResult.ToString()
                                               + "," + Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL.ToString()
                                               + "," + Result[i].CGAWithSelfMinusDIResultwithSelfWithOldDL.ToString()
                                               + "," + Result[i].CGAWithSelfMinusSTDResultwithSelfWithOldDL.ToString()
                                               + "," + Result[i].CGAWithHighestMinusDIandSTDResultWithOldDL.ToString()
                                               + "," + Result[i].CGAWithSelfMinusDIResultWithOldDL.ToString()
                                               + "," + Result[i].CGAWithSelfMinusSTDResultWithOldDL.ToString()
                                               + "," + Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL.ToString()
                                               + "," + Result[i].CGAWithSelfMinusDIResultwithSelfWithNewDL.ToString()
                                               + "," + Result[i].CGAWithSelfMinusSTDResultwithSelfWithNewDL.ToString()
                                               + "," + Result[i].CGAWithHighestMinusDIandSTDResultWithNewDL.ToString()
                                               + "," + Result[i].CGAWithSelfMinusDIResultWithNewDL.ToString()
                                               + "," + Result[i].CGAWithSelfMinusSTDResultWithNewDL.ToString();


                    sw.WriteLine(ResultLineStr);

                    double CAESMixDLDeath = Math.Round((double)(
                        Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelf
                        + Result[i].CGADeathWithSelfMinusDIResultwithSelf
                        + Result[i].CGADeathWithSelfMinusSTDResultwithSelf
                        + Result[i].CGADeathWithHighestMinusDIandSTDResult
                        + Result[i].CGADeathWithSelfMinusDIResult
                        + Result[i].CGADeathWithSelfMinusSTDResult) / (double)6, 15);
                    double CAESOldDLDeath = Math.Round((double)(
                        Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL
                        + Result[i].CGADeathWithSelfMinusDIResultwithSelfWithOldDL
                        + Result[i].CGADeathWithSelfMinusSTDResultwithSelfWithOldDL
                        + Result[i].CGADeathWithHighestMinusDIandSTDResultWithOldDL
                        + Result[i].CGADeathWithSelfMinusDIResultWithOldDL
                        + Result[i].CGADeathWithSelfMinusSTDResultWithOldDL) / (double)6, 15);
                    double CAESNewDLDeath = Math.Round((double)(
                        Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL
                        + Result[i].CGADeathWithSelfMinusDIResultwithSelfWithNewDL
                        + Result[i].CGADeathWithSelfMinusSTDResultwithSelfWithNewDL
                        + Result[i].CGADeathWithHighestMinusDIandSTDResultWithNewDL
                        + Result[i].CGADeathWithSelfMinusDIResultWithNewDL
                        + Result[i].CGADeathWithSelfMinusSTDResultWithNewDL) / (double)6, 15);

                    string DeathResultLineStr = ""
                                                    + "," + ""
                                                    + "," + ""
                                                    + "," + "DeathAmount"
                                                    + "," + Result[i].ShortestPathDeath.ToString()
                                                    + "," + CAESMixDLDeath.ToString()
                                                    + "," + CAESOldDLDeath.ToString()
                                                    + "," + CAESNewDLDeath.ToString()
                                                    + "," + Result[i].MSCGDeath.ToString()
                                                    + "," + Result[i].ESSPlusDeath.ToString()
                                                    + "," + Result[i].LEGSDeath.ToString()
                                                    + "," + Result[i].HEXIncDeath.ToString()
                                                    + "," + Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelf.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusDIResultwithSelf.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusSTDResultwithSelf.ToString()
                                                    + "," + Result[i].CGADeathWithHighestMinusDIandSTDResult.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusDIResult.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusSTDResult.ToString()
                                                    + "," + Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusDIResultwithSelfWithOldDL.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusSTDResultwithSelfWithOldDL.ToString()
                                                    + "," + Result[i].CGADeathWithHighestMinusDIandSTDResultWithOldDL.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusDIResultWithOldDL.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusSTDResultWithOldDL.ToString()
                                                    + "," + Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusDIResultwithSelfWithNewDL.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusSTDResultwithSelfWithNewDL.ToString()
                                                    + "," + Result[i].CGADeathWithHighestMinusDIandSTDResultWithNewDL.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusDIResultWithNewDL.ToString()
                                                    + "," + Result[i].CGADeathWithSelfMinusSTDResultWithNewDL.ToString();

                    sw.WriteLine(DeathResultLineStr);
                }



            }
        }

        public static void WriteThings(Scenario Scenario)
        {
            string ResultFolder = System.Windows.Forms.Application.StartupPath + "\\Temp\\";

            if (!System.IO.File.Exists(ResultFolder))
            {
                System.IO.Directory.CreateDirectory(ResultFolder);
            }

            string FileName = null;
            if (Scenario.AlgorithmType.Equals("CAES"))
                FileName = Scenario.AlgorithmType + Scenario.ThresholdType + Scenario.DLCalculation;
            else
                FileName = Scenario.AlgorithmType;

            using (StreamWriter sw = new StreamWriter(ResultFolder + FileName + ".TXT"))   //小寫TXT     
            {
                sw.WriteLine(Scenario.TotalEvacuationRound.ToString());
                sw.WriteLine(Scenario.DeathVictimsAmount.ToString());
                sw.Close();
                sw.Dispose();
            }
        }

        public static List<int> ReadThings(Scenario Scenario)
        {
            string ResultFolder = System.Windows.Forms.Application.StartupPath + "\\Temp\\";

            if (!System.IO.File.Exists(ResultFolder))
            {
                System.IO.Directory.CreateDirectory(ResultFolder);
            }

            List<int> Result = new List<int>();
            try
            {
                string FileName = null;
                if (Scenario.AlgorithmType.Equals("CAES"))
                    FileName = Scenario.AlgorithmType + Scenario.ThresholdType + Scenario.DLCalculation;
                else
                    FileName = Scenario.AlgorithmType;



                using (StreamReader sr = new StreamReader(ResultFolder + FileName + ".TXT"))     //小寫TXT
                {
                    String line;
                    // Read and display lines from the file until the end of 
                    // the file is reached.
                    while ((line = sr.ReadLine()) != null)
                    {
                        Result.Add(Int32.Parse(line));
                    }
                    sr.Close();
                    sr.Dispose();
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
            return Result;
        }

        private static void BGSimulation_DoWork(object sender, DoWorkEventArgs e)
        {
            Scenario Scenario = (Scenario)e.Argument;
            Scenario = Simulation.NromalSimulation(Scenario);
            e.Result = Scenario;
        }
        private static void BGSimulation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Scenario Scenario = (Scenario)e.Result;
            WriteThings(Scenario);
        }

        public static string CheckWhereStayCounting(Scenario Scenario)
        {

            string FileName = null;
            if (Scenario.AlgorithmType.Equals("CAES"))
                FileName = Scenario.AlgorithmType + Scenario.ThresholdType + Scenario.DLCalculation;
            else
                FileName = Scenario.AlgorithmType;

            return FileName;
        }

        public static void AddNewResultToXlsx(Excel.Application _Excel, Excel.Worksheet sheet, Excel.Workbook book, Excel.Range range, List<ResultStorage> Result)
        {
            object[] TestArgs = new Object[1];
            string NewPosition = "";
            string[] FirstLineStr = { " "
                                      , " "
                                      , " "
                                      , " "
                                      , " "
                                      , " "
                                      , " "
                                      , " "
                                      , " "
                                      , " "
                                      , " "
                                      , " "
                                      , "woSMix"
                                      , "woSMix"
                                      , "woSMix"
                                      , "wSMix"
                                      , "wSMix"
                                      , "wSMix"
                                      , "woSOld"
                                      , "woSOld"
                                      , "woSOld"
                                      , "wSOld"
                                      , "wSOld"
                                      , "wSOld"
                                      , "woSNew"
                                      , "woSNew"
                                      , "woSNew"
                                      , "wSNew"
                                      , "wSNew"
                                      , "wSNew" };
            string[] SecLineStr = { "Lateral Node Amount"
                                    , "Portrait Node Amount"
                                    , "Total Victims"
                                    , "Corridor Limit"
                                    , "SP"
                                    , "CAES MixDL"
                                    , "CAES OldDL"
                                    , "CAES NewDL"
                                    , "MSCG"
                                    , "ESSPlus"
                                    , "LEGS"
                                    , "HEXInc"
                                    , "HighestDL-DI-STD"
                                    , "SelfDL-STD"
                                    , "SelfDL-DI"
                                    , "HighestDL-DI-STD"
                                    , "SelfDL-STD"
                                    , "SelfDL-DI"
                                    , "HighestDL-DI-STD"
                                    , "SelfDL-STD"
                                    , "SelfDL-DI"
                                    , "HighestDL-DI-STD"
                                    , "SelfDL-STD"
                                    , "SelfDL-DI"
                                    , "HighestDL-DI-STD"
                                    , "SelfDL-STD"
                                    , "SelfDL-DI"
                                    , "HighestDL-DI-STD"
                                    , "SelfDL-STD"
                                    , "SelfDL-DI" };

            for (int i = 0; i < FirstLineStr.GetLength(0); i++)
            {
                NewPosition = Simulation.NumberToText(i + 1) + "1";
                TestArgs[0] = FirstLineStr[i];
                range = sheet.get_Range(NewPosition);
                range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, range, TestArgs);
            }

            for (int i = 0; i < SecLineStr.GetLength(0); i++)
            {
                NewPosition = Simulation.NumberToText(i + 1) + "2";
                TestArgs[0] = SecLineStr[i];
                range = sheet.get_Range(NewPosition);
                range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, range, TestArgs);
            }



            for (int i = 0; i < Result.Count; i++)
            {
                double CAESMixDL = Math.Round((double)(
                    Result[i].CGAWithHighestMinusDIandSTDResultwithSelf
                    + Result[i].CGAWithSelfMinusDIResultwithSelf
                    + Result[i].CGAWithSelfMinusSTDResultwithSelf
                    + Result[i].CGAWithHighestMinusDIandSTDResult
                    + Result[i].CGAWithSelfMinusDIResult
                    + Result[i].CGAWithSelfMinusSTDResult) / (double)6, 15);
                double CAESOldDL = Math.Round((double)(
                    Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL
                    + Result[i].CGAWithSelfMinusDIResultwithSelfWithOldDL
                    + Result[i].CGAWithSelfMinusSTDResultwithSelfWithOldDL
                    + Result[i].CGAWithHighestMinusDIandSTDResultWithOldDL
                    + Result[i].CGAWithSelfMinusDIResultWithOldDL
                    + Result[i].CGAWithSelfMinusSTDResultWithOldDL) / (double)6, 15);
                double CAESNewDL = Math.Round((double)(
                    Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL
                    + Result[i].CGAWithSelfMinusDIResultwithSelfWithNewDL
                    + Result[i].CGAWithSelfMinusSTDResultwithSelfWithNewDL
                    + Result[i].CGAWithHighestMinusDIandSTDResultWithNewDL
                    + Result[i].CGAWithSelfMinusDIResultWithNewDL
                    + Result[i].CGAWithSelfMinusSTDResultWithNewDL) / (double)6, 15);

                string[] ResultLineStr = { Result[i].width.ToString()
                                           , Result[i].height.ToString()
                                           , Result[i].totalpeople.ToString()
                                           , Result[i].corridorlimit.ToString()
                                           , Result[i].ShortestPath.ToString()
                                           , CAESMixDL.ToString()
                                           , CAESOldDL.ToString()
                                           , CAESNewDL.ToString()
                                           , Result[i].MSCG.ToString()
                                           , Result[i].ESSPlus.ToString()
                                           , Result[i].LEGS.ToString()
                                           , Result[i].HEXInc.ToString()
                                           , Result[i].CGAWithHighestMinusDIandSTDResultwithSelf.ToString()
                                           , Result[i].CGAWithSelfMinusDIResultwithSelf.ToString()
                                           , Result[i].CGAWithSelfMinusSTDResultwithSelf.ToString()
                                           , Result[i].CGAWithHighestMinusDIandSTDResult.ToString()
                                           , Result[i].CGAWithSelfMinusDIResult.ToString()
                                           , Result[i].CGAWithSelfMinusSTDResult.ToString()
                                           , Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL.ToString()
                                           , Result[i].CGAWithSelfMinusDIResultwithSelfWithOldDL.ToString()
                                           , Result[i].CGAWithSelfMinusSTDResultwithSelfWithOldDL.ToString()
                                           , Result[i].CGAWithHighestMinusDIandSTDResultWithOldDL.ToString()
                                           , Result[i].CGAWithSelfMinusDIResultWithOldDL.ToString()
                                           , Result[i].CGAWithSelfMinusSTDResultWithOldDL.ToString()
                                           , Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL.ToString()
                                           , Result[i].CGAWithSelfMinusDIResultwithSelfWithNewDL.ToString()
                                           , Result[i].CGAWithSelfMinusSTDResultwithSelfWithNewDL.ToString()
                                           , Result[i].CGAWithHighestMinusDIandSTDResultWithNewDL.ToString()
                                           , Result[i].CGAWithSelfMinusDIResultWithNewDL.ToString()
                                           , Result[i].CGAWithSelfMinusSTDResultWithNewDL.ToString() };

                for (int j = 0; j < ResultLineStr.GetLength(0); j++)
                {
                    NewPosition = Simulation.NumberToText(j + 1) + (2 * i + 3).ToString();
                    TestArgs[0] = ResultLineStr[j];
                    range = sheet.get_Range(NewPosition);
                    range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, range, TestArgs);

                    if (j > 3 && j != 4)
                    {
                        if (Convert.ToDouble(ResultLineStr[j]) < Convert.ToDouble(ResultLineStr[4]))
                            range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);
                        if (Convert.ToDouble(ResultLineStr[j]) > Convert.ToDouble(ResultLineStr[4]))
                            range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightPink);
                        if (Convert.ToDouble(ResultLineStr[j]) == Convert.ToDouble(ResultLineStr[4]))
                            range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);
                    }
                }

                double CAESMixDLDeath = Math.Round((double)(
                    Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelf
                    + Result[i].CGADeathWithSelfMinusDIResultwithSelf
                    + Result[i].CGADeathWithSelfMinusSTDResultwithSelf
                    + Result[i].CGADeathWithHighestMinusDIandSTDResult
                    + Result[i].CGADeathWithSelfMinusDIResult
                    + Result[i].CGADeathWithSelfMinusSTDResult) / (double)6, 15);
                double CAESOldDLDeath = Math.Round((double)(
                    Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL
                    + Result[i].CGADeathWithSelfMinusDIResultwithSelfWithOldDL
                    + Result[i].CGADeathWithSelfMinusSTDResultwithSelfWithOldDL
                    + Result[i].CGADeathWithHighestMinusDIandSTDResultWithOldDL
                    + Result[i].CGADeathWithSelfMinusDIResultWithOldDL
                    + Result[i].CGADeathWithSelfMinusSTDResultWithOldDL) / (double)6, 15);
                double CAESNewDLDeath = Math.Round((double)(
                    Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL
                    + Result[i].CGADeathWithSelfMinusDIResultwithSelfWithNewDL
                    + Result[i].CGADeathWithSelfMinusSTDResultwithSelfWithNewDL
                    + Result[i].CGADeathWithHighestMinusDIandSTDResultWithNewDL
                    + Result[i].CGADeathWithSelfMinusDIResultWithNewDL
                    + Result[i].CGADeathWithSelfMinusSTDResultWithNewDL) / (double)6, 15);

                string[] DeathResultLineStr = { ""
                                                , ""
                                                , ""
                                                , "DeathAmount"
                                                , Result[i].ShortestPathDeath.ToString()
                                                , CAESMixDLDeath.ToString()
                                                , CAESOldDLDeath.ToString()
                                                , CAESNewDLDeath.ToString()
                                                , Result[i].MSCGDeath.ToString()
                                                , Result[i].ESSPlusDeath.ToString()
                                                , Result[i].LEGSDeath.ToString()
                                                , Result[i].HEXIncDeath.ToString()
                                                , Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelf.ToString()
                                                , Result[i].CGADeathWithSelfMinusDIResultwithSelf.ToString()
                                                , Result[i].CGADeathWithSelfMinusSTDResultwithSelf.ToString()
                                                , Result[i].CGADeathWithHighestMinusDIandSTDResult.ToString()
                                                , Result[i].CGADeathWithSelfMinusDIResult.ToString()
                                                , Result[i].CGADeathWithSelfMinusSTDResult.ToString()
                                                , Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL.ToString()
                                                , Result[i].CGADeathWithSelfMinusDIResultwithSelfWithOldDL.ToString()
                                                , Result[i].CGADeathWithSelfMinusSTDResultwithSelfWithOldDL.ToString()
                                                , Result[i].CGADeathWithHighestMinusDIandSTDResultWithOldDL.ToString()
                                                , Result[i].CGADeathWithSelfMinusDIResultWithOldDL.ToString()
                                                , Result[i].CGADeathWithSelfMinusSTDResultWithOldDL.ToString()
                                                , Result[i].CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL.ToString()
                                                , Result[i].CGADeathWithSelfMinusDIResultwithSelfWithNewDL.ToString()
                                                , Result[i].CGADeathWithSelfMinusSTDResultwithSelfWithNewDL.ToString()
                                                , Result[i].CGADeathWithHighestMinusDIandSTDResultWithNewDL.ToString()
                                                , Result[i].CGADeathWithSelfMinusDIResultWithNewDL.ToString()
                                                , Result[i].CGADeathWithSelfMinusSTDResultWithNewDL.ToString() };

                for (int j = 0; j < DeathResultLineStr.GetLength(0); j++)
                {
                    NewPosition = Simulation.NumberToText(j + 1) + (2 * i + 4).ToString();
                    TestArgs[0] = DeathResultLineStr[j];
                    range = sheet.get_Range(NewPosition);
                    range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, range, TestArgs);


                    if (j > 3)
                        if (Convert.ToDouble(DeathResultLineStr[j]) > 0)
                            range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightPink);
                }
            }
        }

        public static void WriteALotResultToCSV(List<ResultStorage> Result)
        {
            String CSVFileName = "Result " + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss tt") + ".csv";

            FileStream fs = new FileStream(CSVFileName, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

            sw.WriteLine(" , , , ,withoutSelf,withoutSelf,withoutSelf,withSelf,withSelf,withSelf");
            sw.WriteLine("橫向節點數,縱向節點數,總人數,走廊限制,最高權重-DI-STD回合數,自己權重-STD回合數,自己權重-DI回合數,最高權重-DI-STD回合數,自己權重-STD回合數,自己權重-DI回合數,SP");
            for (int i = 0; i < Result.Count; i++)
            {
                sw.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", Result[i].width, Result[i].height, Result[i].totalpeople, Result[i].corridorlimit, Result[i].CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL, Result[i].CGAWithSelfMinusDIResultwithSelfWithNewDL, Result[i].CGAWithSelfMinusSTDResultwithSelfWithNewDL, Result[i].CGAWithHighestMinusDIandSTDResultWithNewDL, Result[i].CGAWithSelfMinusDIResultWithNewDL, Result[i].CGAWithSelfMinusSTDResultWithNewDL, Result[i].ShortestPath);
            }

            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public static double MeanOfResult(List<int> CalValue)
        {
            int total = 0;
            for (int i = 0; i < CalValue.Count; i++)
                total += CalValue[i];
            return Math.Round((double)total / (double)CalValue.Count, 15);
        }

        public static List<ResultStorage> AddNewResult(
            List<ResultStorage> OriginStorage
            , int CorridorLimit
            , int VictimsAmount
            , int SensorHeight
            , int SensorWidth
            , List<int> CAESWithHmDndS
            , List<int> CAESWithSmS
            , List<int> CAESWithSmD
            , List<int> CAESWithHmDndSwS
            , List<int> CAESWithSmSwS
            , List<int> CAESWithSmDwS
            , List<int> CAESWithHmDndSWithOldDL
            , List<int> CAESWithSmSWithOldDL
            , List<int> CAESWithSmDWithOldDL
            , List<int> CAESWithHmDndSwSWithOldDL
            , List<int> CAESWithSmSwSWithOldDL
            , List<int> CAESWithSmDwSWithOldDL
            , List<int> CAESWithHmDndSWithNewDL
            , List<int> CAESWithSmSWithNewDL
            , List<int> CAESWithSmDWithNewDL
            , List<int> CAESWithHmDndSwSWithNewDL
            , List<int> CAESWithSmSwSWithNewDL
            , List<int> CAESWithSmDwSWithNewDL
            , List<int> MSCG
            , List<int> ESSPlus
            , List<int> LEGS
            , List<int> HEXInc
            , List<int> ShortestPath
            , List<int> CAESDeathWithHmDndS
            , List<int> CAESDeathWithSmS
            , List<int> CAESDeathWithSmD
            , List<int> CAESDeathWithHmDndSwS
            , List<int> CAESDeathWithSmSwS
            , List<int> CAESDeathWithSmDwS
            , List<int> CAESDeathWithHmDndSWithOldDL
            , List<int> CAESDeathWithSmSWithOldDL
            , List<int> CAESDeathWithSmDWithOldDL
            , List<int> CAESDeathWithHmDndSwSWithOldDL
            , List<int> CAESDeathWithSmSwSWithOldDL
            , List<int> CAESDeathWithSmDwSWithOldDL
            , List<int> CAESDeathWithHmDndSWithNewDL
            , List<int> CAESDeathWithSmSWithNewDL
            , List<int> CAESDeathWithSmDWithNewDL
            , List<int> CAESDeathWithHmDndSwSWithNewDL
            , List<int> CAESDeathWithSmSwSWithNewDL
            , List<int> CAESDeathWithSmDwSWithNewDL
            , List<int> MSCGDeath
            , List<int> ESSPlusDeath
            , List<int> LEGSDeath
            , List<int> HEXIncDeath
            , List<int> ShortestPathDeath)
        {
            ResultStorage StorageTemp = new ResultStorage();
            StorageTemp.corridorlimit = CorridorLimit;
            StorageTemp.totalpeople = VictimsAmount;
            StorageTemp.height = SensorHeight;
            StorageTemp.width = SensorWidth;
            StorageTemp.CGAWithHighestMinusDIandSTDResult = MeanOfResult(CAESWithHmDndS);
            StorageTemp.CGAWithSelfMinusSTDResult = MeanOfResult(CAESWithSmS);
            StorageTemp.CGAWithSelfMinusDIResult = MeanOfResult(CAESWithSmD);
            StorageTemp.CGAWithHighestMinusDIandSTDResultwithSelf = MeanOfResult(CAESWithHmDndSwS);
            StorageTemp.CGAWithSelfMinusSTDResultwithSelf = MeanOfResult(CAESWithSmSwS);
            StorageTemp.CGAWithSelfMinusDIResultwithSelf = MeanOfResult(CAESWithSmDwS);
            StorageTemp.CGAWithHighestMinusDIandSTDResultWithOldDL = MeanOfResult(CAESWithHmDndSWithOldDL);
            StorageTemp.CGAWithSelfMinusSTDResultWithOldDL = MeanOfResult(CAESWithSmSWithOldDL);
            StorageTemp.CGAWithSelfMinusDIResultWithOldDL = MeanOfResult(CAESWithSmDWithOldDL);
            StorageTemp.CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL = MeanOfResult(CAESWithHmDndSwSWithOldDL);
            StorageTemp.CGAWithSelfMinusSTDResultwithSelfWithOldDL = MeanOfResult(CAESWithSmSwSWithOldDL);
            StorageTemp.CGAWithSelfMinusDIResultwithSelfWithOldDL = MeanOfResult(CAESWithSmDwSWithOldDL);
            StorageTemp.CGAWithHighestMinusDIandSTDResultWithNewDL = MeanOfResult(CAESWithHmDndSWithNewDL);
            StorageTemp.CGAWithSelfMinusSTDResultWithNewDL = MeanOfResult(CAESWithSmSWithNewDL);
            StorageTemp.CGAWithSelfMinusDIResultWithNewDL = MeanOfResult(CAESWithSmDWithNewDL);
            StorageTemp.CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL = MeanOfResult(CAESWithHmDndSwSWithNewDL);
            StorageTemp.CGAWithSelfMinusSTDResultwithSelfWithNewDL = MeanOfResult(CAESWithSmSwSWithNewDL);
            StorageTemp.CGAWithSelfMinusDIResultwithSelfWithNewDL = MeanOfResult(CAESWithSmDwSWithNewDL);
            StorageTemp.MSCG = MeanOfResult(MSCG);
            StorageTemp.ESSPlus = MeanOfResult(ESSPlus);
            StorageTemp.LEGS = MeanOfResult(LEGS);
            StorageTemp.HEXInc = MeanOfResult(HEXInc);
            StorageTemp.ShortestPath = MeanOfResult(ShortestPath);

            StorageTemp.CGADeathWithHighestMinusDIandSTDResult = MeanOfResult(CAESDeathWithHmDndS);
            StorageTemp.CGADeathWithSelfMinusSTDResult = MeanOfResult(CAESDeathWithSmS);
            StorageTemp.CGADeathWithSelfMinusDIResult = MeanOfResult(CAESDeathWithSmD);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultwithSelf = MeanOfResult(CAESDeathWithHmDndSwS);
            StorageTemp.CGADeathWithSelfMinusSTDResultwithSelf = MeanOfResult(CAESDeathWithSmSwS);
            StorageTemp.CGADeathWithSelfMinusDIResultwithSelf = MeanOfResult(CAESDeathWithSmDwS);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultWithOldDL = MeanOfResult(CAESDeathWithHmDndSWithOldDL);
            StorageTemp.CGADeathWithSelfMinusSTDResultWithOldDL = MeanOfResult(CAESDeathWithSmSWithOldDL);
            StorageTemp.CGADeathWithSelfMinusDIResultWithOldDL = MeanOfResult(CAESDeathWithSmDWithOldDL);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL = MeanOfResult(CAESDeathWithHmDndSwSWithOldDL);
            StorageTemp.CGADeathWithSelfMinusSTDResultwithSelfWithOldDL = MeanOfResult(CAESDeathWithSmSwSWithOldDL);
            StorageTemp.CGADeathWithSelfMinusDIResultwithSelfWithOldDL = MeanOfResult(CAESDeathWithSmDwSWithOldDL);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultWithNewDL = MeanOfResult(CAESDeathWithHmDndSWithNewDL);
            StorageTemp.CGADeathWithSelfMinusSTDResultWithNewDL = MeanOfResult(CAESDeathWithSmSWithNewDL);
            StorageTemp.CGADeathWithSelfMinusDIResultWithNewDL = MeanOfResult(CAESDeathWithSmDWithNewDL);
            StorageTemp.CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL = MeanOfResult(CAESDeathWithHmDndSwSWithNewDL);
            StorageTemp.CGADeathWithSelfMinusSTDResultwithSelfWithNewDL = MeanOfResult(CAESDeathWithSmSwSWithNewDL);
            StorageTemp.CGADeathWithSelfMinusDIResultwithSelfWithNewDL = MeanOfResult(CAESDeathWithSmDwSWithNewDL);
            StorageTemp.MSCGDeath = MeanOfResult(MSCGDeath);
            StorageTemp.ESSPlusDeath = MeanOfResult(ESSPlusDeath);
            StorageTemp.LEGSDeath = MeanOfResult(LEGSDeath);
            StorageTemp.HEXIncDeath = MeanOfResult(HEXIncDeath);
            StorageTemp.ShortestPathDeath = MeanOfResult(ShortestPathDeath);

            OriginStorage.Add(StorageTemp);

            return OriginStorage;
        }

        public static void CloseComExcel()
        {
            foreach (System.Diagnostics.Process proc in System.Diagnostics.Process.GetProcesses())
            {
                //Console.WriteLine(proc.ProcessName.ToString());

                if (proc.ProcessName == "excel")
                {
                    proc.Kill();
                }
            }
        }


        /*
        public static int[,] SetFireSensorRandomly(int FireSensorAmount, int SensorWidth, int SensorHeight, int[,] ExitSensor)
        {
            for (int i = 0; i < FireSensorAmount; i++)
            {
                
            }
            int[,] FireSensor = new int[,] { { (SensorWidth - 1) / 2, (SensorHeight - 1) / 2, FireStartingNumber } };

            return new int[,] { };
        }

        public static int[,] SetExitSensorRandomly(int ExitSensorAmount, int SensorWidth, int SensorHeight)
        {
            int[,] ExitSensor = new int[,] { { 0, 0, -1, 0, 0 }, { SensorWidth, (SensorHeight - 1) / 2, -1, 0, 0 }, { (SensorWidth - 1) / 2, SensorHeight, -1, 0, 0 } };
            return new int[,] { };
        }*/
    }
}