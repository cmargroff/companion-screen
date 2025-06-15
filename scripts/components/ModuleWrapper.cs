using System;
using Godot;

public partial class ModuleWrapper : Panel
{
  private Viewport _viewport;
  private TextureRect _cachedDisplay;
  private bool _isInitialized = false;
  private Node _moduleHolder;
  private PackedScene _moduleScene;

  public string ModuleName;

  public override void _EnterTree()
  {
    _moduleHolder = GetNode<Node>("%Module");
    _moduleScene = ResourceLoader.Load<PackedScene>(ModuleName);
    _viewport = GetViewport();
    _viewport.Connect(Viewport.SignalName.SizeChanged, Callable.From(Viewport_SizeChanged));
    _cachedDisplay = GetNode<TextureRect>("%CachedDisplay");
  }
  private void InitializeModule()
  {
    if (_isInitialized) return;

    if (_moduleScene != null)
    {
      var moduleInstance = _moduleScene.Instantiate<Control>();
      if (moduleInstance != null)
      {
        _moduleHolder.AddChild(moduleInstance);
      }
      else
      {
        GD.PrintErr("Failed to instantiate module: ", ModuleName);
      }
    }
  }
  public void ModuleFocused()
  {
    if (!_isInitialized)
    {
      InitializeModule();
    }
  }
  public void ModuleBlurred()
  {

  }

  private void Viewport_SizeChanged()
  {
    Size = _viewport.GetVisibleRect().Size;
  }

}
