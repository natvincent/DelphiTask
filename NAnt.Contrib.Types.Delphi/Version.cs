using System;
using System.Collections.Generic;
using System.Text;

using NAnt.Core.Attributes;
using NAnt.Core;

namespace Delphi.Nant.Contrib.Tasks.Delphi
{
    public class Version : DataTypeBase
    {
        private int _majorVersion;
        private int _minorVersion;
        private int _release;
        private int _build;

        [TaskAttribute("major")]
        public int Major
        {
            get { return _majorVersion; }
            set { _majorVersion = value; }
        }
        [TaskAttribute("minor")]
        public int Minor
        {
            get { return _minorVersion; }
            set { _minorVersion = value; }
        }
        [TaskAttribute("release")]
        public int Release
        {
            get { return _release; }
            set { _release = value; }
        }
        [TaskAttribute("build")]
        public int Build
        {
            get { return _build; }
            set { _build = value; }
        }

        public string GetString(char Seperator)
        {
            return String.Format("{1:D}{0}{2:D}{0}{3:D}{0}{4:D}", Seperator, _majorVersion, _minorVersion, _release, _build);
        }

    }
}
