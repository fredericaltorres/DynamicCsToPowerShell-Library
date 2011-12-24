using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicCsToPowerShell;
using DynamicSugar;
using System.Reflection;

namespace DynamicCsToPowerShellUnitTests {

    [TestClass]
    public class Dictionary {

         [TestMethod]
        public void Dictionary_IntLoadFromRessource() {
            
            var script = DS.Resources.GetTextResource(@"PowerShell.IntDictionary.ps1", Assembly.GetExecutingAssembly());
            var dpsc   = DynamicPowerShellContext.Create().Load(script).Run();

            Assert.AreEqual(3, dpsc.Dic.Count);
            Assert.AreEqual(1, dpsc.Dic["a"]);
            Assert.AreEqual(2, dpsc.Dic["b"]);
            Assert.AreEqual(3, dpsc.Dic["c"]);
            Assert.AreEqual(6, dpsc.Dic["a"] + dpsc.Dic["b"] + dpsc.Dic["c"]);
        }

        [TestMethod]
        public void Dictionary_IntLoad() {
            
            var script = @"
                function F1([int] $i1, [int] $i2) {	            
	                return $i1 + $i2;
                }
                $Dic = @{
                    a = 1  
                    b = 2 
                    c = (F1 2 1)
                }
            ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual(3, dpsc.Dic.Count);
            Assert.AreEqual(1, dpsc.Dic["a"]);
            Assert.AreEqual(2, dpsc.Dic["b"]);
            Assert.AreEqual(3, dpsc.Dic["c"]);
            Assert.AreEqual(6, dpsc.Dic["a"] + dpsc.Dic["b"] + dpsc.Dic["c"]);
        }
        
        [TestMethod]
        public void Dictionary_Keys() {
            
            var script = @"
                $Dic = @{
                    a = 1  
                    b = 2 
                    c = 3
                }
            ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual(3, dpsc.Dic.Keys.Count);
            
            DS.ListHelper.AssertListEqual(
                DynamicPowerShellContext.MakeList<string>(dpsc.Dic.Keys),
                DS.List("a", "b", "c")
            );
        }
        [TestMethod]
        public void Dictionary_StringLoad() {

            var script = @"
                function F1($i1) {	            
	                return $i1;
                }
                $Dic = @{
                    a = 'A'
                    b = 'B' 
                    c = (F1 'C')
                }
            ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            Assert.AreEqual(3  , dpsc.Dic.Count);
            Assert.AreEqual("A", dpsc.Dic["a"]);
            Assert.AreEqual("B", dpsc.Dic["b"]);
            Assert.AreEqual("C", dpsc.Dic["c"]);
            Assert.AreEqual("ABC", dpsc.Dic["a"] + dpsc.Dic["b"] + dpsc.Dic["c"]);
        }

    }
}
