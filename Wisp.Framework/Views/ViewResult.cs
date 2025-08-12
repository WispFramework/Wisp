using Wisp.Framework.Controllers;

namespace Wisp.Framework.Views;

public class ViewResult : IResultBox<IView>
{

    public ViewResult(IView view) => Value = view;

    public ViewResult() { }

    public IView? Value { get; set; }

    public Type ValueType => typeof(IView);
}