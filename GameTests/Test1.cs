using NUnit.Framework;
using System.IO;
using System;
using Program;

namespace GameTests
{
    public class Tests
    {
        [Test]
        public void AddLaserBolt()
        {
            LaserBolt testLaserBolt = new LaserBolt(1, 2);
            Assert.AreEqual((1,2), (testLaserBolt.GetY(), testLaserBolt.GetX()));
        }

        [Test]
        public void AddLaserBolt2()
        {
            LaserBolt test2LaserBolt = new LaserBolt(0, 3);
            Assert.AreEqual((0,3), (testLaserBolt.GetY(), testLaserBolt.GetX()));
        }

        [Test]
        public void Add2BoltsToList()
        {
            List<LaserBolt> LaserBoltListTest = [];
            LaserBolt testLaserBolt = new LaserBolt(1, 2);
            LaserBolt test2LaserBolt = new LaserBolt(0, 3);
            LaserBoltListTest.Add(testLaserBolt);
            LaserBoltListTest.Add(test2LaserBolt);
            Assert.AreEqual((testLaserBolt, test2LaserBolt), (LaserBoltListTest[0], LaserBoltListTest[1]));
        }

        [Test]
        public void BoltTravel()
        {
            char [,] scene = new char [4,4] {
            {'0', '1', '2', '3'} ,
            {'4', '5', '6', '7'} ,
            {'8', '9', '10', '11'} ,
            {'12', '13', '14', '15'}
            };
            LaserBolt testLaserBolt = new LaserBolt(1, 2);
            testLaserBolt.Travel(4, scene);
            Assert.AreEqual(3, testLaserBolt.GetY());
        }

        [Test]
        public void BoltActivePassCreation()
        {
            LaserBolt testLaserBolt = new LaserBolt(1, 2);
            Assert.AreEqual(true, testLaserBolt.IsActive());
        }

        [Test]
        public void AddAsteroid()
        {
            Asteroid testAsteroid = new Asteroid(3, 2);
            Assert.AreEqual((4,3), (testAsteroid.GetY(), testAsteroid.GetX()));
        }

        [Test]
        public void AsteroidActiveOnCreation()
        {
            Asteroid testAsteroid = new Asteroid(3, 2);
            Assert.AreEqual(true, testAsteroid.IsActive());
        }

        [Test]
        public void AddAsteroidToList()
        {
            Asteroid testAsteroid = new Asteroid(3, 2);
            List<Asteroid> AsteroidListTest = [];
            AsteroidListTest.Add(testAsteroid);
            Assert.AreEqual(testAsteroid, AsteroidListTest[0]);
        }

        [Test]
        public void AddTwoAsteroidsRemoveOne()
        {
            Asteroid testAsteroid = new Asteroid(3, 2);
            Asteroid test2Asteroid = new Asteroid(3, 1);
            List<Asteroid> AsteroidListTest = [];
            AsteroidListTest.Add(testAsteroid);
            AsteroidListTest.Add(test2Asteroid);
            AsteroidListTest.Remove(testAsteroid);
            Assert.AreEqual(test2Asteroid, AsteroidListTest[0]);
        }

        [Test]
        public void DestroyAsteroid()
        {
            //Scene is displayed from bottom to top, meaning that 'highest floor' in real array
            //would be actualy displayed on the bottom on the screen during the game
            char [,] scene = new char [4,4] {
            {'0', '1', '2', '3'} ,
            {'4', '5', '6', '7'} ,
            {'8', '9', '10', '11'} ,
            {'12', '*', '14', '15'}
            };
            Asteroid testAsteroid = new Asteroid(3, 1);
            testAsteroid.Destroy(scene);
            Assert.AreEqual((false, ' '), (testAsteroid.IsActive(), scene[3,1]));
        }

        [Test]
        public void CheckAsteroidCollisionWithPlayerPresent()
        {
            //int XPlayerCoordinate = 3;
            //height is always 1, as player cannot 'move' up during the game
            Asteroid testAsteroid = new Asteroid(1, 3);
            Assert.AreEqual(true, testAsteroid.CollideWithPlayer(3));
        }

        [Test]
        public void CheckAsteroidCollisionWithPlayerNotPresentRightSide()
        {
            //Asteroid flies right side next to the player and is avoided
            //int XPlayerCoordinate = 2;
            //height is always 1, as player cannot 'move' up during the game
            Asteroid testAsteroid = new Asteroid(1, 3);
            Assert.AreEqual(false, testAsteroid.CollideWithPlayer(2));
        }

        [Test]
        public void CheckAsteroidCollisionWithPlayerNotPresentLeftSide()
        {
            //Asteroid flies left side next to the player and is avoided
            //int XPlayerCoordinate = 3;
            //height is always 1, as player cannot 'move' up during the game
            Asteroid testAsteroid = new Asteroid(1, 2);
            Assert.AreEqual(false, testAsteroid.CollideWithPlayer(3));
        }
    }
}