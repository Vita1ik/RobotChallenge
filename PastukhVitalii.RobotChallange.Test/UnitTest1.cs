using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;
using Pastukh.Vitalii.RobotChallange;
using System.Collections.Generic;
using System.Diagnostics;

namespace PastukhVitalii.RobotChallange.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestNearestStation()
        {
            var algorithm = new PastukhVitaliiAlgorithm();
            var map = new Map();
            var stationPos = new Position(3, 2);
            var stationPos2 = new Position(5, 2);
            var station1 = new EnergyStation() { Energy = 1234, Position = stationPos, RecoveryRate = 4 };
            var station2 = new EnergyStation() { Energy = 1454, Position = stationPos2, RecoveryRate = 3 };
            map.Stations.Add(station1);
            map.Stations.Add(station2);

            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot() { Energy = 200, Position = new Position(2, 2), } };

            Assert.AreEqual(algorithm.FindNearestFreeStation(robots[0], map, robots), station1.Position);

        }
        [TestMethod]
        public void TestIsFree()
        {
            var algorithm = new PastukhVitaliiAlgorithm();

            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot() { Energy = 200, Position = new Position(2, 2), } };
            robots.Add(new Robot.Common.Robot() { Energy = 300, Position = new Position(4, 2) });

            Assert.IsTrue(algorithm.IsCellFree(new Position(5, 2), robots[0], robots));
            Assert.IsTrue(algorithm.IsCellFree(new Position(2, 2), robots[0], robots));
        }

        [TestMethod]
        public void TestDoStep()
        {
            var algorithm = new PastukhVitaliiAlgorithm();
            var map = new Map();
            var stationPos = new Position(3, 2);
            var station1 = new EnergyStation() { Energy = 1234, Position = stationPos, RecoveryRate = 4 };
            var stationPos1 = new Position(5, 3);
            var station2 = new EnergyStation() { Energy = 1234, Position = stationPos, RecoveryRate = 4 };

            map.Stations.Add(station1);

            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot() { Energy = 8000, Position = new Position(2, 2), } };
            var x = algorithm.DoStep(robots, 0, map);
            Assert.IsTrue(x is MoveCommand);

        }
        [TestMethod]

        public void TestCreateRobot()
        {
            var algorithm = new PastukhVitaliiAlgorithm();
            var map = new Map();
            map.Stations.Add(new EnergyStation() { Energy = 500, Position = new Position(7, 7) });
            map.Stations.Add(new EnergyStation() { Energy = 500, Position = new Position(8, 8) });
            map.Stations.Add(new EnergyStation() { Energy = 500, Position = new Position(9, 9) });

            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot()
                                                        { Energy = 8000, Position = new Position(1, 1) } };
            robots.Add(new Robot.Common.Robot()
            { Energy = 8000, Position = new Position(0, 0) });

            var testmethod = algorithm.DoStep(robots, 0, map);
            Assert.IsTrue(testmethod is CreateNewRobotCommand);

        }

        [TestMethod]
        public void TestDefaultEnergy()
        {
            var algorithm = new PastukhVitaliiAlgorithm();
            var map = new Map();
            map.Stations.Add(new EnergyStation() { Energy = 500, Position = new Position(7, 7) });
            map.Stations.Add(new EnergyStation() { Energy = 500, Position = new Position(8, 8) });
            map.Stations.Add(new EnergyStation() { Energy = 500, Position = new Position(9, 9) });
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot()
                                                        { Energy = 8000, Position = new Position(5, 8) } };

            var testmethod = algorithm.DoStep(robots, 0, map) as CreateNewRobotCommand;

            Assert.AreEqual(testmethod.NewRobotEnergy, 100);
        }

        [TestMethod]
        public void ChangePosition()
        {
            var algorithm = new PastukhVitaliiAlgorithm();
            var map = new Map();
            var stationPos = new Position(5, 5);
            var station1 = new EnergyStation() { Energy = 1234, Position = stationPos, RecoveryRate = 4 };
            map.Stations.Add(station1);
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot()
                                                        { Energy = 63, Position = new Position(5, 8) } };
            var testmethod = algorithm.DoStep(robots, 0, map) as MoveCommand;

            Assert.AreEqual(map.Stations[0].Position, testmethod.NewPosition);

        }

        [TestMethod]
        public void ChangePositionNextStep()
        {
            var algorithm = new PastukhVitaliiAlgorithm();
            var map = new Map();
            var stationPos = new Position(5, 5);
            var station1 = new EnergyStation() { Energy = 1234, Position = stationPos, RecoveryRate = 4 };
            map.Stations.Add(station1);
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot()
                                                        { Energy = 500, Position = new Position(15, 5) } };
            var testmethod = algorithm.DoStep(robots, 0, map) as MoveCommand;
            Assert.AreEqual(new Position(9, 5), testmethod.NewPosition);
        }

        [TestMethod]
        public void TimeTest()
        {
            var algorithm = new PastukhVitaliiAlgorithm();
            var map = new Map();
            var stationPos = new Position(5, 5);
            var station1 = new EnergyStation() { Energy = 1234, Position = stationPos, RecoveryRate = 4 };
            map.Stations.Add(station1);
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot()
                                                        { Energy = 500, Position = new Position(15, 5) } };
            var time = new Stopwatch();
            time.Start();
            var testmethod = algorithm.DoStep(robots, 0, map) as MoveCommand;
            time.Stop();
            Assert.IsTrue(time.Elapsed < new TimeSpan(0, 0, 1));
        }


        [TestMethod]
        public void TestGetNextPosition()
        {
            Position pos = new Position(5, 5);
            Position pos2 = new Position(6, 6);
            var algorithm = new PastukhVitaliiAlgorithm();
            int i = 0;
            do
            {
                i++;
                pos2 = algorithm.GetNextPosition(pos2, pos, 1);
            } while (i < 100);
            Assert.AreEqual(pos, pos2);
        }


        [TestMethod]
        public void TestFreeStationNull()
        {
            var algorithm = new PastukhVitaliiAlgorithm();
            var map = new Map();
            map.Stations.Add(new EnergyStation() { Energy = 500, Position = new Position(7, 7) });
            var robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot()
                                                        { Energy = 500, Position = new Position(7, 7) } };
            robots.Add(new Robot.Common.Robot()
            { Energy = 8000, Position = new Position(5, 5) });

            Assert.IsNull(algorithm.FindNearestFreeStation(robots[1], map, robots));
        }
    }
}
