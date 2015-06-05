namespace Nant.Contrib.Tasks.Delphi
{
    using System;
    using System.Collections;

    public class CompilerOptionEnumerator : IEnumerator
    {
        private IEnumerator _baseEnumerator;

        internal CompilerOptionEnumerator(CompilerOptionCollection arguments)
        {
            IEnumerable args = arguments;
            this._baseEnumerator = args.GetEnumerator();
        }

        public bool MoveNext()
        {
            return this._baseEnumerator.MoveNext();
        }

        public void Reset()
        {
            this._baseEnumerator.Reset();
        }

        bool IEnumerator.MoveNext()
        {
            return this._baseEnumerator.MoveNext();
        }

        void IEnumerator.Reset()
        {
            this._baseEnumerator.Reset();
        }


        public CompilerOption Current
        {
            get
            {
                return (CompilerOption) this._baseEnumerator.Current;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this._baseEnumerator.Current;
            }
        }
    }
}

