using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using NUnit.Framework.Internal;
using WarehouseAI;
using WarehouseAI.Representation;

namespace WarehouseAITest
{
    [TestFixture]
    class WeightAlgorithmTests
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

        void Generate5Nodes(WarehouseRepresentation rep)
        {
            Node shelf1 = new Shelf
            {
                Id = 2,
                X = 1,
                Y = 1
            };
            Node shelf2 = new Shelf
            {
                Id = 1,
                X = 2,
                Y = 2
            };
            Node node1 = new Node()
            {
                Id = 0,
                X = 1,
                Y = 2,
            };
            Node node2 = new Node()
            {
                Id = 3,
                X = 2,
                Y = 1,
            };
            Node node3 = new Node()
            {
                Id = 4,
                X = 1,
                Y = 3,
            };

            rep.AddNode(node1); // 0
            rep.AddNode(shelf1, 0); // 2
            rep.AddNode(shelf2, 0); // 1
            rep.AddNode(node2, 0); // 3
            rep.AddNode(node3, 1); // 4
        }

        [Test]
        public void WeightAlgorithmTest_GiveItems()
        {
            // Arrange
            WarehouseRepresentation rep = new WarehouseRepresentation();
            Generate5Nodes(rep);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();
            idb.ImportItems(itemPath);
            rep.ItemDatabase = idb;
            Item[] items = idb.Items;
            

            // Act
            rep.Inintialize();
            Algorithms.InitializeCache(idb);
            ((Shelf)rep.Nodes.First(n => n.Id == 2)).AddBook(items[0]);
            ((Shelf)rep.Nodes.First(n => n.Id == 1)).AddBook(items[1]);
            float expected = 4;

            // Assert
            Node[] graph = rep.Nodes;
            Algorithms.InitializeWeight(graph);
            float actual = Algorithms.Weight(items);
            Assert.AreEqual(expected, actual);
        }

        void Generate10NodesWithLargeCoordinates(WarehouseRepresentation rep)
        {
            Node node0 = new Node()
            {
                Id = 0,
                X = 0,
                Y = 0,
            };
            Node node1 = new Shelf()
            {
                Id = 1,
                X = 12.4f,
                Y = 2,
            };
            Node node2 = new Node()
            {
                Id = 2,
                X = 20,
                Y = 10,
            };
            Node node3 = new Node()
            {
                Id = 3,
                X = 15,
                Y = 30,
            };
            Node node4 = new Shelf
            {
                Id = 4,
                X = 121,
                Y = 1
            };
            Node node5 = new Shelf
            {
                Id = 5,
                X = 239,
                Y = 432
            };
            Node node6 = new Node()
            {
                Id = 6,
                X = 19,
                Y = 9,
            };
            Node node7 = new Node()
            {
                Id = 7,
                X = 90,
                Y = 203,
            };
            Node node8 = new Shelf()
            {
                Id = 8,
                X = 123,
                Y = 14,
            };
            Node node9 = new Shelf()
            {
                Id = 9,
                X = 199,
                Y = 332,
            };


            rep.AddNode(node0); 
            rep.AddNode(node1); 
            rep.AddNode(node2, 0); 
            rep.AddNode(node3, 0); 
            rep.AddNode(node4, 2); 
            rep.AddNode(node5, 3); 
            rep.AddNode(node6, 0, 2, 3); 
            rep.AddNode(node7, 1, 4, 6); 
            rep.AddNode(node8, 5, 6, 7); 
            rep.AddNode(node9, 0, 3); 
        }

        [Test]
        public void WeightAlgorithmTest_TestWith5Nodes5ShelvesWithBigDifferencesInPositions()
        {
            // Arrange
            WarehouseRepresentation rep = new WarehouseRepresentation();
            Generate10NodesWithLargeCoordinates(rep);

            string itemPath = GenerateItemFile("item1", "item2", "item3", "item4", "item5");
            ItemDatabase idb = new ItemDatabase();
            idb.ImportItems(itemPath);
            rep.ItemDatabase = idb;
            Item[] items = idb.Items;


            // Act
            rep.Inintialize();
            Algorithms.InitializeCache(idb);
            ((Shelf)rep.Nodes.First(n => n.Id == 9)).AddBook(items[0]);
            ((Shelf)rep.Nodes.First(n => n.Id == 9)).AddBook(items[3]);
            ((Shelf)rep.Nodes.First(n => n.Id == 5)).AddBook(items[0]);
            ((Shelf)rep.Nodes.First(n => n.Id == 5)).AddBook(items[1]);
            ((Shelf)rep.Nodes.First(n => n.Id == 4)).AddBook(items[0]);
            ((Shelf)rep.Nodes.First(n => n.Id == 4)).AddBook(items[2]);
            ((Shelf)rep.Nodes.First(n => n.Id == 4)).AddBook(items[1]);
            ((Shelf)rep.Nodes.First(n => n.Id == 8)).AddBook(items[0]);
            ((Shelf)rep.Nodes.First(n => n.Id == 8)).AddBook(items[3]);
            ((Shelf)rep.Nodes.First(n => n.Id == 1)).AddBook(items[0]);
            ((Shelf)rep.Nodes.First(n => n.Id == 1)).AddBook(items[4]);
            float expected = 669.77f;

            // Assert
            Node[] graph = rep.Nodes;
            Algorithms.InitializeWeight(graph);
            float actual = Algorithms.Weight(items);
            float allowedDeviation = 108.6f / 2; // The allowed deviation is the smallest possible distance between two shelves divided by 2.
            Assert.LessOrEqual(actual - allowedDeviation, expected);

        }
    }
}