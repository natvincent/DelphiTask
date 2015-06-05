namespace Nant.Contrib.Tasks.Delphi
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using System;

    public class CompilerOption : Element
    {
        private string _name;
        private bool _value;

        [TaskAttribute("name", Required=true), StringValidator(AllowEmpty=false)]
        public string OptionName
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

        [TaskAttribute("value", Required=true), StringValidator(AllowEmpty=false)]
        public bool OptionValue
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        public char State
        {
            get
            {
                if (this._value)
                {
                    return '+';
                }
                return '-';
            }
        }
    }
}

