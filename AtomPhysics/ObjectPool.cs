using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPhysics
{
    class ObjectPool<T>
    {
        private static int _defaultSize = 20;
        private int _size;
        private Func<T> _creator;
        private Action<T> _resetter;
        private Queue<T> _objectStorage;

        public ObjectPool(Func<T> creator, Action<T> resetter) : this(creator, resetter, _defaultSize) {}

        public ObjectPool(Func<T> creator, Action<T> resetter, int size)
        {
            _objectStorage = new Queue<T>(size);
            _creator = creator;
            _resetter = resetter;
            for (int i = 0; i < size; i++)
            {
                _objectStorage.Enqueue(_creator());
            }
            _size = size;
        }

        public T Get()
        {
            if (_objectStorage.Count > 0)
            {
                return _objectStorage.Dequeue();
            }
            else
            {
                return _creator();
            }
        }
        public void Add(T obj)
        {
            if (_objectStorage.Count < _size)
            {
                _objectStorage.Enqueue(obj);
            }
        }
    }
}
