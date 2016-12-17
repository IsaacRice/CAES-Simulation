using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scenario = CAES.ScenarioNode.Scenario;
using PathRecord = CAES.ScenarioNode.PathRecord;
using ExitInfo = CAES.ScenarioNode.ExitInfo;

namespace CAES
{
    class ESSPlus
    {
        public static Scenario ESSPlusCalculation(
            Scenario Scenario)
        {
            for (int i = 0; i < Scenario.ExitSensor.Count(); i++)
            {
                Scenario = HEXInc.FindNeighborNFindMinHopWithFire(Scenario, i);
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
                    else if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Exit == true && Scenario.Node[i, j].Activited)
                    {
                        Scenario.Node[i, j].TargetX = i;
                        Scenario.Node[i, j].TargetY = j;
                    }

            Scenario = HEXInc.FireSensorDirection(Scenario);

            int MaxHop = 0;

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (MaxHop < Scenario.Node[i, j].Hop)
                        MaxHop = Scenario.Node[i, j].Hop;



            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                {
                    Scenario.Node[i, j].DistanceToExits.Sort(new DistanceSortAC());

                    for (int k = 0; k < Scenario.Node[i, j].VictimsAtSensor.Count; k++)
                    {
                        Scenario.Node[i, j].VictimsAtSensor[k].TargetX
                            = Scenario.Node[i, j].DistanceToExits[0].X;
                        Scenario.Node[i, j].VictimsAtSensor[k].TargetY
                            = Scenario.Node[i, j].DistanceToExits[0].Y;
                    }

                    Scenario.Node[i, j].DistanceToExits.Sort(new PositionSortAC());
                }



            Scenario = ChoosePathNew(Scenario, MaxHop);

            /*
            MaxHop = 0;
            
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (MaxHop < Scenario.Node[i, j].Hop)
                        MaxHop = Scenario.Node[i, j].Hop;

            Scenario = ESSChangePath(Scenario, MaxHop);
            */

            return Scenario;
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

        public static Scenario ChoosePathNew(Scenario Scenario, int MaxHop)
        {
            for (int k = 0; k <= MaxHop; k++)
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                    for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                        if (Scenario.Node[i, j].Exit != true && Scenario.Node[i, j].Sensor == true
                            && Scenario.Node[i, j].Hop == k && Scenario.Node[i, j].Activited)
                        {
                            VectorCount[] FireVectors = new VectorCount[Scenario.FireInfo.FireSensor.Count()];
                            double FireVectorSumX = 0;
                            double FireVectorSumY = 0;

                            for (int l = 0; l < Scenario.FireInfo.FireSensor.Count(); l++)
                            {
                                FireVectors[l] = new VectorCount(i - Scenario.FireInfo.FireSensor[l].X, j - Scenario.FireInfo.FireSensor[l].Y);

                                FireVectorSumX += FireVectors[l].X;
                                FireVectorSumY += FireVectors[l].Y;
                                /*
                                FireVectorSumX += i - Scenario.FireInfo.FireSensor[l].X;
                                if (i - Scenario.FireInfo.FireSensor[l].X != 0)
                                    FireVectorSumY += (j - Scenario.FireInfo.FireSensor[l].Y) / (i - Scenario.FireInfo.FireSensor[l].X);
                                else
                                    FireVectorSumY = (j - Scenario.FireInfo.FireSensor[l].Y);
                                */
                            }

                            VectorCount FireVectorSum = new VectorCount(FireVectorSumX, FireVectorSumY);

                            VectorCount[] ExitVectors = new VectorCount[Scenario.Node[i, j].DistanceToExits.Count()];

                            double[] TheNovelCost = new double[Scenario.ExitSensor.Count()];
                            double TheMinNovelCost = -1;
                            double[] TheModifiedCost = new double[Scenario.ExitSensor.Count()];
                            double[] TheFinalCost = new double[Scenario.ExitSensor.Count()];
                            double[] TheRealCost = new double[Scenario.ExitSensor.Count()];
                            int MinAcutalDistance = -1;

                            for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                                if (MinAcutalDistance == -1 || MinAcutalDistance > Scenario.Node[i, j].DistanceToExits[l].ActualDistance)
                                    MinAcutalDistance = Scenario.Node[i, j].DistanceToExits[l].ActualDistance;


                            for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                            {
                                ExitVectors[l] = new VectorCount(Scenario.Node[i, j].DistanceToExits[l].X - i, Scenario.Node[i, j].DistanceToExits[l].Y - j);
                                TheRealCost[l] = new double();

                                double CosOfExitNFire = new double();
                                if (FireVectorSum.X == 0 && FireVectorSum.Y == 0)
                                    CosOfExitNFire = -1;
                                else
                                    CosOfExitNFire = (FireVectorSum.X * ExitVectors[l].X + FireVectorSum.Y * ExitVectors[l].Y)
                                        / (Math.Sqrt(Math.Pow(FireVectorSum.X, 2) + Math.Pow(FireVectorSum.Y, 2))
                                        * Math.Sqrt(Math.Pow(ExitVectors[l].X, 2) + Math.Pow(ExitVectors[l].Y, 2)));

                                TheRealCost[l] = ((-CosOfExitNFire + 1)
                                    * Math.Pow(Scenario.Node[i, j].DistanceToExits[l].ActualDistance, 2))
                                    / 2 * MinAcutalDistance;
                            }

                            int TheMinIndex = 0;
                            double TheMinCost = 0;

                            for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                            {
                                if (l == 0)
                                {
                                    TheMinIndex = 0;
                                    TheMinCost = TheRealCost[l];
                                }
                                if (TheRealCost[l] < TheMinCost)
                                {
                                    TheMinIndex = l;
                                    TheMinCost = TheRealCost[l];
                                }
                            }

                            for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                                if (TheMinIndex == l)
                                {
                                    /*
                                    int TargetX = i;
                                    int TargetY = j;

                                    switch (Scenario.Node[i, j].DistanceToExits[l].AheadDirection)
                                    {
                                        case "Left":
                                            TargetX -= 3;
                                            break;
                                        case "Right":
                                            TargetX += 3;
                                            break;
                                        case "Up":
                                            TargetY -= 2;
                                            break;
                                        case "Down":
                                            TargetY += 2;
                                            break;
                                        default:
                                            break;
                                    }

                                    if (Scenario.Node[i, j].DistanceToExits[l].X == Scenario.Node[TargetX, TargetY].TargetX
                                        && Scenario.Node[i, j].DistanceToExits[l].Y == Scenario.Node[TargetX, TargetY].TargetY)
                                    {
                                        Scenario.Node[i, j].TargetX = Scenario.Node[i, j].DistanceToExits[l].X;
                                        Scenario.Node[i, j].TargetY = Scenario.Node[i, j].DistanceToExits[l].Y;
                                        Scenario.Node[i, j].PeopleAheadDirection
                                            = Scenario.Node[i, j].DistanceToExits[l].AheadDirection;
                                        Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[l].Hop;
                                    }
                                    else
                                    {
                                        List<string> OtherDirectionList = DirectionList(Scenario
                                            , i
                                            , j
                                            , Scenario.Node[i, j].DistanceToExits[l].X
                                            , Scenario.Node[i, j].DistanceToExits[l].Y);
                                    }
                                    */
                                    Scenario.Node[i, j].TargetX = Scenario.Node[i, j].DistanceToExits[l].X;
                                    Scenario.Node[i, j].TargetY = Scenario.Node[i, j].DistanceToExits[l].Y;
                                    Scenario.Node[i, j].PeopleAheadDirection
                                        = Scenario.Node[i, j].DistanceToExits[l].AheadDirection;
                                    Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[l].Hop;

                                    for (int m = 0; m < Scenario.Node[i, j].VictimsAtSensor.Count; m++)
                                    {
                                        Scenario.Node[i, j].VictimsAtSensor[m].TargetX = Scenario.Node[i, j].TargetX;
                                        Scenario.Node[i, j].VictimsAtSensor[m].TargetY = Scenario.Node[i, j].TargetY;

                                        Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[m].SN] = Scenario.Node[i, j].VictimsAtSensor[m];
                                    }
                                }

                            /*
                            for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                            {
                                ExitVectors[l] = new VectorCount(Scenario.Node[i, j].DistanceToExits[l].X - i, Scenario.Node[i, j].DistanceToExits[l].Y - j);


                                TheNovelCost[l] = new double();

                                TheNovelCost[l] = (-((ExitVectors[l].X * FireVectorSum.X + ExitVectors[l].Y * FireVectorSum.Y)
                                    / (Math.Sqrt(Math.Pow(ExitVectors[l].X, 2) + Math.Pow(ExitVectors[l].Y, 2))
                                    * Math.Sqrt(Math.Pow(FireVectorSum.X, 2) + Math.Pow(FireVectorSum.Y, 2))))
                                    + 1) / 2 * Scenario.Node[i, j].DistanceToExits[l].ActualDistance;
                                if (TheMinNovelCost == -1 || TheMinNovelCost > TheNovelCost[l])
                                    TheMinNovelCost = TheNovelCost[l];
                            }

                            int TheMinCost = -1;
                            for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                            {
                                TheModifiedCost[l] = (Scenario.Node[i, j].DistanceToExits[l].ActualDistance - MinAcutalDistance)
                                    / MinAcutalDistance * TheMinNovelCost;
                                TheFinalCost[l] = TheNovelCost[l] + TheModifiedCost[l];
                                if (TheMinCost == -1)
                                    TheMinCost = l;
                                else if (TheFinalCost[TheMinCost] > TheFinalCost[l])
                                    TheMinCost = l;
                            }

                            for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                                if (TheMinCost == l)
                                {
                                    Scenario.Node[i, j].TargetX = Scenario.Node[i, j].DistanceToExits[l].X;
                                    Scenario.Node[i, j].TargetY = Scenario.Node[i, j].DistanceToExits[l].Y;
                                    Scenario.Node[i, j].PeopleAheadDirection = Scenario.Node[i, j].DistanceToExits[l].AheadDirection;
                                    Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[l].Hop;
                                }
                             */
                        }


            return Scenario;
        }

