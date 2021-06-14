using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace Octodrome.DB
{
    public interface IColumn
    {
        // The label for the input field.
        public string Label { get; }
        // The placeholder for a text field.
        public string? Placeholder { get; }
        // This is the current value in the database.
        public string CurrText { get; }
        // This is the value being edited by the user.
        public string EditText { get; set; }
        // This is a conflicting edit in the database.
        public string? ConflictText { get; }
        // True iff editing is disabled.
        public bool Disabled { get; }
        // True iff this field is visible.
        public bool Visible { get; }

        public string? ParseAndValidate(string editText);
    }
}
