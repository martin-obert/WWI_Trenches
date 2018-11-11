using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.SpCrsVrPrototypes.Patterns;

namespace Assets.SpCrsVrPrototypes.Singletons
{
    public sealed class EntityMapper : SingletonDisposable<EntityMapper>, IEntityMapper
    {
        private IDictionary<string, int> _nameIdMap;
        private IDictionary<int, string> _idNameMap;

        public int NameToId(string name)
        {
            int result;

            if (!_nameIdMap.TryGetValue(name, out result))
            {
                return MapName(name);
            }

            return result;
        }

        public string IdToName(int id)
        {
            string result;

            if (!_idNameMap.TryGetValue(id, out result))
            {
                return null;
            }

            return result;
        }

        private int _counter;

        public int MapName(string name)
        {
            if (_nameIdMap.ContainsKey(name))
            {
                throw new ArgumentException(name + " is already mapped!");
            }

            _nameIdMap.Add(name, ++_counter);
            _idNameMap.Add(_counter, name);

            return _counter;
        }

        public override void Dispose()
        {
            _idNameMap.Clear();
            _nameIdMap.Clear();
        }

        public EntityMapper()
        {
            _nameIdMap = new ConcurrentDictionary<string, int>();

            _idNameMap = new ConcurrentDictionary<int, string>();
        }
    }
}