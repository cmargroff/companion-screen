using System;
using Godot;

public partial class ModuleWrapper : Panel
{
  private Viewport _viewport;
  private TextureRect _cachedDisplay;

  public string ModuleName;

  public override void _EnterTree()
  {
    _viewport = GetViewport();
    _viewport.Connect(Viewport.SignalName.SizeChanged, Callable.From(Viewport_SizeChanged));
    _cachedDisplay = GetNode<TextureRect>("%CachedDisplay");
  }

  public void ModuleFocused()
  {

  }
  public void ModuleBlurred()
  {

  }

  private void Viewport_SizeChanged()
  {
    Size = _viewport.GetVisibleRect().Size;
  }

}
