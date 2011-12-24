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

    public class DynamicInstance : DynamicObject, IEnumerable<string> {

        System.Array                _array = null;
        Dictionary<string,dynamic>  _dic   = null;

        /// <summary>
        /// Returns true if the data is an array
        /// </summary>
        public bool IsArray {
            get{
                return this._array !=null;
            }
        }
        /// <summary>
        /// Return the keys of the dictionary, if the internal object is a dictionary.
        /// The order of the key is un predictable, most likely it will the order
        /// in which the key were entered.
        /// </summary>
        public List<string> Keys{
            get{
                if(this.IsArray)
                    throw new ParameterBindingException("Instance cannot be an array for the property keys");
                var l = this._dic.Keys.ToList();
                l.Reverse(); // The keys by default are return in the reverse order they were entered
                return l;
            }
        }
        /// <summary>
        /// Return the number of elements of internal data
        /// </summary>
        public int Count{
            get{
                if(this.IsArray)
                    return this._array.GetLength(0);
                return this._dic.Count;
            }
        }
        /// <summary>
        /// Return the number of elements of internal data
        /// </summary>
        public int Length {
            get{
                return this.Count;
            }
        }
        /// <summary>
        /// Consttructor accept System.Array and hasttable for now
        /// </summary>
        /// <param name="a"></param>
        public DynamicInstance(object a){

            if(a.GetType().IsArray){

                this._array = a as System.Array;
            }
            else if(a is Hashtable){

                var h     = a as Hashtable;
                this._dic = new Dictionary<string,dynamic>();
                foreach(var k in h.Keys) {
                    if(h[k].GetType().IsArray)
                        _dic.Add(k.ToString(), new DynamicInstance(h[k]));
                    else
                        _dic.Add(k.ToString(), h[k]);
                }
            }
            else 
                throw new ParameterBindingException("parameter should be of type System.Array or Hashtable, actual type {0} ".format(a.GetType().FullName));
        }
        /// <summary>
        /// TryGetMember
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result) {

            result = null;

            if(this.IsArray)
                throw new ParameterBindingException("Array does not have property '{0}'".format(binder.Name));
            else
                result = GetValueFromDictionary(binder.Name, result);

            return true;
        }
        /// <summary> 
        /// Returns the value of the key in the dictionary, if the internal data is a dictionary
        /// </summary>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private object GetValueFromDictionary(string key, object result) {

            if (this._dic.ContainsKey(key)) {

                result = this._dic[key];
                if (result.GetType().IsArray)
                    result = new DynamicInstance(result);

                if (result is PSObject) {
                    PSObject p = result as PSObject;
                    result = p.ImmediateBaseObject;
                }
            }
            else
                throw new ParameterBindingException("key:{0} not found in dictionary".format(key));

            return result;
        }
        /// <summary>
        /// TryGetIndex
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result) {

            result  = null;

            if(this.IsArray)
                result = _array.GetValue((int)indexes[0]);            
            else                
                result = GetValueFromDictionary(indexes[0].ToString(), result);

            return true;
        }
        IEnumerator<string> IEnumerable<string>.GetEnumerator() {

            return this._dic.Keys.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {

            if(this.IsArray)
                return _array.GetEnumerator();
            else
                return this._dic.Keys.GetEnumerator();
        }
        /// <summary>
        /// Returns the string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString() {

            if(this.IsArray)
                return DynamicSugar.ExtendedFormat.ArrayToString(this._array);
            else
                return DynamicSugar.ExtendedFormat.DictionaryToString(this._dic);
        }
    }
}