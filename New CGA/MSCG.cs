using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scenario = CAES.ScenarioNode.Scenario;
using PathRecord = CAES.ScenarioNode.PathRecord;
using CompareDL = CAES.ScenarioNode.CompareDL;
using CompareDLList = CAES.ScenarioNode.CompareDLList;

namespace CAES
{
    class MSCG
    {
        public static Scenario MSCGCalculation(
            Scenario Scenario)
        {
            for (int i = 0; i < Scenario.ExitSensor.Count(); i++)
                Scenario = FindNeighborNFindMinHop(Scenario, i);

            int MaxHop = 0;

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if ((Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Exit == false)
                        || (Scenario.Node[i, j].Corridor))
                        if (Scenario.Node[i, j].Activited)
                            for (int k = 0; k < Scenario.ExitSensor.Count(); k++)
                            {
                                if (Scenario.Node[i, j].Hop == -1)
                                {
                                    Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[k].Hop;
                                    Scenario.Node[i, j].PeopleAheadDirection
                                        = Scenario.Node[i, j].DistanceToExits[k].AheadDirection;
                                }
                                else if (Scenario.Node[i, j].Hop > Scenario.Node[i, j].DistanceToExits[k].Hop)
                                {
                                    Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[k].Hop;
                                    Scenario.Node[i, j].PeopleAheadDirection
                                        = Scenario.Node[i, j].DistanceToExits[k].AheadDirection;
                                }
                            }

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (MaxHop < Scenario.Node[i, j].Hop)
                        MaxHop = Scenario.Node[i, j].Hop;

            Scenario = FindBasicParameters(Scenario);

            Scenario = FindDeadLock(Scenario, MaxHop);

            return Scenario;
        }

