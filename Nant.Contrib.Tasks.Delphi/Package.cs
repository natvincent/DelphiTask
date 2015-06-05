using NAnt.Core;
using NAnt.Core.Attributes;
using System;
using System.Collections.Generic;

namespace Nant.Contrib.Tasks.Delphi
{
    public class Package : Element
    {
        private string _name;

        [TaskAttribute("name", Required = true), StringValidator(AllowEmpty = false)]
        public string PackageName
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

    }

    [Serializable]
    public class PackageCollection : List<Package> { }

}
