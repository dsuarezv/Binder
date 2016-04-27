using System.ComponentModel;
using NUnit.Framework;
using UI.Data;

namespace BinderTests
{
    [TestFixture]
    public class PathParserTests
    {
        [Test]
        public void SimpleProperty()
        {
            var m = new MyModel() { Value = 5 };
            var r = BindingPathParser.GetTargetValue(m, "Value");

            Assert.AreEqual(r, 5);
        }

        [Test]
        public void IndexedProperty()
        {
            var m = new MyModel();
            m.Data[0] = 5;
            var r = BindingPathParser.GetTargetValue(m, "Data[0]");

            Assert.AreEqual(r, 5);
        }

        [Test]
        public void IndexerProperty()
        {
            var m = new MyModel();
            var r = BindingPathParser.GetTargetValue(m, "Item[0]");

            Assert.AreEqual(r, 3);
        }

        [Test]
        public void IndexerNested()
        {
            var m = new MyModel();
            m.Child = new MyModel();
            var r = BindingPathParser.GetTargetValue(m, "Child.Item[0]");

            Assert.AreEqual(r, 3);
        }


        [Test()]
        public void SetSimpleProperty()
        {
            var m = new MyModel() { Value = 5 };
            BindingPathParser.SetTargetValue(m, "Value", 4);

            Assert.AreEqual(m.Value, 4);
        }

        [Test]
        public void SetIndexedProperty()
        {
            var m = new MyModel();
            BindingPathParser.SetTargetValue(m, "Data[0]", 5);

            Assert.AreEqual(m.Data[0], 5);
        }

        [Test]
        public void SetIndexerProperty()
        {
            var m = new MyModel();
            BindingPathParser.SetTargetValue(m, "Item[0]", 5);

            Assert.AreEqual(m[0], 5);
        }

        [Test]
        public void SetIndexerNested()
        {
            var m = new MyModel() { Name = "parent" };
            m.Child = new MyModel() { Name = "child" };
            BindingPathParser.SetTargetValue(m, "Child.Item[0]", 7);

            Assert.AreEqual(m.Child[0], 7);
        }


        [Test]
        public void CustomTraversal()
        {
            // Custom traversal is used to register propertychanged events
            // in the final binding target object (for the requested property). 
            // If it is a normal property, register for the notification event.
            // If it is an ObservableCollection, register the collection changed notification event

            var m = new MyModel() { Name = "parent" };
            m.Child = new MyModel() { Name = "child" };
            m.Child.Child = new MyModel() { Name = "grandchild" };

            var r = BindingPathParser.TraversePath(m, "Child.Child", 
                (target, property) => {
                    Assert.IsTrue(target is INotifyPropertyChanged);

                    var val = property.GetValue(target) as MyModel;
                    Assert.IsNotNull(val);
                    Assert.AreEqual("grandchild", val.Name);
                    return 16;
                });

            Assert.AreEqual(r, 16);
        }
    }
}

