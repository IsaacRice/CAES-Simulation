using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scenario = CAES.ScenarioNode.Scenario;
using Node = CAES.ScenarioNode.NodeInScenario;
using ICS = CAES.ScenarioNode.InitialCalculateStruct;
using CorridorStruct = CAES.ScenarioNode.CorridorStruct;
using CompareDL = CAES.ScenarioNode.CompareDL;
using CompareDLList = CAES.ScenarioNode.CompareDLList;
using CompareStruct = CAES.ScenarioNode.CompareStruct;
using PathRecord = CAES.ScenarioNode.PathRecord;
using ExitInfo = CAES.ScenarioNode.ExitInfo;
using System.Collections;

namespace CAES
{
    class CAES
    {
        public static Scenario ExecuteCAES(Scenario Scenario)
        {
            Scenario = CalculatePercentage(Scenario);
            Scenario = CalculateShortestPathWithFire(Scenario);
            if (Scenario.DLCalculation == "NewDL")
                Scenario = CalculateBaseDLWithNew(Scenario);
            else if (Scenario.DLCalculation == "MixDL")
                Scenario = CalculateBaseDL(Scenario);
            else if (Scenario.DLCalculation == "OldDL")
                Scenario = CalculateOldBaseDL(Scenario);
            return Scenario;
        }

        public static Scenario ReCalculateCAES(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor && Scenario.Node[i, j].Activited)
                    {
                        Scenario.Node[i, j].DistanceToExits.Sort(new PositionSortAC());

                        if (!Scenario.Node[i, j].Exit)
                        {
                            Scenario.Node[i, j].Hop = -1;

                            for (int k = 0; k < Scenario.ExitSensor.Count(); k++)
                            {
                                Scenario.Node[i, j].DistanceToExits[k].ActualDistance = -1;
                                Scenario.Node[i, j].DistanceToExits[k].EuclideanDistance = -1;
                                Scenario.Node[i, j].DistanceToExits[k].AheadDirection = null;
                                Scenario.Node[i, j].DistanceToExits[k].Hop = -1;
                            }
                        }
                        else
                            for (int k = 0; k < Scenario.ExitSensor.Count(); k++)
                            {
                                Scenario.Node[i, j].Hop = 0;

                                if (Scenario.Node[i, j].DistanceToExits[k].X == i
                                    && Scenario.Node[i, j].DistanceToExits[k].Y == j)
                                {
                                    Scenario.Node[i, j].DistanceToExits[k].ActualDistance = 0;
                                    Scenario.Node[i, j].DistanceToExits[k].EuclideanDistance = 0;
                                    Scenario.Node[i, j].DistanceToExits[k].AheadDirection = null;
                                    Scenario.Node[i, j].DistanceToExits[k].Hop = 0;
                                }
                                else
                                {
                                    Scenario.Node[i, j].DistanceToExits[k].ActualDistance = -1;
                                    Scenario.Node[i, j].DistanceToExits[k].EuclideanDistance = -1;
                                    Scenario.Node[i, j].DistanceToExits[k].AheadDirection = null;
                                    Scenario.Node[i, j].DistanceToExits[k].Hop = -1;
                                }
                            }

                    }

            Scenario = CalculatePercentage(Scenario);
            Scenario = CalculateShortestPathWithFire(Scenario);
            if (Scenario.DLCalculation == "NewDL")
                Scenario = CalculateBaseDLWithNew(Scenario);
            else if (Scenario.DLCalculation == "MixDL")
                Scenario = CalculateBaseDL(Scenario);
            else if (Scenario.DLCalculation == "OldDL")
                Scenario = CalculateOldBaseDL(Scenario);

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor)
                        if (!Scenario.Node[i, j].Exit)
                            if (!Scenario.Node[i, j].FireSensor)
                            {
                                if (i > 0)
                                    for (int n = 1; n < 3; n++)
                                        if (Scenario.Node[i - n, j].VictimsAtSensor.Count > 0)
                                            for (int k = 0; k < Scenario.Node[i - n, j].VictimsAtSensor.Count; k++)
                                                if (Scenario.Node[i - n, j].VictimsAtSensor[k].MovingDirection == "Right")
                                                    Scenario.Node[i, j].PeopleWeight += Scenario.BasicWeightPerPerson;

                                if (i < Scenario.Node.GetLength(0) - 1)
                                    for (int n = 1; n < 3; n++)
                                        if (Scenario.Node[i + n, j].VictimsAtSensor.Count > 0)
                                            for (int k = 0; k < Scenario.Node[i + n, j].VictimsAtSensor.Count; k++)
                                                if (Scenario.Node[i + n, j].VictimsAtSensor[k].MovingDirection == "Left")
                                                    Scenario.Node[i, j].PeopleWeight += Scenario.BasicWeightPerPerson;

                                if (j > 0)
                                    for (int n = 1; n < 2; n++)
                                        if (Scenario.Node[i, j - n].VictimsAtSensor.Count > 0)
                                            for (int k = 0; k < Scenario.Node[i, j - n].VictimsAtSensor.Count; k++)
                                                if (Scenario.Node[i, j - n].VictimsAtSensor[k].MovingDirection == "Down")
                                                    Scenario.Node[i, j].PeopleWeight += Scenario.BasicWeightPerPerson;

                                if (j < Scenario.Node.GetLength(1) - 1)
                                    for (int n = 1; n < 2; n++)
                                        if (Scenario.Node[i, j + n].VictimsAtSensor.Count > 0)
                                            for (int k = 0; k < Scenario.Node[i, j + n].VictimsAtSensor.Count; k++)
                                                if (Scenario.Node[i, j + n].VictimsAtSensor[k].MovingDirection == "Up")
                                                    Scenario.Node[i, j].PeopleWeight += Scenario.BasicWeightPerPerson;

                                Scenario.Node[i, j].Weight = Scenario.Node[i, j].BaseWeight + Scenario.Node[i, j].PeopleWeight;
                            }

