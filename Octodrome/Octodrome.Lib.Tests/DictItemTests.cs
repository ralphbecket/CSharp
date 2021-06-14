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
            var dictID = NewID();
            var createID = NewID();
            doc = DictItem.Create(doc, createID, dictID);
            var dict = doc.Item<DictItem>(dictID);
        }

        [TestMethod()]
        public void SetTest()
        {
            var doc = Doc.New();
            var dictID = NewID();
            var createID = NewID();
            doc = DictItem.Create(doc, createID, dictID);
            Assert.IsTrue(doc.HasItem(dictID));
            Assert.IsInstanceOfType(doc[dictID], typeof(DictItem));
            var stringID = NewID();
            createID = NewID();
            doc = StringItem.Create(doc, createID, stringID, "abc");
            var dict = doc.Item<DictItem>(dictID);
            var setID = NewID();
            doc = DictItem.Set(doc, setID, dict, "Foo", stringID);
            var stringItem = doc.DictItem<StringItem>(dictID, "Foo");
            Assert.AreEqual("abc", stringItem.Value);
            // Now change the string value and verify.
            doc = StringItem.Change(doc, NewID(), stringItem, "xyz");
            stringItem = doc.DictItem<StringItem>(dictID, "Foo");
            Assert.AreEqual("xyz", stringItem.Value);
            // Now change the field entry and verify.
            var newStringID = NewID();
            doc = StringItem.Create(doc, NewID(), newStringID, "pqr");
            dict = doc.Item<DictItem>(dictID);
            doc = DictItem.Set(doc, NewID(), dict, "Foo", newStringID);
            var newStringItem = doc.DictItem<StringItem>(dictID, "Foo");
            Assert.AreEqual("pqr", newStringItem.Value);
        }

        [TestMethod()]
        public void RemoveTest()
        {
            var doc = Doc.New();
            var dictID = NewID();
            var createID = NewID();
            doc = DictItem.Create(doc, createID, dictID);
            Assert.IsTrue(doc.HasItem(dictID));
            Assert.IsInstanceOfType(doc[dictID], typeof(DictItem));
            var stringID = NewID();
            createID = NewID();
            doc = StringItem.Create(doc, createID, stringID, "abc");
            var dict = doc.Item<DictItem>(dictID);
            var setID = NewID();
            doc = DictItem.Set(doc, setID, dict, "Foo", stringID);
            dict = doc.Item<DictItem>(dictID);
            var removeID = NewID();
            doc = DictItem.Remove(doc, removeID, dict, "Foo");
            dict = doc.Item<DictItem>(dictID);
            Assert.IsTrue(!dict.ContainsKey("Foo"));
        }

        [TestMethod()]
        public void UndoRedoTest()
        {
            var doc = Doc.New();
            Assert.IsFalse(doc.CanRedo);
            Assert.IsFalse(doc.CanUndo);
            var dictID = NewID();
            var createID = NewID();
            doc = DictItem.Create(doc, createID, dictID);
            Assert.IsFalse(doc.CanRedo);
            Assert.IsTrue(doc.CanUndo);
            Assert.IsTrue(doc.HasItem(dictID));
            Assert.IsInstanceOfType(doc[dictID], typeof(DictItem));
            doc = doc.Undo();
            Assert.IsTrue(doc.CanRedo);
            Assert.IsFalse(doc.CanUndo);
            Assert.IsFalse(doc.HasItem(dictID));
            doc = doc.Redo();
            Assert.IsFalse(doc.CanRedo);
            Assert.IsTrue(doc.CanUndo);
            Assert.IsTrue(doc.HasItem(dictID));
            Assert.IsInstanceOfType(doc[dictID], typeof(DictItem));
            var stringID = NewID();
            doc = StringItem.Create(doc, NewID(), stringID, "abc");
            var dict = doc.Item<DictItem>(dictID);
            doc = DictItem.Set(doc, NewID(), dict, "Foo", stringID);
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