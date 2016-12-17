using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;

namespace CAES
{
    class ScenarioNode
    {
        public class CompareStruct
        {
            public double[] Compare;
            public double FindMinDL, FindMaxDL;
            public string[] HeadingDirection;
            public int MinDangerousLevelX, MinDangerousLevelY;
            public int[] MinTargetX, MinTargetY;
        }

        public class CompareDL
        {
            public List<CompareDLList> DLList;
            public double FindMinDL, FindMaxDL;
            public int MinDangerousLevelX, MinDangerousLevelY;
        }

        public class CompareDLList
        {
            public int X { get; set; }
            public int Y { get; set; }
            public string Direction { get; set; }
            public double Weight { get; set; }

            public CompareDLList(int X, int Y, string Direction, double Weight)
            {
                this.X = X;
                this.Y = Y;
                this.Direction = Direction;
                this.Weight = Weight;
            }
        }

        public class CorridorStruct
        {
            public int CorridorPeopleTemp;
            public bool ExistPeople;
            public bool HavedRun;
            public string PeopleAheadDirectionInCorridor;
        }

        public class InitialCalculateStruct
        {
            public ArrayList ary;
            public int TotalNeighbor, X, Y;
            public double TotalWeight, SumOfSquare, HighestWeight, SelfWeight, MeanWeight, MeanWightwithSelf;
        }

        public class Scenario
        {
            public EachVictim[] WholeVictims;
            public NodeInScenario[,] Node;
            public List<ExitInfo> ExitSensor;
            public FireInfo FireInfo;
            public ExcelProcess ExcelProcess;
            public string AlgorithmType;
            public string ThresholdType;
            public string DLCalculation;
            public double Alpha;
            public double Beta;
            public double Gamma;
            public int SensorHeight;
            public int SensorWidth;
            public int ScenarioHeight;
            public int ScenarioWidth;
            public int CorridorCapacity;
            public int LoopRound;
            public int ScenarioType;
            public int TotalEvacuationRound;
            public int DeathVictimsAmount;
            public bool EvacuationFinished;
            public double BasicLengthPerCorridor;
            public double BasicWeightPerPerson;
        }

        public class EachVictim
        {
            //Serial number of the vicims
            public int SN { get; set; }

            //The position of each victim now
            public int X { get; set; }
            public int Y { get; set; }

            //The first place these victims stayed
            public int StartX { get; set; }
            public int StartY { get; set; }

            //The first place these victims stayed
            public int TargetX { get; set; }
            public int TargetY { get; set; }

            //Check if it is at corridors.
            public string MovingDirection { get; set; }

            //To check if victim have moved this round
            public int LastMovedRound { get; set; }
            public bool HaveMovedThisRound { get; set; }

            //Path recording
            public List<PathRecord> Path { get; set; }

            //If this victim will follow the path we give
            public bool RuleFollower { get; set; }

            public bool Live { get; set; }

            public int TheRoundVictimDead { get; set; }

            //Check if escape already
            public bool HaveEscaped { get; set; }

            public bool Activated { get; set; }
            
            public EachVictim()
            {
                this.SN = new int();
                this.X = new int();
                this.Y = new int();
                this.StartX = new int();
                this.StartY = new int();
                this.TargetX = new int();
                this.TargetY = new int();
                this.MovingDirection = null;
                this.LastMovedRound = new int();
                this.HaveMovedThisRound = new bool();
                this.Path = new List<PathRecord>();
                this.RuleFollower = new bool();
                this.Live = new bool();
                this.TheRoundVictimDead = new int();
                this.HaveEscaped = new bool();
                this.Activated = new bool();
            }

