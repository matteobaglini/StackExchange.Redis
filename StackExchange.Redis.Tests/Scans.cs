﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace StackExchange.Redis.Tests
{
    [TestFixture]
    public class Scans : TestBase
    {

        protected override string GetConfiguration()
        {
            return "ubuntu";
        }
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SetScan(bool supported)
        {
            string[] disabledCommands = supported ? null : new[] { "sscan" };
            using(var conn = Create(disabledCommands: disabledCommands))
            {
                RedisKey key = Me();
                var db = conn.GetDatabase();
                db.KeyDelete(key);

                db.SetAdd(key, "a");
                db.SetAdd(key, "b");
                db.SetAdd(key, "c");
                var arr = db.SetScan(key).ToArray();
                Assert.AreEqual(3, arr.Length);
                Assert.IsTrue(arr.Contains("a"), "a");
                Assert.IsTrue(arr.Contains("b"), "b");
                Assert.IsTrue(arr.Contains("c"), "c");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SortedSetScan(bool supported)
        {
            string[] disabledCommands = supported ? null : new[] { "zscan" };
            using (var conn = Create(disabledCommands: disabledCommands))
            {
                RedisKey key = Me();
                var db = conn.GetDatabase();
                db.KeyDelete(key);

                db.SortedSetAdd(key, "a", 1);
                db.SortedSetAdd(key, "b", 2);
                db.SortedSetAdd(key, "c", 3);

                var arr = db.SortedSetScan(key).ToArray();
                Assert.AreEqual(3, arr.Length);
                Assert.IsTrue(arr.Any(x => x.Key == "a" && x.Value == 1), "a");
                Assert.IsTrue(arr.Any(x => x.Key == "b" && x.Value == 2), "b");
                Assert.IsTrue(arr.Any(x => x.Key == "c" && x.Value == 3), "c");
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void HashScan(bool supported)
        {
            string[] disabledCommands = supported ? null : new[] { "hscan" };
            using (var conn = Create(disabledCommands: disabledCommands))
            {
                RedisKey key = Me();
                var db = conn.GetDatabase();
                db.KeyDelete(key);

                db.HashSet(key, "a", "1");
                db.HashSet(key, "b", "2");
                db.HashSet(key, "c", "3");

                var arr = db.HashScan(key).ToArray();
                Assert.AreEqual(3, arr.Length);
                Assert.IsTrue(arr.Any(x => x.Key == "a" && x.Value == "1"), "a");
                Assert.IsTrue(arr.Any(x => x.Key == "b" && x.Value == "2"), "b");
                Assert.IsTrue(arr.Any(x => x.Key == "c" && x.Value == "3"), "c");
            }
        }
    }
}
