using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicSugar;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Management.Automation.Remoting;
using System.Dynamic;
using System.Collections.ObjectModel;
using System.Collections;

namespace DynamicCsToPowerShell {

    // http://msdn.microsoft.com/en-us/library/system.management.automation.powershell.aspx

    /// <summary>
    /// Provide an access to the PowerShell world, but allowing 
    /// a dynamic like syntax in the C# code.
    /// </summary>
    public class DynamicPowerShellContext : DynamicObject {

        PowerShell _psRt = null;

        public static List<T> MakeList<T>(object boxedList){

            var e = boxedList as IEnumerable;
            var l = new List<T>();

            foreach(var o in e) {
                T result = (T)Convert.ChangeType(o, typeof(T));
                l.Add(result);
            }
            return l;
        }
        public static dynamic Create(){

            return new DynamicPowerShellContext();
        }
        public DynamicPowerShellContext(){

            this._psRt = PowerShell.Create();
        }
        public DynamicPowerShellContext Load(string script){
                        
            this._psRt.AddScript(script);
            return this;
        }
        public DynamicPowerShellContext LoadFile(string fileName){

            var ps1 = System.IO.File.ReadAllText(fileName);
            this.Load(ps1);
            return this;
        }
        public DynamicPowerShellContext Run(string script = null){

            if(script!=null)
                this.Load(script);
            Collection<PSObject> r =  this._psRt.Invoke();
            return this;
        }
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {
            
            result = this._psRt.Runspace.SessionStateProxy.GetVariable(indexes[0].ToString());

            if((result.GetType().IsArray)||(result is Hashtable))
                result = new DynamicInstance(result);
                
            return true;
        }
        /// <summary>
        /// Get a global variable value from the JavaScript context
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result) {
                                
            result = this._psRt.Runspace.SessionStateProxy.GetVariable(binder.Name);

            if((result.GetType().IsArray)||(result is Hashtable))
                result = new DynamicInstance(result);
                
            return true;
        }
    }   
}
