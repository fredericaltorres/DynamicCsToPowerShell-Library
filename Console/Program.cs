using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Remoting;
using DynamicCsToPowerShell;

namespace DynamicCsToPowerShellConsole {

    class Program {

        static void Main(string[] args) {

            var script = @"
                $IntArray = 1,2,3,4,5,6
                $dic = @{
                    a   = 1  
                    b   = 2 
                    c   = 'aa','bb','cc'
                }
";
            var dpsc = DynamicPowerShellContext.Create().Run(script);
            
            Console.WriteLine("Array:");
            Console.WriteLine(dpsc.IntArray[0]);
            Console.WriteLine(dpsc.IntArray.Length);

            foreach(var o in dpsc.IntArray)
                Console.WriteLine(o);

            Console.WriteLine("Dictionary:");
            Console.WriteLine(dpsc.dic.Count);
            Console.WriteLine(dpsc.dic["c"].ToString());
            Console.WriteLine(dpsc.dic.ToString());

            foreach(var k in dpsc.dic)
                Console.WriteLine(String.Format("{0} = {1}", k, dpsc.dic[k]));

            Console.WriteLine("Hit enter");
            Console.ReadLine();
        }
    }
}