        public static List<string> DirectionList(Scenario Scenario, int x, int y, int TargetX, int TargetY)
        {
            List<string> DirectionResult = new List<string>();





            return DirectionResult;
        }

        public static bool DirectionBool(Scenario Scenario, int x, int y, string Direction)
        {
            switch (Direction)
            {
                case "Left":
                    x -= 3;
                    break;
                case "Right":
                    x += 3;
                    break;
                case "Up":
                    y -= 2;
                    break;
                case "Down":
                    y += 2;
                    break;
                default:
                    break;
            }

            if (!Scenario.Node[x, y].Sensor)
                return false;
            if (!Scenario.Node[x, y].Activited)
                return false;

            return true;
        }


        public static Scenario ChoosePath(Scenario Scenario, int MaxHop)
        {
            //for (int k = 0; k < MaxHop; k++)
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Exit != true && Scenario.Node[i, j].Sensor == true)
                    {
                        VectorCount[] FireVectors = new VectorCount[Scenario.FireInfo.FireSensor.Count()];
                        double FireVectorSumX = 0;
                        double FireVectorSumY = 0;
                        for (int l = 0; l < Scenario.FireInfo.FireSensor.Count(); l++)
                        {
                            FireVectors[l] = new VectorCount(i - Scenario.FireInfo.FireSensor[l].X, j - Scenario.FireInfo.FireSensor[l].Y);
                            FireVectorSumX += i - Scenario.FireInfo.FireSensor[l].X;
                            if (i - Scenario.FireInfo.FireSensor[l].X != 0)
                                FireVectorSumY += (j - Scenario.FireInfo.FireSensor[l].Y) / (i - Scenario.FireInfo.FireSensor[l].X);
                            else
                                FireVectorSumY = (j - Scenario.FireInfo.FireSensor[l].Y);
                        }

                        VectorCount FireVectorSum = new VectorCount(FireVectorSumX, FireVectorSumY);

                        VectorCount[] ExitVectors = new VectorCount[Scenario.Node[i, j].DistanceToExits.Count()];
                        double[] TheNovelCost = new double[Scenario.ExitSensor.Count()];
                        double TheMinNovelCost = -1;
                        double[] TheModifiedCost = new double[Scenario.ExitSensor.Count()];
                        double[] TheFinalCost = new double[Scenario.ExitSensor.Count()];
                        int MinAcutalDistance = -1;
                        for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                            if (MinAcutalDistance == -1 || MinAcutalDistance > Scenario.Node[i, j].DistanceToExits[l].ActualDistance)
                                MinAcutalDistance = Scenario.Node[i, j].DistanceToExits[l].ActualDistance;

                        for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                        {
                            ExitVectors[l] = new VectorCount(Scenario.Node[i, j].DistanceToExits[l].X - i, Scenario.Node[i, j].DistanceToExits[l].Y - j);
                            TheNovelCost[l] = new double();
                            TheNovelCost[l] = (-((ExitVectors[l].X * FireVectorSum.X + ExitVectors[l].Y * FireVectorSum.Y)
                                / (Math.Sqrt(Math.Pow(ExitVectors[l].X, 2) + Math.Pow(ExitVectors[l].Y, 2))
                                * Math.Sqrt(Math.Pow(FireVectorSum.X, 2) + Math.Pow(FireVectorSum.Y, 2))))
                                + 1) / 2 * Scenario.Node[i, j].DistanceToExits[l].ActualDistance;
                            if (TheMinNovelCost == -1 || TheMinNovelCost > TheNovelCost[l])
                                TheMinNovelCost = TheNovelCost[l];
                        }

                        int TheMinCost = -1;
                        for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                        {
                            TheModifiedCost[l] = (Scenario.Node[i, j].DistanceToExits[l].ActualDistance - MinAcutalDistance)
                                / MinAcutalDistance * TheMinNovelCost;
                            TheFinalCost[l] = TheNovelCost[l] + TheModifiedCost[l];
                            if (TheMinCost == -1)
                                TheMinCost = l;
                            else if (TheFinalCost[TheMinCost] < TheFinalCost[l])
                                TheMinCost = l;
                        }

                        for (int l = 0; l < Scenario.Node[i, j].DistanceToExits.Count(); l++)
                            if (TheMinCost == l)
                            {
                                Scenario.Node[i, j].TargetX = Scenario.Node[i, j].DistanceToExits[l].X;
                                Scenario.Node[i, j].TargetY = Scenario.Node[i, j].DistanceToExits[l].Y;
                                Scenario.Node[i, j].PeopleAheadDirection = Scenario.Node[i, j].DistanceToExits[l].AheadDirection;
                                Scenario.Node[i, j].Hop = Scenario.Node[i, j].DistanceToExits[l].Hop;
                            }
                    }