            return Scenario;
        }

        public static Scenario CalculateShortestPathWithFire(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.ExitSensor.Count(); i++)
            {
                Scenario = FindNeighborNFindMinHopWithFire(Scenario, i);
            }

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Exit == false && Scenario.Node[i, j].Activited)
                        for (int k = 0; k < Scenario.ExitSensor.Count(); k++)
                        {
                            if (Scenario.Node[i, j].Hop == -1)
                            {
                                Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[k].Hop;
                                Scenario.Node[i, j].PeopleAheadDirection = Scenario.Node[i, j].DistanceToExits[k].AheadDirection;
                            }
                            else if (Scenario.Node[i, j].Hop > Scenario.Node[i, j].DistanceToExits[k].Hop)
                            {
                                Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[k].Hop;
                                Scenario.Node[i, j].PeopleAheadDirection = Scenario.Node[i, j].DistanceToExits[k].AheadDirection;
                            }
                        }
            return Scenario;
        }

        public static Scenario FindNeighborNFindMinHopWithFire(Scenario Scenario, int ExitIndex)
        {
            int HopCount = 0;

            while (!CheckIfCountedAllHopWithFire(Scenario, ExitIndex))
            {
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                    for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    {
                        if (Scenario.Node[i, j].Sensor && Scenario.Node[i, j].Activited)
                            if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == HopCount)
                                if (Scenario.Node[i, j].FireSensor == false)
                                {
                                    if (Scenario.Node[i, j].LeftSensorX != -1 && Scenario.Node[i, j].LeftSensorY != -1)
                                        if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].Activited == true)
                                            if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].FireSensor == false)
                                                if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].Hop == -1 || Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].Hop > HopCount)
                                                {
                                                    Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                                    Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].ActualDistance = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 3;
                                                    Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].EuclideanDistance = Math.Sqrt(Math.Pow(Scenario.Node[i, j].LeftSensorX - Scenario.Node[i, j].DistanceToExits[ExitIndex].X, 2) + Math.Pow(Scenario.Node[i, j].LeftSensorY - Scenario.Node[i, j].DistanceToExits[ExitIndex].Y, 2));
                                                    Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].AheadDirection = "Right";
                                                }
                                    if (Scenario.Node[i, j].RightSensorX != -1 && Scenario.Node[i, j].RightSensorY != -1)
                                        if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].Activited == true)
                                            if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].FireSensor == false)
                                                if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].Hop == -1 || Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].Hop > HopCount)
                                                {
                                                    Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                                    Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].ActualDistance = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 3;
                                                    Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].EuclideanDistance = Math.Sqrt(Math.Pow(Scenario.Node[i, j].RightSensorX - Scenario.Node[i, j].DistanceToExits[ExitIndex].X, 2) + Math.Pow(Scenario.Node[i, j].RightSensorY - Scenario.Node[i, j].DistanceToExits[ExitIndex].Y, 2));
                                                    Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].AheadDirection = "Left";
                                                }
                                    if (Scenario.Node[i, j].TopSensorX != -1 && Scenario.Node[i, j].TopSensorY != -1)
                                        if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].Activited == true)
                                            if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].FireSensor == false)
                                                if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].Hop == -1 || Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].Hop > HopCount)
                                                {
                                                    Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                                    Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].ActualDistance = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 2;
                                                    Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].EuclideanDistance = Math.Sqrt(Math.Pow(Scenario.Node[i, j].TopSensorX - Scenario.Node[i, j].DistanceToExits[ExitIndex].X, 2) + Math.Pow(Scenario.Node[i, j].TopSensorY - Scenario.Node[i, j].DistanceToExits[ExitIndex].Y, 2));
                                                    Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].AheadDirection = "Down";
                                                }
                                    if (Scenario.Node[i, j].BottemSensorX != -1 && Scenario.Node[i, j].BottemSensorY != -1)
                                        if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].Activited == true)
                                            if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].FireSensor == false)
                                                if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].DistanceToExits[ExitIndex].Hop == -1 || Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].DistanceToExits[ExitIndex].Hop > HopCount)
                                                {
                                                    Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                                    Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].DistanceToExits[ExitIndex].ActualDistance = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 2;
                                                    Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].DistanceToExits[ExitIndex].EuclideanDistance = Math.Sqrt(Math.Pow(Scenario.Node[i, j].BottemSensorX - Scenario.Node[i, j].DistanceToExits[ExitIndex].X, 2) + Math.Pow(Scenario.Node[i, j].BottemSensorY - Scenario.Node[i, j].DistanceToExits[ExitIndex].Y, 2));
                                                    Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].DistanceToExits[ExitIndex].AheadDirection = "Up";
                                                }
                                }
                                else
                                {

                                }
                    }
                HopCount++;
            }
            return Scenario;
        }

        public static bool CheckIfCountedAllHopWithFire(Scenario Scenario, int ExitIndex)
        {

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Activited == true)
                        if (Scenario.Node[i, j].FireSensor == false)
                            if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == -1)
                                return false;
                }
            return true;
        }

        public static Scenario CalculatePercentage(Scenario Scenario)
        {
            /*
            int WidthForCalculate = (Scenario.Node.GetLength(0) - 1) / 3,
                HeightForCalculate = (Scenario.Node.GetLength(1) - 1) / 2,
                TotalLength = 2 * WidthForCalculate * (HeightForCalculate + 1)
                + HeightForCalculate * (WidthForCalculate + 1);
            */

            int TotalLength = 2 * (Scenario.SensorWidth - 1) * Scenario.SensorHeight
                + (Scenario.SensorHeight - 1) * Scenario.SensorWidth + Scenario.SensorWidth * Scenario.SensorHeight;

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Activited)
                    {
                        if (Scenario.Node[i, j].Sensor == true)
                            Scenario.Node[i, j].PeopleWeight
                                = (double)Scenario.Node[i, j].People / Scenario.WholeVictims.GetLength(0);
                        if (Scenario.Node[i, j].Corridor == true)
                            Scenario.Node[i, j].BasePercentageDistance = (double)1 / TotalLength;
                    }
            }

            Scenario.BasicLengthPerCorridor = (double)1 / TotalLength;
            Scenario.BasicWeightPerPerson = (double)1 / Scenario.WholeVictims.GetLength(0); ;

            return Scenario;
        }

        public static Scenario CalculateOldBaseDL(Scenario Scenario)
        {
            int MaxHop = 0;
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Activited
                        && Scenario.Node[i, j].Sensor)
                    {
                        Scenario.Node[i, j].DistanceToExits.Sort(new DistanceSortAC());
                        if (Scenario.Node[i, j].DistanceToExits[0].Hop > MaxHop)
                            MaxHop = Scenario.Node[i, j].DistanceToExits[0].Hop;
                    }

            for (int k = 0; k < Scenario.ExitSensor.Count; k++)
                for (int l = 1; l < MaxHop + 1; l++)
                    for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                        for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                            if (!Scenario.Node[i, j].Exit
                                && Scenario.Node[i, j].Activited
                                && Scenario.Node[i, j].Sensor
                                && !Scenario.Node[i, j].FireSensor)
                                if (Scenario.Node[i, j].DistanceToExits[0].Hop == l
                                    && Scenario.Node[i, j].DistanceToExits[0].X == Scenario.ExitSensor[k].X
                                    && Scenario.Node[i, j].DistanceToExits[0].Y == Scenario.ExitSensor[k].Y)
                                {
                                    double CompareDLTemp = -1;

                                    for (int m = 0; m < Scenario.Node.GetLength(0); m++)
                                        for (int n = 0; n < Scenario.Node.GetLength(1); n++)
                                            if (Scenario.Node[m, n].DistanceToExits[0].Hop == l - 1
                                                && Scenario.Node[m, n].DistanceToExits[0].X == Scenario.ExitSensor[k].X
                                                && Scenario.Node[m, n].DistanceToExits[0].Y == Scenario.ExitSensor[k].Y
                                                && Scenario.Node[m, n].FireSensor == false)
                                            {
                                                if (CompareDLTemp == -1)
                                                    CompareDLTemp = (Math.Abs(m - i) + Math.Abs(n - j))
                                                        * Scenario.BasicLengthPerCorridor
                                                        * l
                                                        + Scenario.Node[m, n].BaseWeight;
                                                else if (CompareDLTemp > (Math.Abs(m - i) + Math.Abs(n - j))
                                                                        * Scenario.BasicLengthPerCorridor
                                                                        + Scenario.Node[m, n].Weight)
                                                    CompareDLTemp = (Math.Abs(m - i) + Math.Abs(n - j))
                                                        * Scenario.BasicLengthPerCorridor
                                                        * l
                                                        + Scenario.Node[m, n].BaseWeight;
                                            }
                                    Scenario.Node[i, j].BaseWeight = CompareDLTemp;
                                }

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor)
                        if (!Scenario.Node[i, j].Exit)
                            Scenario.Node[i, j].Weight = Scenario.Node[i, j].BaseWeight
                                + Scenario.Node[i, j].PeopleWeight;

            return Scenario;
        }

        public static Scenario CalculateBaseDL(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (!Scenario.Node[i, j].Exit
                        && Scenario.Node[i, j].Activited
                        && Scenario.Node[i, j].Sensor
                        && !Scenario.Node[i, j].FireSensor)
                    {
                        Scenario.Node[i, j].DistanceToExits.Sort(new DistanceSortAC());
                        Scenario.Node[i, j].BaseWeight =
                            (double)Scenario.Node[i, j].DistanceToExits[0].ActualDistance
                            * Scenario.BasicLengthPerCorridor;


                        /*
                        if (Scenario.Node[k * 2, l * 3].Hop == j
                            && Scenario.Node[k * 2, l * 3].TargetX == DefaultExitSensor[i, 0] * 3
                            && Scenario.Node[k * 2, l * 3].TargetY == DefaultExitSensor[i, 1] * 2
                            && Scenario.Node[k * 2, l * 3].Exit != true)
                        {
                            double Comparision = -1;
                            if (l * 3 - Scenario.Node[k * 2, l * 3].TargetX < 0) //往右
                            {
                                Scenario.Node[k * 2, l * 3].BaseWeight = Scenario.Node[k * 2, l * 3 + 3].BaseWeight
                                       + Scenario.Node[k * 2, l * 3 + 2].BasePercentageDistance * (j + 1)
                                       + Scenario.Node[k * 2, l * 3 + 1].BasePercentageDistance * (j + 1);
                                Comparision = Scenario.Node[k * 2, l * 3].BaseWeight;
                            }
                            else if (l * 3 - Scenario.Node[k * 2, l * 3].TargetX > 0)//往左
                            {
                                Scenario.Node[k * 2, l * 3].BaseWeight = Scenario.Node[k * 2, l * 3 - 3].BaseWeight
                                       + Scenario.Node[k * 2, l * 3 - 2].BasePercentageDistance * (j + 1)
                                       + Scenario.Node[k * 2, l * 3 - 1].BasePercentageDistance * (j + 1);
                                Comparision = Scenario.Node[k * 2, l * 3].BaseWeight;
                            }
                            if (k * 2 - Scenario.Node[k * 2, l * 3].TargetY < 0) //往下
                                Scenario.Node[k * 2, l * 3].BaseWeight = Scenario.Node[k * 2 + 2, l * 3].BaseWeight
                                    + Scenario.Node[k * 2 + 1, l * 3].BasePercentageDistance * (j + 1);
                            else if (k * 2 - Scenario.Node[k * 2, l * 3].TargetY > 0)//往上
                                Scenario.Node[k * 2, l * 3].BaseWeight = Scenario.Node[k * 2 - 2, l * 3].BaseWeight
                                    + Scenario.Node[k * 2 - 1, l * 3].BasePercentageDistance * (j + 1);
                            if (Comparision != -1 && Comparision < Scenario.Node[k * 2, l * 3].BaseWeight)
                                Scenario.Node[k * 2, l * 3].BaseWeight = Comparision;
                        }
                         */
                    }

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor)
                        if (!Scenario.Node[i, j].Exit)
                            Scenario.Node[i, j].Weight = Scenario.Node[i, j].BaseWeight
                                + Scenario.Node[i, j].PeopleWeight;

            return Scenario;
        }

        public static Scenario CalculateBaseDLWithNew(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (!Scenario.Node[i, j].Exit
                        && Scenario.Node[i, j].Activited
                        && Scenario.Node[i, j].Sensor
                        && !Scenario.Node[i, j].FireSensor)
                    {
                        Scenario.Node[i, j].DistanceToExits.Sort(new DistanceSortAC());
                        /*
                        double[] SortWeight = new double[Scenario.ExitSensor.Count];
                        for (int k = 0; k < Scenario.ExitSensor.Count; k++)
                            SortWeight[k] = -1;

                        for (int k = 0; k < Scenario.ExitSensor.Count; k++)
                        {
                            double WidthDifference = 0, HeightDifference = 0;
                            WidthDifference = Math.Abs((double)i - Scenario.ExitSensor[k].X * 3);
                            HeightDifference = Math.Abs((double)j - Scenario.ExitSensor[k].Y * 2);
                            SortWeight[k] = HeightDifference + WidthDifference;
                        }

                        double TempSort;

                        for (int m = 0; m < Scenario.ExitSensor.Count - 1; m++)
                            for (int n = 0; n < Scenario.ExitSensor.Count - 1 - m; n++)
                                if (SortWeight[n] != -1
                                    && SortWeight[n + 1] != -1
                                    && SortWeight[n] > SortWeight[n + 1])
                                {
                                    TempSort = SortWeight[n];
                                    SortWeight[n] = SortWeight[n + 1];
                                    SortWeight[n + 1] = TempSort;
                                }
                        */

                        Scenario.Node[i, j].BaseWeight
                            = Scenario.Alpha * Scenario.Node[i, j].DistanceToExits[0].ActualDistance
                            + Scenario.Beta * Scenario.Node[i, j].DistanceToExits[1].ActualDistance
                            + Scenario.Gamma * Scenario.Node[i, j].DistanceToExits[2].ActualDistance;

                        Scenario.Node[i, j].BaseWeight *= Scenario.BasicLengthPerCorridor;
                    }


            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor)
                        if (!Scenario.Node[i, j].Exit)
                            Scenario.Node[i, j].Weight = Scenario.Node[i, j].BaseWeight
                                + Scenario.Node[i, j].PeopleWeight;

            return Scenario;
        }

        public static Scenario CAESSimulation(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].VictimsAtSensor.Count > 0 && Scenario.Node[i, j].Activited)
                    {
                        int CanMovePeople = 0;
                        for (int CheckIfVictimsCanMove = 0
                            ; CheckIfVictimsCanMove < Scenario.Node[i, j].VictimsAtSensor.Count
                            ; CheckIfVictimsCanMove++)
                            if (Scenario.Node[i, j].VictimsAtSensor[CheckIfVictimsCanMove].HaveMovedThisRound == false)
                                CanMovePeople++;

                        if (CanMovePeople > 0)
                        {
                            if ((Scenario.Node[i, j].Sensor == true) && (Scenario.Node[i, j].Exit == false))
                            {

                                /// <summary>
                                ///  Find parameter around the sensor[i,j]
                                /// </summary>

                                bool ThisRoundRunCheck = false;

                                ICS InitialNode = InitialForCalculate(Scenario, i, j);

                                double STDwithoutSelf = StandardDeviationParent(InitialNode),
                                       DIwithoutSelf = DispersionIndexParent(InitialNode),
                                       STDwithSelf = StandardDeviationParentwithSelf(InitialNode),
                                       DIwithSelf = DispersionIndexParentwithSelf(InitialNode);

                                if (MeanOfWeight(InitialNode) == 0)
                                    DIwithoutSelf = 0;

                                if (Scenario.ThresholdType == "SmSTDwoS")
                                    Scenario.Node[i, j].ThresholdValue
                                        = Math.Round(InitialNode.SelfWeight - STDwithoutSelf, 8);
                                else if (Scenario.ThresholdType == "SmDIwoS")
                                    Scenario.Node[i, j].ThresholdValue
                                        = Math.Round(InitialNode.SelfWeight - DIwithoutSelf, 8);
                                else if (Scenario.ThresholdType == "HmSTDnDIwoS")
                                    Scenario.Node[i, j].ThresholdValue
                                        = Math.Round(InitialNode.HighestWeight - DIwithoutSelf - STDwithoutSelf, 8);
                                else if (Scenario.ThresholdType == "SmSTDwS")
                                    Scenario.Node[i, j].ThresholdValue
                                        = Math.Round(InitialNode.SelfWeight - STDwithSelf, 8);
                                else if (Scenario.ThresholdType == "SmDIwS")
                                    Scenario.Node[i, j].ThresholdValue
                                        = Math.Round(InitialNode.SelfWeight - DIwithSelf, 8);
                                else if (Scenario.ThresholdType == "HmSTDnDIwS")
                                    Scenario.Node[i, j].ThresholdValue
                                        = Math.Round(InitialNode.HighestWeight - DIwithSelf - STDwithSelf, 8);

                                if (Scenario.Node[i, j].ThresholdValue < 0)
                                    Scenario.Node[i, j].ThresholdValue = 0;

                                CompareDL CompareWeightList = new CompareDL();
                                CompareWeightList.DLList = new List<CompareDLList>();
                                CompareWeightList.FindMinDL = -1;
                                CompareWeightList.FindMaxDL = -1;
                                CompareWeightList.MinDangerousLevelX = -1;
                                CompareWeightList.MinDangerousLevelY = -1;

                                if (j != 0)
                                    if (Scenario.Node[i, j - 2].Activited == true)
                                        if (Scenario.Node[i, j - 2].FireSensor != true)
                                            CompareWeightList.DLList.Add(new CompareDLList(i, j - 2, "Up", Scenario.Node[i, j - 2].Weight));

                                if (j != Scenario.Node.GetLength(1) - 1)
                                    if (Scenario.Node[i, j + 2].Activited == true)
                                        if (Scenario.Node[i, j + 2].FireSensor != true)
                                            CompareWeightList.DLList.Add(new CompareDLList(i, j + 2, "Down", Scenario.Node[i, j + 2].Weight));

                                if (i != 0)
                                    if (Scenario.Node[i - 3, j].Activited == true)
                                        if (Scenario.Node[i - 3, j].FireSensor != true)
                                            CompareWeightList.DLList.Add(new CompareDLList(i - 3, j, "Left", Scenario.Node[i - 3, j].Weight));

                                if (i != Scenario.Node.GetLength(0) - 1)
                                    if (Scenario.Node[i + 3, j].Activited == true)
                                        if (Scenario.Node[i + 3, j].FireSensor != true)
                                            CompareWeightList.DLList.Add(new CompareDLList(i + 3, j, "Right", Scenario.Node[i + 3, j].Weight));

                                /// <summary>
                                ///  New Moving Part Start
                                /// </summary>

                                /// <code>
                                ///  Sort and delete list item over threshold Start
                                /// </code>

                                CompareWeightList.DLList.Sort(new WeightSortAC());

                                List<CompareDLList> DListBackup = new List<CompareDLList>();
                                for (int k = 0; k < CompareWeightList.DLList.Count; k++)
                                    DListBackup.Add(
                                        new CompareDLList(
                                            CompareWeightList.DLList[k].X
                                            , CompareWeightList.DLList[k].Y
                                            , CompareWeightList.DLList[k].Direction
                                            , CompareWeightList.DLList[k].Weight));

                                //CompareWeightList.DLList.ForEach(CopyPos => DListBackup.Add(CopyPos));
                                /*for (int CopyPos = 0; CopyPos < CompareWeightList.DLList.Count; CopyPos++)
                                    DListBackup[CopyPos] = CompareWeightList.DLList[CopyPos];
                                */

                                if (Scenario.Node[i, j].FireSensor != true)
                                    for (int ListCurPos = 0; ListCurPos < CompareWeightList.DLList.Count; ListCurPos++)
                                    {
                                        if (CompareWeightList.DLList[ListCurPos].Weight > Scenario.Node[i, j].ThresholdValue)
                                        {
                                            CompareWeightList.DLList.RemoveRange(ListCurPos, CompareWeightList.DLList.Count - ListCurPos);
                                            if (ListCurPos == 0)
                                                CompareWeightList.DLList = null;
                                            break;
                                        }
                                    }

                                /// <code>
                                ///  Sort and delete list item over threshold End
                                /// </code>

                                /// <code>
                                ///  Decide where to move Start
                                /// </code>

                                int MovingPeople = 0;
                                if (CompareWeightList.DLList != null)
                                    for (int ListCurPos = 0; ListCurPos < CompareWeightList.DLList.Count; ListCurPos++)
                                    {
                                        /// <code>
                                        ///  Decide how many people to move Start
                                        /// </code>

                                        // Confirm if this loop execuated
                                        ThisRoundRunCheck = true;

                                        if (CanMovePeople >= Scenario.CorridorCapacity)
                                        {
                                            MovingPeople = Scenario.CorridorCapacity;
                                            CanMovePeople -= MovingPeople;
                                        }
                                        else
                                        {
                                            MovingPeople = CanMovePeople;
                                            CanMovePeople -= MovingPeople;
                                        }
                                        /// <code>
                                        ///  Decide how many people to move End
                                        /// </code>

                                        /// <code>
                                        ///  Move people Start
                                        /// </code>

                                        int TargetX = i, TargetY = j;

                                        switch (CompareWeightList.DLList[ListCurPos].Direction)
                                        {
                                            case "Up":
                                                TargetY -= 1;
                                                break;
                                            case "Down":
                                                TargetY += 1;
                                                break;
                                            case "Left":
                                                TargetX -= 1;
                                                break;
                                            case "Right":
                                                TargetX += 1;
                                                break;
                                            default:
                                                break;
                                        }

                                        int ActualMovedVictims = 0;
                                        int MovingPos = 0;
                                        while (ActualMovedVictims != MovingPeople)
                                        {
                                            if (Scenario.Node[i, j].VictimsAtSensor.Count <= MovingPos)
                                                break;
                                            if (Scenario.Node[i, j].VictimsAtSensor.Count != 0)
                                                if (Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound == false)
                                                {
                                                    //Count, add or minus weight
                                                    if (Scenario.DLCalculation != "OldDL")
                                                        if (!Scenario.Node[i, j].FireSensor)
                                                            Scenario.Node[i, j].TempWeight -= (double)1 / Scenario.WholeVictims.Count();

                                                    if (Scenario.Node[CompareWeightList.DLList[ListCurPos].X
                                                        , CompareWeightList.DLList[ListCurPos].Y].Exit == false)
                                                        Scenario.Node[CompareWeightList.DLList[ListCurPos].X
                                                            , CompareWeightList.DLList[ListCurPos].Y].TempWeight
                                                            += (double)1 / Scenario.WholeVictims.Count();

                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound = true;
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path.Add(
                                                        new PathRecord(
                                                            TargetX
                                                            , TargetY
                                                            , Scenario.TotalEvacuationRound
                                                            , false));
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].X = TargetX;
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y = TargetY;
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection
                                                        = CompareWeightList.DLList[ListCurPos].Direction;
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound
                                                        = Scenario.TotalEvacuationRound;
                                                    Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN]
                                                        = Scenario.Node[i, j].VictimsAtSensor[MovingPos];
                                                    Scenario.Node[TargetX, TargetY].VictimsAtSensor.Add(
                                                        new ScenarioNode.EachVictim(
                                                            Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].X
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartX
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartY
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].RuleFollower
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Live
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].TheRoundVictimDead
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveEscaped
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Activated));
                                                    Scenario.Node[i, j].VictimsAtSensor.RemoveAt(MovingPos);
                                                    ActualMovedVictims++;
                                                    MovingPos--;
                                                }
                                            MovingPos++;
                                        }

                                        /*
                                        for (int MovingPos = 0; MovingPos < MovingPeople; MovingPos++)
                                        {
                                            if (Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound == false)
                                            {
                                                Scenario.Node[TargetX, TargetY].People++;

                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound = true;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path.Add(
                                                    new PathRecord(
                                                        TargetX
                                                        , TargetY
                                                        , Scenario.TotalEvacuationRound
                                                        , false));
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].X = TargetX;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y = TargetY;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection = CompareWeightList.DLList[ListCurPos].Direction;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound = Scenario.TotalEvacuationRound;

                                                Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN] = Scenario.Node[i, j].VictimsAtSensor[MovingPos];
                                                Scenario.Node[TargetX, TargetY].VictimsAtSensor.Add(
                                                    new ScenarioNode.EachVictim(
                                                        Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].X
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartX
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartY
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].RuleFollower
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveEscaped));
                                                Scenario.Node[i, j].VictimsAtSensor.RemoveAt(MovingPos);
                                            }
                                            else
                                                MovingPos--;
                                        }
                                        */

                                        /// <code>
                                        ///  Move people End
                                        /// </code>

                                    }

                                // If there is no moving because the threshold, guide victims with number of corridor limit to lowest neighbor
                                if (ThisRoundRunCheck == false)
                                {

                                    /// <code>
                                    ///  Decide how many people to move Start
                                    /// </code>

                                    // Confirm if this loop execuated
                                    ThisRoundRunCheck = true;

                                    if (CanMovePeople >= Scenario.CorridorCapacity)
                                        MovingPeople = Scenario.CorridorCapacity;
                                    else
                                        MovingPeople = CanMovePeople;

                                    int TargetX = i, TargetY = j;

                                    switch (DListBackup[0].Direction)
                                    {
                                        case "Up":
                                            TargetY -= 1;
                                            break;
                                        case "Down":
                                            TargetY += 1;
                                            break;
                                        case "Left":
                                            TargetX -= 1;
                                            break;
                                        case "Right":
                                            TargetX += 1;
                                            break;
                                        default:
                                            break;
                                    }

                                    /// <code>
                                    ///  Decide how many people to move End
                                    /// </code>

                                    int ActualMovedVictims = 0;
                                    int MovingPos = 0;
                                    while (ActualMovedVictims != MovingPeople)
                                    {
                                        if (Scenario.Node[i, j].VictimsAtSensor.Count <= MovingPos)
                                            break;
                                        if (Scenario.Node[i, j].VictimsAtSensor.Count != 0)
                                            if (Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound == false)
                                            {
                                                //Count, add or minus weight

                                                if (Scenario.DLCalculation != "OldDL")
                                                    if (!Scenario.Node[i, j].FireSensor)
                                                        Scenario.Node[i, j].TempWeight -= (double)1 / Scenario.WholeVictims.Count();

                                                if (Scenario.Node[DListBackup[0].X, DListBackup[0].Y].Exit == false)
                                                    Scenario.Node[DListBackup[0].X, DListBackup[0].Y].TempWeight
                                                        += (double)1 / Scenario.WholeVictims.Count();

                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound = true;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path.Add(
                                                    new PathRecord(
                                                        TargetX
                                                        , TargetY
                                                        , Scenario.TotalEvacuationRound
                                                        , false));
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].X = TargetX;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y = TargetY;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection = DListBackup[0].Direction;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound = Scenario.TotalEvacuationRound;

                                                Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN] = Scenario.Node[i, j].VictimsAtSensor[MovingPos];
                                                Scenario.Node[TargetX, TargetY].VictimsAtSensor.Add(
                                                    new ScenarioNode.EachVictim(
                                                        Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].X
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartX
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartY
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].RuleFollower
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Live
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].TheRoundVictimDead
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveEscaped
                                                        , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Activated));
                                                Scenario.Node[i, j].VictimsAtSensor.RemoveAt(MovingPos);
                                                ActualMovedVictims++;
                                                MovingPos--;
                                            }
                                        MovingPos++;
                                    }
                                    /*
                                    for (int MovingPos = 0; MovingPos < MovingPeople; MovingPos++)
                                    {
                                        if (Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound == false)
                                        {
                                            Scenario.Node[TargetX, TargetY].People++;

                                            Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound = true;
                                            Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path.Add(
                                                new PathRecord(
                                                    TargetX
                                                    , TargetY
                                                    , Scenario.TotalEvacuationRound
                                                    , false));
                                            Scenario.Node[i, j].VictimsAtSensor[MovingPos].X = TargetX;
                                            Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y = TargetY;
                                            Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection = DListBackup[0].Direction;
                                            Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound = Scenario.TotalEvacuationRound;

                                            Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN] = Scenario.Node[i, j].VictimsAtSensor[MovingPos];
                                            Scenario.Node[TargetX, TargetY].VictimsAtSensor.Add(
                                                new ScenarioNode.EachVictim(
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].X
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartX
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartY
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].RuleFollower
                                                    , Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveEscaped));
                                            Scenario.Node[i, j].VictimsAtSensor.RemoveAt(MovingPos);
                                        }
                                        else if (Scenario.Node[i, j].VictimsAtSensor.Count == 1)
                                            break;
                                        else
                                            MovingPos--;
                                    }
                                    */
                                }

                                /// <code>
                                ///  Decide where to move End
                                /// </code>

                                /// <summary>
                                ///  New Moving Part End
                                /// </summary>

                            }
                            else if (Scenario.Node[i, j].Corridor == true)
                            {
                                if (Scenario.Node[i, j].VictimsAtSensor.Count > 0)
                                    for (int CorridorMoveCount = 0; CorridorMoveCount < Scenario.Node[i, j].VictimsAtSensor.Count; CorridorMoveCount++)
                                        if (Scenario.Node[i, j].VictimsAtSensor[CorridorMoveCount].HaveMovedThisRound == false)
                                        {
                                            int TargetX = i, TargetY = j;

                                            switch (Scenario.Node[i, j].VictimsAtSensor[CorridorMoveCount].MovingDirection)
                                            {
                                                case "Up":
                                                    TargetY -= 1;
                                                    break;
                                                case "Down":
                                                    TargetY += 1;
                                                    break;
                                                case "Left":
                                                    TargetX -= 1;
                                                    break;
                                                case "Right":
                                                    TargetX += 1;
                                                    break;
                                                default:
                                                    break;
                                            }

                                            Scenario.Node[TargetX, TargetY].People++;

                                            Scenario.Node[i, j].VictimsAtSensor[0].HaveMovedThisRound = true;
                                            Scenario.Node[i, j].VictimsAtSensor[0].Path.Add(
                                                new PathRecord(
                                                    TargetX
                                                    , TargetY
                                                    , Scenario.TotalEvacuationRound
                                                    , false));
                                            Scenario.Node[i, j].VictimsAtSensor[0].X = TargetX;
                                            Scenario.Node[i, j].VictimsAtSensor[0].Y = TargetY;
                                            Scenario.Node[i, j].VictimsAtSensor[0].LastMovedRound = Scenario.TotalEvacuationRound;

                                            if (Scenario.Node[TargetX, TargetY].Exit == true)
                                            {
                                                Scenario.Node[i, j].VictimsAtSensor[0].HaveEscaped = true;
                                                Scenario.Node[i, j].VictimsAtSensor[0].Activated = false;
                                            }

                                            if (Scenario.Node[TargetX, TargetY].FireSensor == true)
                                            {
                                                Scenario.Node[i, j].VictimsAtSensor[0].Live = false;
                                                Scenario.Node[i, j].VictimsAtSensor[0].TheRoundVictimDead = Scenario.TotalEvacuationRound;
                                                Scenario.Node[i, j].VictimsAtSensor[0].Activated = false;
                                            }

                                            Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[0].SN] = Scenario.Node[i, j].VictimsAtSensor[0];

                                            if (Scenario.Node[TargetX, TargetY].FireSensor == false)
                                                Scenario.Node[TargetX, TargetY].VictimsAtSensor.Add(
                                                    new ScenarioNode.EachVictim(
                                                        Scenario.Node[i, j].VictimsAtSensor[0].SN
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].X
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].Y
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].StartX
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].StartY
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].MovingDirection
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].LastMovedRound
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].HaveMovedThisRound
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].Path
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].RuleFollower
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].Live
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].TheRoundVictimDead
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].HaveEscaped
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].Activated));
                                            Scenario.Node[i, j].VictimsAtSensor.RemoveAt(0);
                                            CorridorMoveCount--;
                                        }
                            }
                        }
                    }
            // Add temp weight to node
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    Scenario.Node[i, j].People = Scenario.Node[i, j].VictimsAtSensor.Count;
                    if (Scenario.Node[i, j].Exit == false)
                    {
                        Scenario.Node[i, j].Weight += Scenario.Node[i, j].TempWeight;
                        Scenario.Node[i, j].TempWeight = 0;
                        for (int k = 0; k < Scenario.Node[i, j].VictimsAtSensor.Count; k++)
                        {
                            Scenario.Node[i, j].VictimsAtSensor[k].HaveEscaped = false;
                            Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[k].SN].HaveEscaped = false;
                        }
                    }
                }
            // Set all haverun parameter to false
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    for (int k = 0; k < Scenario.Node[i, j].VictimsAtSensor.Count; k++)
                        Scenario.Node[i, j].VictimsAtSensor[k].HaveMovedThisRound = false;

                    for (int k = 0; k < Scenario.WholeVictims.Count(); k++)
                        Scenario.WholeVictims[k].HaveMovedThisRound = false;

                    Scenario.Node[i, j].HaveRunThisRound = false;
                }

            return Scenario;
        }

        public static ICS InitialForCalculate(Scenario Scenario, int TestX, int TestY)
        {
            ICS InitialNode = new ICS();
            InitialNode.ary = new ArrayList();
            InitialNode.TotalWeight = 0;
            InitialNode.TotalNeighbor = 0;
            InitialNode.SumOfSquare = 0;
            InitialNode.SelfWeight = Scenario.Node[TestX, TestY].Weight;
            InitialNode.HighestWeight = -1;
            InitialNode.MeanWeight = -1;
            InitialNode.MeanWightwithSelf = -1;
            InitialNode.X = TestY;
            InitialNode.Y = TestX;

            //if it's not at top border
            if (TestX != 0 && TestX > 2)
                if (Scenario.Node[TestX - 3, TestY].Activited == true)
                    if (Scenario.Node[TestX - 3, TestY].Activited == true)
                    {
                        InitialNode.TotalWeight += Scenario.Node[TestX - 3, TestY].Weight;
                        InitialNode.TotalNeighbor += 1;
                        InitialNode.ary.Add(Scenario.Node[TestX - 3, TestY].Weight);
                        if (InitialNode.HighestWeight == -1 || InitialNode.HighestWeight < Scenario.Node[TestX - 3, TestY].Weight)
                            InitialNode.HighestWeight = Scenario.Node[TestX - 3, TestY].Weight;
                    }

            ////if it's not at down border
            if (TestX != Scenario.Node.GetLength(0) - 1 && TestX < Scenario.Node.GetLength(0) - 3)
                if (Scenario.Node[TestX + 3, TestY].Activited == true)
                    if (Scenario.Node[TestX + 3, TestY].FireSensor == true)
                    {
                        InitialNode.TotalWeight += Scenario.Node[TestX + 3, TestY].Weight;
                        InitialNode.TotalNeighbor += 1;
                        InitialNode.ary.Add(Scenario.Node[TestX + 3, TestY].Weight);
                        if (InitialNode.HighestWeight == -1 || InitialNode.HighestWeight < Scenario.Node[TestX + 3, TestY].Weight)
                            InitialNode.HighestWeight = Scenario.Node[TestX + 3, TestY].Weight;
                    }

            //if it's not at left border
            if (TestY != 0 && TestY > 1)
                if (Scenario.Node[TestX, TestY - 2].Activited == true)
                    if (Scenario.Node[TestX, TestY - 2].FireSensor == true)
                    {
                        InitialNode.TotalWeight += Scenario.Node[TestX, TestY - 2].Weight;
                        InitialNode.TotalNeighbor += 1;
                        InitialNode.ary.Add(Scenario.Node[TestX, TestY - 2].Weight);
                        if (InitialNode.HighestWeight == -1 || InitialNode.HighestWeight < Scenario.Node[TestX, TestY - 2].Weight)
                            InitialNode.HighestWeight = Scenario.Node[TestX, TestY - 2].Weight;
                    }

            //if it's not at right border
            if (TestY != Scenario.Node.GetLength(1) - 1 && TestY < Scenario.Node.GetLength(1) - 2)
                if (Scenario.Node[TestX, TestY + 2].Activited == true)
                    if (Scenario.Node[TestX, TestY + 2].FireSensor == true)
                    {
                        InitialNode.TotalWeight += Scenario.Node[TestX, TestY + 2].Weight;
                        InitialNode.TotalNeighbor += 1;
                        InitialNode.ary.Add(Scenario.Node[TestX, TestY + 2].Weight);
                        if (InitialNode.HighestWeight == -1 || InitialNode.HighestWeight < Scenario.Node[TestX, TestY + 2].Weight)
                            InitialNode.HighestWeight = Scenario.Node[TestX, TestY + 2].Weight;
                    }

            if (InitialNode.TotalNeighbor != 0)
            {
                InitialNode.MeanWeight = InitialNode.TotalWeight / InitialNode.TotalNeighbor;
                InitialNode.MeanWightwithSelf = (InitialNode.TotalWeight + InitialNode.SelfWeight) / (InitialNode.TotalNeighbor + 1);
            }

            if (InitialNode.HighestWeight == -1)
                InitialNode.HighestWeight = InitialNode.SelfWeight;

            if (InitialNode.MeanWeight == -1)
                InitialNode.MeanWeight = InitialNode.SelfWeight;

            if (InitialNode.MeanWightwithSelf == -1)
                InitialNode.MeanWightwithSelf = InitialNode.SelfWeight;

            return InitialNode;
        }


        public static double DispersionIndexParent(ICS InitialNode)
        {
            if (InitialNode.TotalNeighbor > 1)
            {
                for (int i = 0; i < InitialNode.ary.Count; i++)
                {
                    double value = double.Parse(InitialNode.ary[i].ToString());
                    InitialNode.SumOfSquare += Math.Pow((value - MeanOfWeight(InitialNode)), 2);
                }
                return (InitialNode.SumOfSquare / InitialNode.ary.Count) / MeanOfWeight(InitialNode);
            }
            else if (InitialNode.TotalNeighbor == 1)
                return 0;
            else
                return 0;
        }


        public static double DispersionIndexParentwithSelf(ICS InitialNode)
        {
            if (InitialNode.TotalNeighbor >= 1)
            {
                for (int i = 0; i < InitialNode.ary.Count; i++)
                {
                    double value = double.Parse(InitialNode.ary[i].ToString());
                    InitialNode.SumOfSquare += Math.Pow((value - MeanOfWeightwithSelf(InitialNode)), 2);
                }
                InitialNode.SumOfSquare += Math.Pow((InitialNode.SelfWeight - MeanOfWeightwithSelf(InitialNode)), 2);
                return (InitialNode.SumOfSquare / (InitialNode.ary.Count + 1)) / MeanOfWeightwithSelf(InitialNode);
            }
            else
                return 0;
        }

        public static double MeanOfWeight(ICS InitialNode)
        {
            return Math.Round(InitialNode.TotalWeight / InitialNode.ary.Count, 15);
        }

        public static double MeanOfWeightwithSelf(ICS InitialNode)
        {
            return Math.Round((InitialNode.TotalWeight + InitialNode.SelfWeight) / (InitialNode.ary.Count + 1), 15);
        }
        public static double StandardDeviationParent(ICS InitialNode)
        {
            if (InitialNode.TotalNeighbor > 1)
            {
                for (int i = 0; i < InitialNode.ary.Count; i++)
                {
                    double value = double.Parse(InitialNode.ary[i].ToString());
                    InitialNode.SumOfSquare += Math.Pow((value - MeanOfWeight(InitialNode)), 2);
                }
                return Math.Round(Math.Sqrt(InitialNode.SumOfSquare / InitialNode.ary.Count), 15);
            }
            else if (InitialNode.TotalNeighbor == 1)
                return 0;
            else
                return 0;
        }

        public static double StandardDeviationParentwithSelf(ICS InitialNode)
        {
            if (InitialNode.TotalNeighbor >= 1)
            {
                for (int i = 0; i < InitialNode.ary.Count; i++)
                {
                    double value = double.Parse(InitialNode.ary[i].ToString());
                    InitialNode.SumOfSquare += Math.Pow((value - MeanOfWeightwithSelf(InitialNode)), 2);
                }
                InitialNode.SumOfSquare += Math.Pow((InitialNode.SelfWeight - MeanOfWeightwithSelf(InitialNode)), 2);
                return Math.Sqrt(InitialNode.SumOfSquare / (InitialNode.ary.Count + 1));
            }
            else
                return 0;
        }

        public static CompareStruct CompareMethod(CompareStruct CompareWeight, Scenario Scenario, int OriginX, int OriginY, int TestTargetX, int TestTargetY, string Direction)
        {
            if (CompareWeight.FindMaxDL == -1 || CompareWeight.FindMaxDL < Scenario.Node[TestTargetX, TestTargetY].Weight)
                CompareWeight.FindMaxDL = Scenario.Node[TestTargetX, TestTargetY].Weight;

            if (CompareWeight.FindMinDL == -1 || CompareWeight.FindMinDL > Scenario.Node[TestTargetX, TestTargetY].Weight)
            {
                CompareWeight.FindMinDL = Scenario.Node[TestTargetX, TestTargetY].Weight;
                CompareWeight.MinDangerousLevelX = TestTargetX;
                CompareWeight.MinDangerousLevelY = TestTargetY;
            }
            if (Scenario.Node[OriginX, OriginY].Weight > Scenario.Node[TestTargetX, TestTargetY].Weight)
            {
                for (int m = 0; m < 4; m++)
                {
                    if (CompareWeight.Compare[m] == -1)//|| Compare[m] > Scenario.Node[k - 2, l].Weight)
                    {
                        CompareWeight.Compare[m] = Scenario.Node[TestTargetX, TestTargetY].Weight;
                        CompareWeight.MinTargetX[m] = TestTargetX;
                        CompareWeight.MinTargetY[m] = TestTargetY;
                        CompareWeight.HeadingDirection[m] = Direction;
                        break;
                    }
                }
            }
            return CompareWeight;
        }

        public class WeightSortAC : IComparer<CompareDLList>
        {
            // 遞增排序
            public int Compare(CompareDLList x, CompareDLList y)
            {
                return (x.Weight.CompareTo(y.Weight));
            }
        }

        public class DistanceSortAC : IComparer<ExitInfo>
        {
            // 遞增排序
            public int Compare(ExitInfo x, ExitInfo y)
            {
                return (x.ActualDistance.CompareTo(y.ActualDistance));
            }
        }

        public class PositionSortAC : IComparer<ExitInfo>
        {
            // 遞增排序
            public int Compare(ExitInfo x, ExitInfo y)
            {
                if (x.X == y.X)
                    return (x.Y.CompareTo(y.Y));
                else
                    return (x.X.CompareTo(y.X));

                //return (x.ActualDistance.CompareTo(y.ActualDistance));
            }
        }
    }
}
