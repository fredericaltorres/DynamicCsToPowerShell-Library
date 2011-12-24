using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicCsToPowerShell;
using DynamicSugar;

namespace DynamicCsToPowerShellUnitTests {

    [TestClass]
    public class Array {

        [TestMethod]
        public void Array_IntLoad() {
            
            var script = @" $IntArray = 0,1,2,3,4,5 ";
            var dpsc   = DynamicPowerShellContext.Create().Run(script);
            
            Assert.AreEqual(6, dpsc.IntArray.Length);
            
            foreach(var i in DS.Range(5))
                Assert.AreEqual(i, dpsc.IntArray[i]);                
        }
    }
}
