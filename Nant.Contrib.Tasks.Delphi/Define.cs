namespace Nant.Contrib.Tasks.Delphi
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using System;

    public class Define : Element
    {
        private string _define;

        public Define()
        {
        }

        public Define(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this._define = name;
        }

        [TaskAttribute("name", Required=true), StringValidator(AllowEmpty=false)]
        public string DefineName
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
    }
}

