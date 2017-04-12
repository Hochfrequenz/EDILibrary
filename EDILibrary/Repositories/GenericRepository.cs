using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDILibrary.Repositories
{
    public class GenericRepository<T> where T: BaseKeyValue
    {
        protected List<T> _internalList;
        protected bool _initialized = false;
        public virtual void Initialize(List<T> initialList)
        {
            _internalList = initialList;
            _initialized = true;
        }
        public T GetValueByKey(string key)
        {
            return (from  T reason in GetValueList() where reason.Key == key select reason).FirstOrDefault();
        }
        public virtual List<T> GetValueList(bool allowNullValue=true)
        {
            return new List<T>();
        }
    }
}
