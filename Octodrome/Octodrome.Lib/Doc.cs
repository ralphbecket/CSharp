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
        public static Doc New()
        {
            return new Doc
            {
                Items = ImmutableDictionary<Guid, IItem>.Empty,
                Edits = ImmutableStack<IEdit>.Empty,
                Redos = ImmutableStack<IEdit>.Empty,
            };
        }
        public bool HasItem(Guid itemID) => Items.ContainsKey(itemID);
        public IItem this[Guid itemID] => Items[itemID];
        public Doc Apply(IEdit edit)
        {
            var items = edit.Apply(Items);
            var edits = Edits.Push(edit);
            var redos = ImmutableStack.Create<IEdit>();
            return new Doc { Items = items, Edits = edits, Redos = redos };
        }
        public bool CanUndo => !Edits.IsEmpty;
        public Doc Undo()
        {
            var edits = Edits.Pop(out var edit);
            var redos = Redos.Push(edit);
            var items = edit.Revert(Items);
            var doc = new Doc { Items = items, Edits = edits, Redos = redos };
            return doc;
        }
        public bool CanRedo => !Redos.IsEmpty;
        public Doc Redo()
        {
            var redos = Redos.Pop(out var edit);
            var edits = Edits.Push(edit);
            var items = edit.Apply(Items);
            var doc = new Doc { Items = items, Edits = edits, Redos = redos };
            return doc;
        }
    }
}
