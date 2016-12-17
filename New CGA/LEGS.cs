using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scenario = CAES.ScenarioNode.Scenario;

namespace CAES
{
    class LEGS
    {
        public static Scenario LEGSCalculation(
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
                    else if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Exit == true && Scenario.Node[i, j].Activited)
                    {
                        Scenario.Node[i, j].TargetX = i;
                        Scenario.Node[i, j].TargetY = j;
                    }

            Scenario = FireSensorDirection(Scenario);

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Activited)
                    {
                        int CountIfBoundary = 0;
                        if (IfNotFiredNActived(Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY, Scenario))
                            if (IfNotSameTarget(Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY, i, j, Scenario))
                            {
                                CountIfBoundary++;
                            }
                        if (IfNotFiredNActived(Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY, Scenario))
                            if (IfNotSameTarget(Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY, i, j, Scenario))
                            {
                                CountIfBoundary++;
                            }
                        if (IfNotFiredNActived(Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY, Scenario))
                            if (IfNotSameTarget(Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY, i, j, Scenario))
                            {
                                CountIfBoundary++;
                            }
                        if (IfNotFiredNActived(Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY, Scenario))
                            if (IfNotSameTarget(Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY, i, j, Scenario))
                            {
                                CountIfBoundary++;
                            }
                        if (CountIfBoundary > 0)
                            Scenario.Node[i, j].BoundarySensor = true;
                    }

            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] InitialTree = BuildInitialTree(Scenario);

            /*
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor)
                    {
                        if (Scenario.Node[i, j].BoundarySensor)
                            Console.Write("1 ");
                        else
                            Console.Write("0 ");
                    }
                if (i % 3 == 0)
                    Console.WriteLine(" ");
            }

            for (int i = 0; i < InitialTree.GetLength(0); i++)
                Console.WriteLine(FindEvecuationTime(InitialTree[i], InitialTree[i], Scenario).ToString());

            for (int i = 0; i < InitialTree.GetLength(0); i++)
            {
                List<ScenarioNode.PerTreeNodeSet> ListA = TreeClass.BreadthFirst<ScenarioNode.PerTreeNodeSet>(InitialTree[i]);
                Console.Write(InitialTree[i].Count + "  ");
                for (int j = 0; j < ListA.Count; j++)
                {
                    Console.Write(ListA[j].X + "," + ListA[j].Y + "," + ListA[j].BoundarySensor + " ");
                }
                Console.WriteLine(" ");
            }
            */


            InitialTree = TreeSwapBoundary(InitialTree, Scenario);

            SetBoundaryToScenario(InitialTree, Scenario);
            InitialTree = CheckNewBoundary(InitialTree, Scenario);
            /*
            Console.WriteLine(" ");
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
            {
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor)
                    {
                        if (Scenario.Node[i, j].BoundarySensor)
                            Console.Write("1 ");
                        else
                            Console.Write("0 ");
                    }
                if (i % 3 == 0)
                    Console.WriteLine(" ");
            }
            for (int i = 0; i < InitialTree.GetLength(0); i++)
                Console.WriteLine(FindEvecuationTime(InitialTree[i], InitialTree[i], Scenario).ToString());

            for (int i = 0; i < InitialTree.GetLength(0); i++)
            {
                List<ScenarioNode.PerTreeNodeSet> ListA = TreeClass.BreadthFirst<ScenarioNode.PerTreeNodeSet>(InitialTree[i]);
                Console.Write(InitialTree[i].Count + "  ");
                for (int j = 0; j < ListA.Count; j++)
                {
                    Console.Write(ListA[j].X + "," + ListA[j].Y + "," + ListA[j].BoundarySensor + " ");
                }
                Console.WriteLine(" ");
            }
            */
            Scenario = SetNewDirection(InitialTree, Scenario);

            return Scenario;
        }

        public static Scenario SetNewDirection(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Trees
            , Scenario Scenario)
        {
            for (int i = 0; i < Trees.GetLength(0); i++)
            {

                Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>> queue
                    = new Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>>();
                queue.Enqueue(Trees[i]);
                while (queue.Count > 0)
                {
                    TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tmpTree = queue.Dequeue();

                    int X = tmpTree.Value.X;
                    int Y = tmpTree.Value.Y;
                    if (Scenario.Node[X, Y].Exit == false)
                    {
                        int ParentX = tmpTree.Parent.Value.X;
                        int ParentY = tmpTree.Parent.Value.Y;
                        string Direction = null;

                        if (ParentX > X)
                            Direction = "Right";
                        if (ParentX < X)
                            Direction = "Left";
                        if (ParentY > Y)
                            Direction = "Down";
                        if (ParentY < Y)
                            Direction = "Up";
                        if (Scenario.Node[X, Y].PeopleAheadDirection != Direction)
                            Scenario.Node[X, Y].PeopleAheadDirection = Direction;
                    }

                    foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> child in tmpTree.Children)
                        queue.Enqueue(child);
                }
            }

            return Scenario;
        }

        public static double FindEvecuationTime(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> RootNode
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> CurrentNode
            , Scenario Scenario)
        {
            if (CurrentNode.Equals(RootNode))
                if (CurrentNode.Children.Count() == 0)
                    return 0;
                else if (CurrentNode.Children.Count() == 1)
                    foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> n in CurrentNode.Children)
                        return FindEvecuationTime(RootNode, n, Scenario);
                else if (CurrentNode.Children.Count() > 1)
                {
                    double[] Result = new double[CurrentNode.Children.Count()];
                    int CountChildren = 0;
                    foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> n in CurrentNode.Children)
                    {
                        Result[CountChildren] = FindEvecuationTime(RootNode, n, Scenario);
                        CountChildren++;
                    }

                    return MutiMax(Result);
                }

            if (CurrentNode.Children.Count() == 0)
                return Math.Abs((CurrentNode.Parent.Value.X - CurrentNode.Value.X)
                    + (CurrentNode.Parent.Value.Y - CurrentNode.Value.Y));
            else if (CurrentNode.Children.Count() == 1)
                foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> n in CurrentNode.Children)
                    return FindEvecuationTime(RootNode, n, Scenario)
                        + Math.Abs((CurrentNode.Parent.Value.X - CurrentNode.Value.X)
                        + (CurrentNode.Parent.Value.Y - CurrentNode.Value.Y));
            else
            {
                List<ScenarioNode.PerTreeNodeSet> ChildList
                    = TreeClass.GetChildlist<ScenarioNode.PerTreeNodeSet>(CurrentNode);
                ChildList = SortListByDistance(ChildList, CurrentNode.Value, Scenario);

                double CalculateCapacity = Scenario.CorridorCapacity;
                int ListLength = 1;
                for (ListLength = 1; ListLength < ChildList.Count() && CalculateCapacity <= Scenario.CorridorCapacity; ListLength++)
                    CalculateCapacity += Scenario.CorridorCapacity;

                double[] Result = new double[CurrentNode.Children.Count()];
                int CountChildren = 0;
                int VictimsAmount = 0;
                foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> n in CurrentNode.Children)
                {
                    Result[CountChildren] = FindEvecuationTime(RootNode, n, Scenario);
                    VictimsAmount += Scenario.Node[n.Value.X, n.Value.Y].VictimsAtSensor.Count();
                    CountChildren++;
                }

                if (Math.Abs((ChildList[ListLength - 1].X - CurrentNode.Value.X) + (ChildList[ListLength - 1].Y - CurrentNode.Value.Y))
                    <= (Scenario.Node[CurrentNode.Value.X, CurrentNode.Value.Y].VictimsAtSensor.Count
                    / Scenario.CorridorCapacity))
                    return MutiMax(Result)
                        + (VictimsAmount
                        + Scenario.Node[CurrentNode.Value.X, CurrentNode.Value.Y].VictimsAtSensor.Count())
                        / Scenario.CorridorCapacity
                        + Math.Abs((CurrentNode.Parent.Value.X - CurrentNode.Value.X)
                        + (CurrentNode.Parent.Value.Y - CurrentNode.Value.Y));
                else
                {
                    double SumOfTci = 0;
                    for (int i = 0; i < ListLength; i++)
                        SumOfTci += Scenario.CorridorCapacity
                            * (Math.Abs((ChildList[ListLength - 1].X - CurrentNode.Value.X) + (ChildList[ListLength - 1].Y - CurrentNode.Value.Y))
                            - Math.Abs((ChildList[i].X - CurrentNode.Value.X) + (ChildList[i].Y - CurrentNode.Value.Y)));

                    return MutiMax(Result)
                        + Math.Abs((ChildList[ListLength - 1].X - CurrentNode.Value.X) + (ChildList[ListLength - 1].Y - CurrentNode.Value.Y))
                        + Math.Abs((CurrentNode.Parent.Value.X - CurrentNode.Value.X)
                        + (CurrentNode.Parent.Value.Y - CurrentNode.Value.Y))
                        + (VictimsAmount - SumOfTci)
                        / Scenario.CorridorCapacity;
                }
            }
            return 0;
        }

        public static double MutiMax(params double[] values)
        {
            return Enumerable.Max(values);
        }

        public static List<ScenarioNode.PerTreeNodeSet> SortListByDistance(
            List<ScenarioNode.PerTreeNodeSet> OriginList
            , ScenarioNode.PerTreeNodeSet ParentPosition
            , Scenario Scenario)
        {
            int n = OriginList.Count();
            ScenarioNode.PerTreeNodeSet temp = new ScenarioNode.PerTreeNodeSet();
            int Flag = 1; //旗標
            int i;
            for (i = 1; i <= n - 1 && Flag == 1; i++)
            {    // 外層迴圈控制比較回數
                Flag = 0;
                for (int j = 1; j <= n - i; j++)
                {  // 內層迴圈控制每回比較次數
                    //double jVictims = Scenario.Node[OriginList[j].X, OriginList[j].Y].VictimsAtSensor.Count();
                    //double jMinusOneVictims = Scenario.Node[OriginList[j - 1].X, OriginList[j - 1].Y].VictimsAtSensor.Count();
                    double jDistance = Math.Abs((OriginList[j].X - ParentPosition.X) + (OriginList[j].Y - ParentPosition.Y));
                    double jMinusOneDistance = Math.Abs((OriginList[j - 1].X - ParentPosition.X) + (OriginList[j - 1].Y - ParentPosition.Y));

                    if (jDistance < jMinusOneDistance)
                    {  // 比較鄰近兩個物件，右邊比左邊小時就互換。	       
                        temp = OriginList[j];
                        OriginList[j] = OriginList[j - 1];
                        OriginList[j - 1] = temp;
                        Flag = 1;
                    }
                }
            }
            return OriginList;
        }


        public static TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] SortTrees(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Trees
            , Scenario Scenario)
        {
            int[] depth = new int[Trees.GetLength(0)];
            for (int i = 0; i < depth.GetLength(0); i++)
                depth[i] = Trees[i].Depth;

            double[] EvecuationTimes = new double[Trees.GetLength(0)];
            for (int i = 0; i < depth.GetLength(0); i++)
                EvecuationTimes[i] = FindEvecuationTime(Trees[i], Trees[i], Scenario);
            double TotalEvecuationTimes = MutiMax(EvecuationTimes);

            int n = EvecuationTimes.GetLength(0);
            double tempEvecuationTime = new double();
            int tempDepth = new int();
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tempTreeNode
                = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>(new ScenarioNode.PerTreeNodeSet());
            int Flag = 1; //旗標
            int RunTime;
            for (RunTime = 1; RunTime <= n - 1 && Flag == 1; RunTime++)
            {    // 外層迴圈控制比較回數
                Flag = 0;
                for (int j = 1; j <= n - RunTime; j++)
                {  // 內層迴圈控制每回比較次數
                    if (EvecuationTimes[j] < EvecuationTimes[j - 1])
                    {  // 比較鄰近兩個物件，右邊比左邊小時就互換。
                        tempEvecuationTime = EvecuationTimes[j];
                        EvecuationTimes[j] = EvecuationTimes[j - 1];
                        EvecuationTimes[j - 1] = tempEvecuationTime;

                        tempDepth = depth[j];
                        depth[j] = depth[j - 1];
                        depth[j - 1] = tempDepth;

                        tempTreeNode
                            = (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>)Trees[j].Clone();
                        Trees[j]
                            = (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>)Trees[j - 1].Clone();
                        Trees[j - 1]
                            = (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>)tempTreeNode.Clone();

                        Flag = 1;
                    }
                }
            }

            return Trees;
        }


        public static TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] TreeSwapBoundary(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Trees
            , Scenario Scenario)
        {
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] SwappedTrees = SortTrees(Trees, Scenario);
            Trees = SortTrees(Trees, Scenario);

            int TreesParity = Trees.GetLength(0) % 2;
            int TreesSwappedTimes = Trees.GetLength(0) / 2;

            List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> ProcessingTree
                = new List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]>();

            for (int TreeCount = 0; TreeCount < SwappedTrees.GetLength(0); TreeCount++)
            {
                ProcessingTree.Add(
                    new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] { SwappedTrees[TreeCount] });
            }

            //List<int> 
            List<int> RoundCount = CountSecRound(Trees.GetLength(0));

            for (int TreeCount = 0; TreeCount < RoundCount.Count(); TreeCount++)
            {
                ProcessingTree = SortTreesByEvecuationDesc(ProcessingTree, Scenario);

                List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> tempProcessingTree
                    = new List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]>();

                for (int EachRound = 0; EachRound < RoundCount[TreeCount]; EachRound++)
                {

                    TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] tempMax
                        = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[
                            ProcessingTree[ProcessingTree.Count - 1].GetLength(0)];

                    for (int MaxClone = 0
                        ; MaxClone < ProcessingTree[ProcessingTree.Count - 1].GetLength(0)
                        ; MaxClone++)
                        tempMax[MaxClone] = ProcessingTree[ProcessingTree.Count - 1][MaxClone];

                    TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] tempMin = null;

                    for (int i = 0; i < ProcessingTree.Count - 1; i++)
                    {
                        List<ScenarioNode.PerTreeNodeSet> Hello = BoundarySensorInTree(
                            ProcessingTree[ProcessingTree.Count - 1], ProcessingTree[i], Scenario);

                        if (BoundarySensorInTree(
                            ProcessingTree[ProcessingTree.Count - 1], ProcessingTree[i], Scenario) != null)
                        {
                            tempMin = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>
                                [ProcessingTree[i].GetLength(0)];

                            for (int MinClone = 0; MinClone < ProcessingTree[i].GetLength(0); MinClone++)
                                tempMin[MinClone] = ProcessingTree[i][MinClone];

                            ProcessingTree.RemoveAt(ProcessingTree.Count - 1);
                            ProcessingTree.RemoveAt(i);
                            break;
                        }
                    }

                    TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] temp
                        = SwapTrees(tempMax, tempMin, ProcessingTree, tempProcessingTree, Trees, Scenario);

                    ///Add Swapped Tree to temp tree
                    tempProcessingTree.Add(temp);

                    ///CheckNewBoundary
                    SetBoundaryToScenario(tempProcessingTree, ProcessingTree, Scenario);
                    tempProcessingTree = CheckNewBoundaryForList(tempProcessingTree, Scenario);
                    ProcessingTree = CheckNewBoundaryForList(ProcessingTree, Scenario);

                    //TotalEvecuationTimes

                    /*
                    TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] tempProcessingTreeArray
                        = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[
                            tempMax.GetLength(0) + tempMin.GetLength(0)];

                    double[] NewEvecuationTimes = new double[tempMax.GetLength(0) + tempMin.GetLength(0)];

                    for (int FirstCountTimes = 0; FirstCountTimes < tempMax.GetLength(0); FirstCountTimes++)
                    {
                        tempProcessingTreeArray[FirstCountTimes]
                            = tempMax[FirstCountTimes];
                    }

                    for (int SecondCountTimes = 0; SecondCountTimes < tempMin.GetLength(0); SecondCountTimes++)
                    {
                        tempProcessingTreeArray[SecondCountTimes + tempMax.GetLength(0)] 
                            = tempMin[SecondCountTimes];
                    }
                    */
                }

                if (ProcessingTree.Count == 1)
                    tempProcessingTree.Add(ProcessingTree[0]);

                ProcessingTree = tempProcessingTree;

            }


            //List<ScenarioNode.PerTreeNodeSet> list = BoundarySensorInTree(Trees, Trees, Scenario);

            for (int i = 0; i < ProcessingTree[0].GetLength(0); i++)
            {
                SwappedTrees[i] = ProcessingTree[0][i];

            }

            return SwappedTrees;
        }

        public static List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> SortTreesByEvecuationDesc(
            List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> SourceTrees
            , Scenario Scenario)
        {

            int n = SourceTrees.Count();
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] temp;
            int Flag = 1; //旗標
            int i;
            for (i = 1; i <= n - 1 && Flag == 1; i++)
            {    // 外層迴圈控制比較回數
                Flag = 0;
                for (int j = 1; j <= n - i; j++)
                {  // 內層迴圈控制每回比較次數

                    double[] jTimes = new double[SourceTrees[j].GetLength(0)];
                    for (int k = 0; k < SourceTrees[j].GetLength(0); k++)
                        jTimes[k] = FindEvecuationTime(SourceTrees[j][k], SourceTrees[j][k], Scenario);
                    double jTime = MutiMax(jTimes);
                    double[] jMinusOneTimes = new double[SourceTrees[j - 1].GetLength(0)];
                    for (int k = 0; k < SourceTrees[j - 1].GetLength(0); k++)
                        jMinusOneTimes[k] = FindEvecuationTime(SourceTrees[j - 1][k], SourceTrees[j - 1][k], Scenario);
                    double jMinusTime = MutiMax(jMinusOneTimes);

                    if (jTime < jMinusTime)
                    {
                        temp = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[SourceTrees[j].GetLength(0)];
                        temp = SourceTrees[j];
                        SourceTrees[j] = SourceTrees[j - 1];
                        SourceTrees[j - 1] = temp;
                        Flag = 1;
                    }
                }
            }

            return SourceTrees;
        }



        public static TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] SwapTrees(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MaxTrees
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MinTrees
            , List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> OtherTrees
            , List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> OtherProcessedTrees
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] OringinTrees
            , Scenario Scenario)
        {
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] ProcessingTree
                = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[
                    MaxTrees.GetLength(0) + MinTrees.GetLength(0)];

            double MaxEscapeTime = new double();
            MaxEscapeTime = 0;
            double OldMaxEscapeTime = new double();
            OldMaxEscapeTime = 1;

            while (MaxEscapeTime < OldMaxEscapeTime)
            {

                OldMaxEscapeTime = MaxEvecuation(MaxTrees, MinTrees, OtherTrees, Scenario);

                List<ScenarioNode.PerTreeNodeSet> list = new List<ScenarioNode.PerTreeNodeSet>();
                list = BoundarySensorInTree(MaxTrees, MinTrees, Scenario);

                for (int i = 0; i < list.Count(); i++)
                {
                    //List<double> TryingEscapeTime = new List<double>();
                    List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>> TryingEscapeTrees
                        = new List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>>();

                    MaxEscapeTime = MaxEvecuation(MaxTrees, MinTrees, OtherTrees, Scenario);

                    for (int j = 0; j < MinTrees.GetLength(0); j++)
                    {
                        if (IfBoundaryToSpecificTree(
                            list[i]
                            , new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] { MinTrees[j] }
                            , Scenario))
                        {
                            bool Swapped = false;

                            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tempMax
                                = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>(
                                    new ScenarioNode.PerTreeNodeSet());
                            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tempMin
                                = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>(
                                    new ScenarioNode.PerTreeNodeSet());

                            int tempMaxIndex = -1;
                            for (int k = 0; k < MaxTrees.GetLength(0); k++)
                                if (TreeClass.FindSpecific(MaxTrees[k], list[i]) != null)
                                    tempMaxIndex = k;


                            int TempMaxEvecuationTimeIndex = -1;
                            double TempMaxEvecuationTime = -1;

                            if (tempMaxIndex == -1)
                                continue;

                            double TempMaxOthersEvecuationTime = new double();
                            /*
                            double TempMaxOthersEvecuationTime = MaxOthersEvecuation(
                                MaxTrees[tempMaxIndex]
                                , MinTrees[j]
                                , MaxTrees
                                , MinTrees
                                , OtherTrees
                                , Scenario);

                            if (MaxEscapeTime == TempMaxOthersEvecuationTime)
                                TempMaxOthersEvecuationTime = -1;
                             * */

                            List<string> FindTarget = FindTargetParentNode(MinTrees[j], list[i], Scenario);

                            for (int k = 0; k < FindTarget.Count(); k++)
                            {
                                tempMax = (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>)
                                    MaxTrees[tempMaxIndex].Clone();
                                tempMin = (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>)
                                    MinTrees[j].Clone();

                                TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tempMinTreeAdded
                                    = TreeClass.AddSpecific<ScenarioNode.PerTreeNodeSet>(
                                    TreeClass.FindSpecific<ScenarioNode.PerTreeNodeSet>(tempMax, list[i])
                                    , tempMin
                                    , FindSpecificNeighborSensor(list[i], FindTarget[k], tempMin, Scenario).Value);

                                TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tempMaxTreeAdded
                                    = TreeClass.RemoveSpecific<ScenarioNode.PerTreeNodeSet>(tempMax, list[i]);

                                TempMaxOthersEvecuationTime = MaxOthersEvecuation(
                                tempMax
                                , tempMin
                                , MaxTrees
                                , MinTrees
                                , OtherTrees
                                , Scenario);

                                if (MutiMax(new double[] { 
                                    FindEvecuationTime(
                                    tempMinTreeAdded
                                    , tempMinTreeAdded
                                    , Scenario)
                                    ,FindEvecuationTime(
                                    tempMaxTreeAdded
                                    , tempMaxTreeAdded
                                    , Scenario)
                                    ,TempMaxOthersEvecuationTime })
                                    < MaxEscapeTime)
                                {
                                    MinTrees[j] = TreeClass.AddSpecific<ScenarioNode.PerTreeNodeSet>(
                                        TreeClass.FindSpecific<ScenarioNode.PerTreeNodeSet>(MaxTrees[tempMaxIndex], list[i])
                                        , MinTrees[j]
                                        , FindSpecificNeighborSensor(list[i], FindTarget[k], MinTrees[j], Scenario).Value);

                                    MaxTrees[tempMaxIndex] = TreeClass.RemoveSpecific<ScenarioNode.PerTreeNodeSet>(
                                        MaxTrees[tempMaxIndex]
                                        , list[i]);
                                    Swapped = true;
                                    break;
                                }
                            }

                            /**
                            ScenarioNode.PerTreeNodeSet

                            double TryingEscapeTime = MaxEvecuation(
                                MaxTrees[tempMaxIndex].Remove()
                                ,MinTrees[j].add
                                ,OtherTrees
                                ,Scenario);

                            */
                            if (Swapped == true)
                                break;
                        }
                    }

                }

                ///Record New Evecuation Time
                MaxEscapeTime = MaxEvecuation(MaxTrees, MinTrees, OtherTrees, Scenario);

                ///Check New Boundary here
                Scenario = SetBoundaryToScenario(
                    MaxTrees, MinTrees, OtherTrees, OtherProcessedTrees, Scenario);
                MaxTrees = CheckNewBoundary(MaxTrees, Scenario);
                OtherTrees = CheckNewBoundaryForList(OtherTrees, Scenario);
                OtherProcessedTrees = CheckNewBoundaryForList(OtherProcessedTrees, Scenario);
                MinTrees = CheckNewBoundary(MinTrees, Scenario);
            }
            for (int i = 0; i < MaxTrees.GetLength(0); i++)
                ProcessingTree[i] = MaxTrees[i];
            for (int i = 0; i < MinTrees.GetLength(0); i++)
                ProcessingTree[MaxTrees.GetLength(0) + i] = MinTrees[i];


            return ProcessingTree;
        }


        public static Scenario SetBoundaryToScenario(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Tree
            , Scenario Scenario)
        {

            for (int i = 0; i < Tree.GetLength(0); i++)
                Scenario = SetNewTargetExit(Tree[i], Scenario);

            Scenario = SetNewBoundaryNode(Scenario);

            return Scenario;
        }

        public static Scenario SetBoundaryToScenario(
            List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> Tree
            , Scenario Scenario)
        {

            for (int i = 0; i < Tree.Count(); i++)
                for (int j = 0; j < Tree[i].GetLength(0); j++)
                    Scenario = SetNewTargetExit(Tree[i][j], Scenario);

            Scenario = SetNewBoundaryNode(Scenario);

            return Scenario;
        }

        public static Scenario SetBoundaryToScenario(
            List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> TreeA
            , List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> TreeB
            , Scenario Scenario)
        {

            for (int i = 0; i < TreeA.Count(); i++)
                for (int j = 0; j < TreeA[i].GetLength(0); j++)
                    Scenario = SetNewTargetExit(TreeA[i][j], Scenario);

            for (int i = 0; i < TreeB.Count(); i++)
                for (int j = 0; j < TreeB[i].GetLength(0); j++)
                    Scenario = SetNewTargetExit(TreeB[i][j], Scenario);

            Scenario = SetNewBoundaryNode(Scenario);

            return Scenario;
        }


        public static Scenario SetBoundaryToScenario(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MaxTrees
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MinTrees
            , List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> OtherTrees
            , List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> OtherProcessedTrees
            , Scenario Scenario)
        {
            for (int i = 0; i < MaxTrees.GetLength(0); i++)
                Scenario = SetNewTargetExit(MaxTrees[i], Scenario);

            for (int i = 0; i < MinTrees.GetLength(0); i++)
                Scenario = SetNewTargetExit(MinTrees[i], Scenario);

            for (int i = 0; i < OtherTrees.Count(); i++)
                for (int j = 0; j < OtherTrees[i].GetLength(0); j++)
                    Scenario = SetNewTargetExit(OtherTrees[i][j], Scenario);

            for (int i = 0; i < OtherProcessedTrees.Count(); i++)
                for (int j = 0; j < OtherProcessedTrees[i].GetLength(0); j++)
                    Scenario = SetNewTargetExit(OtherProcessedTrees[i][j], Scenario);

            Scenario = SetNewBoundaryNode(Scenario);

            return Scenario;
        }

        public static Scenario SetNewTargetExit(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> Tree
            , Scenario Scenario)
        {
            int TargetX = Tree.Value.X;
            int TargetY = Tree.Value.Y;

            Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>> queue
                = new Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>>();
            queue.Enqueue(Tree);
            while (queue.Count > 0)
            {
                TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tmpTree = queue.Dequeue();
                Scenario.Node[tmpTree.Value.X, tmpTree.Value.Y].TargetX = TargetX;
                Scenario.Node[tmpTree.Value.X, tmpTree.Value.Y].TargetY = TargetY;

                foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> child in tmpTree.Children)
                    queue.Enqueue(child);
            }

            return Scenario;
        }

        public static Scenario SetNewBoundaryNode(Scenario Scenario)
        {
            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor == true)
                    {
                        int CountIfBoundary = 0;
                        if (IfNotFiredNActived(Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY, Scenario))
                            if (IfNotSameTarget(Scenario.Node[i, j].LeftSensorX, Scenario.Node[i, j].LeftSensorY, i, j, Scenario))
                            {
                                CountIfBoundary++;
                            }
                        if (IfNotFiredNActived(Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY, Scenario))
                            if (IfNotSameTarget(Scenario.Node[i, j].RightSensorX, Scenario.Node[i, j].RightSensorY, i, j, Scenario))
                            {
                                CountIfBoundary++;
                            }
                        if (IfNotFiredNActived(Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY, Scenario))
                            if (IfNotSameTarget(Scenario.Node[i, j].TopSensorX, Scenario.Node[i, j].TopSensorY, i, j, Scenario))
                            {
                                CountIfBoundary++;
                            }
                        if (IfNotFiredNActived(Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY, Scenario))
                            if (IfNotSameTarget(Scenario.Node[i, j].BottemSensorX, Scenario.Node[i, j].BottemSensorY, i, j, Scenario))
                            {
                                CountIfBoundary++;
                            }
                        if (CountIfBoundary > 0)
                            Scenario.Node[i, j].BoundarySensor = true;
                        else
                            Scenario.Node[i, j].BoundarySensor = false;
                    }

            return Scenario;
        }

        public static TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] CheckNewBoundary(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Trees
            , Scenario Scenario)
        {
            for (int i = 0; i < Trees.GetLength(0); i++)
            {
                Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>> queue
                    = new Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>>();
                queue.Enqueue(Trees[i]);
                while (queue.Count > 0)
                {
                    TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tmpTree = queue.Dequeue();

                    tmpTree.Value.BoundarySensor
                        = Scenario.Node[tmpTree.Value.X, tmpTree.Value.Y].BoundarySensor;

                    foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> child in tmpTree.Children)
                        queue.Enqueue(child);
                }
            }

            return Trees;
        }
        public static List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> CheckNewBoundaryForList(
            List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> Trees
            , Scenario Scenario)
        {
            for (int i = 0; i < Trees.Count(); i++)
                for (int j = 0; j < Trees[i].GetLength(0); j++)
                {
                    Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>> queue
                        = new Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>>();
                    queue.Enqueue(Trees[i][j]);
                    while (queue.Count > 0)
                    {
                        TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tmpTree = queue.Dequeue();

                        tmpTree.Value.BoundarySensor
                            = Scenario.Node[tmpTree.Value.X, tmpTree.Value.Y].BoundarySensor;

                        foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> child in tmpTree.Children)
                            queue.Enqueue(child);
                    }
                }

            return Trees;
        }

        public static List<string> FindTargetParentNode(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> TargetTree
            , ScenarioNode.PerTreeNodeSet SourceValue
            , Scenario Scenario)
        {
            List<string> Direction = new List<string>();

            if (NeighborSensor(SourceValue.X, SourceValue.Y, "Left"
                , new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] { TargetTree }, Scenario))
                Direction.Add("Left");
            if (NeighborSensor(SourceValue.X, SourceValue.Y, "Right"
                , new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] { TargetTree }, Scenario))
                Direction.Add("Right");
            if (NeighborSensor(SourceValue.X, SourceValue.Y, "Up"
                , new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] { TargetTree }, Scenario))
                Direction.Add("Up");
            if (NeighborSensor(SourceValue.X, SourceValue.Y, "Down"
                , new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] { TargetTree }, Scenario))
                Direction.Add("Down");

            return Direction;
        }

        public static TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> FindSpecificNeighborSensor(
            ScenarioNode.PerTreeNodeSet SourceValue
            , string Direction
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> Mintree
            , Scenario Scenario)
        {
            int TargetX = -1;
            int TargetY = -1;

            switch (Direction)
            {
                case "Left":
                    TargetX = Scenario.Node[SourceValue.X, SourceValue.Y].LeftSensorX;
                    TargetY = Scenario.Node[SourceValue.X, SourceValue.Y].LeftSensorY;
                    break;
                case "Right":
                    TargetX = Scenario.Node[SourceValue.X, SourceValue.Y].RightSensorX;
                    TargetY = Scenario.Node[SourceValue.X, SourceValue.Y].RightSensorY;
                    break;
                case "Up":
                    TargetX = Scenario.Node[SourceValue.X, SourceValue.Y].TopSensorX;
                    TargetY = Scenario.Node[SourceValue.X, SourceValue.Y].TopSensorY;
                    break;
                case "Down":
                    TargetX = Scenario.Node[SourceValue.X, SourceValue.Y].BottemSensorX;
                    TargetY = Scenario.Node[SourceValue.X, SourceValue.Y].BottemSensorY;
                    break;
                default:
                    break;
            }

            if (TargetX == -1 || TargetY == -1)
                return null;
            if (Scenario.Node[TargetX, TargetY].FireSensor == true)
                return null;

            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> TargetParentNode = null;

            if (TreeClass.FindSpecific<ScenarioNode.PerTreeNodeSet>(
                Mintree, new ScenarioNode.PerTreeNodeSet(TargetX, TargetY, true)) != null)
            {
                TargetParentNode = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>(
                    new ScenarioNode.PerTreeNodeSet());
                TargetParentNode = TreeClass.FindSpecific<ScenarioNode.PerTreeNodeSet>(
                    Mintree, new ScenarioNode.PerTreeNodeSet(TargetX, TargetY, true));
            }

            return TargetParentNode;
        }

        public static double MaxEvecuation(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MaxTrees
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MinTrees
            , List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> OtherTrees
            , Scenario Scenario)
        {
            double OldMaxEscapeTime = new double();
            List<double> EvecuationTimes = new List<double>();
            for (int i = 0; i < MaxTrees.GetLength(0); i++)
                EvecuationTimes.Add(FindEvecuationTime(MaxTrees[i], MaxTrees[i], Scenario));
            for (int i = 0; i < MinTrees.GetLength(0); i++)
                EvecuationTimes.Add(FindEvecuationTime(MinTrees[i], MinTrees[i], Scenario));
            for (int i = 0; i < OtherTrees.Count(); i++)
                for (int j = 0; j < OtherTrees[i].GetLength(0); j++)
                    EvecuationTimes.Add(FindEvecuationTime(OtherTrees[i][j], OtherTrees[i][j], Scenario));

            OldMaxEscapeTime = MutiMax(EvecuationTimes.ToArray());
            /*
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] TempMaxTrees =
                new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[MaxTrees.GetLength(0)];
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] TempMinTrees =
                new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[MinTrees.GetLength(0)];
            for (int MaxClone = 0; MaxClone < MaxTrees.GetLength(0); MaxClone++)
                TempMaxTrees[MaxClone] = MaxTrees[MaxClone];
            for (int MinClone = 0; MinClone < MinTrees.GetLength(0); MinClone++)
                TempMinTrees[MinClone] = MinTrees[MinClone];


            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> SourceTree = MaxTrees[0];

            for (int MaxCount = 0; MaxCount < MaxTrees.GetLength(0); MaxCount++)
                if (TreeClass.FindSpecific<ScenarioNode.PerTreeNodeSet>(MaxTrees[MaxCount], BoundarySensor) != null)
                {
                    SourceTree = MaxTrees[MaxCount];
                    break;
                }

            if (NeighborSensor(BoundarySensor.X, BoundarySensor.Y, "Left", MinTrees, Scenario))
            {
                int TargetX = BoundarySensor.X - 3;
                int TargetY = BoundarySensor.Y;

                TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> TempTree = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>(BoundarySensor);

                for (int MinCount = 0; MinCount < MinTrees.GetLength(0); MinCount++)
                    if (TreeClass.FindSpecific<ScenarioNode.PerTreeNodeSet>
                        (MinTrees[MinCount], new ScenarioNode.PerTreeNodeSet(TargetX, TargetY, true)) != null)
                    {
                        TempTree = MinTrees[MinCount];
                        break;
                    }


            }

            if (NeighborSensor(BoundarySensor.X, BoundarySensor.Y, "Right", MinTrees, Scenario))
                return 0;
            if (NeighborSensor(BoundarySensor.X, BoundarySensor.Y, "Up", MinTrees, Scenario))
                return 0;
            if (NeighborSensor(BoundarySensor.X, BoundarySensor.Y, "Down", MinTrees, Scenario))
                return 0;
            */


            return OldMaxEscapeTime;
        }

        public static double MaxOthersEvecuation(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> MaxTree
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> MinTree
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MaxTrees
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MinTrees
            , List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> OtherTrees
            , Scenario Scenario)
        {
            double OldMaxEscapeTime = new double();
            List<double> EvecuationTimes = new List<double>();

            EvecuationTimes.Add(FindEvecuationTime(MaxTree, MaxTree, Scenario));
            EvecuationTimes.Add(FindEvecuationTime(MinTree, MinTree, Scenario));

            for (int i = 0; i < MaxTrees.GetLength(0); i++)
                //if (!MaxTrees[i].Value.Equals(MaxTree.Value))
                if (!TreeClass.FindRoot<ScenarioNode.PerTreeNodeSet>(MaxTrees[i]).Value.Equals(
                    TreeClass.FindRoot<ScenarioNode.PerTreeNodeSet>(MaxTree).Value))
                    EvecuationTimes.Add(FindEvecuationTime(MaxTrees[i], MaxTrees[i], Scenario));
            for (int i = 0; i < MinTrees.GetLength(0); i++)
                //if (!MinTrees[i].Value.Equals(MinTree.Value))
                if (!TreeClass.FindRoot<ScenarioNode.PerTreeNodeSet>(MinTrees[i]).Value.Equals(
                    TreeClass.FindRoot<ScenarioNode.PerTreeNodeSet>(MinTree).Value))
                    EvecuationTimes.Add(FindEvecuationTime(MinTrees[i], MinTrees[i], Scenario));
            for (int i = 0; i < OtherTrees.Count(); i++)
                for (int j = 0; j < OtherTrees[i].GetLength(0); j++)
                    EvecuationTimes.Add(FindEvecuationTime(OtherTrees[i][j], OtherTrees[i][j], Scenario));

            OldMaxEscapeTime = MutiMax(EvecuationTimes.ToArray());

            return OldMaxEscapeTime;
        }

        public static List<int> CountSecRound(int TreesNumber)
        {
            List<int> List = new List<int>();
            int Remainder = 0;

            while (true)
            {

                if (List.Count() == 0)
                {
                    List.Add(TreesNumber / 2);
                    if (TreesNumber % 2 != 0)
                        Remainder += TreesNumber % 2;
                }
                else
                {
                    if (List[List.Count() - 1] % 2 == 0)
                        List.Add(List[List.Count() - 1] / 2);
                    else
                    {
                        if (Remainder != 0)
                        {
                            List.Add((List[List.Count() - 1] + 1) / 2);
                            Remainder = 0;
                        }
                        else
                        {
                            List.Add(List[List.Count() - 1] / 2);
                            Remainder = 1;
                        }
                    }
                }

                if (Remainder == 0 && List[List.Count() - 1] == 1)
                    break;
            }

            return List;
        }

        public static TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] ChoosingMinTrees(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] CompareTreesSource
            , List<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[]> MinTreesSource)
        {
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] MinTrees
                = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[0];

            for (int OriginTreesCount = 0; OriginTreesCount < CompareTreesSource.GetLength(0); OriginTreesCount++)
            {
                for (int MinTreesCount = 0; MinTreesCount < MinTreesSource.Count(); MinTreesCount++)
                {

                }
            }



            return MinTrees;
        }

        public static List<ScenarioNode.PerTreeNodeSet> BoundarySensorInTree(
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Maxtree
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Mintree
            , Scenario Scenario)
        {
            List<ScenarioNode.PerTreeNodeSet> list = new List<ScenarioNode.PerTreeNodeSet>();
            Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>> queue = new Queue<TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>>();
            for (int MaxRunTime = 0; MaxRunTime < Maxtree.GetLength(0); MaxRunTime++)
            {
                queue.Enqueue(Maxtree[MaxRunTime]);
                while (queue.Count > 0)
                {
                    TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> tmpTree = queue.Dequeue();
                    //if (tmpTree.Value.BoundarySensor == true)
                    //{
                    if (IfBoundaryToSpecificTree(tmpTree.Value, Mintree, Scenario))
                        list.Add(tmpTree.Value);
                    //}
                    foreach (TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> child in tmpTree.Children)
                        queue.Enqueue(child);
                }
            }


            int n = list.Count();
            ScenarioNode.PerTreeNodeSet temp = new ScenarioNode.PerTreeNodeSet();
            int Flag = 1; //旗標
            int i;
            for (i = 1; i <= n - 1 && Flag == 1; i++)
            {    // 外層迴圈控制比較回數
                Flag = 0;
                for (int j = 1; j <= n - i; j++)
                {  // 內層迴圈控制每回比較次數
                    int jVictims = Scenario.Node[list[j].X, list[j].Y].VictimsAtSensor.Count();
                    int jMinusOneVictims = Scenario.Node[list[j - 1].X, list[j - 1].Y].VictimsAtSensor.Count();

                    if (jVictims > jMinusOneVictims)
                    {
                        temp = list[j];
                        list[j] = list[j - 1];
                        list[j - 1] = temp;
                        Flag = 1;
                    }
                }
            }

            return list;
            /*
            if (n != 0)
                return list;
            else
                return null;*/
        }
        public static bool IfBoundaryToSpecificTree(
            ScenarioNode.PerTreeNodeSet TreeNodeValue
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Mintree
            , Scenario Scenario)
        {
            if (NeighborSensor(TreeNodeValue.X, TreeNodeValue.Y, "Left", Mintree, Scenario))
                return true;
            if (NeighborSensor(TreeNodeValue.X, TreeNodeValue.Y, "Right", Mintree, Scenario))
                return true;
            if (NeighborSensor(TreeNodeValue.X, TreeNodeValue.Y, "Up", Mintree, Scenario))
                return true;
            if (NeighborSensor(TreeNodeValue.X, TreeNodeValue.Y, "Down", Mintree, Scenario))
                return true;

            return false;
        }

        public static bool NeighborSensor(
            int X
            , int Y
            , string Direction
            , TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] Mintree
            , Scenario Scenario)
        {
            int TargetX = -1;
            int TargetY = -1;

            switch (Direction)
            {
                case "Left":
                    TargetX = Scenario.Node[X, Y].LeftSensorX;
                    TargetY = Scenario.Node[X, Y].LeftSensorY;
                    break;
                case "Right":
                    TargetX = Scenario.Node[X, Y].RightSensorX;
                    TargetY = Scenario.Node[X, Y].RightSensorY;
                    break;
                case "Up":
                    TargetX = Scenario.Node[X, Y].TopSensorX;
                    TargetY = Scenario.Node[X, Y].TopSensorY;
                    break;
                case "Down":
                    TargetX = Scenario.Node[X, Y].BottemSensorX;
                    TargetY = Scenario.Node[X, Y].BottemSensorY;
                    break;
                default:
                    break;
            }

            if (TargetX == -1 || TargetY == -1)
                return false;
            if (Scenario.Node[TargetX, TargetY].FireSensor == true)
                return false;

            bool CheckIfBoundaryInTrees = false;

            for (int i = 0; i < Mintree.GetLength(0); i++)
            {
                if (TreeClass.FindSpecific<ScenarioNode.PerTreeNodeSet>(
                    Mintree[i], new ScenarioNode.PerTreeNodeSet(TargetX, TargetY, true)) != null)
                    CheckIfBoundaryInTrees = true;
            }

            return CheckIfBoundaryInTrees;

        }



        public static TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] BuildInitialTree(Scenario Scenario)
        {
            TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[] InitialTree
                = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>[Scenario.ExitSensor.Count()];

            for (int ExitSN = 0; ExitSN < Scenario.ExitSensor.Count(); ExitSN++)
            {
                InitialTree[ExitSN] = new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>(
                    new ScenarioNode.PerTreeNodeSet(
                        Scenario.Node[Scenario.ExitSensor[ExitSN].X, Scenario.ExitSensor[ExitSN].Y]));

                for (int HopNow = 1; HopNow < MaxHopOfExit(Scenario)[ExitSN] + 1; HopNow++)
                    for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                        for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                            if (IfSameExitNHop(HopNow, i, j, ExitSN, Scenario) && Scenario.Node[i, j].Activited)
                            {
                                int ParentX = i;
                                int ParentY = j;

                                switch (Scenario.Node[i, j].PeopleAheadDirection)
                                {
                                    case "Left":
                                        ParentX -= 3;
                                        break;
                                    case "Right":
                                        ParentX += 3;
                                        break;
                                    case "Up":
                                        ParentY -= 2;
                                        break;
                                    case "Down":
                                        ParentY += 2;
                                        break;
                                    default:
                                        break;
                                }

                                ScenarioNode.PerTreeNodeSet CompareNode
                                    = new ScenarioNode.PerTreeNodeSet(Scenario.Node[ParentX, ParentY]);

                                TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet> ParentNode
                                    = TreeClass.FindSpecific<ScenarioNode.PerTreeNodeSet>(InitialTree[ExitSN], CompareNode);

                                ParentNode.Add(new TreeClass.LinkedTree<ScenarioNode.PerTreeNodeSet>(
                                    new ScenarioNode.PerTreeNodeSet(Scenario.Node[i, j])));
                            }
            }

            return InitialTree;
        }

        public static int MaxHop(Scenario Scenario)
        {
            int MaxHop = 0;

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    for (int ExitSN = 0; ExitSN < Scenario.ExitSensor.Count(); ExitSN++)
                        if (MaxHop < Scenario.Node[i, j].Hop)
                            MaxHop = Scenario.Node[i, j].Hop;

            return MaxHop;
        }

        public static int[] MaxHopOfExit(Scenario Scenario)
        {
            int[] MaxHopOfExit = new int[Scenario.ExitSensor.Count()];

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    for (int ExitSN = 0; ExitSN < Scenario.ExitSensor.Count(); ExitSN++)
                        if (Scenario.Node[i, j].TargetX == Scenario.ExitSensor[ExitSN].X
                            && Scenario.Node[i, j].TargetY == Scenario.ExitSensor[ExitSN].Y)
                            if (MaxHopOfExit[ExitSN] < Scenario.Node[i, j].Hop)
                                MaxHopOfExit[ExitSN] = Scenario.Node[i, j].Hop;

            return MaxHopOfExit;
        }

        public static bool IfNotFiredNActived(int X, int Y, Scenario Scenario)
        {
            if (X == -1)
                return false;
            if (Y == -1)
                return false;
            if (Scenario.Node[X, Y].Activited == false)
                return false;
            if (Scenario.Node[X, Y].FireSensor == true)
                return false;

            return true;
        }

        public static bool IfNotSameTarget(int X, int Y, int i, int j, Scenario Scenario)
        {
            if (Scenario.Node[X, Y].TargetX == Scenario.Node[i, j].TargetX && Scenario.Node[X, Y].TargetY == Scenario.Node[i, j].TargetY)
                return false;
            return true;
        }

        public static bool IfSameExitNHop(int Hop, int i, int j, int ExitSN, Scenario Scenario)
        {
            if (Scenario.ExitSensor[ExitSN].X == Scenario.Node[i, j].TargetX
                && Scenario.ExitSensor[ExitSN].Y == Scenario.Node[i, j].TargetY
                && Scenario.Node[i, j].Hop == Hop)
                return true;
            return false;
        }

        public static Scenario FindNeighborNFindMinHopWithFire(Scenario Scenario, int ExitIndex)
        {
            int HopCount = 0;

            while (!CheckIfCountedAllHopWithFire(Scenario, ExitIndex))
            {
                for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                    for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                        if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Activited)
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
                HopCount++;
            }


            return Scenario;
        }

        public static bool CheckIfCountedAllHopWithFire(Scenario Scenario, int ExitIndex)
        {

            for (int i = 0; i < Scenario.Node.GetLength(0); i++)
                for (int j = 0; j < Scenario.Node.GetLength(1); j++)
                    if (Scenario.Node[i, j].Sensor == true && Scenario.Node[i, j].Activited)
                        if (Scenario.Node[i, j].FireSensor == false)
                            if (Scenario.Node[i, j].DistanceToExits[ExitIndex].Hop == -1)
                                return false;
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
                    }
            return Scenario;
        }

        public static Scenario LEGSSimulation(
            Scenario Scenario)
        {
            return Scenario;
        }


    }
}
