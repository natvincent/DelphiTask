using System;
using System.Collections.Generic;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Nant.Contrib.Tasks.Delphi
{
    public class Warning : Element
    {
        private string _name;
        private string _enabled = Boolean.TrueString;

        [TaskAttribute("name", Required = true)]
        [StringValidator()]
        public string WarningName
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        [TaskAttribute("enabled", Required = false)]
        [StringValidator(AllowEmpty = false, 
            Expression = "true|false|error", 
            ExpressionErrorMessage = "Only 'true', 'false' or 'error' are acceptable values")]
        public string Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
            }
        }

        public string State
        {
            get
            {
                bool truebool = false;
                if (Boolean.TryParse(_enabled, out truebool))
                {
                    if (truebool)
                    {
                        return "+";
                    }
                    else
                    {
                        return "-";
                    }

                }
                return "^";
            }
        }
    }

    public class WarningCollection : List<Warning>
    { }
}
