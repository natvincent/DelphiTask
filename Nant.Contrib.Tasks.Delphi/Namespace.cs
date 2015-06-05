using System;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace Nant.Contrib.Tasks.Delphi
{
    public class NameSpace : Element
    {
        private string _nameSpace;

        public NameSpace()
        {

        }
        
        public NameSpace(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            this._nameSpace = name;
        }

        [TaskAttribute("name", Required=true), StringValidator(AllowEmpty=false)]
        public string NameSpaceName 
        {
            get
            {
                return this._nameSpace;
            }
            set
            {
                this._nameSpace = value;
            }
        }


    }
}
