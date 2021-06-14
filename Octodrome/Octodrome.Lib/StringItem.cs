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
        public static Doc Change(Doc doc, Guid editID, StringItem before, string value)
        {
            var id = before.ID;
            var after = new StringItem(id, value);
            var edit = new ChangeStringEdit(editID, before, after);
            doc = doc.Apply(edit);
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
