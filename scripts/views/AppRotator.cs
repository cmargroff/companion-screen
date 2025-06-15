using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Godot;
using Windows.Networking.Vpn;

public partial class AppRotator : Control
{
  [Export]
  public PackedScene ModuleWrapper;
  const string MODULES_PATH = "res://modules/";
  private string[] _availableModules;
  private GridContainer _appsWrapper;
  private Camera2D _camera;
  private AnimationPlayer _animationPlayer;
  private bool _isZoomed = false;
  private int _currentModuleIndex = 0;
  private List<ModuleWrapper> _modules = new List<ModuleWrapper>();
  public override void _EnterTree()
  {
    _appsWrapper = GetNode<GridContainer>("%Apps");
    _camera = GetNode<Camera2D>("%Camera2D");
    _animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

    IndexModules();
    InitializeModules();

    GetViewport().SizeChanged += ResizeStrip;
    ResizeStrip();
    ResetStripPosition();

  }
  public override void _Ready()
  {
    if (_modules.Count > 0)
    {
      _modules[0].ModuleFocused();
    }
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventKey eventKey)
    {
      if (eventKey.IsPressed() && eventKey.Keycode == Key.Space)
      {
        if (_isZoomed)
        {
          _isZoomed = false;
          _animationPlayer.PlayBackwards("zoom");
        }
        else
        {
          _isZoomed = true;
          _animationPlayer.Play("zoom");
        }
      }
    }
  }

  private void IndexModules()
  {
    _availableModules = ResourceLoader.ListDirectory(MODULES_PATH)
      .Select(path => path + DirAccess.GetFilesAt(MODULES_PATH + path)
        .Where(file => ModuleRegex().IsMatch(file))
        .FirstOrDefault()
      )
      .Select(file => MODULES_PATH + file)
      .ToArray();
  }

  private void InitializeModules()
  {
    foreach (var modulePath in _availableModules)
    {
      var module = ModuleWrapper.Instantiate<ModuleWrapper>();
      if (module != null)
      {
        GD.Print($"Initializing module: {modulePath}");
        module.ModuleName = modulePath;
        _modules.Add(module);
        _appsWrapper.AddChild(module);
      }
      else
      {
        GD.PrintErr($"Failed to instantiate module from path: {modulePath}");
      }
    }
  }

  private void ResizeStrip()
  {
    var modulesCount = _availableModules.Length;
    var size = GetViewportRect().Size;
    _appsWrapper.Columns = _availableModules.Length;
    _appsWrapper.Size = new Vector2(
      size.X * modulesCount + (100 * (modulesCount - 1)),
      size.Y
    );
  }

  private void ResetStripPosition()
  {
    Position = Vector2.Zero;
  }
  [GeneratedRegex("main.tscn$", RegexOptions.IgnoreCase)]
  private static partial Regex ModuleRegex();
}
