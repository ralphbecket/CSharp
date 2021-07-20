using Microsoft.VisualStudio.TestTools.UnitTesting;
using Octodrome.Lib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Octodrome.Lib.Tests
{
    [TestClass()]
    public class DictItemTests
    {
        Guid NewID() => Util.NewID();

        [TestMethod()]
        public void CreateTest()
        {
            var doc = Doc.New();
            Assert.IsFalse(doc.CanRedo);
            Assert.IsFalse(doc.CanUndo);
            doc = DictItem.Create(doc, out var dictID);
            var dict = doc.Item<DictItem>(dictID);
        }

        [TestMethod()]
        public void SetTest()
        {
            var doc = Doc.New();
            doc = DictItem.Create(doc, out var dictID);
            var dict = doc.Item<DictItem>(dictID);
            doc = StringItem.Create(doc, out var stringID, "abc");
            doc = DictItem.Set(doc, dict, "Foo", stringID);
            var stringItem = doc.DictItem<StringItem>(dictID, "Foo");
            Assert.AreEqual("abc", stringItem.Value);
            // Now change the string value and verify.
            doc = StringItem.Change(doc, stringItem, "xyz");
            stringItem = doc.DictItem<StringItem>(dictID, "Foo");
            Assert.AreEqual("xyz", stringItem.Value);
            // Now change the field entry and verify.
            doc = StringItem.Create(doc, out var newStringID, "pqr");
            dict = doc.Item<DictItem>(dictID);
            doc = DictItem.Set(doc, NewID(), dict, "Foo", newStringID);
            var newStringItem = doc.DictItem<StringItem>(dictID, "Foo");
            Assert.AreEqual("pqr", newStringItem.Value);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            var doc = Doc.New();
            doc = DictItem.Create(doc, out var dictID);
            var dict = doc.Item<DictItem>(dictID);
            doc = StringItem.Create(doc, out var stringID, "abc");
            dict = doc.Item<DictItem>(dictID);
            doc = DictItem.Set(doc, dict, "Foo", stringID);
            dict = doc.Item<DictItem>(dictID);
            doc = DictItem.Remove(doc, "Foo", dict);
            dict = doc.Item<DictItem>(dictID);
            Assert.IsFalse(dict.ContainsKey("Foo"));
        }

        [TestMethod()]
        public void UndoRedoTest()
        {
            var doc = Doc.New();
            Assert.IsFalse(doc.CanRedo);
            Assert.IsFalse(doc.CanUndo);
            doc = DictItem.Create(doc, out var dictID);
            Assert.IsFalse(doc.CanRedo);
            Assert.IsTrue(doc.CanUndo);
            var dict = doc.Item<DictItem>(dictID);
            doc = doc.Undo();
            Assert.IsTrue(doc.CanRedo);
            Assert.IsFalse(doc.CanUndo);
            Assert.IsFalse(doc.HasItem(dictID));
            doc = doc.Redo();
            Assert.IsFalse(doc.CanRedo);
            Assert.IsTrue(doc.CanUndo);
            dict = doc.Item<DictItem>(dictID);
            doc = StringItem.Create(doc, out var stringID, "abc");
            doc = DictItem.Set(doc, dict, "Foo", stringID);
            var stringItem = doc.DictItem<StringItem>(dictID, "Foo");
            Assert.AreEqual("abc", stringItem.Value);
            Assert.IsFalse(doc.CanRedo);
            Assert.IsTrue(doc.CanUndo);
            doc = doc.Undo();
            dict = doc.Item<DictItem>(dictID);
            Assert.IsFalse(dict.ContainsKey("Foo"));
            doc = doc.Undo().Undo();
            Assert.IsFalse(doc.HasItem(dictID));
            doc = doc.Redo().Redo().Redo();
            Assert.IsTrue(doc.HasItem(dictID));
            dict = doc.Item<DictItem>(dictID);
            Assert.IsTrue(dict.ContainsKey("Foo"));
            stringItem = doc.DictItem<StringItem>(dictID, "Foo");
            Assert.AreEqual("abc", stringItem.Value);
        }
    }
}