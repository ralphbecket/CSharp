using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Octodrome.Lib
{
    public class FormItem: IItem
    {
        public Guid ID { get; internal set; } = Guid.NewGuid();
        public ImmutableList<KeyValuePair<string, Guid>> Value { get; internal set; }
        public Guid this[string key] => Value.First(kv => kv.Key == key).Value;
        public bool ContainsKey(string key) => Value.Any(kv => kv.Key == key);
        public FormItem(Guid FormID, ImmutableList<KeyValuePair<string, Guid>>? value = null)
        {
            ID = FormID;
            Value = value ?? ImmutableList<KeyValuePair<string, Guid>>.Empty;
        }
        public FormItem(Guid FormID, IEnumerable<KeyValuePair<string, Guid>> value)
        {
            ID = FormID;
            Value = ImmutableList<KeyValuePair<string, Guid>>.Empty.AddRange(value);
        }
        public static Doc Create(Doc doc, Guid editID, Guid FormID)
        {
            var edit = new CreateFormEdit(editID, FormID);
            doc = doc.Apply(edit);
            return doc;
        }
        public static Doc Create(Doc doc, out Guid FormID)
        {
            doc = doc.NewID(out var editID);
            doc = doc.NewID(out FormID);
            return Create(doc, editID, FormID);
        }
        public static Doc Set(Doc doc, Guid editID, FormItem before, string key, Guid valueID)
        {
            var id = before.ID;
            var edit = new SetFormFieldEdit(editID, before, key, valueID);
            doc = doc.Apply(edit);
            return doc;
        }
        public static Doc Set(Doc doc, FormItem before, string key, Guid valueID)
        {
            doc = doc.NewID(out var editID);
            return Set(doc, editID, before, key, valueID);
        }
        public static Doc Remove(Doc doc, Guid editID, string key, FormItem before)
        {
            var id = before.ID;
            var edit = new RemoveFormFieldEdit(editID, before, key);
            doc = doc.Apply(edit);
            return doc;
        }
        public static Doc Remove(Doc doc, string key, FormItem before)
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
            doc = Create(doc, out var FormID);
            var FormClone = (FormItem)doc[FormID];
            clone = FormClone;
            foreach (var kv in Value)
            {
                var key = kv.Key;
                var valueID = kv.Value;
                var value = doc[kv.Value];
                doc = value.DeepClone(doc, out var valueClone, clones);
                doc = FormItem.Set(doc, FormClone, key, valueClone.ID);
            }
            return doc;
        }

        public class CreateFormEdit: IEdit
        {
            public Guid ID { get; internal set; }
            internal FormItem NewFormItem;
            public CreateFormEdit(Guid editID, Guid FormID)
            {
                ID = editID;
                NewFormItem = new FormItem(FormID);
            }
            public Guid ItemID => NewFormItem.ID;
            public ImmutableDictionary<Guid, IItem> Apply(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.SetItem(ItemID, NewFormItem);
                return items;
            }
            public ImmutableDictionary<Guid, IItem> Revert(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.Remove(ItemID);
                return items;
            }
        }

        public class SetFormFieldEdit: IEdit
        {
            public Guid ID { get; internal set; }
            internal string Key;
            internal Guid ValueID;
            internal FormItem Before;
            internal FormItem After;
            public Guid ItemID => Before.ID;
            internal SetFormFieldEdit(Guid editID, FormItem before, string key, Guid valueID)
            {
                ID = editID;
                Key = key;
                ValueID = valueID;
                Before = before;
                var kvs = Before.Value;
                var newKV = new KeyValuePair<string, Guid>(key, valueID);
                After =
                    ( Before.ContainsKey(key)
                    ? new FormItem(ItemID, kvs.Select(kv => (kv.Key == key ? newKV : kv)))
                    : new FormItem(ItemID, kvs.Add(newKV))
                    );
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

        public class RemoveFormFieldEdit: IEdit
        {
            public Guid ID { get; internal set; }
            internal string Key;
            internal FormItem Before;
            internal FormItem After;
            public Guid ItemID => Before.ID;
            internal RemoveFormFieldEdit(Guid editID, FormItem before, string key)
            {
                ID = editID;
                Key = key;
                Before = before;
                var kvs = Before.Value;
                After =
                    ( Before.ContainsKey(key)
                    ? new FormItem(ItemID, kvs.Where(x => x.Key != key))
                    : Before
                    );
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