            return Scenario;
        }

        public class VectorCount
        {
            public double X { get; set; }
            public double Y { get; set; }

            public VectorCount(double X, double Y)
            {
                this.X = X;
                this.Y = Y;
            }
            public VectorCount()
            {
                this.X = new double();
                this.Y = new double();
            }
        }

        /*public static Scenario ESSPlusSimulation(
            Scenario Scenario)
        {
            return Scenario;
        }
        */

        public static Scenario ESSChangePath(Scenario Scenario, int MaxHop)
        {
            for (int k = MaxHop; k >= 0; k--)
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                    for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                        if (Scenario.Node[i, j].Activited)
                            if (Scenario.Node[i, j].Hop == k)
                            {
                                int TargetX = i;
                                int TargetY = j;
                                string NextDirection = "";

                                switch (Scenario.Node[i, j].PeopleAheadDirection)
                                {
                                    case "Right":
                                        TargetX += 3;
                                        NextDirection = "Left";
                                        break;
                                    case "Left":
                                        TargetX -= 3;
                                        NextDirection = "Right";
                                        break;
                                    case "Up":
                                        TargetY += 2;
                                        NextDirection = "Down";
                                        break;
                                    case "Down":
                                        TargetY -= 2;
                                        NextDirection = "Up";
                                        break;
                                }

                                if (Scenario.Node[TargetX, TargetY].PeopleAheadDirection.Equals(NextDirection))
                                    if (!Scenario.Node[i, j].Exit && !Scenario.Node[TargetX, TargetY].Exit)
                                    {
                                        int MinHopX = i;
                                        int MinHopY = j;
                                        bool TwiceChecked = false;

                                        if (Scenario.Node[TargetX, TargetY].Hop < Scenario.Node[i, j].Hop)
                                        {
                                            MinHopX = TargetX;
                                            MinHopY = TargetY;
                                        }

                                        //Scenario.Node[MinHopX,MinHopY].LeftSensorX

                                    here:

                                        List<ChangePathList> ListPath = new List<ChangePathList>();

                                        if (CheckDirection(Scenario, MinHopX, MinHopY, "Left"))
                                        {
                                            ListPath.Add(new ChangePathList(
                                                Scenario.Node[MinHopX, MinHopY].LeftSensorX
                                                , Scenario.Node[MinHopX, MinHopY].LeftSensorY
                                                , Scenario.Node[Scenario.Node[MinHopX, MinHopY].LeftSensorX
                                                    , Scenario.Node[MinHopX, MinHopY].LeftSensorY].Hop
                                                , "Left"));
                                        }
                                        if (CheckDirection(Scenario, MinHopX, MinHopY, "Right"))
                                        {
                                            ListPath.Add(new ChangePathList(
                                                Scenario.Node[MinHopX, MinHopY].RightSensorX
                                                , Scenario.Node[MinHopX, MinHopY].RightSensorY
                                                , Scenario.Node[Scenario.Node[MinHopX, MinHopY].RightSensorX
                                                    , Scenario.Node[MinHopX, MinHopY].RightSensorY].Hop
                                                , "Right"));
                                        }
                                        if (CheckDirection(Scenario, MinHopX, MinHopY, "Up"))
                                        {
                                            ListPath.Add(new ChangePathList(
                                                Scenario.Node[MinHopX, MinHopY].TopSensorX
                                                , Scenario.Node[MinHopX, MinHopY].TopSensorY
                                                , Scenario.Node[Scenario.Node[MinHopX, MinHopY].TopSensorX
                                                    , Scenario.Node[MinHopX, MinHopY].TopSensorY].Hop
                                                , "Up"));
                                        }
                                        if (CheckDirection(Scenario, MinHopX, MinHopY, "Down"))
                                        {
                                            ListPath.Add(new ChangePathList(
                                                Scenario.Node[MinHopX, MinHopY].BottemSensorX
                                                , Scenario.Node[MinHopX, MinHopY].BottemSensorY
                                                , Scenario.Node[Scenario.Node[MinHopX, MinHopY].BottemSensorX
                                                    , Scenario.Node[MinHopX, MinHopY].BottemSensorY].Hop
                                                , "Down"));
                                        }

                                        ListPath.Sort(new HopSortAC());

                                        Scenario.Node[MinHopX, MinHopY].TargetX = Scenario.Node[ListPath[0].X, ListPath[0].Y].TargetX;
                                        Scenario.Node[MinHopX, MinHopY].TargetY = Scenario.Node[ListPath[0].X, ListPath[0].Y].TargetY;
                                        Scenario.Node[MinHopX, MinHopY].Hop = ListPath[0].Hop + 1;
                                        Scenario.Node[MinHopX, MinHopY].PeopleAheadDirection = ListPath[0].AheadDirection;

                                        if (ListPath.Count == 0 && !TwiceChecked)
                                        {
                                            TwiceChecked = true;
                                            goto here;
                                        }
                                    }
                            }

            return Scenario;
        }

