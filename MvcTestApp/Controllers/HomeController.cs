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

namespace MvcTestApp.Controllers {

    public class ControllerExtended : Controller {

        dynamic     _dynamicPowerShellContext;
        int         _powerShellDictionaryCounter = 0;

        Dictionary<string, object> _dictionary = null;

        Dictionary<string, object> Dictionary{
            get{
                if(_dictionary==null){
                    if(this.Session["_DICTIONARY"]==null){
                        this._dictionary = LoadPowerShellFiles();
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
            _dynamicPowerShellContext  = DynamicPowerShellContext.Create();
            var psDictionaryVarName    = @"$Dic = @{";
            var psDictionaryNewVarName = @"$Dic{0} = @{{";

            foreach(var psFile in psFiles){

                var psText  = System.IO.File.ReadAllText(psFile);
                psText      = psText.Replace(psDictionaryVarName, psDictionaryNewVarName.format(_powerShellDictionaryCounter));
                _powerShellDictionaryCounter++;
                _dynamicPowerShellContext.Load(psText);
            }
            _dynamicPowerShellContext.Run();

            var dic = new Dictionary<string,object>();
            for(var i=0; i<_powerShellDictionaryCounter; i++){

                var psDic = _dynamicPowerShellContext["Dic{0}".format(i)];
                var keys  = psDic .Keys;
                foreach(var key in keys){
                    dic.Add(key, psDic [key]);
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

    public class HomeController : ControllerExtended {

        public ActionResult Index() {
    
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            return View();
        }

        public ActionResult About() {

            return View();
        }
    }
}
