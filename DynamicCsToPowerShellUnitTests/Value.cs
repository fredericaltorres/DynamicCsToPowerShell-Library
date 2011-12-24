using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicCsToPowerShell;
using DynamicSugar;
using System.Management.Automation;

namespace DynamicCsToPowerShellUnitTests {

    [TestClass]
    public class Value {

        [TestMethod]
        public void Value_IntLoad() {
            
            var script = @" $i1 = 123 ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual(123, dpsc.i1);
        }
        [TestMethod]
        public void Value_DecimalLoad() {

            var script = @" [decimal] $d1 = 123.456 ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual(123.456M, dpsc.d1);
        }
        [TestMethod]
        public void Value_DoubleLoad() {
            
            var script = @" $d1 = 123.456 ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual(123.456, dpsc.d1);
        }
        [TestMethod]
        public void Value_StringLoad() {
            
            var script = @" $s1 = 'ABC' ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual("ABC", dpsc.s1);
        }
        [TestMethod]
        public void Value_BooleanLoad() {
            
            var script = @"
                $bTrue  = $true
                $bFalse = $false
             ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual(true  , dpsc.bTrue);
            Assert.AreEqual(false , dpsc.bFalse);
            Assert.AreEqual(!true , !dpsc.bTrue);
            Assert.AreEqual(!false, !dpsc.bFalse);
        }
    }
}
