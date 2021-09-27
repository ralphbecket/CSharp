using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Octodrome.Lib.Tests
{
    [TestClass]
    public class DocTests
    {
        Guid NewID() => Util.NewID();

        [TestMethod]
        public void GroupEditsTest()
        {
            var doc = Doc.New();
            var string1ID = default(Guid);
            var string2ID = default(Guid);
            var editID = NewID();
            Assert.IsFalse(doc.HasItem(string1ID));
            Assert.IsFalse(doc.HasItem(string2ID));
            Assert.IsFalse(doc.CanUndo);
            Assert.IsFalse(doc.CanRedo);
            doc = doc.GroupEdits(editID, d =>
            {
                d = StringItem.Create(d, out string1ID, "abc");
                d = StringItem.Create(d, out string2ID, "xyz");
                return d;
            });
            Assert.IsTrue(doc.HasItem(string1ID));
            Assert.IsTrue(doc.HasItem(string2ID));
            Assert.IsTrue(doc.CanUndo);
            Assert.IsFalse(doc.CanRedo);
            doc = doc.Undo();
            Assert.IsFalse(doc.HasItem(string1ID));
            Assert.IsFalse(doc.HasItem(string2ID));
            Assert.IsFalse(doc.CanUndo);
            Assert.IsTrue(doc.CanRedo);
            doc = doc.Redo();
            Assert.IsTrue(doc.HasItem(string1ID));
            Assert.IsTrue(doc.HasItem(string2ID));
            Assert.IsTrue(doc.CanUndo);
            Assert.IsFalse(doc.CanRedo);
        }
    }
}
