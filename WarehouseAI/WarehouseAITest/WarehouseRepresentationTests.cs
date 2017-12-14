using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WarehouseAI.Representation;

namespace WarehouseAITest
{
    [TestFixture]
    class WarehouseRepresentationTests
    {
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
        public void WarehouseRepTest_Evaluate_DoesNotThrow()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.Evaluate());
        }

        [Test]
        public void WarehouseRepTest_Initialize_DoesNotThrowException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            

            // Assert;
            Assert.DoesNotThrow(() => warehouseRepresentation.Initialize());
        }


        [Test]
        public void WarehouseRepTest_Initialize_NoItemDatabaseAdded_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);

            // Assert
            Assert.Throws<NullReferenceException>(() => warehouseRepresentation.Initialize());
        }

        [Test]
        public void WarehouseRepTest_AddNode_RegularNodeAdded_DoesNotThrow()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            Node node = new Node
            {
                Id = 2,
                X = 1,
                Y = 1
            };
            

            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node));
        }

        [Test]
        public void WarehouseRepTest_AddNode_ShelfNodeAdded_DoesNotThrow()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            Node node = new Shelf
            {
                Id = 2,
                X = 1,
                Y = 1
            };


            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node));
        }

        [Test]
        public void WarehouseRepTest_AddNode_ShelfAndRegularNodeAdded_DoesNotThrow()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            Node node = new Shelf
            {
                Id = 2,
                X = 1,
                Y = 1
            };
            Node node1 = new Node
            {
                Id = 1,
                X = 2,
                Y = 1
            };


            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node));
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node1));
        }

        [Test]
        public void WarehouseRepTest_AddNode_ShelfAndRegularNodeAdded_NodesAreNeighbours_DoesNotThrow()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            Node node = new Shelf
            {
                Id = 2,
                X = 1,
                Y = 1
            };
            Node node1 = new Node
            {
                Id = 1,
                X = 2,
                Y = 1
            };


            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node));
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node1, 2));
        }

        [Test]
        public void WarehouseRepTest_AddNode_ShelfAndRegularNodeAdded_NodeIsAddedTwice_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            Node node = new Shelf
            {
                Id = 2,
                X = 1,
                Y = 1
            };


            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node));
            Assert.Throws<ArgumentException>(() => warehouseRepresentation.AddNode(node));
        }

        [Test]
        public void WarehouseRepTest_AddNode_ShelfAndRegularNodeAdded_NodeIsOwnNeighbour_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            Node node = new Shelf
            {
                Id = 2,
                X = 1,
                Y = 1
            };


            // Assert
            Assert.Throws<NullReferenceException>(() => warehouseRepresentation.AddNode(node, 2));
        }

        [Test]
        public void WarehouseRepTest_AddNode_ShelfAndRegularNodeAdded_NodeWithSameIdIsAlreadyAdded_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            Node node = new Shelf
            {
                Id = 2,
                X = 1,
                Y = 1
            };
            Node node1 = new Node
            {
                Id = 2,
                X = 2,
                Y = 1
            };


            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node));
            Assert.Throws<ArgumentException>(() => warehouseRepresentation.AddNode(node1, 2));
        }

        [Test]
        public void WarehouseRepTest_AddNode_ShelfAndRegularNodeAdded_NodeWithSamePlacementIsAlreadyAdded_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;

            Node node = new Shelf
            {
                Id = 2,
                X = 1,
                Y = 1
            };
            Node node1 = new Node
            {
                Id = 1,
                X = 1,
                Y = 1
            };

            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node));
            Assert.DoesNotThrow(() => warehouseRepresentation.AddNode(node1, 2));
        }
        
        [Test]
        public void WarehouseRepTest_AddBooks_AddOneBook()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();
            int expected = 1;

            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddBooks(idb.Items.First(b => b.Id == 0)));
            int actual = warehouseRepresentation.Nodes.Where(n => n is Shelf).Count(s => ((Shelf)s).Contains(idb.Items.First(b => b.Id == 0)));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WarehouseRepTest_AddBooks_AddTwoBooks()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();
            int expected = 2;

            // Assert
            Assert.DoesNotThrow(() => warehouseRepresentation.AddBooks(idb.Items.First(b => b.Id == 0), idb.Items.First(b => b.Id == 0)));
            int actual = warehouseRepresentation.ItemDatabase.Items.Length;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WarehouseRepTest_AddBooks_AddNoBooks_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();

            // Assert
            Assert.Throws<ArgumentException>(() => warehouseRepresentation.AddBooks());
        }

        [Test]
        public void WarehouseRepTest_AddBooks_AddNullBooks_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();

            // Assert
            Assert.Throws<NullReferenceException>(() => warehouseRepresentation.AddBooks(null));
        }

        [Test]
        public void WarehouseRepTest_AddBooks_AddNonexisting_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();

            // Assert
            Assert.Throws<ArgumentException>(() => warehouseRepresentation.AddBooks(new Item(234,"Nonexisting book")));
        }

        public void WarehouseRepTest_AddBooks_AddNonexistingAndExisting_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();

            // Assert
            Assert.Throws<ArgumentException>(() => warehouseRepresentation.AddBooks(idb.Items.First(b => b.Id == 0), new Item(234, "Nonexisting book")));
        }

        [Test]
        public void WarehouseRepTest_AddBooks_AddMultipleNonexisting_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();

            // Assert
            Assert.Throws<ArgumentException>(() => warehouseRepresentation.AddBooks(new Item(234, "Nonexisting book"), new Item(2342, "Nonexisting book2")));
        }

        [Test]
        public void WarehouseRepTest_AddBook_SmallNetworkAdded1Item_NumberOfItemsInNetworkEquals1Expected()
        {
            // Arrange
            WarehouseRepresentation rep = new WarehouseRepresentation();
            ItemDatabase idb = new ItemDatabase();
            idb.AddBook(new Item(0, "item1"));
            rep.ItemDatabase = idb;

            // Act
            rep.AddNode(new Node { Id = 0, X = 0, Y = 0 });
            rep.AddNode(new Shelf { Id = 1, X = 1, Y = 1 }, 0);
            rep.AddNode(new Shelf { Id = 2, X = 2, Y = 5 }, 1);
            int expected = 1;
            rep.Initialize();


            // Assert
            Assert.DoesNotThrow(() => rep.AddBook(idb.Items.First(b => b.Id == 0)));
            int actual = rep.Nodes.Where(n => n is Shelf).Count(s => ((Shelf)s).Contains(idb.Items.First(b => b.Id == 0)));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WarehouseRepTest_AddBook_SmallNetworkSingleBookAdded_DropoffPointIsShelf_NoErrorExpected()
        {
            // Arrange
            WarehouseRepresentation rep = new WarehouseRepresentation();
            ItemDatabase idb = new ItemDatabase();
            idb.AddBook(new Item(0, "item1"));
            rep.ItemDatabase = idb;

            // Act
            rep.AddNode(new Shelf { Id = 0, X = 0, Y = 0 });
            rep.AddNode(new Shelf { Id = 1, X = 1, Y = 1 }, 0);
            rep.AddNode(new Shelf { Id = 2, X = 2, Y = 5 }, 1);
            int expected = 1;
            rep.Initialize();

            // Assert
            Assert.DoesNotThrow(() => rep.AddBook(idb.Items.First(b => b.Id == 0)));
            int actual = rep.Nodes.Where(n => n is Shelf).Count(s => ((Shelf)s).Contains(idb.Items.First(b => b.Id == 0)));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        [TestCase(5, "b0", "b1", "b1", "b0", "b2")]
        [TestCase(5, "b0", "b1", "b2", "b3", "b4")]
        [TestCase(5, "b0", "b0", "b0", "b0", "b0")]
        [TestCase(10, "b0", "b1", "b1", "b0", "b2", "b3", "b2", "b0", "b3", "b2")]
        [TestCase(10, "b0", "b1", "b2", "b0", "b3", "b4", "b2", "b5", "b3", "b2")]
        [TestCase(10, "b0", "b0", "b0", "b0", "b0", "b0", "b0", "b0", "b0", "b0")]
        public void WarehouseRepTest_AddBook_MutipleAddBookCalls_CorrectNumberOfBooksAdded(int expectedItemsAdded, params string[] items)
        {
            // Arrange
            WarehouseRepresentation rep = new WarehouseRepresentation();
            ItemDatabase idb = new ItemDatabase();
            List<string> sItems = new List<string>();

            int i = 0;
            foreach (string item in items)
            {
                if (!sItems.Contains(item))
                {
                    idb.AddBook(new Item(i++, item));
                    sItems.Add(item);
                }
            }


            rep.ItemDatabase = idb;

            // Act
            rep.AddNode(new Node { Id = 0, X = 0, Y = 0 });
            rep.AddNode(new Shelf { Id = 1, X = 0, Y = 1 }, 0);
            rep.AddNode(new Shelf { Id = 2, X = 0, Y = 2 }, 0, 1);
            rep.AddNode(new Node { Id = 3, X = 1, Y = 0 }, 0);
            rep.AddNode(new Shelf { Id = 4, X = 1, Y = 1 }, 1, 3);
            rep.AddNode(new Shelf { Id = 5, X = 1, Y = 2 }, 2, 3, 4);
            rep.AddNode(new Node { Id = 6, X = 2, Y = 0 }, 0, 3);
            rep.AddNode(new Shelf { Id = 7, X = 2, Y = 1 }, 1, 4, 6);
            rep.AddNode(new Shelf { Id = 8, X = 2, Y = 2 }, 2, 5, 6, 7);
            int expected = expectedItemsAdded;
            rep.Initialize();
            int actual = 0;

            // Assert
            foreach (string item in items)
            {
                Assert.DoesNotThrow(() => rep.AddBook(idb.Items.First(b => b.Name == item)));
            }

            foreach (Node repNode in rep.Nodes)
            {
                if (repNode is Shelf)
                    foreach (Item item in ((Shelf)repNode).Items)
                    {
                        actual += ((Shelf)repNode).GetNumberOfItem(item);
                    }
            }
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void WarehouseRepTest_AddBook_SmallNetworkRepresentaionNotInitialized_ExceptionExpected()
        {
            // Arrange
            WarehouseRepresentation rep = new WarehouseRepresentation();
            ItemDatabase idb = new ItemDatabase();
            idb.AddBook(new Item(0, "item1"));
            rep.ItemDatabase = idb;

            // Act
            rep.AddNode(new Node { Id = 0, X = 0, Y = 0 });
            rep.AddNode(new Shelf { Id = 1, X = 1, Y = 1 }, 0);
            rep.AddNode(new Shelf { Id = 2, X = 2, Y = 5 }, 1);

            // Assert
            Assert.Throws<NullReferenceException>(() => rep.AddBook(idb.Items.First(b => b.Id == 0)));
        }

        [Test]
        public void WarehouseRepTest_AddBook_AddNonexisting_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();

            // Assert
            Assert.Throws<ArgumentException>(() => warehouseRepresentation.AddBook(new Item(234, "Nonexisting book")));
        }

        [Test]
        public void WarehouseRepTest_AddBook_AddNull_ThrowsException()
        {
            // Arrange
            WarehouseRepresentation warehouseRepresentation = new WarehouseRepresentation();
            Generate5Nodes(warehouseRepresentation);

            string itemPath = GenerateItemFile("item1", "item2");
            ItemDatabase idb = new ItemDatabase();

            // Act
            idb.ImportItems(itemPath);
            warehouseRepresentation.ItemDatabase = idb;
            warehouseRepresentation.Initialize();

            // Assert
            Assert.Throws<NullReferenceException>(() => warehouseRepresentation.AddBook(null));
        }
    }
}
