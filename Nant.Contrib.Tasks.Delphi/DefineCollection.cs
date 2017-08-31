namespace Nant.Contrib.Tasks.Delphi
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    [Serializable]
    public class DefineCollection : List<Define> { }
    /*public class DefineCollection : CollectionBase
    {
        public int Add(Define item)
        {
            return base.List.Add(item);
        }

        public void AddRange(Define[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                this.Add(items[i]);
            }
        }

        public void AddRange(DefineCollection items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                this.Add(items[i]);
            }
        }

        public bool Contains(Define item)
        {
            return base.List.Contains(item);
        }

        public void CopyTo(Define[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(Define item)
        {
            return base.List.IndexOf(item);
        }

        public void Insert(int index, Define item)
        {
            base.List.Insert(index, item);
        }

        public void Remove(Define item)
        {
            base.List.Remove(item);
        }

        public Define this[int index]
        {
            get
            {
                return (Define) base.List[index];
            }
            set
            {
                base.List[index] = value;
            }
        }
    }*/
}

