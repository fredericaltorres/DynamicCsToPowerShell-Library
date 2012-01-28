using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using DynamicCsToPowerShell;
using DynamicSugar;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.Text;
using IronPython.Runtime;

namespace Dynamic4MVC {
    
    public class ControllerExtended : Controller {

        Dictionary<string, object> _dictionary = null;

        Dictionary<string, object> Dictionary{
            get{
                if(_dictionary==null){
                    
                    // Do not cache
                    this.Session["_DICTIONARY"] = null;

                    if(this.Session["_DICTIONARY"]==null){
                        this._dictionary = LoadPowerShellFiles();
                        this._dictionary = LoadIronPythonFiles(this._dictionary);
                        this.Session["_DICTIONARY"] = this._dictionary; 
                    }
                    this._dictionary = this.Session["_DICTIONARY"] as Dictionary<string, object>;
                }
                return this._dictionary;
            }
        }
        private Dictionary<string,object> LoadPowerShellFiles(){

            var PsPath                 = System.Web.HttpContext.Current.Server.MapPath("~/Dictionary");
            var psFiles                = System.IO.Directory.GetFiles(PsPath,"*.ps1");
            dynamic powerShellContext  = DynamicPowerShellContext.Create();
            var psDictionaryVarName    = @"$Dic = @{";
            var psDictionaryNewVarName = @"$Dic{0} = @{{";
            var psDictionaryCounter    = 0;

            foreach(var psFile in psFiles){

                var psText  = System.IO.File.ReadAllText(psFile);
                psText      = psText.Replace(psDictionaryVarName, psDictionaryNewVarName.format(psDictionaryCounter));
                psDictionaryCounter++;
                powerShellContext.Load(psText);
            }
            powerShellContext.Run();

            var dic = new Dictionary<string,object>();

            for(var i=0; i<psDictionaryCounter; i++){

                var psDic = powerShellContext["Dic{0}".format(i)];
                var keys  = psDic.Keys;
                foreach(var key in keys) {
                    if(dic.ContainsKey(key))
                        dic.Remove(key);
                    dic.Add(key, psDic [key]);
                }
            }
            return dic;
        }
        private Dictionary<string,object> LoadIronPythonFiles(Dictionary<string,object> dic){

            var PsPath                 = System.Web.HttpContext.Current.Server.MapPath("~/Dictionary");
            var IronPythonFiles        = System.IO.Directory.GetFiles(PsPath,"*.py");
            
            var ipDictionaryVarName    = @"Dic = {";
            var irDictionaryNewVarName = @"Dic{0} = {{";
            var ironPythonSource       = new StringBuilder(1024);
            var ipDictionaryCounter    = 0;

            foreach(var ipFile in IronPythonFiles){

                var ipText  = System.IO.File.ReadAllText(ipFile);
                ipText      = ipText.Replace(ipDictionaryVarName, irDictionaryNewVarName.format(ipDictionaryCounter));
                ipDictionaryCounter++;
                ironPythonSource.Append(ipText).AppendLine();
            }

            var tmpPythonFile = @"{0}\Dynamic4MVC.ControllerExtended.py".format(Environment.GetEnvironmentVariable("TEMP"));
            System.IO.File.WriteAllText(tmpPythonFile, ironPythonSource.ToString());

            ScriptRuntime PythonScriptRuntime = Python.CreateRuntime();
            var PythonScript                  = PythonScriptRuntime.UseFile(tmpPythonFile);

            for(var i=0; i<ipDictionaryCounter; i++){

                var d = PythonScript.GetVariable("Dic{0}".format(i)) as PythonDictionary;
                foreach(var k in d.keys()) {
                    if(dic.ContainsKey(k.ToString()))
                        dic.Remove(k.ToString());
                    dic.Add(k.ToString(), d.get(k));
                }
            }
            return dic;
        }
        private void PopulateViewData(ViewDataDictionary v){

            foreach(var k in this.Dictionary.Keys)
                v.Add(k, this.Dictionary[k]);
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext) {

            base.OnActionExecuting(filterContext);
            this.PopulateViewData(this.ViewData);
        }        
    }
}
