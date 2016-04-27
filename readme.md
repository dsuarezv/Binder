A simple data binding library for .Net.

# Usage

```c#
var slider = new Slider() { MinValue = 0,MaxValue = 10};
var model = new MyModel() { Data = 5 };

// Bind the model to the control
var b = new Binding(model, "Data", slider, "Value", BindingMode.TwoWay);
```

In this sample, two-way binding is used, meaning that a change in the model will update the slider control, and viceversa. It is also posible to setup bindings in a single direction (Source to Target or Target to Source).

For the binding to work, the source object has to implement the [INotifiyPropertyChanged](https://www.google.com/search?q=INotifiyPropertyChanged) interface. 

# License

This code is licensed under the MIT license.