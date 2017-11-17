using System;
using System.Linq;
using NUnit.Framework;
using WarehouseAI;

namespace WarehouseAITest
{
    [TestFixture]
    class ExtensionTests
    {
        [Test]
        public void PowerTest()
        {
            int[] testCase = { 1, 2, 3, 4, 5 };

            int[][] result = testCase.Power().ToArray();

            foreach (int[] ints in result)
            {
                foreach (int i in ints)
                {
                    Console.Write(i + " ");
                }
                Console.WriteLine("-");
            }

            int[][] expectedResult = new[]
            {
                new int[0],
                new []{1},
                new []{2},
                new []{3},
                new []{4},
                new []{5},
                new []{1, 2},
                new []{1, 3},
                new []{1, 4},
                new []{1, 5},
                new []{2, 3},
                new []{2, 4},
                new []{2, 5},
                new []{3, 4},
                new []{3, 5},
                new []{4, 5},
                new []{1, 2, 3},
                new []{1, 2, 4},
                new []{1, 2, 5},
                new []{1, 3, 4},
                new []{1, 3, 5},
                new []{1, 4, 5},
                new []{2, 3, 4},
                new []{2, 3, 5},
                new []{2, 4, 5},
                new []{3, 4, 5},
                new []{1, 2, 3, 4},
                new []{1, 2, 3, 5},
                new []{1, 2, 4, 5},
                new []{1, 3, 4, 5},
                new []{2, 3, 4, 5},
                new []{1, 2, 3, 4, 5}
            }.ToArray();

            Assert.AreEqual(expectedResult, result);
        }
    }
}