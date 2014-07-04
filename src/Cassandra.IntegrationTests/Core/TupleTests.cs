﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Cassandra.IntegrationTests.Core
{
    [TestCassandraVersion(2, 1)]
    public class TupleTests : SingleNodeClusterTest
    {
        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();
            if (Options.Default.CassandraVersion >= new Version(2, 1))
            {
                const string cqlTable1 = "CREATE TABLE users_tuples (id int PRIMARY KEY, phone tuple<text, text, int>, achievements list<tuple<text,int>>)";

                Session.Execute(cqlTable1);
            }
        }

        [Test]
        public void DecodeTupleValuesSingleSample()
        {
            Session.Execute(
                "INSERT INTO users_tuples (id, phone) values " +
                "(1, " +
                "('home', '1234556', 1))");
            var row = Session.Execute("SELECT * FROM users_tuples WHERE id = 1").First();
            var phone1 = row.GetValue<Tuple<string, string, int>>("phone");
            var phone2 = row.GetValue<Tuple<string, string, int>>("phone");
            Assert.IsNotNull(phone1);
            Assert.IsNotNull(phone2);
            Assert.AreEqual("home", phone1.Item1);
            Assert.AreEqual("1234556", phone1.Item2);
            Assert.AreEqual(1, phone1.Item3);
        }

        [Test]
        public void DecodeTupleNullValuesSingleSample()
        {
            Session.Execute(
                "INSERT INTO users_tuples (id, phone) values " +
                "(11, " +
                "('MOBILE'))");
            var row = Session.Execute("SELECT * FROM users_tuples WHERE id = 11").First();
            var phone1 = row.GetValue<Tuple<string, string, int>>("phone");
            var phone2 = row.GetValue<Tuple<string, string, int>>("phone");
            Assert.IsNotNull(phone1);
            Assert.IsNotNull(phone2);
            Assert.AreEqual("MOBILE", phone1.Item1);
            Assert.AreEqual(null, phone1.Item2);
            Assert.AreEqual(0, phone1.Item3);
        }

        [Test]
        public void DecodeTupleAsNestedSample()
        {
            Session.Execute(
                "INSERT INTO users_tuples (id, achievements) values " +
                "(21, " +
                "[('Tenacious', 100), ('Altruist', 12)])");
            var row = Session.Execute("SELECT * FROM users_tuples WHERE id = 21").First();

            var achievements = row.GetValue<List<Tuple<string, int>>>("achievements");
            Assert.IsNotNull(achievements);
        }
    }
}
