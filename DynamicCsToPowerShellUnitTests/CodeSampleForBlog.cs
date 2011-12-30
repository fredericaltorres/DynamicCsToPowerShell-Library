using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DynamicCsToPowerShell;
using DynamicSugar;
using System.Management.Automation;
using System.Reflection;

namespace CodeSampleForBlogUnitTests {

    [TestClass]
    public class Value {

        const string powerShellScriptFolder =  @"C:\FredericTorres.Net.Src\DynamicCsToPowerShell\DynamicCsToPowerShell\DynamicCsToPowerShellUnitTests\PowerShell";

        [TestMethod]
        public void NormalWay() {
            
            var script = @" $i1 = 123 ";            
            var ps = PowerShell.Create();
            ps.AddScript(script);
            ps.Invoke();            

            int i1 = (int)ps.Runspace.SessionStateProxy.GetVariable("i1");

            Assert.AreEqual(123, i1);
        }
        [TestMethod]
        public void DynamicPowerShellContextWay() {
            
            var script = @" $i1 = 123 ";
            var dpsc = DynamicPowerShellContext.Create().Run(script);

            int i1 = dpsc.i1;

            Assert.AreEqual(123, i1);
        }   
 
        [TestMethod]
        public void Dictionary_IntLoadFromFile() {
            
            var ps1ConfigFile = @"{0}\IntDictionary.ps1".format(powerShellScriptFolder);
            var dpsc   = DynamicPowerShellContext.Create();
            dpsc.LoadFile(ps1ConfigFile).Run();

            Assert.AreEqual(3, dpsc.Dic.Count);
            Assert.AreEqual(1, dpsc.Dic["a"]);
            Assert.AreEqual(2, dpsc.Dic["b"]);
            Assert.AreEqual(3, dpsc.Dic["c"]);
            Assert.AreEqual(6, dpsc.Dic["a"] + dpsc.Dic["b"] + dpsc.Dic["c"]);
        }        
        [TestMethod]
        public void Dictionary_IntLoad2() {
            
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
    }
}
