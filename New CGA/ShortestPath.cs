using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scenario = CAES.ScenarioNode.Scenario;
using PathRecord = CAES.ScenarioNode.PathRecord;

namespace CAES
{
    class ShortestPath
    {
        public static Scenario CalculateShortestPath(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.ExitSensor.Count(); i++)
            {
                Scenario = FindNeighborNFindMinHop(Scenario, i);
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

        public static Scenario SPSimulation(Scenario Scenario)
        {

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].VictimsAtSensor.Count() > 0 && Scenario.Node[i, j].Activited)
                    {
                        int CanMovePeople = 0;
                        for (int CheckIfVictimsCanMove = 0
                            ; CheckIfVictimsCanMove < Scenario.Node[i, j].VictimsAtSensor.Count
                            ; CheckIfVictimsCanMove++)
                            if (Scenario.Node[i, j].VictimsAtSensor[CheckIfVictimsCanMove].Activated == true)
                                if (Scenario.Node[i, j].VictimsAtSensor[CheckIfVictimsCanMove].HaveMovedThisRound == false)
                                    CanMovePeople++;

                        if (CanMovePeople > 0)
                            if ((Scenario.Node[i, j].Sensor == true) && (Scenario.Node[i, j].Exit == false))
                            {
                                int MovingPeople = 0;


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

                                int TargetX = i, TargetY = j;

                                switch (Scenario.Node[i, j].PeopleAheadDirection)
                                {
                                    case "Right":
                                        TargetX++;
                                        break;
                                    case "Left":
                                        TargetX--;
                                        break;
                                    case "Up":
                                        TargetY--;
                                        break;
                                    case "Down":
                                        TargetY++;
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
                                        if (Scenario.Node[i, j].VictimsAtSensor[MovingPos].Activated == true)
                                            if (Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound == false)
                                            {
                                                Scenario.Node[i, j].VictimsAtSensor[0].HaveMovedThisRound = true;
                                                Scenario.Node[i, j].VictimsAtSensor[0].Path.Add(
                                                    new PathRecord(
                                                        TargetX
                                                        , TargetY
                                                        , Scenario.TotalEvacuationRound
                                                        , false));
                                                Scenario.Node[i, j].VictimsAtSensor[0].X = TargetX;
                                                Scenario.Node[i, j].VictimsAtSensor[0].Y = TargetY;
                                                Scenario.Node[i, j].VictimsAtSensor[0].MovingDirection = Scenario.Node[i, j].PeopleAheadDirection;
                                                Scenario.Node[i, j].VictimsAtSensor[0].LastMovedRound = Scenario.TotalEvacuationRound;

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
                                                ActualMovedVictims++;
                                                MovingPos--;
                                            }
                                    MovingPos++;
                                }

                                Scenario.Node[TargetX, TargetY].PeopleAheadDirection = Scenario.Node[i, j].PeopleAheadDirection;

                            }
                            else if (Scenario.Node[i, j].Corridor == true)
                            {
                                for (int CorridorMoveCount = 0; CorridorMoveCount < Scenario.Node[i, j].VictimsAtSensor.Count(); CorridorMoveCount++)
                                    if (Scenario.Node[i, j].VictimsAtSensor[CorridorMoveCount].Activated == true)
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

        public static Scenario FindNeighborNFindMinHop(Scenario Scenario, int ExitIndex)
        {
            int HopCount = 0;

            while (!CheckIfCountedAllHop(Scenario, ExitIndex))
            {
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                    for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    {
                        if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Activited)
                            if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == HopCount)
                            {
                                if (Scenario.Node[i, j].LeftSensorX != -1 && Scenario.Node[i, j].LeftSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].Activited == true)
                                        if (Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].Hop == -1 || Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].ActualDistance = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 3;
                                            Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].EuclideanDistance = Math.Sqrt(Math.Pow(Scenario.Node[i, j].LeftSensorX - Scenario.Node[i, j].DistanceToExits[ExitIndex].X, 2) + Math.Pow(Scenario.Node[i, j].LeftSensorY - Scenario.Node[i, j].DistanceToExits[ExitIndex].Y, 2));
                                            Scenario.Node[Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY].DistanceToExits[ExitIndex].AheadDirection = "Right";
                                        }
                                if (Scenario.Node[i, j].RightSensorX != -1 && Scenario.Node[i, j].RightSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].Activited == true)
                                        if (Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].Hop == -1 || Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].ActualDistance = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 3;
                                            Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].EuclideanDistance = Math.Sqrt(Math.Pow(Scenario.Node[i, j].RightSensorX - Scenario.Node[i, j].DistanceToExits[ExitIndex].X, 2) + Math.Pow(Scenario.Node[i, j].RightSensorY - Scenario.Node[i, j].DistanceToExits[ExitIndex].Y, 2));
                                            Scenario.Node[Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY].DistanceToExits[ExitIndex].AheadDirection = "Left";
                                        }
                                if (Scenario.Node[i, j].TopSensorX != -1 && Scenario.Node[i, j].TopSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].Activited == true)
                                        if (Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].Hop == -1 || Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].Hop > HopCount)
                                        {
                                            Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].Hop = HopCount + 1;
                                            Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].ActualDistance = Scenario.Node[i, j].DistanceToExits[ExitIndex].ActualDistance + 2;
                                            Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].EuclideanDistance = Math.Sqrt(Math.Pow(Scenario.Node[i, j].TopSensorX - Scenario.Node[i, j].DistanceToExits[ExitIndex].X, 2) + Math.Pow(Scenario.Node[i, j].TopSensorY - Scenario.Node[i, j].DistanceToExits[ExitIndex].Y, 2));
                                            Scenario.Node[Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY].DistanceToExits[ExitIndex].AheadDirection = "Down";
                                        }
                                if (Scenario.Node[i, j].BottemSensorX != -1 && Scenario.Node[i, j].BottemSensorY != -1)
                                    if (Scenario.Node[Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY].Activited == true)
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


        public static bool CheckIfCountedAllHop(Scenario Scenario, int ExitIndex)
        {

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Activited)
                        if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == -1)
                            return false;
                }
            return true;
        }
    }
}