        public class HopSortAC : IComparer<ChangePathList>
        {
            // 遞增排序
            public int Compare(ChangePathList x, ChangePathList y)
            {
                return (x.Hop.CompareTo(y.Hop));
            }
        }
        public class ChangePathList
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Hop { get; set; }
            public string AheadDirection { get; set; }

            public ChangePathList()
            {
                this.X = new int();
                this.Y = new int();
                this.Hop = new int();
            }

            public ChangePathList(int x, int y, int hop, string direction)
            {
                this.X = x;
                this.Y = y;
                this.Hop = hop;
                this.AheadDirection = direction;
            }
        }

        public static bool CheckDirection(Scenario Scenario, int x, int y, string Direction)
        {
            int TargetX = x;
            int TargetY = y;
            string NextDirection = "";

            switch (Direction)
            {
                case "Right":
                    TargetX = Scenario.Node[x, y].RightSensorX;
                    TargetY = Scenario.Node[x, y].RightSensorY;
                    NextDirection = "Left";
                    break;
                case "Left":
                    TargetX = Scenario.Node[x, y].LeftSensorX;
                    TargetY = Scenario.Node[x, y].LeftSensorY;
                    NextDirection = "Right";
                    break;
                case "Up":
                    TargetX = Scenario.Node[x, y].TopSensorX;
                    TargetY = Scenario.Node[x, y].TopSensorY;
                    NextDirection = "Down";
                    break;
                case "Down":
                    TargetX = Scenario.Node[x, y].BottemSensorX;
                    TargetY = Scenario.Node[x, y].BottemSensorY;
                    NextDirection = "Up";
                    break;
            }

            if (TargetY == -1 || TargetX == -1)
                return false;
            if (!Scenario.Node[TargetX, TargetY].Activited)
                return false;
            if (Scenario.Node[TargetX, TargetY].FireSensor)
                return false;
            if (Scenario.Node[TargetX, TargetY].PeopleAheadDirection == NextDirection)
                return false;


            return true;
        }


        public static Scenario ESSPlusSimulation(Scenario Scenario)
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

                                /*
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
                                */



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
                                                int TargetX = i, TargetY = j;

                                                for (int k = 0; k < Scenario.Node[i, j].DistanceToExits.Count; k++)
                                                    if (Scenario.Node[i, j].DistanceToExits[k].X
                                                        == Scenario.Node[i, j].VictimsAtSensor[MovingPos].TargetX
                                                        && Scenario.Node[i, j].DistanceToExits[k].Y
                                                        == Scenario.Node[i, j].VictimsAtSensor[MovingPos].TargetY)
                                                    {
                                                        Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection
                                                            = Scenario.Node[i, j].DistanceToExits[k].AheadDirection;

                                                        switch (Scenario.Node[i, j].VictimsAtSensor[MovingPos].MovingDirection)
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
                                                    }



                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].HaveMovedThisRound = true;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].Path.Add(
                                                    new PathRecord(
                                                        TargetX
                                                        , TargetY
                                                        , Scenario.TotalEvacuationRound
                                                        , false));
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].X = TargetX;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y = TargetY;
                                                Scenario.Node[i, j].VictimsAtSensor[MovingPos].LastMovedRound = Scenario.TotalEvacuationRound;

                                                if (Scenario.Node[TargetX, TargetY].FireSensor == true)
                                                {
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].Live = false;
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].TheRoundVictimDead = Scenario.TotalEvacuationRound;
                                                    Scenario.Node[i, j].VictimsAtSensor[MovingPos].Activated = false;
                                                }

                                                Scenario.WholeVictims[Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN] = Scenario.Node[i, j].VictimsAtSensor[MovingPos];

                                                if (Scenario.Node[TargetX, TargetY].FireSensor == false)
                                                    Scenario.Node[TargetX, TargetY].VictimsAtSensor.Add(
                                                        new ScenarioNode.EachVictim(
                                                            Scenario.Node[i, j].VictimsAtSensor[MovingPos].SN
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].X
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].Y
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartX
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].StartY
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].TargetX
                                                            , Scenario.Node[i, j].VictimsAtSensor[MovingPos].TargetY
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
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].TargetX
                                                        , Scenario.Node[i, j].VictimsAtSensor[0].TargetY
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
    }
}