        public static bool CheckIfCountedAllHop(Scenario Scenario, int ExitIndex)
        {

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (Scenario.Node[i, j].Corridor == true || Scenario.Node[i, j].Sensor == true)
                        if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == -1 
                            && Scenario.Node[i, j].Activited)
                            return false;
                }
            return true;
        }

        public static Scenario FindNeighborNFindMinHop(Scenario Scenario, int ExitIndex)
        {
            int HopCount = 0;

            while (!CheckIfCountedAllHop(Scenario, ExitIndex))
            {
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                    for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    {
                        if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == HopCount)
                        {
                            if (Scenario.Node[i, j].Sensor && Scenario.Node[i, j].Activited)
                            {
                                if (Scenario.Node[i, j].LeftSensorX != -1
                                    && Scenario.Node[i, j].LeftSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].LeftSensorX
                                        , Scenario.Node[i, j].LeftSensorY].Activited == true)
                                        if (Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].Hop == -1
                                            || Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].ActualDistance
                                                = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 1;
                                            Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].AheadDirection = "Right";
                                        }
                                if (Scenario.Node[i, j].RightSensorX != -1
                                    && Scenario.Node[i, j].RightSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].RightSensorX
                                        , Scenario.Node[i, j].RightSensorY].Activited == true)
                                        if (Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].Hop == -1
                                            || Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].ActualDistance
                                                = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 1;
                                            Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].AheadDirection = "Left";
                                        }
                                if (Scenario.Node[i, j].TopSensorX != -1
                                    && Scenario.Node[i, j].TopSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].TopSensorX
                                        , Scenario.Node[i, j].TopSensorY].Activited == true)
                                        if (Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].Hop == -1
                                            || Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].ActualDistance
                                                = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 1;
                                            Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].AheadDirection = "Down";
                                        }
                                if (Scenario.Node[i, j].BottemSensorX != -1
                                    && Scenario.Node[i, j].BottemSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].BottemSensorX
                                        , Scenario.Node[i, j].BottemSensorY].Activited == true)
                                        if (Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].Hop == -1
                                            || Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].ActualDistance
                                                = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 1;
                                            Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].AheadDirection = "Up";
                                        }
                            }
                            else if (Scenario.Node[i, j].Corridor && Scenario.Node[i, j].Activited)
                            {
                                switch (Scenario.Node[i, j].DistanceToExits[ExitIndex].AheadDirection)
                                {
                                    case "Right":
                                        if (Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].Hop == -1
                                            || Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].ActualDistance
                                                = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 1;
                                            Scenario.Node[i - 1, j].DistanceToExits[ExitIndex].AheadDirection = "Right";
                                        }
                                        break;
                                    case "Left":
                                        if (Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].Hop == -1
                                            || Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].ActualDistance
                                                = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 1;
                                            Scenario.Node[i + 1, j].DistanceToExits[ExitIndex].AheadDirection = "Left";
                                        }
                                        break;
                                    case "Down":
                                        if (Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].Hop == -1
                                            || Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].ActualDistance
                                                = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 1;
                                            Scenario.Node[i, j - 1].DistanceToExits[ExitIndex].AheadDirection = "Down";
                                        }
                                        break;
                                    case "Up":
                                        if (Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].Hop == -1
                                            || Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].ActualDistance
                                                = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 1;
                                            Scenario.Node[i, j + 1].DistanceToExits[ExitIndex].AheadDirection = "Up";
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                HopCount++;
            }
            return Scenario;
        }

        public static Scenario FindBasicParameters(
            Scenario Scenario)
        {
            double CapacityOfNode = 0;

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].VictimsAtSensor.Count() > CapacityOfNode)
                        CapacityOfNode = Scenario.Node[i, j].VictimsAtSensor.Count();

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    Scenario.Node[i, j].CrowdRatio
                        = Scenario.Node[i, j].VictimsAtSensor.Count() / CapacityOfNode * 100;

            Scenario = FindFEII(Scenario);

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    double MaxValue = Scenario.Node[i, j].CrowdRatio;
                    double MinValue;

                    if (Scenario.Node[i, j].FEII > MaxValue)
                    {
                        MinValue = MaxValue;
                        MaxValue = Scenario.Node[i, j].FEII;
                    }
                    else
                        MinValue = Scenario.Node[i, j].FEII;

                    if (MaxValue >= 90)
                        Scenario.Node[i, j].Weight = MaxValue;
                    else
                        Scenario.Node[i, j].Weight = MaxValue * 0.8 + MinValue * 0.2;

                    if (Scenario.Node[i, j].Exit == true)
                        Scenario.Node[i, j].Weight = -5;
                }


            return Scenario;
        }

        public static Scenario FindFEII(
            Scenario Scenario)
        {

            for (int k = 0; k < Scenario.ExitSensor.Count(); k++)
                Scenario.Node[Scenario.ExitSensor[k].X, Scenario.ExitSensor[k].Y].FEII = -5;

            for (int k = 0; k < Scenario.FireInfo.FireSensor.Count(); k++)
            {
                int FireX = Scenario.FireInfo.FireSensor[k].X;
                int FireY = Scenario.FireInfo.FireSensor[k].Y;

                Scenario.Node[FireX, FireY].FEII = 110;

                if (Scenario.Node[FireX, FireY].LeftSensorX != -1
                    && Scenario.Node[FireX, FireY].LeftSensorY != -1)
                    if (Scenario.Node[Scenario.Node[FireX, FireY].LeftSensorX
                        , Scenario.Node[FireX, FireY].LeftSensorY].Activited == true)
                    {
                        if (Scenario.Node[FireX - 1, FireY].FEII < 95)
                            Scenario.Node[FireX - 1, FireY].FEII = 95;

                        if (Scenario.Node[FireX - 2, FireY].FEII < 85)
                            Scenario.Node[FireX - 2, FireY].FEII = 85;

                        if (Scenario.Node[FireX - 3, FireY].Exit == false)
                            if (Scenario.Node[FireX - 3, FireY].FEII < 75)
                                Scenario.Node[FireX - 3, FireY].FEII = 75;
                    }
                if (Scenario.Node[FireX, FireY].RightSensorX != -1
                    && Scenario.Node[FireX, FireY].RightSensorY != -1)
                    if (Scenario.Node[Scenario.Node[FireX, FireY].RightSensorX
                        , Scenario.Node[FireX, FireY].RightSensorY].Activited == true)
                    {
                        if (Scenario.Node[FireX + 1, FireY].FEII < 95)
                            Scenario.Node[FireX + 1, FireY].FEII = 95;

                        if (Scenario.Node[FireX + 2, FireY].FEII < 85)
                            Scenario.Node[FireX + 2, FireY].FEII = 85;

                        if (Scenario.Node[FireX + 3, FireY].Exit == false)
                            if (Scenario.Node[FireX + 3, FireY].FEII < 75)
                                Scenario.Node[FireX + 3, FireY].FEII = 75;
                    }
                if (Scenario.Node[FireX, FireY].TopSensorX != -1
                    && Scenario.Node[FireX, FireY].TopSensorY != -1)
                    if (Scenario.Node[Scenario.Node[FireX, FireY].TopSensorX
                        , Scenario.Node[FireX, FireY].TopSensorY].Activited == true)
                    {
                        if (Scenario.Node[FireX, FireY - 1].FEII < 95)
                            Scenario.Node[FireX, FireY - 1].FEII = 95;

                        if (Scenario.Node[FireX, FireY - 2].FEII < 85)
                            Scenario.Node[FireX, FireY - 2].FEII = 85;

                        if (Scenario.Node[FireX, FireY - 2].LeftSensorX != -1
                            && Scenario.Node[FireX, FireY - 2].LeftSensorY != -1)
                            if (Scenario.Node[Scenario.Node[FireX, FireY - 2].LeftSensorX
                                , Scenario.Node[FireX, FireY - 2].LeftSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[FireX, FireY - 2].LeftSensorX
                                    , Scenario.Node[FireX, FireY - 2].LeftSensorY].Exit == false)
                                    if (Scenario.Node[FireX - 1, FireY - 2].FEII < 75)
                                        Scenario.Node[FireX - 1, FireY - 2].FEII = 75;

                        if (Scenario.Node[FireX, FireY - 2].RightSensorX != -1
                            && Scenario.Node[FireX, FireY - 2].RightSensorY != -1)
                            if (Scenario.Node[Scenario.Node[FireX, FireY - 2].RightSensorX
                                , Scenario.Node[FireX, FireY - 2].RightSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[FireX, FireY - 2].RightSensorX
                                    , Scenario.Node[FireX, FireY - 2].RightSensorY].Exit == false)
                                    if (Scenario.Node[FireX + 1, FireY - 2].FEII < 75)
                                        Scenario.Node[FireX + 1, FireY - 2].FEII = 75;

                        if (Scenario.Node[FireX, FireY - 2].TopSensorX != -1
                            && Scenario.Node[FireX, FireY - 2].TopSensorY != -1)
                            if (Scenario.Node[Scenario.Node[FireX, FireY - 2].TopSensorX
                                , Scenario.Node[FireX, FireY - 2].TopSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[FireX, FireY - 2].TopSensorX
                                    , Scenario.Node[FireX, FireY - 2].TopSensorY].Exit == false)
                                    if (Scenario.Node[FireX, FireY - 3].FEII < 75)
                                        Scenario.Node[FireX, FireY - 3].FEII = 75;
                    }
                if (Scenario.Node[FireX, FireY].BottemSensorX != -1
                    && Scenario.Node[FireX, FireY].BottemSensorY != -1)
                    if (Scenario.Node[Scenario.Node[FireX, FireY].BottemSensorX
                        , Scenario.Node[FireX, FireY].BottemSensorY].Activited == true)
                    {
                        if (Scenario.Node[FireX, FireY + 1].FEII < 95)
                            Scenario.Node[FireX, FireY + 1].FEII = 95;

                        if (Scenario.Node[FireX, FireY + 2].FEII < 85)
                            Scenario.Node[FireX, FireY + 2].FEII = 85;

                        if (Scenario.Node[FireX, FireY + 2].LeftSensorX != -1
                            && Scenario.Node[FireX, FireY + 2].LeftSensorY != -1)
                            if (Scenario.Node[Scenario.Node[FireX, FireY + 2].LeftSensorX
                                , Scenario.Node[FireX, FireY + 2].LeftSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[FireX, FireY + 2].LeftSensorX
                                    , Scenario.Node[FireX, FireY + 2].LeftSensorY].Exit == false)
                                    if (Scenario.Node[FireX - 1, FireY + 2].FEII < 75)
                                        Scenario.Node[FireX - 1, FireY + 2].FEII = 75;

                        if (Scenario.Node[FireX, FireY + 2].RightSensorX != -1
                            && Scenario.Node[FireX, FireY + 2].RightSensorY != -1)
                            if (Scenario.Node[Scenario.Node[FireX, FireY + 2].RightSensorX
                                , Scenario.Node[FireX, FireY + 2].RightSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[FireX, FireY + 2].RightSensorX
                                    , Scenario.Node[FireX, FireY + 2].RightSensorY].Exit == false)
                                    if (Scenario.Node[FireX + 1, FireY + 2].FEII < 75)
                                        Scenario.Node[FireX + 1, FireY + 2].FEII = 75;

                        if (Scenario.Node[FireX, FireY + 2].BottemSensorX != -1
                            && Scenario.Node[FireX, FireY + 2].BottemSensorY != -1)
                            if (Scenario.Node[Scenario.Node[FireX, FireY + 2].BottemSensorX
                                , Scenario.Node[FireX, FireY + 2].BottemSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[FireX, FireY + 2].BottemSensorX
                                    , Scenario.Node[FireX, FireY + 2].BottemSensorY].Exit == false)
                                    if (Scenario.Node[FireX, FireY + 3].FEII < 75)
                                        Scenario.Node[FireX, FireY + 3].FEII = 75;
                    }
            }

            return Scenario;
        }

        public static Scenario FindDeadLock(
            Scenario Scenario, int MaxHop)
        {
            while (CheckDeadLock(Scenario, MaxHop))
            {
                for (int k = MaxHop; k >= 0; k--)
                {
                    for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                        for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                            if (Scenario.Node[i, j].Hop == k)
                            {
                                if (Scenario.Node[i, j].Sensor && Scenario.Node[i, j].Activited)
                                {
                                    int CountDeadLock = 0;
                                    if (Scenario.Node[i, j].LeftSensorX != -1
                                        && Scenario.Node[i, j].LeftSensorY != -1)
                                        if (Scenario.Node[Scenario.Node[i, j].LeftSensorX
                                            , Scenario.Node[i, j].LeftSensorY].Activited == true)
                                            if (Scenario.Node[i - 1, j].Weight < Scenario.Node[i, j].Weight)
                                                CountDeadLock++;
                                    if (Scenario.Node[i, j].RightSensorX != -1
                                        && Scenario.Node[i, j].RightSensorY != -1)
                                        if (Scenario.Node[Scenario.Node[i, j].RightSensorX
                                            , Scenario.Node[i, j].RightSensorY].Activited == true)
                                            if (Scenario.Node[i + 1, j].Weight < Scenario.Node[i, j].Weight)
                                                CountDeadLock++;
                                    if (Scenario.Node[i, j].TopSensorX != -1
                                        && Scenario.Node[i, j].TopSensorY != -1)
                                        if (Scenario.Node[Scenario.Node[i, j].TopSensorX
                                            , Scenario.Node[i, j].TopSensorY].Activited == true)
                                            if (Scenario.Node[i, j - 1].Weight < Scenario.Node[i, j].Weight)
                                                CountDeadLock++;
                                    if (Scenario.Node[i, j].BottemSensorX != -1
                                        && Scenario.Node[i, j].BottemSensorY != -1)
                                        if (Scenario.Node[Scenario.Node[i, j].BottemSensorX
                                            , Scenario.Node[i, j].BottemSensorY].Activited == true)
                                            if (Scenario.Node[i, j + 1].Weight < Scenario.Node[i, j].Weight)
                                                CountDeadLock++;
                                    if (CountDeadLock == 0)
                                        Scenario.Node[i, j].Weight = Scenario.Node[i, j].Weight + 5;
                                }
                                else if (Scenario.Node[i, j].Corridor == true)
                                {
                                    int CountDeadLock = 0;
                                    if (i != 0)
                                        if (Scenario.Node[i - 1, j].Corridor == true || Scenario.Node[i - 1, j].Sensor == true)
                                            if (Scenario.Node[i - 1, j].Weight < Scenario.Node[i, j].Weight)
                                                CountDeadLock++;
                                    if (i != Scenario.Node.GetLength(0) - 1)
                                        if (Scenario.Node[i + 1, j].Corridor == true || Scenario.Node[i + 1, j].Sensor == true)
                                            if (Scenario.Node[i + 1, j].Weight < Scenario.Node[i, j].Weight)
                                                CountDeadLock++;
                                    if (j != 0)
                                        if (Scenario.Node[i, j - 1].Corridor == true || Scenario.Node[i, j - 1].Sensor == true)
                                            if (Scenario.Node[i, j - 1].Weight < Scenario.Node[i, j].Weight)
                                                CountDeadLock++;
                                    if (j != Scenario.Node.GetLength(1) - 1)
                                        if (Scenario.Node[i, j + 1].Corridor == true || Scenario.Node[i, j + 1].Sensor == true)
                                            if (Scenario.Node[i, j + 1].Weight < Scenario.Node[i, j].Weight)
                                                CountDeadLock++;
                                    if (CountDeadLock == 0)
                                        Scenario.Node[i, j].Weight = Scenario.Node[i, j].Weight + 5;
                                }
                            }
                }
            }
            return Scenario;
        }

        public static bool CheckDeadLock(
            Scenario Scenario, int MaxHop)
        {
            for (int k = MaxHop; k > 0; k--)
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                    for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                        if (Scenario.Node[i, j].Hop == k)
                        {
                            if (Scenario.Node[i, j].Sensor == true)
                            {
                                int CountDeadLock = 0;
                                if (Scenario.Node[i, j].LeftSensorX != -1
                                    && Scenario.Node[i, j].LeftSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].LeftSensorX
                                        , Scenario.Node[i, j].LeftSensorY].Activited == true)
                                        if (Scenario.Node[i - 1, j].Weight < Scenario.Node[i, j].Weight)
                                            CountDeadLock++;
                                if (Scenario.Node[i, j].RightSensorX != -1
                                    && Scenario.Node[i, j].RightSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].RightSensorX
                                        , Scenario.Node[i, j].RightSensorY].Activited == true)
                                        if (Scenario.Node[i + 1, j].Weight < Scenario.Node[i, j].Weight)
                                            CountDeadLock++;
                                if (Scenario.Node[i, j].TopSensorX != -1
                                    && Scenario.Node[i, j].TopSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].TopSensorX
                                        , Scenario.Node[i, j].TopSensorY].Activited == true)
                                        if (Scenario.Node[i, j - 1].Weight < Scenario.Node[i, j].Weight)
                                            CountDeadLock++;
                                if (Scenario.Node[i, j].BottemSensorX != -1
                                    && Scenario.Node[i, j].BottemSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].BottemSensorX
                                        , Scenario.Node[i, j].BottemSensorY].Activited == true)
                                        if (Scenario.Node[i, j + 1].Weight < Scenario.Node[i, j].Weight)
                                            CountDeadLock++;
                                if (CountDeadLock == 0)
                                    return true;
                            }
                            else if (Scenario.Node[i, j].Corridor == true)
                            {
                                int CountDeadLock = 0;
                                if (i != 0)
                                    if (Scenario.Node[i - 1, j].Corridor == true || Scenario.Node[i - 1, j].Sensor == true)
                                        if (Scenario.Node[i - 1, j].Weight < Scenario.Node[i, j].Weight)
                                            CountDeadLock++;
                                if (i != Scenario.Node.GetLength(0) - 1)
                                    if (Scenario.Node[i + 1, j].Corridor == true || Scenario.Node[i + 1, j].Sensor == true)
                                        if (Scenario.Node[i + 1, j].Weight < Scenario.Node[i, j].Weight)
                                            CountDeadLock++;
                                if (j != 0)
                                    if (Scenario.Node[i, j - 1].Corridor == true || Scenario.Node[i, j - 1].Sensor == true)
                                        if (Scenario.Node[i, j - 1].Weight < Scenario.Node[i, j].Weight)
                                            CountDeadLock++;
                                if (j != Scenario.Node.GetLength(1) - 1)
                                    if (Scenario.Node[i, j + 1].Corridor == true || Scenario.Node[i, j + 1].Sensor == true)
                                        if (Scenario.Node[i, j + 1].Weight < Scenario.Node[i, j].Weight)
                                            CountDeadLock++;
                                if (CountDeadLock == 0)
                                    return true;
                            }
                        }
            return false;
        }

        public static Scenario MSCGSimulation(Scenario Scenario)
        {
            int MaxHop = 0;

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Hop > MaxHop && Scenario.Node[i, j].Activited)
                        MaxHop = Scenario.Node[i, j].Hop;

            for (int k = 1; k <= MaxHop; k++)
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                    for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                        if (Scenario.Node[i, j].Hop == k && Scenario.Node[i, j].Activited)
                            if (Scenario.Node[i, j].VictimsAtSensor.Count() > 0)
                            {
                                int CanMovePeople = 0;

                                for (int CheckIfVictimsCanMove = 0
                                    ; CheckIfVictimsCanMove < Scenario.Node[i, j].VictimsAtSensor.Count
                                    ; CheckIfVictimsCanMove++)
                                    if (Scenario.Node[i, j].VictimsAtSensor[CheckIfVictimsCanMove].Activated == true)
                                        if (Scenario.Node[i, j].VictimsAtSensor[CheckIfVictimsCanMove].HaveMovedThisRound == false)
                                            CanMovePeople++;

                                if (CanMovePeople > 0)
                                    if (Scenario.Node[i, j].Sensor == true)
                                    {

                                        CompareDL CompareWeightList = new CompareDL();
                                        CompareWeightList.DLList = new List<CompareDLList>();
                                        CompareWeightList.FindMinDL = -1;
                                        CompareWeightList.FindMaxDL = -1;
                                        CompareWeightList.MinDangerousLevelX = -1;
                                        CompareWeightList.MinDangerousLevelY = -1;

                                        if (j != 0)
                                            if (Scenario.Node[i, j - 1].Activited == true)
                                                CompareWeightList.DLList.Add(new CompareDLList(i, j - 1, "Up", Scenario.Node[i, j - 2].Weight));

                                        if (j != Scenario.Node.GetLength(1) - 1)
                                            if (Scenario.Node[i, j + 1].Activited == true)
                                                CompareWeightList.DLList.Add(new CompareDLList(i, j + 1, "Down", Scenario.Node[i, j + 2].Weight));

                                        if (i != 0)
                                            if (Scenario.Node[i - 1, j].Activited == true)
                                                CompareWeightList.DLList.Add(new CompareDLList(i - 1, j, "Left", Scenario.Node[i - 3, j].Weight));

                                        if (i != Scenario.Node.GetLength(0) - 1)
                                            if (Scenario.Node[i + 1, j].Activited == true)
                                                CompareWeightList.DLList.Add(new CompareDLList(i + 1, j, "Right", Scenario.Node[i + 3, j].Weight));

                                        CompareWeightList.DLList.Sort(new WeightSortAC());

                                        if (Scenario.Node[i, j].FireSensor != true)
                                            for (int ListCurPos = 0; ListCurPos < CompareWeightList.DLList.Count; ListCurPos++)
                                            {
                                                if (CompareWeightList.DLList[ListCurPos].Weight > Scenario.Node[i, j].Weight)
                                                {
                                                    CompareWeightList.DLList.RemoveRange(ListCurPos, CompareWeightList.DLList.Count - ListCurPos);
                                                    if (ListCurPos == 0)
                                                        CompareWeightList.DLList = null;
                                                    break;
                                                }
                                            }

                                        int MovingPeople = 0;
                                        if (CompareWeightList.DLList != null)
                                            for (int ListCurPos = 0; ListCurPos < CompareWeightList.DLList.Count; ListCurPos++)
                                            {
                                                /// <code>
                                                ///  Check if any victims left
                                                /// </code>
                                                if (CanMovePeople == 0)
                                                    break;
                                                /// 
                                                /// <code>
                                                ///  Decide how many people to move Start
                                                /// </code>
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
                                            }
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

            // Set all haverun parameter to false
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    for (int k = 0; k < Scenario.Node[i, j].VictimsAtSensor.Count(); k++)
                        Scenario.Node[i, j].VictimsAtSensor[k].HaveMovedThisRound = false;

                    for (int k = 0; k < Scenario.WholeVictims.Count(); k++)
                        Scenario.WholeVictims[k].HaveMovedThisRound = false;

                    Scenario.Node[i, j].HaveRunThisRound = false;

                    // Set all victims not at exit to not escaped
                    if (Scenario.Node[i, j].Exit == false)
                        for (int k = 0; k < Scenario.Node[i, j].VictimsAtSensor.Count; k++)
                        {
                            Scenario.Node[i, j].VictimsAtSensor[k].HaveEscaped = false;
                            Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[k].SN].HaveEscaped = false;
                        }

                }

            return Scenario;
        }

        public class WeightSortAC : IComparer<CompareDLList>
        {
            // 遞增排序
            public int Compare(CompareDLList x, CompareDLList y)
            {
                return (x.Weight.CompareTo(y.Weight));
            }
        }
    }
}