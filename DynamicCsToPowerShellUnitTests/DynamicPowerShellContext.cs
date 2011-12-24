using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicCsToPowerShell;
using DynamicSugar;

namespace DynamicCsToPowerShellUnitTests {

    [TestClass]
    public class DynamicPowerShellContextUnitTests {

        [TestMethod]
        public void SetAndResetInt() {
            
            var script = @" $i1 = 123 ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual(123, dpsc.i1);

            script = @" $i1 = 124 ";
            dpsc.Run(script);

            Assert.AreEqual(124, dpsc.i1);
        }
       
    }
}
