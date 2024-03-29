﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WarehouseAI;
using WarehouseAI.Representation;

namespace WarehouseAITest
{
    [TestFixture]
    class ImportanceAlgorithmTests
    {
        public static List<Item> GenerateTestData(int numberOfItems, int numberOfRelations)
        {
            List<Item> items = new List<Item>();
            for (int i = 0; i < numberOfItems; i++)
            {
                items.Add(new Item(i, "item" + i.ToString()));
            }
            for (int i = 0; i < numberOfRelations; i++)
            {
                items[i % numberOfItems].AddOutgoingRelation(items.Find(item => item.Id == (i + 1) % numberOfItems));
            }
            return items;
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(5, 0)]
        [TestCase(5, 1)]
        [TestCase(5, 5)]
        [TestCase(5, 10)]
        [TestCase(5, 50)]
        [TestCase(5, 100)]
        [TestCase(10, 0)]
        [TestCase(10, 1)]
        [TestCase(10, 5)]
        [TestCase(10, 10)]
        [TestCase(10, 50)]
        [TestCase(10, 100)]
        [TestCase(50, 0)]
        [TestCase(50, 1)]
        [TestCase(50, 5)]
        [TestCase(50, 10)]
        [TestCase(50, 50)]
        [TestCase(50, 100)]
        [TestCase(100, 0)]
        [TestCase(100, 1)]
        [TestCase(100, 5)]
        [TestCase(100, 10)]
        [TestCase(100, 50)]
        [TestCase(100, 100)]
        public void ImportanceAlgorithmTest(int numberOfItems, int numberOfRelations)
        {
            // Arrange 
            List<Item> items = GenerateTestData(numberOfItems, numberOfRelations);

            // Act
            float expected;
            try
            {
                expected = Algorithms.Importance(items.ToArray());
            }
            catch
            {
                expected = 0;
            }
            // Assert
            if (numberOfItems == 0)
            {
                Assert.AreEqual(0, Algorithms.Importance(items.ToArray()));
            }
            else
            {
                float actual = (float)numberOfRelations / numberOfItems;
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
