using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using WarehouseAI;

namespace WarehouseAITest
{
    [TestFixture]
    class WarehouseIOTests
    {
        public string GenerateFile(params string[] itemNames)
        {
            string path = Directory.GetCurrentDirectory() + "TestItems";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < itemNames.Length; i++)
            {
                sb.Append(i + ", " + itemNames[i] + "\n");
            }
            File.Create(path).Close();
            File.WriteAllText(path, sb.ToString());
            return path;
        }

        [Test]
        [TestCase("test", "Test", "TEST", "tEst")]
        [TestCase("t,est", "T,est", "T,EST", "t,Est")]
        [TestCase("t,e,st", "T,e,st", "T,E,ST", "t,E,st")]
        [TestCase("t,e,st,", "T,e,st,", "T,E,ST,", "t,E,st,")]
        [TestCase("t,e,st1,", "T,e,st2,", "T,E,ST3,", "t,E,st4,")]
        [TestCase("t,e,st#,", "T,e,st#,", "T,E,ST#,", "t,E,st#,")]
        public void LoadAllItemsFromFilTest(params string[] itemNames)
        {
            // Arrange 
            string path = GenerateFile(itemNames);

            // Act
            List<Item> items = WarehouseIO.LoadAllItemsFromFile(path);
            int expectedNr = items.Count;

            // Assert
            int actualNr = itemNames.Length;

            Assert.AreEqual(actualNr, expectedNr);
        }
    }
}