            public EachVictim(
                int SN
                , int X
                , int Y
                , int StartX
                , int StartY
                , string MovingDirection
                , int LastMovedRound
                , bool HaveMovedThisRound
                , List<PathRecord> Path
                , bool RuleFollower
                , bool Live
                , int TheRoundVictimDead
                , bool HaveEscaped
                , bool Activated)
            {
                this.SN = SN;
                this.X = X;
                this.Y = Y;
                this.StartX = StartX;
                this.StartY = StartY;
                this.TargetX = new int();
                this.TargetY = new int();
                this.MovingDirection = MovingDirection;
                this.LastMovedRound = LastMovedRound;
                this.HaveMovedThisRound = HaveMovedThisRound;
                this.Path = new List<PathRecord>();
                if (Path != null)
                    for (int i = 0; i < Path.Count; i++)
                    {
                        this.Path.Add(new PathRecord(
                            Path[i].X
                            , Path[i].Y
                            , Path[i].Round
                            , Path[i].FireHappened));
                    }
                this.RuleFollower = RuleFollower;
                this.Live = Live;
                this.TheRoundVictimDead = TheRoundVictimDead;
                this.HaveEscaped = HaveEscaped;
                this.Activated = Activated;
            }

            public EachVictim(
                int SN
                , int X
                , int Y
                , int StartX
                , int StartY
                , int TargetX
                , int TargetY
                , string MovingDirection
                , int LastMovedRound
                , bool HaveMovedThisRound
                , List<PathRecord> Path
                , bool RuleFollower
                , bool Live
                , int TheRoundVictimDead
                , bool HaveEscaped
                , bool Activated)
            {
                this.SN = SN;
                this.X = X;
                this.Y = Y;
                this.StartX = StartX;
                this.StartY = StartY;
                this.TargetX = TargetX;
                this.TargetY = TargetY;
                this.MovingDirection = MovingDirection;
                this.LastMovedRound = LastMovedRound;
                this.HaveMovedThisRound = HaveMovedThisRound;
                this.Path = new List<PathRecord>();
                if (Path != null)
                    for (int i = 0; i < Path.Count; i++)
                    {
                        this.Path.Add(new PathRecord(
                            Path[i].X
                            , Path[i].Y
                            , Path[i].Round
                            , Path[i].FireHappened));
                    }
                this.RuleFollower = RuleFollower;
                this.Live = Live;
                this.TheRoundVictimDead = TheRoundVictimDead;
                this.HaveEscaped = HaveEscaped;
                this.Activated = Activated;
            }
        }

        public class NodeInScenario //struct setting
        {
            public List<EachVictim> VictimsAtSensor;
            public List<ExitInfo> DistanceToExits;

            //category
            public bool Exit,
                        Sensor,
                        Corridor,
                        Obstacle,
                        Activited,
                        FireSensor,
                        BoundarySensor,
                        HaveRunThisRound;

            public string PeopleAheadDirection;

            public int PeopleTemp;

            public double Weight, //Sum of weight
                          BaseWeight, //Weight of sensor without people
                          PeopleWeight,  //Weight of people at sensor
                          BasePercentageDistance, //Base percentage distance for itself
                          ShortestEuclideanDistanceToExit,
                          TempWeight, //ShortestEuclideanDistanceToExit
                          ThresholdValue, //Threshold of the node
                          CrowdRatio, //MSCG parameters
                          FEII; //MSCG parameters: Fire Event Influence Index

            public int X, Y, //Coordinates
                       TargetX, TargetY, //Coordinate of Target Exit
                       People, //People at Sensor or Corridor
                       Hop, //Hop from sensor to nearest exit
                       FireRound,
                       LeftSensorX,
                       LeftSensorY,
                       RightSensorX,
                       RightSensorY,
                       TopSensorX,
                       TopSensorY,
                       BottemSensorX,
                       BottemSensorY;
        }

        public class ExitInfo
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Hop { get; set; }
            public int ActualDistance { get; set; }
            public double EuclideanDistance { get; set; }
            public string AheadDirection { get; set; }

            public ExitInfo()
            {
                this.X = new int();
                this.Y = new int();
                this.Hop = new int();
                this.ActualDistance = new int();
                this.EuclideanDistance = new double();
            }

            public ExitInfo(int x, int y, int hop, int actualdistance, double euclideandistance)
            {
                this.X = x;
                this.Y = y;
                this.Hop = hop;
                this.ActualDistance = actualdistance;
                this.EuclideanDistance = euclideandistance;
                this.AheadDirection = null;
            }
        }

        public class FireInfo
        {
            public List<FireSensor> FireSensor;
            public List<FireSensor> WaitingFireSensor;
            public int FireSpreadTime;            
            public int StartingFireNumber;
        }

        public class FireSensor
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int FireStartRound { get; set; }

