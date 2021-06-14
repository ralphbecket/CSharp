using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorFluentUI;

#nullable enable

namespace Octodrome.DB
{
    public class Column<T>: IColumn
    {
        // The label for the input field.
        public string Label { get; set; }
        // The placeholder for a text field.
        public string? Placeholder { get; set; }
        // This is the current value in the database.
        public string CurrText { get; set; }
        // This is the value being edited by the user.
        public string EditText { get; set; }
        // This is a conflicting edit in the database.
        public string? ConflictText { get; set; }
        // True iff editing is disabled.
        public bool Disabled { get; set; }
        // True iff this field is visible.
        public bool Visible { get; set; }
        // A parsing or validation error message set by ParseAndValidate.
        // Null if the value is valid.
        public string? ValidationError { get; set; }
        // The parsed and validated value.
        // This is meaningful iff this is valid.
        public T ParsedValue { get; set; }
        // A function to parse the EditText to set ParsedValue.
        // Throws an exception if the input cannot be parsed.
        public Func<string, T> Parse { get; set; }
        // This validates ParsedValue, returning an error message
        // if the value is invalid; otherwise it returns null.
        public Func<T, string?>? Validate { get; set; }
        // Parse the EditText, set ParsedValue, and call Validate,
        // setting ValidationError.  Returns an error message if
        // validation fails.
        public string? ParseAndValidate(string editText)
        {
            try
            {
                ParsedValue = Parse(editText);
                ValidationError = Validate?.Invoke(ParsedValue);
            }
            catch (Exception e)
            {
                ValidationError = e.Message;
            }
            return ValidationError;
        }
        public Column(
            string label,
            string currText,
            Func<string, T> parse,
            Func<T, string?>? validate = null,
            string? placeholder = null,
            string? conflictText = null,
            bool disabled = false,
            bool visible = true
        ) {
            Label = label;
            Placeholder = placeholder;
            CurrText = currText;
            EditText = currText;
            ConflictText = conflictText;
            Disabled = disabled;
            Visible = visible;
            ValidationError = null;
            Parse = parse;
            Validate = validate;
            try
            {
                ParsedValue = parse(currText);
            }
            catch
            {
                ParsedValue = default;
            }
        }
    }
}
