using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elphysics
{
    [Serializable]
    public class DBase<T>
        where T: DBase<T>
    {
      #region Static
      static private readonly Dictionary<Guid, DBase<T>> current_c_items = new Dictionary<Guid, DBase<T>>();
      static public Dictionary<Guid, DBase<T>> Items { get { return current_c_items; } }

      static public T GetItemById(Guid g) { return (T)Items[g]; }


      static public List<T> MyItems
      {
         get
         {
            var res = new List<T>();
            foreach (var obj in current_c_items.Values)
               if (obj is T)
                  res.Add((T)obj);
            return res;
         }
      }

      static public void ClearMyList()
      {
              Items.Clear();
              current_c_items.Clear();
      }
      #endregion

      public Guid GUID { get; private set; }

      public DBase() { GUID = Guid.NewGuid(); Items.Add(GUID, this); }
    }
}
