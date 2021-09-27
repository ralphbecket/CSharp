using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Octodrome.Lib
{
    public struct Doc
    {
        ImmutableDictionary<Guid, IItem> Items;
        ImmutableStack<IEdit> Edits;
        ImmutableStack<IEdit> Redos;
        IGuidSupply GuidSupply;

        internal readonly static ImmutableDictionary<Guid, IItem> EmptyItemDict =
            ImmutableDictionary<Guid, IItem>.Empty;
        internal readonly static ImmutableStack<IEdit> EmptyEditStack =
            ImmutableStack<IEdit>.Empty;

        public Doc(IGuidSupply? guidSupply = null)
        {
            Items = EmptyItemDict;
            Edits = EmptyEditStack;
            Redos = EmptyEditStack;
            GuidSupply = guidSupply ?? new GuidSupply();
        }
        public static Doc New(IGuidSupply? guidSupply = null)
        {
            return new Doc
            {
                Items = EmptyItemDict,
                Edits = EmptyEditStack,
                Redos = EmptyEditStack,
                GuidSupply = guidSupply ?? new GuidSupply()
            };
        }
        public bool HasItem(Guid itemID) => Items.ContainsKey(itemID);
        public IItem this[Guid itemID] => Items[itemID];
        public Doc NewID(out Guid id)
        {
            GuidSupply = GuidSupply.NewID(out id);
            return this;
        }
        public Doc Apply(IEdit edit)
        {
            Items = edit.Apply(Items);
            Edits = Edits.Push(edit);
            Redos = EmptyEditStack;
            return this;
        }
        public bool CanUndo => !Edits.IsEmpty;
        public Doc Undo()
        {
            Edits = Edits.Pop(out var edit);
            Redos = Redos.Push(edit);
            Items = edit.Revert(Items);
            return this;
        }
        public bool CanRedo => !Redos.IsEmpty;
        public Doc Redo()
        {
            Redos = Redos.Pop(out var edit);
            Edits = Edits.Push(edit);
            Items = edit.Apply(Items);
            return this;
        }
        // Now, we need to have atomic group updates for certain situations.
        // Here is how manage that.
        internal class EditGroupBoundary : IEdit
        {
            public Guid ItemID => default(Guid);
            public ImmutableDictionary<Guid, IItem> Apply(ImmutableDictionary<Guid, IItem> items) =>
                throw new ApplicationException("Mismatched EditGroupBoundary");
            public ImmutableDictionary<Guid, IItem> Revert(ImmutableDictionary<Guid, IItem> items) =>
                throw new ApplicationException("Mismatched EditGroupBoundary");
        }
        internal class EditGroup : IEdit
        {
            public Guid ItemID { get; internal set; }
            internal IEdit[] Edits;
            internal EditGroup(Guid id, IEdit[] edits)
            {
                ItemID = id;
                Edits = edits;
            }
            public ImmutableDictionary<Guid, IItem> Apply(ImmutableDictionary<Guid, IItem> items)
            {
                for (var i = 0; i < Edits.Length; i++)
                {
                    var edit = Edits[i];
                    items = edit.Apply(items);
                }
                return items;
            }
            public ImmutableDictionary<Guid, IItem> Revert(ImmutableDictionary<Guid, IItem> items)
            {
                for (var i = Edits.Length - 1; 0 <= i; i--)
                {
                    var edit = Edits[i];
                    items = edit.Revert(items);
                }
                return items;
            }
        }
        internal static readonly EditGroupBoundary Egb = new EditGroupBoundary();
        public Doc GroupEdits(Guid editGroupID, Func<Doc, Doc> groupEditFn)
        {
            // Push an EditGroupBoundary marker object.
            var doc = this;
            doc.Edits = doc.Edits.Push(Egb);
            // Apply the group edit function.
            doc = groupEditFn(doc);
            // Pop the edits back to the marker, collecting as we go.
            var revEdits = new Stack<IEdit> { };
            var edits = doc.Edits;
            while (true)
            {
                if (edits.IsEmpty)
                {
                    throw new ApplicationException("Edit stack exhausted looking for EditGroupBoundary");
                }
                edits = edits.Pop(out var edit);
                if (edit == Egb) break;
                revEdits.Push(edit);
            }
            // Construct a new EditGroup from the collected edits.
            var editGroup = new EditGroup(editGroupID, revEdits.ToArray());
            edits = edits.Push(editGroup);
            // Return an updated version.
            doc.Edits = edits;
            return doc;
        }
    }
}
