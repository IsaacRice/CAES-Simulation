using System;
using System.Collections.Generic;
using System.IO; //Scenario Represent Scenario In Another Class
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Drawing;
using Excel = Microsoft.Office.Interop.Excel;
using NIS = CAES.ScenarioNode.NodeInScenario; //NIS Represent Node In Scenario
using Scenario = CAES.ScenarioNode.Scenario;

namespace CAES
{
    class Simulation
    {
        public static Scenario FindHopDistanceToExit(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    for (int k = 0; k < Scenario.ExitSensor.Count(); k++)
                        if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Exit == false)
                        {
                            Scenario.Node[i, j].DistanceToExits[k].X = Scenario.ExitSensor[k].X;
                            Scenario.Node[i, j].DistanceToExits[k].Y = Scenario.ExitSensor[k].Y;
                            Scenario.Node[i, j].DistanceToExits[k].Hop = -1;
                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance = -1;
                            Scenario.Node[i, j].DistanceToExits[k].EuclideanDistance = -1;
                        }

            return Scenario;
        }

        public static Scenario NromalSimulation(Scenario Scenario)
        {
            Boolean IfAllEscaped = false;

            Scenario.TotalEvacuationRound = 0;

            //Console.WriteLine("開始演算法" + Scenario.AlgorithmType + "、門檻" + Scenario.ThresholdType + "之逃生…");

            //前處理
            switch (Scenario.AlgorithmType)
            {
                case "CAES":
                    Scenario = CAES.ExecuteCAES(Scenario);
                    break;
                case "MSCG":
                    Scenario = MSCG.MSCGCalculation(Scenario);
                    break;
                case "ESSPlus":
                    Scenario = ESSPlus.ESSPlusCalculation(Scenario);
                    break;
                case "LEGS":
                    Scenario = LEGS.LEGSCalculation(Scenario);
                    break;
                case "HEXInc":
                    Scenario = HEXInc.HEXIncCalculation(Scenario);
                    break;
                case "ShortestPath":
                    Scenario = ShortestPath.CalculateShortestPath(Scenario);
                    break;
                default:
                    break;
            }

            string thresholdset = "";
            string DLset = "";
            if (Scenario.ThresholdType == "HmSTDnDIwS")
                thresholdset = "T1";
            else if (Scenario.ThresholdType == "SmSTDwS")
                thresholdset = "T2";
            else if (Scenario.ThresholdType == "SmDIwS")
                thresholdset = "T3";
            else if (Scenario.ThresholdType == "HmSTDnDIwoS")
                thresholdset = "T4";
            else if (Scenario.ThresholdType == "SmSTDwoS")
                thresholdset = "T5";
            else if (Scenario.ThresholdType == "SmDIwoS")
                thresholdset = "T6";

            if (Scenario.DLCalculation == "OldDL")
                DLset = "OD";
            else if (Scenario.DLCalculation == "NewDL")
                DLset = "ND";
            else if (Scenario.DLCalculation == "MixDL")
                DLset = "MD";
            /*
            if (Scenario.ExcelProcess.IfRecordXlsx == true)
            {
                Scenario.ExcelProcess.Sheet = (Excel.Worksheet)Scenario.ExcelProcess.ExcelApp.Application.Worksheets.Add();
                Scenario.ExcelProcess.Sheet = (Excel.Worksheet)Scenario.ExcelProcess.Book.Worksheets[1];

                if (Scenario.AlgorithmType == "CAES")
                    Scenario.ExcelProcess.Sheet.Name =
                        "R" + Scenario.LoopRound.ToString()
                        + "W" + Scenario.SensorWidth.ToString()
                        + "H" + Scenario.SensorHeight.ToString()
                        + "C" + Scenario.CorridorCapacity.ToString()
                        + "P" + Scenario.WholeVictims.GetLength(0).ToString()
                        + Scenario.AlgorithmType
                        + thresholdset
                        + DLset;
                else
                    Scenario.ExcelProcess.Sheet.Name =
                        "R " + Scenario.LoopRound.ToString()
                        + "W" + Scenario.SensorWidth.ToString()
                        + "H" + Scenario.SensorHeight.ToString()
                        + "C" + Scenario.CorridorCapacity.ToString()
                        + "P" + Scenario.WholeVictims.GetLength(0).ToString()
                        + Scenario.AlgorithmType;

                Scenario.ExcelProcess.Range = null;
            }
            //WriteScenarioToCSV(Scenario);
            if (Scenario.ExcelProcess.IfRecordXlsx == true)
                WriteToExcel(Scenario);
            */

            bool PrintScenario = false;
            bool PrintWeight = false;
            bool PrintPeople = false;

            bool Check200Round = true;

            string Algorithm = "";
            string DLType = "";
            string ThresholdType = "";

            while (!IfAllEscaped)
            {
                IfAllEscaped = true;

                for (int i = 0; i < Scenario.WholeVictims.GetLength(0); i++)
                {
                    if (Scenario.WholeVictims[i].Activated == true)
                        IfAllEscaped = false;
                }

                if (IfAllEscaped == true)
                {
                    Scenario.EvacuationFinished = true;

                    int DeathCountTemp = 0;

                    for (int i = 0; i < Scenario.WholeVictims.GetLength(0); i++)
                        if (Scenario.WholeVictims[i].Live == false)
                            DeathCountTemp += 1;

                    Scenario.DeathVictimsAmount = DeathCountTemp;

                    return Scenario;
                }

                Scenario.TotalEvacuationRound++;//移動回合數+1
                //Console.WriteLine("開始演算法" + Scenario.AlgorithmType + "、門檻" + Scenario.ThresholdType + " 第" + Scenario.TotalEvacuationRound.ToString() + "回合之逃生…");
                
                if (Scenario.FireInfo.WaitingFireSensor.Count() > 0)
                    for (int i = 1; i < Scenario.FireInfo.FireSensor.Count() + Scenario.FireInfo.WaitingFireSensor.Count(); i++)
                        if (Scenario.TotalEvacuationRound == Scenario.FireInfo.FireSpreadTime * i)
                        {
                            Scenario = SettingParameter.SetNewRNGFire(Scenario, 1);
                            
                            if (Scenario.AlgorithmType == "CAES")
                                Scenario = CAES.ReCalculateCAES(Scenario);
                 
                        }
                /*
                //moving
                if (PrintScenario)
                    ConsoleWriteScenarioType(Scenario);

                if (PrintWeight)
                    ConsoleWriteScenarioWeight(Scenario);

                if (PrintPeople)
                    ConsoleWriteScenarioPeople(Scenario);
                if (Check200Round)
                    if (Scenario.TotalEvacuationRound > 200)
                    {
                        Console.WriteLine(Scenario.AlgorithmType + "," + Scenario.ThresholdType + "," + "Kinda weird, check the code.");
                        Algorithm = Scenario.AlgorithmType;
                        if (Algorithm == "CAES")
                        {
                            DLType = Scenario.DLCalculation;
                            ThresholdType = Scenario.ThresholdType;
                        }
                        Check200Round = false;
                    }
                if (Algorithm == Scenario.AlgorithmType)
                    if (Algorithm == "CAES")
                    {
                        if (DLType == Scenario.DLCalculation && ThresholdType == "SmDIwS")
                        {
                            Console.WriteLine(Algorithm + " " + DLType + " Rount " + Scenario.TotalEvacuationRound.ToString());
                            ConsoleWriteCAESScenWeightNdTypeNdPeople(Scenario);
                            Console.ReadLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine(Algorithm + " Rount " + Scenario.TotalEvacuationRound.ToString());
                        ConsoleWriteOtherScenDirectionNdTypeNdPeople(Scenario);
                        Console.ReadLine();
                    }
                */
                switch (Scenario.AlgorithmType)
                {
                    case "CAES":
                        Scenario = CAES.CAESSimulation(Scenario);
                        break;
                    case "MSCG":
                        Scenario = MSCG.MSCGSimulation(Scenario);
                        break;
                    case "ESSPlus":
                        Scenario = ESSPlus.ESSPlusSimulation(Scenario);
                        break;
                    case "LEGS":
                        Scenario = ShortestPath.SPSimulation(Scenario);
                        break;
                    case "HEXInc":
                        Scenario = ShortestPath.SPSimulation(Scenario);
                        break;
                    case "ShortestPath":
                        Scenario = ShortestPath.SPSimulation(Scenario);
                        break;
                    default:
                        break;
                }
                /*
                if (Scenario.ExcelProcess.IfRecordXlsx == true)
                    WriteToExcel(Scenario);
                */
                /// <code>
                ///  Random Fire Start
                /// </code>
                // if()
                /// <code>
                ///  Random Fire End
                /// </code>
            }

            return Scenario;
        }

