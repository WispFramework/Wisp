// This file is part of Wisp Framework.
// 
// Licensed under either of
//   * Apache License, Version 2.0 (https://www.apache.org/licenses/LICENSE-2.0)
//   * MIT License (https://opensource.org/licenses/MIT)
// at your option.

using Wisp.Framework.Controllers;

namespace Wisp.Framework.Views;

public class ViewResult : IResultBox<IView>
{

    public ViewResult(IView view) => Value = view;

    public ViewResult() { }

    public IView? Value { get; set; }

    public Type ValueType => typeof(IView);
}