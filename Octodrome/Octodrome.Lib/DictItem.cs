using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Octodrome.Lib
{
    public class DictItem: IItem
    {
        public Guid ID { get; internal set; } = Guid.NewGuid();
        public ImmutableDictionary<string, Guid> Value { get; internal set; }
        public Guid this[string key] => Value[key];
        public bool ContainsKey(string key) => Value.ContainsKey(key);
        public DictItem(Guid dictID, ImmutableDictionary<string, Guid>? value = null)
        {
            ID = dictID;
            Value = value ?? ImmutableDictionary<string, Guid>.Empty;
        }
        public static Doc Create(Doc doc, Guid editID, Guid dictID)
        {
            var edit = new CreateDictEdit(editID, dictID);
            doc = doc.Apply(edit);
            return doc;
        }
        public static Doc Create(Doc doc, out Guid dictID)
        {
            doc = doc.NewID(out var editID);
            doc = doc.NewID(out dictID);
            return Create(doc, editID, dictID);
        }
        public static Doc Set(Doc doc, Guid editID, DictItem before, string key, Guid valueID)
        {
            var id = before.ID;
            var edit = new SetDictFieldEdit(editID, before, key, valueID);
            doc = doc.Apply(edit);
            return doc;
        }
        public static Doc Set(Doc doc, DictItem before, string key, Guid valueID)
        {
            doc = doc.NewID(out var editID);
            return Set(doc, editID, before, key, valueID);
        }
        public static Doc Remove(Doc doc, Guid editID, string key, DictItem before)
        {
            var id = before.ID;
            var edit = new RemoveDictFieldEdit(editID, before, key);
            doc = doc.Apply(edit);
            return doc;
        }
        public static Doc Remove(Doc doc, string key, DictItem before)
        {
            doc = doc.NewID(out var editID);
            return Remove(doc, editID, key, before);
        }
        public Doc DeepClone(Doc doc, out IItem clone, Dictionary<Guid, IItem>? clones = null)
        {
            clones = clones ?? new Dictionary<Guid, IItem> { };
            if (clones.ContainsKey(ID))
            {
                clone = clones[ID];
                return doc;
            }
            doc = Create(doc, out var dictID);
            var dictClone = (DictItem)doc[dictID];
            clone = dictClone;
            foreach (var key in Value.Keys)
            {
                var valueID = Value[key];
                var value = doc[valueID];
                doc = value.DeepClone(doc, out var valueClone, clones);
                doc = DictItem.Set(doc, dictClone, key, valueClone.ID);
            }
            return doc;
        }

        public class CreateDictEdit: IEdit
        {
            public Guid ID { get; internal set; }
            internal DictItem NewDictItem;
            public CreateDictEdit(Guid editID, Guid dictID)
            {
                ID = editID;
                NewDictItem = new DictItem(dictID);
            }
            public Guid ItemID => NewDictItem.ID;
            public ImmutableDictionary<Guid, IItem> Apply(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.SetItem(ItemID, NewDictItem);
                return items;
            }
            public ImmutableDictionary<Guid, IItem> Revert(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.Remove(ItemID);
                return items;
            }
        }

        public class SetDictFieldEdit: IEdit
        {
            public Guid ID { get; internal set; }
            internal string Key;
            internal Guid ValueID;
            internal DictItem Before;
            internal DictItem After;
            public Guid ItemID => Before.ID;
            internal SetDictFieldEdit(Guid editID, DictItem before, string key, Guid valueID)
            {
                ID = editID;
                Key = key;
                ValueID = valueID;
                Before = before;
                var dict = before.Value.SetItem(key, valueID);
                After = new DictItem(ItemID, dict);
            }
            public ImmutableDictionary<Guid, IItem> Apply(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.SetItem(ItemID, After);
                return items;
            }
            public ImmutableDictionary<Guid, IItem> Revert(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.SetItem(ItemID, Before);
                return items;
            }
        }

        public class RemoveDictFieldEdit: IEdit
        {
            public Guid ID { get; internal set; }
            internal string Key;
            internal DictItem Before;
            internal DictItem After;
            public Guid ItemID => Before.ID;
            internal RemoveDictFieldEdit(Guid editID, DictItem before, string key)
            {
                ID = editID;
                Key = key;
                Before = before;
                var dict = before.Value.Remove(key);
                After = new DictItem(ItemID, dict);
            }
            public ImmutableDictionary<Guid, IItem> Apply(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.SetItem(ItemID, After);
                return items;
            }
            public ImmutableDictionary<Guid, IItem> Revert(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.SetItem(ItemID, Before);
                return items;
            }
        }
    }
}
