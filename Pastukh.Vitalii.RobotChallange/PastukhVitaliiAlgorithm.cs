using System;
using System.Collections.Generic;
using Robot.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pastukh.Vitalii.RobotChallange
{
    public class DistanceHelper
    {
        public static int FindDistance(Position a, Position b)
        {
            return (int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
    class StationInfo
    {
        public StationInfo(Position Pos, int RecRate)
        {
            this.Pos = Pos;
            this.RecRate = RecRate;
        }

        public Position Pos;
        public int RecRate;

        public override string ToString()
        {
            return "StationPosition: (" + Pos.X + ";" + Pos.Y + ")" + "RecoveryRate: " + RecRate;
        }
    }
    class Aim
    {
        public Position station;
        public bool inProgress;

        public Aim(Position station, bool inProgress)
        {
            this.station = station;
            this.inProgress = inProgress;
        }
    }
    class ListAim
    {
        public List<Aim> listAim = new List<Aim>();
        public List<Aim> listMyStation = new List<Aim>();
        List<Position> temp = new List<Position>();

        public ListAim()
        {

        }

        public ListAim(List<StationInfo> listPosition)
        {
            for (int i = 0; i < listPosition.Count; i++)
                listAim.Add(new Aim(listPosition[i].Pos, false));
        }

        public bool SetAim(Aim aim)
        {
            for (int i = 0; i < listAim.Count; i++)
            {
                if (listAim[i].station == aim.station && listAim[i].inProgress == false)
                {
                    listAim[i].inProgress = true;
                    return true;
                }
            }
            return false;
        }

        public void Refresh()
        {
            for (int i = 0; i < listAim.Count; i++)
            {
                ChangeAim(listMyStation[i].station, false);
            }
        }

        public void ChangeAim(Position pos, bool condition)
        {
            for (int i = 0; i < listAim.Count; i++)
            {
                if (listAim[i].station == pos)
                    listAim[i].inProgress = condition;
            }
        }

        public void GetFreeAim()
        {
            for (int i = 0; i < listAim.Count; i++)
            {
                ChangeAim(listAim[i].station, false);
            }

            for (int i = 0; i < listAim.Count; i++)
            {
                for (int j = 0; j < listMyStation.Count; j++)
                {
                    if (listAim[i].station == listMyStation[j].station)
                    {
                        ChangeAim(listAim[i].station, true);
                        break;
                    }
                }
            }
        }

        private List<Position> Compare(List<Position> list)
        {
            bool tmp = true;
            for (int i = 0; i < listMyStation.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (listMyStation[i].station == list[j])
                    {
                        tmp = false;
                        break;
                    }
                }
                if (tmp)
                {
                    temp.Add(listMyStation[i].station);
                }
                tmp = true;

            }

            foreach (var item in temp)
            {
                listMyStation.Remove(new Aim(item, true));
            }
            return temp;
        }

        public void DeleteErrorAim(List<Position> list)
        {
            list = Compare(list);
            if (list.Count != 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < listAim.Count; j++)
                    {
                        if (list[i] == listAim[j].station)
                        {
                            ChangeAim(listAim[j].station, false);
                        }
                    }
                }
            }
            temp.Clear();
        }

        public bool IsMyStation(Position position)
        {
            for (int i = 0; i < listMyStation.Count; i++)
            {
                if (listMyStation[i].station == position)
                    return true;
            }
            return false;
        }

    }
    public class PastukhVitaliiAlgorithm : IRobotAlgorithm
    {
        List<StationInfo> listStationPosition = new List<StationInfo>();
        List<Position> listEnemyStationPosition = new List<Position>();
        ListAim li = new ListAim();
        public int Round { get; set; }
        public PastukhVitaliiAlgorithm()
        {
            Logger.OnLogRound += Logger_OnLogRound;
        }

        private void Logger_OnLogRound(object sender, LogRoundEventArgs e)
        {
            Round++;
        }
        public static int counter = 0;
        public static int counter2 = 0;
        public static int robotCount = 10;
        public static bool counter3 = true;

        public string Author {
            get { return "Pastukh Vitalii"; }
        }
        public string Description
        {
            get { return "Algorithm by Pastukh Vitalii"; }
        }
        public Position FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map,
IList<Robot.Common.Robot> robots)
        {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations)


            {
                if (IsStationFree(station, movingRobot, robots))
                {
                    int d = DistanceHelper.FindDistance(station.Position, movingRobot.Position);
                    if (d < minDistance)
                    {
                        minDistance = d;
                        nearest = station;
                    }
                }
            }
            return nearest == null ? null : nearest.Position;
        }


        public bool IsStationFree(EnergyStation station, Robot.Common.Robot movingRobot,
IList<Robot.Common.Robot> robots)
        {
            return IsCellFree(station.Position, movingRobot, robots);
        }

        public bool IsCellFree(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
            {
                if (robot != movingRobot)
                {
                    if (robot.Position == cell)
                        return false;
                }
            }
            return true;
        }

        public bool IsCellFreeFromMyRobot(Position cell, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            foreach (var robot in robots)
            {
                if (robot != movingRobot)
                {
                    if (robot.Position == cell && (String.Compare(robot.Owner.Name, movingRobot.Owner.Name) == 0))
                        return false;
                }
            }
            return true;
        }


        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            if (counter3)
            {
                FindAllStation(map);
                li = new ListAim(listStationPosition);
                counter3 = false;
            }
            if (counter % 2 == 0)
            {
                li.GetFreeAim();
            }


            Robot.Common.Robot movingRobot = robots[robotToMoveIndex];
            li.DeleteErrorAim(FindAllMyStation(movingRobot, robots));
            Position nearestFreeStation = FindNearestFreeStation(movingRobot, map, robots);
            Position movingRobotPosition = movingRobot.Position;
            Position nearestEnemyStation = FindNearEnemyStation(movingRobot, robots);
            Position nearestAvailableStation = FindNearAvailableStation(movingRobot, robots);


            int x = 8;
            if (DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition) <= 200 && nearestAvailableStation != null && robotCount < 200)
            {
                if (nearestFreeStation != null)
                {
                    if (Math.Sqrt(DistanceHelper.FindDistance(nearestFreeStation, movingRobotPosition)) <= 7 && (movingRobot.Energy >= (counter <= x ? 300 : 450)))
                    {
                        robotCount++;
                        li.SetAim(new Aim(nearestAvailableStation, true));
                        return new CreateNewRobotCommand() { NewRobotEnergy = (counter <= x ? 100 : 250) };
                    }

                    else if (Math.Sqrt(DistanceHelper.FindDistance(nearestFreeStation, movingRobotPosition)) < 12 && movingRobot.Energy > (counter <= x ? 350 : 500) && counter < 40)
                    {
                        robotCount++;
                        return new CreateNewRobotCommand() { NewRobotEnergy = (counter <= x ? 150 : 300) };
                    }
                }

                if (Math.Sqrt(DistanceHelper.FindDistance(nearestEnemyStation, movingRobotPosition)) <= 7 && movingRobot.Energy >= (counter <= x ? 400 : 500))
                {
                    robotCount++;
                    li.SetAim(new Aim(nearestAvailableStation, true));
                    return new CreateNewRobotCommand() { NewRobotEnergy = (counter <= x ? 200 : 300) };
                }
                else if (Math.Sqrt(DistanceHelper.FindDistance(nearestEnemyStation, movingRobotPosition)) < 12 && movingRobot.Energy > (counter <= x ? 450 : 600) && counter < 40)
                {
                    robotCount++;
                    return new CreateNewRobotCommand() { NewRobotEnergy = (counter <= x ? 250 : 400) };
                }

            }
            else if (DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition) <= 625 && nearestAvailableStation != null && robotCount < 200 && counter < 40)
            {
                if (nearestFreeStation != null)
                {
                    if (Math.Sqrt(DistanceHelper.FindDistance(nearestFreeStation, movingRobotPosition)) < 25 && movingRobot.Energy > 700 && counter <= 35)
                    {
                        robotCount++;
                        return new CreateNewRobotCommand() { NewRobotEnergy = 400 };
                    }
                }
                else if (Math.Sqrt(DistanceHelper.FindDistance(nearestEnemyStation, movingRobotPosition)) < 20 && movingRobot.Energy > 700 && counter <= 35)
                {
                    robotCount++;
                    return new CreateNewRobotCommand() { NewRobotEnergy = 450 };
                }
            }


            for (int i = 0; i < listStationPosition.Count; i++)
            {
                if (movingRobotPosition == listStationPosition[i].Pos)
                {
                    if (!li.IsMyStation(listStationPosition[i].Pos))
                    {
                        li.listMyStation.Add(new Aim(listStationPosition[i].Pos, true));
                        return new CollectEnergyCommand();
                    }
                    else
                    {
                        return new CollectEnergyCommand();
                    }
                }
            }
            if (nearestFreeStation != null)
            {
                if (Math.Sqrt(DistanceHelper.FindDistance(nearestFreeStation, movingRobotPosition)) <= 12 && movingRobot.Energy - (DistanceHelper.FindDistance(nearestFreeStation, movingRobotPosition)) > 0)
                {
                    li.SetAim(new Aim(nearestFreeStation, true));
                    return new MoveCommand() { NewPosition = nearestFreeStation };
                }
            }

            if (Math.Sqrt(DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition)) <= 12 && movingRobot.Energy - (DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition) + 50) > 0)
            {
                li.SetAim(new Aim(nearestAvailableStation, true));
                return new MoveCommand() { NewPosition = nearestAvailableStation };
            }
            if (Math.Sqrt(DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition)) < 20 && movingRobot.Energy >= 300)
            {
                return new MoveCommand() { NewPosition = GetNextPosition(movingRobotPosition, nearestAvailableStation, (movingRobot.Energy / DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition)) > 1 ? movingRobot.Energy / (int)DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition) + 4 : 1) };
            }
            if (Math.Sqrt(DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition)) < 30 && movingRobot.Energy >= 200)
            {
                return new MoveCommand() { NewPosition = GetNextPosition(movingRobotPosition, nearestAvailableStation, (movingRobot.Energy / DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition)) > 1 ? movingRobot.Energy / (int)DistanceHelper.FindDistance(nearestAvailableStation, movingRobotPosition) + 1 : 1) };
            }
            else
                return new MoveCommand() { NewPosition = GetNextPosition(movingRobotPosition, nearestAvailableStation, 1) };
        }

        public Position GetNextPosition(Position robotPosition, Position nextPosition, int step)
        {
            Position x = new Position(nextPosition.X, nextPosition.Y);
            if (robotPosition.X != nextPosition.X)
            {
                if (robotPosition.X > nextPosition.X && robotPosition.X - nextPosition.X >= step)
                    x.X = robotPosition.X - step;
                if (robotPosition.X < nextPosition.X && nextPosition.X - robotPosition.X >= step)
                    x.X = robotPosition.X + step;
            }

            if (robotPosition.Y != nextPosition.Y)
            {
                if (robotPosition.Y > nextPosition.Y && robotPosition.Y - nextPosition.Y >= step)
                    x.Y = robotPosition.Y - step;
                if (robotPosition.Y < nextPosition.Y && nextPosition.Y - robotPosition.Y >= step)
                    x.Y = robotPosition.Y + step;
            }
            return x;
        }

        public void FindAllStation(Map map)
        {
            for (int i = 0; i < map.Stations.Count; i++)
            {
                listStationPosition.Add(new StationInfo(map.Stations[i].Position, map.Stations[i].RecoveryRate));
            }
        }

        public Position FindNearEnemyStation(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            Position stationPosition = null;
            for (int i = 0; i < listStationPosition.Count; i++)
            {
                if (IsCellFreeFromMyRobot(listStationPosition[i].Pos, movingRobot, robots))
                {
                    if (stationPosition != null)
                    {
                        if (DistanceHelper.FindDistance(movingRobot.Position, listStationPosition[i].Pos) < DistanceHelper.FindDistance(movingRobot.Position, stationPosition))
                            stationPosition = listStationPosition[i].Pos;
                    }
                    else
                        stationPosition = listStationPosition[i].Pos;
                }
            }
            return stationPosition;
        }

        public Position FindNearAvailableStation(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            Position stationPosition = null;
            for (int i = 0; i < li.listAim.Count; i++)
            {
                if (IsCellFreeFromMyRobot(li.listAim[i].station, movingRobot, robots) && li.listAim[i].inProgress == false)
                {
                    if (stationPosition != null)
                    {
                        if (DistanceHelper.FindDistance(movingRobot.Position, li.listAim[i].station) < DistanceHelper.FindDistance(movingRobot.Position, stationPosition))
                            stationPosition = li.listAim[i].station;
                    }
                    else
                        stationPosition = li.listAim[i].station;
                }
            }
            return stationPosition;
        }

        public List<Position> FindAllMyStation(Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots)
        {
            List<Position> temp = new List<Position>();

            for (int i = 0; i < listStationPosition.Count; i++)
            {
                for (int j = 0; j < robots.Count; j++)
                {
                    if (listStationPosition[i].Pos == robots[j].Position && robots[j].Owner.Name == movingRobot.Owner.Name)
                    {
                        temp.Add(listStationPosition[i].Pos);
                    }
                }
            }

            return temp;
        }
        /*
        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            var myRobot = robots[robotToMoveIndex];
            var newPosition = myRobot.Position;
            newPosition.X = newPosition.X + 1;
            newPosition.Y = newPosition.Y + 2;
            return new MoveCommand() { NewPosition = newPosition };
        }*/
    }
}
