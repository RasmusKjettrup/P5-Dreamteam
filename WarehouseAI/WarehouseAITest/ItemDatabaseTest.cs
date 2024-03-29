﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WarehouseAI;
using WarehouseAI.Representation;

namespace WarehouseAITest
{
    [TestFixture]
    class ItemDatabaseTest
    {
        public string GenerateItemFile(params string[] itemNames)
        {
            int j = 0;
            return GenerateFileFromLines("TestItems", itemNames.Select(s => j++ + ", " + s).ToArray());
        }

        public string GenerateFileFromLines(string name, params string[] lines)
        {
            string path = Directory.GetCurrentDirectory() + "/" + name + ".txt";
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                sb.Append(lines[i] + "\n");
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
        public void LoadAllItemsFromFileCountTest(params string[] itemNames)
        {
            // Arrange 
            string path = GenerateItemFile(itemNames);
            ItemDatabase itemDatabase = new ItemDatabase();

            // Act
            itemDatabase.ImportItems(path);
            int actualCount = itemDatabase.Items.Length;

            int expectedLength = itemNames.Length;

            // Assert
            Assert.AreEqual(expectedLength, actualCount);
        }

        [Test]
        [TestCase("test", "Test", "TEST", "tEst")]
        [TestCase("t,est", "T,est", "T,EST", "t,Est")]
        [TestCase("t,e,st", "T,e,st", "T,E,ST", "t,E,st")]
        [TestCase("t,e,st,", "T,e,st,", "T,E,ST,", "t,E,st,")]
        [TestCase("t,e,st1,", "T,e,st2,", "T,E,ST3,", "t,E,st4,")]
        [TestCase("t,e,st#,", "T,e,st#,", "T,E,ST#,", "t,E,st#,")]
        public void LoadAllItemsFromFileDataTest(params string[] itemNames)
        {
            // Arrange 
            string path = GenerateItemFile(itemNames);
            ItemDatabase itemDatabase = new ItemDatabase();

            // Act
            itemDatabase.ImportItems(path);
            string[] actualData = itemDatabase.Items.Select(item => item.Id + ", " + item.Name).ToArray();

            int i = 0;
            string[] expectedData = itemNames.Select(s => i++ + ", " + s).ToArray();

            // Assert
            Assert.AreEqual(expectedData, actualData);
        }

        [Test]
        [TestCase("0, 1", "1, 2", "2, 3", "3, 0")]
        [TestCase("0, 1", "0, 2", "1, 2", "1, 3", "2, 3", "2, 0", "3, 0", "3, 1")]
        [TestCase("0, 1", "0, 2", "0, 3", "1, 2", "1, 3", "1, 0", "2, 3", "2, 0", "2, 1", "3, 0", "3, 1", "3, 2")]
        [TestCase("0, 2", "1, 3", "2, 0", "3, 1")]
        [TestCase("0, 2", "0, 3", "1, 3", "1, 0", "2, 0", "2, 1", "3, 1", "3, 2")]
        [TestCase("0, 2", "1, 3", "2, 0", "3, 1")]
        public void LoadAllRelationsFromFileTest(params string[] itemRelations)
        {
            // Arrange
            string itemPath = GenerateItemFile("test", "Test", "TEST", "tEst");
            string relationPath = GenerateFileFromLines("RelationTest", itemRelations);
            ItemDatabase itemDatabase = new ItemDatabase();

            // Act
            itemDatabase.ImportItems(itemPath);
            itemDatabase.ImportRelations(relationPath);

            // Assert
            foreach (string relation in itemRelations)
            {
                string[] r = relation.Split(',').Select(s => s.Trim()).ToArray();
                Assert.Contains(itemDatabase.Items.ToList().Find(item => item.Id == int.Parse(r[1])),
                    itemDatabase.Items.ToList().Find(item => item.Id == int.Parse(r[0])).Neighbours());
            }
        }
    }
}
