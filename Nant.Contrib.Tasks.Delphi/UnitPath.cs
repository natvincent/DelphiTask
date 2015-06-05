using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

namespace Nant.Contrib.Tasks.Delphi
{
    public class UnitPath : FileSet
    {
        private bool _includeDelphiLib = false;

        [TaskAttribute("includedelphilib", Required = false)]
        [BooleanValidator()]
        public bool IncludeDelphiLib 
        {
            get {
                return _includeDelphiLib;
            }
            set {
                _includeDelphiLib = value;
            }
        }
    }
}
