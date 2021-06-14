using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Octodrome.Lib.Tests
{
    [TestClass]
    public class StringItemTests
    {
        Guid NewID() => Util.NewID();

        [TestMethod]
        public void CreateTest()
        {
            var doc = Doc.New();
            Assert.IsFalse(doc.CanRedo);
            Assert.IsFalse(doc.CanUndo);
            var stringID = NewID();
            var createID = NewID();
            doc = StringItem.Create(doc, createID, stringID, "abc");
            var stringItem = doc.Item<StringItem>(stringID);
            Assert.AreEqual("abc", stringItem.Value);
        }

        [TestMethod]
        public void ChangeTest()
        {
            var doc = Doc.New();
            var stringID = NewID();
            var createID = NewID();
            doc = StringItem.Create(doc, createID, stringID, "abc");
            var changeID = NewID();
            doc = StringItem.Change(doc, changeID, (StringItem)doc[stringID], "xyz");
            var stringItem = doc.Item<StringItem>(stringID);
            Assert.AreEqual("xyz", stringItem.Value);
        }

        [TestMethod]
        public void UndoRedoTest()
        {
            var doc = Doc.New();
            var stringID = NewID();
            var createID = NewID();
            doc = StringItem.Create(doc, createID, stringID, "abc");
            var changeID = NewID();
            doc = StringItem.Change(doc, changeID, (StringItem)doc[stringID], "xyz");
            doc = doc.Undo();
            var stringItem = doc.Item<StringItem>(stringID);
            Assert.AreEqual("abc", stringItem.Value);
            doc = doc.Undo();
            Assert.IsFalse(doc.HasItem(stringID));
            doc = doc.Redo().Redo();
            stringItem = doc.Item<StringItem>(stringID);
            Assert.AreEqual("xyz", stringItem.Value);
            Assert.IsFalse(doc.CanRedo);
            Assert.IsTrue(doc.CanUndo);
        }
    }
}
