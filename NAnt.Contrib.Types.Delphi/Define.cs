namespace NAnt.Contrib.Types.Delphi
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using System;

    public class Define : Element
    {
        private string _define;
        private bool _ifDefined;
        private bool _unlessDefined;

        public Define()
        {
            this._ifDefined = true;
        }

        public Define(string name)
        {
            this._ifDefined = true;
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this._define = name;
        }

        [BooleanValidator, TaskAttribute("if")]
        public bool IfDefined
        {
            get
            {
                return this._ifDefined;
            }
            set
            {
                this._ifDefined = value;
            }
        }

        [TaskAttribute("name", Required=true), StringValidator(AllowEmpty=false)]
        public string SymbolName
        {
            get
            {
                return this._define;
            }
            set
            {
                this._define = value;
            }
        }

        [TaskAttribute("unless"), BooleanValidator]
        public bool UnlessDefined
        {
            get
            {
                return this._unlessDefined;
            }
            set
            {
                this._unlessDefined = value;
            }
        }
    }
}

