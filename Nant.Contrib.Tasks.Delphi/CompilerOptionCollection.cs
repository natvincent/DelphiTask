namespace Nant.Contrib.Tasks.Delphi
{
    using System;
    using System.Collections;
    using System.Reflection;

    [Serializable]
    public class CompilerOptionCollection : CollectionBase
    {
        public int Add(CompilerOption item)
        {
            return base.List.Add(item);
        }

        public void AddRange(CompilerOption[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                this.Add(items[i]);
            }
        }

        public void AddRange(CompilerOptionCollection items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                this.Add(items[i]);
            }
        }

        public bool Contains(CompilerOption item)
        {
            return base.List.Contains(item);
        }

        public void CopyTo(CompilerOption[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(CompilerOption item)
        {
            return base.List.IndexOf(item);
        }

        public void Insert(int index, CompilerOption item)
        {
            base.List.Insert(index, item);
        }

        public void Remove(CompilerOption item)
        {
            base.List.Remove(item);
        }

        public CompilerOption this[int index]
        {
            get
            {
                return (CompilerOption) base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }
    }
}

