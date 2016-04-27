using NUnit.Framework;
using UI.Data;

namespace BinderTests
{
    [TestFixture]
    public class BindingTests
    {
        [Test]
        public void BindingTwoWay()
        {
            var control = new MyControl() { Value = 0 };
            var model = new MyModel() { Value = 5 };

            var b = new Binding(model, "Value", control, "Value", BindingMode.TwoWay);
            Assert.AreEqual(model.Value, control.Value);

            model.Value = 6;
            Assert.AreEqual(model.Value, control.Value);

            control.Value = 8;
            Assert.AreEqual(model.Value, control.Value);

            b.Dispose();
        }


        [Test]
        public void BindingDestToSource()
        {
            var control = new MyControl() { Value = 0 };
            var model = new MyModel() { Value = 5 };

            var b = new Binding(model, "Value", control, "Value", BindingMode.TargetToSource);
            Assert.AreEqual(model.Value, control.Value);

            control.Value = 6;
            Assert.AreEqual(model.Value, control.Value);

            model.Value = 8;
            Assert.AreEqual(6, control.Value);

            b.Dispose();
        }

        [Test]
        public void BindingSourceToDest()
        {
            var control = new MyControl() { Value = 0 };
            var model = new MyModel() { Value = 5 };

            var b = new Binding(model, "Value", control, "Value", BindingMode.SourceToTarget);
            Assert.AreEqual(model.Value, control.Value);

            model.Value = 6;
            Assert.AreEqual(model.Value, control.Value);

            control.Value = 8;
            Assert.AreEqual(6, model.Value);

            b.Dispose();
        }
    }
}

