using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Octodrome.Lib
{
    public class StringItem: IItem
    {
        public Guid ID { get; internal set; }
        public string Value { get; internal set; }
        public StringItem(Guid stringID, string value = "")
        {
            ID = stringID;
            Value = value;
        }
        public static Doc Create(Doc doc, Guid editID, Guid stringID, string value = "")
        {
            var edit = new CreateStringEdit(editID, stringID, value);
            doc = doc.Apply(edit);
            return doc;
        }
        public static Doc Create(Doc doc, out Guid stringID, string value = "")
        {
            doc = doc.NewID(out var editID);
            doc = doc.NewID(out stringID);
            return Create(doc, editID, stringID, value);
        }
        public static Doc Change(Doc doc, Guid editID, StringItem before, string value)
        {
            var id = before.ID;
            var after = new StringItem(id, value);
            var edit = new ChangeStringEdit(editID, before, after);
            doc = doc.Apply(edit);
            return doc;
        }
        public static Doc Change(Doc doc, StringItem before, string value)
        {
            doc = doc.NewID(out var editID);
            return Change(doc, editID, before, value);
        }
        public Doc DeepClone(Doc doc, out IItem clone, Dictionary<Guid, IItem>? clones = null)
        {
            if (clones != null && clones.ContainsKey(ID))
            {
                clone = clones[ID];
                return doc;
            }
            doc = Create(doc, out var cloneID, value: Value);
            clone = doc[cloneID];
            return doc;
        }

        public class CreateStringEdit: IEdit
        {
            public Guid ID { get; internal set; }
            internal StringItem NewStringItem;
            public CreateStringEdit(Guid editID, Guid stringID, string value = "")
            {
                ID = editID;
                NewStringItem = new StringItem(stringID, value);
            }
            public Guid ItemID => NewStringItem.ID;
            public ImmutableDictionary<Guid, IItem> Apply(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.SetItem(NewStringItem.ID, NewStringItem);
                return items;
            }
            public ImmutableDictionary<Guid, IItem> Revert(ImmutableDictionary<Guid, IItem> items)
            {
                items = items.Remove(NewStringItem.ID);
                return items;
            }
        }

        public class ChangeStringEdit: IEdit
        {
            public Guid ID { get; internal set; }
            internal StringItem Before;
            internal StringItem After;
            internal ChangeStringEdit(Guid editID, StringItem before, StringItem after)
            {
                ID = editID;
                Before = before;
                After = after;
            }
            public Guid ItemID => After.ID;
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