            public FireSensor()
            {
                this.X = new int();
                this.Y = new int();
                this.FireStartRound = new int();
            }
            public FireSensor(int X, int Y, int FireStartRound)
            {
                this.X = X;
                this.Y = Y;
                this.FireStartRound = FireStartRound;
            }
        }

        public class ExcelProcess
        {
            public Excel.Application ExcelApp;
            public Excel.Workbook Book;
            public Excel.Worksheet Sheet;
            public Excel.Range Range;
            public bool IfRecordXlsx;
        }

        public class ResultStorage
        {
            public int width { get; set; }
            public int height { get; set; }
            public int totalpeople { get; set; }
            public int corridorlimit { get; set; }
            public double CGAWithHighestMinusDIandSTDResult { get; set; }
            public double CGAWithSelfMinusSTDResult { get; set; }
            public double CGAWithSelfMinusDIResult { get; set; }
            public double CGAWithHighestMinusDIandSTDResultwithSelf { get; set; }
            public double CGAWithSelfMinusSTDResultwithSelf { get; set; }
            public double CGAWithSelfMinusDIResultwithSelf { get; set; }
            public double CGAWithHighestMinusDIandSTDResultWithOldDL { get; set; }
            public double CGAWithSelfMinusSTDResultWithOldDL { get; set; }
            public double CGAWithSelfMinusDIResultWithOldDL { get; set; }
            public double CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL { get; set; }
            public double CGAWithSelfMinusSTDResultwithSelfWithOldDL { get; set; }
            public double CGAWithSelfMinusDIResultwithSelfWithOldDL { get; set; }
            public double CGAWithHighestMinusDIandSTDResultWithNewDL { get; set; }
            public double CGAWithSelfMinusSTDResultWithNewDL { get; set; }
            public double CGAWithSelfMinusDIResultWithNewDL { get; set; }
            public double CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL { get; set; }
            public double CGAWithSelfMinusSTDResultwithSelfWithNewDL { get; set; }
            public double CGAWithSelfMinusDIResultwithSelfWithNewDL { get; set; }
            public double MSCG { get; set; }
            public double ESSPlus { get; set; }
            public double LEGS { get; set; }
            public double HEXInc { get; set; }
            public double ShortestPath { get; set; }

            public double CGADeathWithHighestMinusDIandSTDResult { get; set; }
            public double CGADeathWithSelfMinusSTDResult { get; set; }
            public double CGADeathWithSelfMinusDIResult { get; set; }
            public double CGADeathWithHighestMinusDIandSTDResultwithSelf { get; set; }
            public double CGADeathWithSelfMinusSTDResultwithSelf { get; set; }
            public double CGADeathWithSelfMinusDIResultwithSelf { get; set; }
            public double CGADeathWithHighestMinusDIandSTDResultWithOldDL { get; set; }
            public double CGADeathWithSelfMinusSTDResultWithOldDL { get; set; }
            public double CGADeathWithSelfMinusDIResultWithOldDL { get; set; }
            public double CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL { get; set; }
            public double CGADeathWithSelfMinusSTDResultwithSelfWithOldDL { get; set; }
            public double CGADeathWithSelfMinusDIResultwithSelfWithOldDL { get; set; }
            public double CGADeathWithHighestMinusDIandSTDResultWithNewDL { get; set; }
            public double CGADeathWithSelfMinusSTDResultWithNewDL { get; set; }
            public double CGADeathWithSelfMinusDIResultWithNewDL { get; set; }
            public double CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL { get; set; }
            public double CGADeathWithSelfMinusSTDResultwithSelfWithNewDL { get; set; }
            public double CGADeathWithSelfMinusDIResultwithSelfWithNewDL { get; set; }
            public double MSCGDeath { get; set; }
            public double ESSPlusDeath { get; set; }
            public double LEGSDeath { get; set; }
            public double HEXIncDeath { get; set; }
            public double ShortestPathDeath { get; set; }

            public ResultStorage()
            {
                this.width = new int();
                this.height = new int();
                this.totalpeople = new int();
                this.corridorlimit = new int();
                this.CGAWithHighestMinusDIandSTDResult = new double();
                this.CGAWithSelfMinusSTDResult = new double();
                this.CGAWithSelfMinusDIResult = new double();
                this.CGAWithHighestMinusDIandSTDResultwithSelf = new double();
                this.CGAWithSelfMinusSTDResultwithSelf = new double();
                this.CGAWithSelfMinusDIResultwithSelf = new double();
                this.CGAWithHighestMinusDIandSTDResultWithOldDL = new double();
                this.CGAWithSelfMinusSTDResultWithOldDL = new double();
                this.CGAWithSelfMinusDIResultWithOldDL = new double();
                this.CGAWithHighestMinusDIandSTDResultwithSelfWithOldDL = new double();
                this.CGAWithSelfMinusSTDResultwithSelfWithOldDL = new double();
                this.CGAWithSelfMinusDIResultwithSelfWithOldDL = new double();
                this.CGAWithHighestMinusDIandSTDResultWithNewDL = new double();
                this.CGAWithSelfMinusSTDResultWithNewDL = new double();
                this.CGAWithSelfMinusDIResultWithNewDL = new double();
                this.CGAWithHighestMinusDIandSTDResultwithSelfWithNewDL = new double();
                this.CGAWithSelfMinusSTDResultwithSelfWithNewDL = new double();
                this.CGAWithSelfMinusDIResultwithSelfWithNewDL = new double();
                this.MSCG = new double();
                this.ESSPlus = new double();
                this.LEGS = new double();
                this.HEXInc = new double();
                this.ShortestPath = new double();

                this.CGADeathWithHighestMinusDIandSTDResult = new double();
                this.CGADeathWithSelfMinusSTDResult = new double();
                this.CGADeathWithSelfMinusDIResult = new double();
                this.CGADeathWithHighestMinusDIandSTDResultwithSelf = new double();
                this.CGADeathWithSelfMinusSTDResultwithSelf = new double();
                this.CGADeathWithSelfMinusDIResultwithSelf = new double();
                this.CGADeathWithHighestMinusDIandSTDResultWithOldDL = new double();
                this.CGADeathWithSelfMinusSTDResultWithOldDL = new double();
                this.CGADeathWithSelfMinusDIResultWithOldDL = new double();
                this.CGADeathWithHighestMinusDIandSTDResultwithSelfWithOldDL = new double();
                this.CGADeathWithSelfMinusSTDResultwithSelfWithOldDL = new double();
                this.CGADeathWithSelfMinusDIResultwithSelfWithOldDL = new double();
                this.CGADeathWithHighestMinusDIandSTDResultWithNewDL = new double();
                this.CGADeathWithSelfMinusSTDResultWithNewDL = new double();
                this.CGADeathWithSelfMinusDIResultWithNewDL = new double();
                this.CGADeathWithHighestMinusDIandSTDResultwithSelfWithNewDL = new double();
                this.CGADeathWithSelfMinusSTDResultwithSelfWithNewDL = new double();
                this.CGADeathWithSelfMinusDIResultwithSelfWithNewDL = new double();
                this.MSCGDeath = new double();
                this.ESSPlusDeath = new double();
                this.LEGSDeath = new double();
                this.HEXIncDeath = new double();
                this.ShortestPathDeath = new double();
            }
        }

        public class PathRecord
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Round { get; set; }
            public bool FireHappened { get; set; }

            public PathRecord(int X, int Y, int Round, bool FireHappened)
            {
                this.X = X;
                this.Y = Y;
                this.Round = Round;
                this.FireHappened = FireHappened;
            }
        }

        public class PerTreeNodeSet
        {
            public int X { get; set; }
            public int Y { get; set; }
            public bool BoundarySensor { get; set; }


            public PerTreeNodeSet()
            {
                this.X = new int();
                this.Y = new int();
                this.BoundarySensor = new bool();
            }

            public PerTreeNodeSet(int X, int Y, bool BoundarySensor)
            {
                this.X = X;
                this.Y = Y;
                this.BoundarySensor = BoundarySensor;
            }
            public PerTreeNodeSet(NodeInScenario Node)
            {
                this.X = Node.X;
                this.Y = Node.Y;
                this.BoundarySensor = Node.BoundarySensor;
            }
            public override string ToString()
            {
                return X.ToString()+","+Y.ToString();
            }
        }
    }

}
