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
            doc = StringItem.Create(doc, out var stringID, "abc");
            var stringItem = doc.Item<StringItem>(stringID);
            Assert.AreEqual("abc", stringItem.Value);
        }

        [TestMethod]
        public void ChangeTest()
        {
            var doc = Doc.New();
            doc = StringItem.Create(doc, out var stringID, "abc");
            doc = StringItem.Change(doc, doc.Item<StringItem>(stringID), "xyz");
            var stringItem = doc.Item<StringItem>(stringID);
            Assert.AreEqual("xyz", stringItem.Value);
        }

        [TestMethod]
        public void UndoRedoTest()
        {
            var doc = Doc.New();
            doc = StringItem.Create(doc, out var stringID, "abc");
            doc = StringItem.Change(doc, doc.Item<StringItem>(stringID), "xyz");
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
