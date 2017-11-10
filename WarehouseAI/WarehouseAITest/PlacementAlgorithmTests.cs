using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WarehouseAI;

namespace WarehouseAITest
{
    [TestFixture]
    class PlacementAlgorithmClassTests
    {
        public static List<Item> GenerateTestData(int numberOfItems, int numberOfRelations)
        {
            List<Item> items = new List<Item>();
            for (int i = 0; i < numberOfItems; i++)
            {
                items.Add(new Item(i.ToString(), "item" + i.ToString()));
            }
            for (int i = 0; i < numberOfRelations; i++)
            {
                items[i % numberOfItems].AddOutgoingRelation(items.Find(item => item.ID == ((i + 1) % numberOfItems).ToString()));
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
        public void ImportanceAlgorithmTests(int numberOfItems, int numberOfRelations)
        {
            // Arrange 
            var items = GenerateTestData(numberOfItems, numberOfRelations);

            // Act
            float expected;
            try
            {
                expected = PlacementAlgorithmClass.ImportanceCoefficientAlgorithm(items.ToArray());
            }
            catch (Exception e)
            {
                expected = 0;
            }
            // Assert
            if (numberOfItems == 0)
            {
                Assert.Throws<ArgumentException>(() => PlacementAlgorithmClass.ImportanceCoefficientAlgorithm(items.ToArray()));
            }
            else
            {
                float actual = (float) numberOfRelations / numberOfItems;
                Assert.AreEqual(expected, actual);
            }
        }
    }
}
