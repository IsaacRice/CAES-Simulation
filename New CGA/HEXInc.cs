using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scenario = CAES.ScenarioNode.Scenario;

namespace CAES
{
    class HEXInc
    {
        public static Scenario HEXIncCalculation(
            Scenario Scenario)
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
                                Scenario.Node[i, j].TargetX = Scenario.Node[i, j].DistanceToExits[k].X;
                                Scenario.Node[i, j].TargetY = Scenario.Node[i, j].DistanceToExits[k].Y;
                                Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[k].Hop;
                                Scenario.Node[i, j].PeopleAheadDirection = Scenario.Node[i, j].DistanceToExits[k].AheadDirection;
                            }
                            else if (Scenario.Node[i, j].Hop > Scenario.Node[i, j].DistanceToExits[k].Hop
                                && Scenario.Node[i, j].DistanceToExits[k].Hop != -1)
                            {
                                Scenario.Node[i, j].TargetX = Scenario.Node[i, j].DistanceToExits[k].X;
                                Scenario.Node[i, j].TargetY = Scenario.Node[i, j].DistanceToExits[k].Y;
                                Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[k].Hop;
                                Scenario.Node[i, j].PeopleAheadDirection = Scenario.Node[i, j].DistanceToExits[k].AheadDirection;
                            }
                        }
                    else if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Exit == true)
                    {
                        Scenario.Node[i, j].TargetX = i;
                        Scenario.Node[i, j].TargetY = j;
                    }

            Scenario = FireSensorDirection(Scenario);

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
                        if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i,j].Activited)
                            if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == HopCount)
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
                    if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Activited)
                        if (Scenario.Node[i, j].FireSensor == false)
                            if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == -1)
                                return false;
                }
            return true;
        }

        public static Scenario FireSensorDirection(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].FireSensor == true && Scenario.Node[i, j].Activited)
                    {
                        if (Scenario.Node[i, j].LeftSensorX != -1 && Scenario.Node[i, j].LeftSensorY != -1)
                            if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].FireSensor == false)
                                    if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].Exit == true)
                                    {
                                        Scenario.Node[i, j].TargetX
                                            = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].TargetX;
                                        Scenario.Node[i, j].TargetY
                                            = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].TargetY;
                                        Scenario.Node[i, j].Hop = 1;
                                        Scenario.Node[i, j].PeopleAheadDirection = "Left";
                                    }
                                    else if (Scenario.Node[i, j].Hop == -1 ||
                                        Scenario.Node[i, j].Hop
                                        > Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].Hop + 1)
                                    {
                                        Scenario.Node[i, j].TargetX
                                            = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].TargetX;
                                        Scenario.Node[i, j].TargetY
                                            = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].TargetY;
                                        Scenario.Node[i, j].Hop = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].Hop + 1;
                                        Scenario.Node[i, j].PeopleAheadDirection = "Left";
                                    }
                        if (Scenario.Node[i, j].RightSensorX != -1 && Scenario.Node[i, j].RightSensorY != -1)
                            if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].FireSensor == false)
                                    if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].Exit == true)
                                    {
                                        Scenario.Node[i, j].TargetX
                                            = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].TargetX;
                                        Scenario.Node[i, j].TargetY
                                            = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].TargetY;
                                        Scenario.Node[i, j].Hop = 1;
                                        Scenario.Node[i, j].PeopleAheadDirection = "Right";
                                    }
                                    else if (Scenario.Node[i, j].Hop == -1 ||
                                        Scenario.Node[i, j].Hop
                                        > Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].Hop + 1)
                                    {
                                        Scenario.Node[i, j].TargetX
                                            = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].TargetX;
                                        Scenario.Node[i, j].TargetY
                                            = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].TargetY;
                                        Scenario.Node[i, j].Hop = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].Hop + 1;
                                        Scenario.Node[i, j].PeopleAheadDirection = "Right";
                                    }
                        if (Scenario.Node[i, j].TopSensorX != -1 && Scenario.Node[i, j].TopSensorY != -1)
                            if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].FireSensor == false)
                                    if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].Exit == true)
                                    {
                                        Scenario.Node[i, j].TargetX
                                            = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].TargetX;
                                        Scenario.Node[i, j].TargetY
                                            = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].TargetY;
                                        Scenario.Node[i, j].Hop = 1;
                                        Scenario.Node[i, j].PeopleAheadDirection = "Up";
                                    }
                                    else if (Scenario.Node[i, j].Hop == -1 ||
                                        Scenario.Node[i, j].Hop
                                        > Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].Hop + 1)
                                    {
                                        Scenario.Node[i, j].TargetX
                                            = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].TargetX;
                                        Scenario.Node[i, j].TargetY
                                            = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].TargetY;
                                        Scenario.Node[i, j].Hop = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].Hop + 1;
                                        Scenario.Node[i, j].PeopleAheadDirection = "Up";
                                    }
                        if (Scenario.Node[i, j].BottemSensorX != -1 && Scenario.Node[i, j].BottemSensorY != -1)
                            if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].Activited == true)
                                if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].FireSensor == false)
                                    if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].Exit == true)
                                    {
                                        Scenario.Node[i, j].TargetX
                                            = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].TargetX;
                                        Scenario.Node[i, j].TargetY
                                            = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].TargetY;
                                        Scenario.Node[i, j].Hop = 1;
                                        Scenario.Node[i, j].PeopleAheadDirection = "Down";
                                    }
                                    else if (Scenario.Node[i, j].Hop == -1 ||
                                        Scenario.Node[i, j].Hop
                                        > Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].Hop + 1)
                                    {
                                        Scenario.Node[i, j].TargetX
                                            = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].TargetX;
                                        Scenario.Node[i, j].TargetY
                                            = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].TargetY;
                                        Scenario.Node[i, j].Hop = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].Hop + 1;
                                        Scenario.Node[i, j].PeopleAheadDirection = "Down";
                                    }

                        for (int k = 0; k < Scenario.ExitSensor.Count(); k++)
                        {
                            Scenario.Node[i, j].DistanceToExits[k].EuclideanDistance
                                = Math.Sqrt(Math.Pow(i - Scenario.Node[i, j].DistanceToExits[k].X, 2)
                                + Math.Pow(j - Scenario.Node[i, j].DistanceToExits[k].Y, 2));

                            if (Scenario.Node[i, j].LeftSensorX != -1 && Scenario.Node[i, j].LeftSensorY != -1)
                                if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].Activited == true)
                                    if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].FireSensor == false)
                                        if (Scenario.Node[i, j].DistanceToExits[k].X == Scenario.Node[i, j].LeftSensorX
                                            && Scenario.Node[i, j].DistanceToExits[k].Y == Scenario.Node[i, j].LeftSensorY)
                                        {
                                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance 
                                                = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY]
                                                .DistanceToExits[k].ActualDistance + 3;
                                            Scenario.Node[i, j].DistanceToExits[k].Hop
                                                = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY]
                                                .DistanceToExits[k].Hop + 1;
                                            Scenario.Node[i, j].DistanceToExits[k].AheadDirection = "Left";
                                        }
                                        else if (Scenario.Node[i, j].DistanceToExits[k].Hop == -1
                                            || Scenario.Node[i, j].Hop >
                                            Scenario.Node[Scenario.Node[i, j].LeftSensorX
                                            , Scenario.Node[i, j].LeftSensorY].DistanceToExits[k].Hop)
                                        {
                                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance
                                                = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY]
                                                .DistanceToExits[k].ActualDistance + 3;
                                            Scenario.Node[i, j].DistanceToExits[k].Hop
                                                = Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY]
                                                .DistanceToExits[k].Hop + 1;
                                            Scenario.Node[i, j].DistanceToExits[k].AheadDirection = "Left";
                                        }

                            if (Scenario.Node[i, j].RightSensorX != -1 && Scenario.Node[i, j].RightSensorY != -1)
                                if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].Activited == true)
                                    if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].FireSensor == false)
                                        if (Scenario.Node[i, j].DistanceToExits[k].X == Scenario.Node[i, j].RightSensorX
                                                && Scenario.Node[i, j].DistanceToExits[k].Y == Scenario.Node[i, j].RightSensorY)
                                        {
                                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance
                                                = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY]
                                                .DistanceToExits[k].ActualDistance + 3;
                                            Scenario.Node[i, j].DistanceToExits[k].Hop
                                                = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY]
                                                .DistanceToExits[k].Hop + 1;
                                            Scenario.Node[i, j].DistanceToExits[k].AheadDirection = "Right";
                                        }
                                        else if (Scenario.Node[i, j].DistanceToExits[k].Hop == -1
                                            || Scenario.Node[i, j].Hop >
                                            Scenario.Node[Scenario.Node[i, j].RightSensorX
                                            , Scenario.Node[i, j].RightSensorY].DistanceToExits[k].Hop)
                                        {
                                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance
                                                = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY]
                                                .DistanceToExits[k].ActualDistance + 3;
                                            Scenario.Node[i, j].DistanceToExits[k].Hop
                                                = Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY]
                                                .DistanceToExits[k].Hop + 1;
                                            Scenario.Node[i, j].DistanceToExits[k].AheadDirection = "Right";
                                        }

                            if (Scenario.Node[i, j].TopSensorX != -1 && Scenario.Node[i, j].TopSensorY != -1)
                                if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].Activited == true)
                                    if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].FireSensor == false)
                                        if (Scenario.Node[i, j].DistanceToExits[k].X == Scenario.Node[i, j].TopSensorX
                                                   && Scenario.Node[i, j].DistanceToExits[k].Y == Scenario.Node[i, j].TopSensorY)
                                        {
                                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance
                                                = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY]
                                                .DistanceToExits[k].ActualDistance + 2;
                                            Scenario.Node[i, j].DistanceToExits[k].Hop
                                                = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY]
                                                .DistanceToExits[k].Hop + 1;
                                            Scenario.Node[i, j].DistanceToExits[k].AheadDirection = "Up";
                                        }
                                        else if (Scenario.Node[i, j].DistanceToExits[k].Hop == -1
                                            || Scenario.Node[i, j].Hop >
                                            Scenario.Node[Scenario.Node[i, j].TopSensorX
                                            , Scenario.Node[i, j].TopSensorY].DistanceToExits[k].Hop)
                                        {
                                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance
                                                = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY]
                                                .DistanceToExits[k].ActualDistance + 2;
                                            Scenario.Node[i, j].DistanceToExits[k].Hop
                                                = Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY]
                                                .DistanceToExits[k].Hop + 1;
                                            Scenario.Node[i, j].DistanceToExits[k].AheadDirection = "Up";
                                        }

                            if (Scenario.Node[i, j].BottemSensorX != -1 && Scenario.Node[i, j].BottemSensorY != -1)
                                if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].Activited == true)
                                    if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].FireSensor == false)
                                        if (Scenario.Node[i, j].DistanceToExits[k].X == Scenario.Node[i, j].BottemSensorX
                                                   && Scenario.Node[i, j].DistanceToExits[k].Y == Scenario.Node[i, j].BottemSensorY)
                                        {
                                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance
                                                = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY]
                                                .DistanceToExits[k].ActualDistance + 2;
                                            Scenario.Node[i, j].DistanceToExits[k].Hop
                                                = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY]
                                                .DistanceToExits[k].Hop + 1;
                                            Scenario.Node[i, j].DistanceToExits[k].AheadDirection = "Down";
                                        }
                                        else if (Scenario.Node[i, j].DistanceToExits[k].Hop == -1
                                            || Scenario.Node[i, j].Hop >
                                            Scenario.Node[Scenario.Node[i, j].BottemSensorX
                                            , Scenario.Node[i, j].BottemSensorY].DistanceToExits[k].Hop)
                                        {
                                            Scenario.Node[i, j].DistanceToExits[k].ActualDistance
                                                = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY]
                                                .DistanceToExits[k].ActualDistance + 2;
                                            Scenario.Node[i, j].DistanceToExits[k].Hop
                                                = Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY]
                                                .DistanceToExits[k].Hop + 1;
                                            Scenario.Node[i, j].DistanceToExits[k].AheadDirection = "Down";
                                        }
                        }
                    }
            return Scenario;
        }
        public static Scenario HEXIncSimulation(
            Scenario Scenario)
        {
            return Scenario;
        }
    }
}
