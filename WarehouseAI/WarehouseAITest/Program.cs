using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseAI;
using NUnit.Framework;

namespace WarehouseAITest
{
    [TestFixture]
    class Test
    {
        

            // unit test code  
            [Test]
            public static void ImportanceCoefficientAlgorithm_Test()
            {
            // arrange  
                string Item1Name= "HP";
                string Item2Name = "TW";
                string Item1Id = "1";
                string Item2Id = "2";
                Item item1=new Item(Item1Id,Item1Name );
                Item item2 = new Item(Item2Id, Item2Name);
                List<Item> setofItems = new List<Item>();

                Arc arc1 =new Arc(item1,item2);
                List<Arc> arcs=new List<Arc>();
                setofItems.Add(item1);
                setofItems.Add(item2);

                // act  
                float expected=WarehouseAI.Program.ImportanceCoefficientAlgorithm(setofItems, arcs);

                // assert  
                float actual = arcs.Count/setofItems.Count;
                Assert.AreEqual(expected,actual);


            }
    }
}
