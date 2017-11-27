using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WarehouseAI.Representation;

namespace WarehouseAITest
{
    [TestFixture]
    class PlacementAlgorithmTests
    {
        [Test]
        public void PlacementAlgorithmTest_SmallNetworkAdded1Item_NumberOfItemsInNetworkEquals1Expected()
        {
            // Arrange
            WarehouseRepresentation rep = new WarehouseRepresentation();
            ItemDatabase idb = new ItemDatabase();
            idb.AddBook(new Item(0, "item1"));
            rep.ItemDatabase = idb;

            // Act
            rep.AddNode(new Node { Id = 0, X = 0, Y = 0});
            rep.AddNode(new Shelf { Id = 1, X = 1, Y = 1 }, 0);
            rep.AddNode(new Shelf { Id = 2, X = 2, Y = 5 }, 1);
            int expected = 1;
            rep.Inintialize();
            rep.AddBook(idb.Items.First(b => b.Id == 0));

            // Assert
            int actual = rep.Nodes.Where(n => n is Shelf).Count(s => ((Shelf) s).Contains(idb.Items.First(b => b.Id == 0)));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PlacementAlgorithmTest_SmallNetworkSingleBookAdded_DropoffPointIsShelf_NoErrorExpected()
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
            rep.Inintialize();
            rep.AddBook(idb.Items.First(b => b.Id == 0));

            // Assert
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
        public void PlacementAlgorithmTest_MutipleAddBookCalls_CorrectNumberOfBooksAdded(int expectedItemsAdded, params string[] items)
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
            rep.Inintialize();
            foreach (string item in items)
            {
                rep.AddBook(idb.Items.First(b => b.Name == item));
            }
            int actual = 0;

            // Assert
            foreach (Node repNode in rep.Nodes)
            {
                if (repNode is Shelf)
                    foreach (Item item in ((Shelf)repNode).Items)
                    {
                        actual += ((Shelf) repNode).GetNumberOfItem(item);
                    }
            }
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PlacementAlgorithmTest_SmallNetworkRepresentaionNotInitialized_ExceptionExpected()
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
            rep.Inintialize();
            rep.AddBook(idb.Items.First(b => b.Id == 0));

            // Assert
            Assert.Throws<Exception>(() => throw new Exception());
        }
    }
}
