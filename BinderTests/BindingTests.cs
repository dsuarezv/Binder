using NUnit.Framework;
using UI.Data;

namespace BinderTests
{
    [TestFixture]
    public class BindingTests
    {
        [Test]
        public void Binding1()
        {
            var control = new MyControl() { Value = 0 };
            var model = new MyModel() { Value = 5 };

            var b = new Binding(model, "Value", control, "Value");

            Assert.AreEqual(model.Value, control.Value);

            model.Value = 6;

            Assert.AreEqual(model.Value, control.Value);
        }
    }
}

