using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;

#nullable enable

namespace Octodrome.DB
{
    public class Table: ITable
    {
        public string Label { get; }

        public IEnumerable<IColumn> Columns { get; }

        public bool IsDeletable { get; set; } = true;

        private int DeleteCount = 0;

        public void Delete() {
            DeleteCount++;
        }

        public void Undelete() {
            DeleteCount--;
        }

        public bool IsToBeDeleted => 0 < DeleteCount;

        public Table(string label, params IColumn[] columns)
        {
            Label = label;
            Columns = columns;
        }

        public IEnumerable<IColumn> _ColumnFields()
        {
            var t = GetType();
            var ic = typeof(IColumn);
            var cs =
                t
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Select(x => x.GetValue(this) as IColumn)
                .Where(x => x != null);
            return cs;
        }
    }
}
