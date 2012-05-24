using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlDataProvider.Data;
using Game.Logic;
using Bussiness.Managers;

namespace Road.Test
{
    [TestClass]
    public class TestDropMgr
    {
        [TestInitialize]
        public void Init()
        {
            DropMgr.Init();
        }

        [TestCleanup]
        public void Clear()
        {
 
        }

        [TestMethod]
        public void TestDropPerformance()
        {
            List<ItemInfo> tempItem = null;
            for (int i = 0; i < 100000; i++)
            {
                DropInventory.PvEQuestsDrop(1, ref tempItem);
                if (tempItem != null)
                {
                    Console.WriteLine(tempItem.First().Template.Name);
                }
                DropInventory.NPCDrop(3, ref tempItem);
                DropInventory.CopyDrop(1071, 1, ref tempItem);
            }
        }
    }
}