        public static bool FireSensorCheck(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].FireSensor == true)
                        if (Scenario.Node[i, j].VictimsAtSensor.Count > 0)
                            return true;
            return false;
        }

        public static void WriteScenarioToCSV(Scenario Scenario)
        {
            FileStream fs = new FileStream(Scenario.AlgorithmType + " " + Scenario.ThresholdType + " Scenario.csv", FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

            sw.WriteLine("People");
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (j != 0)
                        sw.Write(",");
                    if (Scenario.Node[i, j].Exit == true)
                        sw.Write("Exit");
                    else if (Scenario.Node[i, j].Corridor == true)
                        sw.Write("1");
                    else if (Scenario.Node[i, j].Sensor == true)
                        sw.Write(Scenario.Node[i, j].People);
                    else if (Scenario.Node[i, j].Obstacle == true)
                        sw.Write("Obstacle");
                }
                sw.WriteLine();
            }

            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("People Weight");
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (j != 0)
                        sw.Write(",");
                    if (Scenario.Node[i, j].Exit == true)
                        sw.Write("Exit");
                    else if (Scenario.Node[i, j].Corridor == true)
                        sw.Write("1");
                    else if (Scenario.Node[i, j].Sensor == true)
                        sw.Write(Scenario.Node[i, j].PeopleWeight);
                    else if (Scenario.Node[i, j].Obstacle == true)
                        sw.Write("Obstacle");
                }
                sw.WriteLine();
            }

            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("Corridor Weight");
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (j != 0)
                        sw.Write(",");
                    if (Scenario.Node[i, j].Exit == true)
                        sw.Write("Exit");
                    else if (Scenario.Node[i, j].Corridor == true)
                        sw.Write(Scenario.Node[i, j].BasePercentageDistance * (Scenario.Node[i, j].Hop + 1));
                    else if (Scenario.Node[i, j].Sensor == true)
                        sw.Write(Scenario.Node[i, j].BaseWeight);
                    else if (Scenario.Node[i, j].Obstacle == true)
                        sw.Write("Obstacle");
                }
                sw.WriteLine();
            }

            sw.WriteLine();
            sw.WriteLine();

            sw.WriteLine("Initial Final Weight");
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (j != 0)
                        sw.Write(",");
                    if (Scenario.Node[i, j].Exit == true)
                        sw.Write("Exit");
                    else if (Scenario.Node[i, j].Corridor == true)
                        sw.Write(Scenario.Node[i, j].BasePercentageDistance * (Scenario.Node[i, j].Hop + 1));
                    else if (Scenario.Node[i, j].Sensor == true)
                        sw.Write(Scenario.Node[i, j].Weight);
                    else if (Scenario.Node[i, j].Obstacle == true)
                        sw.Write("Obstacle");
                }
                sw.WriteLine();
            }

            sw.WriteLine();
            sw.WriteLine();

            sw.Flush();
            sw.Close();
            fs.Close();
            //Console.WriteLine("\n已將地圖人數，初始DL存檔至Test Scenario.csv");
        }

        public static void WriteToExcel(Scenario Scenario)
        {
            Object[] TestArgs = new Object[1];
            String NewPosition = "";

            TestArgs[0] = "第" + Scenario.TotalEvacuationRound.ToString() + "回合人數分佈";
            NewPosition = "A" + ((Scenario.TotalEvacuationRound) * 2 * (Scenario.Node.GetLength(0) + 1) + 1).ToString();
            Scenario.ExcelProcess.Range = Scenario.ExcelProcess.Sheet.get_Range(NewPosition);
            Scenario.ExcelProcess.Range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, Scenario.ExcelProcess.Range, TestArgs);

            TestArgs[0] = "第" + Scenario.TotalEvacuationRound.ToString() + "回合權重分佈";
            NewPosition = "A" + (((Scenario.TotalEvacuationRound) * 2 * (Scenario.Node.GetLength(0) + 1)) + Scenario.Node.GetLength(0) + 2).ToString();
            Scenario.ExcelProcess.Range = Scenario.ExcelProcess.Sheet.get_Range(NewPosition);
            Scenario.ExcelProcess.Range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, Scenario.ExcelProcess.Range, TestArgs);

            for (int i = 1; i < Scenario.Node.GetLength(0) + 1; i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    NewPosition = NumberToText(j + 1) + ((Scenario.TotalEvacuationRound) * (Scenario.Node.GetLength(0) + 1) * 2 + i + 1).ToString();
                    Scenario.ExcelProcess.Range = Scenario.ExcelProcess.Sheet.get_Range(NewPosition);
                    if (Scenario.Node[i - 1, j].Exit == true)
                    {
                        TestArgs[0] = "Exit";
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);
                    }
                    else if (Scenario.Node[i - 1, j].Obstacle == true)
                    {
                        TestArgs[0] = "Obstacle";
                        Scenario.ExcelProcess.Range.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    }
                    else if (Scenario.Node[i - 1, j].Sensor == true)
                    {
                        TestArgs[0] = Scenario.Node[i - 1, j].VictimsAtSensor.Count();
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);
                    }
                    else if (Scenario.Node[i - 1, j].Corridor == true)
                    {
                        TestArgs[0] = Scenario.Node[i - 1, j].VictimsAtSensor.Count();
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);
                    }
                    if (Scenario.Node[i - 1, j].FireSensor == true)
                    {
                        TestArgs[0] = Scenario.Node[i - 1, j].VictimsAtSensor.Count();
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.HotPink);
                    }
                    else if (Scenario.Node[i - 1, j].Activited == false)
                    {
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    }
                    Scenario.ExcelProcess.Range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, Scenario.ExcelProcess.Range, TestArgs);

                    NewPosition = NumberToText(j + 1) + ((Scenario.TotalEvacuationRound) * (Scenario.Node.GetLength(0) + 1) * 2 + Scenario.Node.GetLength(0) + 1 + i + 1).ToString();
                    Scenario.ExcelProcess.Range = Scenario.ExcelProcess.Sheet.get_Range(NewPosition);
                    if (Scenario.Node[i - 1, j].Exit == true)
                    {
                        TestArgs[0] = Scenario.Node[i - 1, j].Weight;
                        TestArgs[0] = "Exit";
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGreen);
                    }
                    //TestArgs[0] = "Exit";
                    else if (Scenario.Node[i - 1, j].Obstacle == true)
                    {
                        TestArgs[0] = "Obstacle";
                        Scenario.ExcelProcess.Range.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    }
                    else if (Scenario.Node[i - 1, j].Corridor == true)
                    {
                        TestArgs[0] = Scenario.Node[i - 1, j].Weight;
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);
                    }
                    else if (Scenario.Node[i - 1, j].Sensor == true)
                    {
                        TestArgs[0] = Scenario.Node[i - 1, j].Weight;
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightYellow);
                    }
                    if (Scenario.Node[i - 1, j].FireSensor == true)
                    {
                        TestArgs[0] = Scenario.Node[i - 1, j].Weight;
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.HotPink);
                    }
                    else if (Scenario.Node[i - 1, j].Activited == false)
                    {
                        Scenario.ExcelProcess.Range.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black);
                    }

                    Scenario.ExcelProcess.Range.GetType().InvokeMember("Value", BindingFlags.SetProperty, null, Scenario.ExcelProcess.Range, TestArgs);
                }
            }
        }

        public static string[] LetterMeans = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q" };

        public static string NumberToText(int number)
        {
            int ColumnBase = 26,
                DigitMax = 7;
            String Digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            if (number <= 0)
                throw new IndexOutOfRangeException("index must be a positive number");

            if (number <= ColumnBase)
                return Digits[number - 1].ToString();

            var sb = new StringBuilder().Append(' ', DigitMax);
            var current = number;
            var offset = DigitMax;
            while (current > 0)
            {
                sb[--offset] = Digits[--current % ColumnBase];
                current /= ColumnBase;
            }

            return sb.ToString(offset, DigitMax - offset);
        }


        public static void ConsoleWriteScenarioWeight(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    Console.Write(Scenario.Node[i, j].Weight.ToString("f4") + " \t");
                }

                Console.WriteLine();
            }
        }

        public static void ConsoleWriteScenarioWeightNdType(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write("Ex" + Scenario.Node[i, j].Weight.ToString("f4") + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write("Fi:" + Scenario.Node[i, j].Weight.ToString("f4") + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write("Se:" + Scenario.Node[i, j].Weight.ToString("f4") + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write("Cr:" + Scenario.Node[i, j].Weight.ToString("f4") + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t\t");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void ConsoleWriteScenarioPeopleNdType(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write("Ex" + Scenario.Node[i, j].VictimsAtSensor.Count.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write("Fi:" + Scenario.Node[i, j].VictimsAtSensor.Count.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write("Se:" + Scenario.Node[i, j].VictimsAtSensor.Count.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write("Cr:" + Scenario.Node[i, j].VictimsAtSensor.Count.ToString() + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void ConsoleWriteScenarioPeople(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    Console.Write(Scenario.Node[i, j].VictimsAtSensor.Count.ToString() + "\t");
                }

                Console.WriteLine();
            }
        }

        public static void ConsoleWriteScenarioType(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write("Exit" + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write("Fire" + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write("Sens" + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write("Corr" + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }

                Console.WriteLine();
            }
        }

        public static void ConsoleWriteCAESScenWeightNdTypeNdPeople(Scenario Scenario)
        {
            for (int j = 0; j < Scenario.Node.GetLength(1); j++)
            {
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write("Ex:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write("Fi:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write("Se:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write("Cr:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }
                Console.WriteLine();

                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write(Scenario.Node[i, j].Weight.ToString("f4") + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write(Scenario.Node[i, j].Weight.ToString("f4") + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write(Scenario.Node[i, j].Weight.ToString("f4") + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write(Scenario.Node[i, j].Weight.ToString("f4") + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }


        public static void ConsoleWriteCAESScenHopNdTypeNdPeople(Scenario Scenario, int ExitSN)
        {
            for (int j = 0; j < Scenario.Node.GetLength(1); j++)
            {
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write("Ex:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write("Fi:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write("Se:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write("Cr:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }
                Console.WriteLine();

                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write(Scenario.Node[i, j].DistanceToExits[ExitSN].Hop.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write(Scenario.Node[i, j].DistanceToExits[ExitSN].Hop.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write(Scenario.Node[i, j].DistanceToExits[ExitSN].Hop.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write(Scenario.Node[i, j].DistanceToExits[ExitSN].Hop.ToString() + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void ConsoleWriteOtherScenDirectionNdTypeNdPeople(Scenario Scenario)
        {
            for (int j = 0; j < Scenario.Node.GetLength(1); j++)
            {
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write("Ex:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write("Fi:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write("Se:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write("Cr:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }

                Console.WriteLine();

                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write(Scenario.Node[i, j].PeopleAheadDirection + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write(Scenario.Node[i, j].PeopleAheadDirection + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write(Scenario.Node[i, j].PeopleAheadDirection + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write(Scenario.Node[i, j].PeopleAheadDirection + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        public static void ConsoleWriteOtherScenActivitedNdTypeNdPeople(Scenario Scenario)
        {
            for (int j = 0; j < Scenario.Node.GetLength(1); j++)
            {
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write("Ex:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write("Fi:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write("Se:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write("Cr:" + Scenario.Node[i, j].VictimsAtSensor.Count().ToString() + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }

                Console.WriteLine();

                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                {
                    if (Scenario.Node[i, j].Exit)
                    {
                        Console.Write(Scenario.Node[i, j].Activited.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].FireSensor)
                    {
                        Console.Write(Scenario.Node[i, j].Activited.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Sensor)
                    {
                        Console.Write(Scenario.Node[i, j].Activited.ToString() + "\t");
                        continue;
                    }
                    if (Scenario.Node[i, j].Corridor)
                    {
                        Console.Write(Scenario.Node[i, j].Activited.ToString() + "\t");
                        continue;
                    }
                    Console.Write("--" + "\t");
                }

                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
