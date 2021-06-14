using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome.Lib.Tests
{
    public static class Util
    {
        public static Guid NewID() => Guid.NewGuid();

        public static T Item<T>(this Doc doc, Guid itemID)
        {
            Assert.IsTrue(doc.HasItem(itemID));
            var item = doc[itemID];
            Assert.IsInstanceOfType(item, typeof(T));
            return (T)item;
        }

        public static T DictItem<T>(this Doc doc, Guid dictID, string key)
        {
            var dictItem = doc.Item<DictItem>(dictID);
            Assert.IsNotNull(dictItem);
            Assert.IsTrue(dictItem.ContainsKey(key));
            var itemID = dictItem[key];
            return doc.Item<T>(itemID);
        }
    }
}
