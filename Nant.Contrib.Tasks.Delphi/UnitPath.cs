using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Configuration;
using NAnt.Core.Types;
using System.IO;

namespace Nant.Contrib.Tasks.Delphi
{
    [Serializable]
    public class PathElement : Element
    {
        private string _name;

        #region Public Instance Properties

        [TaskAttribute("name", Required = true)]
        public string DirName
        {
            get { return _name; }
            set { _name = value; }
        }

        #endregion

        #region Protected Instance Properties

        #endregion Protected Instance Properties

        #region Explicit Interface Implementation

        #endregion
    }


    [ElementName("unitpath")]
    public class UnitPath : PathSet
    {
        private PathElement[] _directoryNames;
        private bool _includeDelphiLib = false;
        private bool _usePathsAsIs = false;

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

        [TaskAttribute("usepathsasis", Required = false)]
        [BooleanValidator()]
        public bool UsePathsAsIs
        {
            get
            {
                return _usePathsAsIs;
            }

            set
            {
                _usePathsAsIs = value;
            }
        }

        [BuildElementArray("path")]
        public PathElement[] Paths
        {
            get { return _directoryNames; }
            set { _directoryNames = value; }
        }

        public virtual object Clone()
        {
            UnitPath clone = new UnitPath();
            CopyTo(clone);
            return clone;
        }


    }
}
