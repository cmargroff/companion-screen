using System;
using System.IO;
using Godot;
using Windows.Media.Control;
using WindowsMediaController;

public partial class Main : Control
{
  MediaManager _mediaManager;
  TextureRect _albumArt;
  TextureRect _bg;
  TextureRect _bgRender;
  SubViewport _subViewport;
  Label _title;
  Label _artist;
  ImageTexture _albumTexture;

  public override void _Ready()
  {
    _albumArt = GetNode<TextureRect>("%AlbumArt");
    _bg = GetNode<TextureRect>("%BG");
    _title = GetNode<Label>("%Title");
    _artist = GetNode<Label>("%Artist");
    _bgRender = GetNode<TextureRect>("%BGRender");
    _bgRender.Connect(TextureRect.SignalName.Draw, Callable.From(BGRender_Draw));
    _subViewport = GetNode<SubViewport>("%SubViewport");

    _mediaManager = new MediaManager();

    // _mediaManager.OnAnySessionOpened += MediaManager_OnAnySessionOpened;
    // _mediaManager.OnAnySessionClosed += MediaManager_OnAnySessionClosed;
    // _mediaManager.OnFocusedSessionChanged += MediaManager_OnFocusedSessionChanged;
    _mediaManager.OnAnyMediaPropertyChanged += MediaManager_OnAnyMediaPropertyChanged;
    _mediaManager.OnAnyPlaybackStateChanged += MediaManager_OnAnyPlaybackStateChanged;
    _mediaManager.OnAnyTimelinePropertyChanged += MediaManager_OnAnyTimelinePropertyChanged;

    _mediaManager.StartAsync();
  }

  private void MediaManager_OnAnyTimelinePropertyChanged(MediaManager.MediaSession mediaSession, GlobalSystemMediaTransportControlsSessionTimelineProperties timelineProperties)
  {
    GD.Print("AnyTimelinePropertyChanged");
    GD.Print(timelineProperties.Position);
  }


  private void MediaManager_OnAnyPlaybackStateChanged(MediaManager.MediaSession mediaSession, GlobalSystemMediaTransportControlsSessionPlaybackInfo playbackInfo)
  {
    GD.Print("AnyPlaybackStateChanged");
    GD.Print(playbackInfo.PlaybackStatus);
  }


  private async void MediaManager_OnAnyMediaPropertyChanged(MediaManager.MediaSession mediaSession, GlobalSystemMediaTransportControlsSessionMediaProperties mediaProperties)
  {
    GD.Print("AnyMediaPropertyChanged");
    _title.CallDeferred("set_text", mediaProperties.Title);
    _artist.CallDeferred("set_text", mediaProperties.Artist);
    if (mediaProperties.Thumbnail is not null)
    {
      var accessStream = await mediaProperties.Thumbnail.OpenReadAsync();

      var stream = accessStream.AsStreamForRead();
      byte[] buf = new byte[accessStream.Size];
      await stream.ReadAsync(buf, 0, (int)accessStream.Size);

      var img = new Image();
      img.LoadPngFromBuffer(buf);

      _albumTexture = ImageTexture.CreateFromImage(img);
      _albumArt.CallDeferred("set_texture", _albumTexture);
      _bgRender.CallDeferred("set_texture", _albumTexture);

      GD.Print(accessStream.ContentType);
    }
  }

  private void BGRender_Draw()
  {
    GD.Print("BGRender Draw");
    _subViewport.RenderTargetUpdateMode = SubViewport.UpdateMode.Once;
  }

  // private void MediaManager_OnFocusedSessionChanged(MediaManager.MediaSession mediaSession)
  // {
  //   GD.Print("FocusedSessionChanged");
  // }


  // private void MediaManager_OnAnySessionClosed(MediaManager.MediaSession mediaSession)
  // {
  //   GD.Print("AnySessionClosed");
  // }


  // private void MediaManager_OnAnySessionOpened(MediaManager.MediaSession mediaSession)
  // {
  //   GD.Print("AnySessionOpened");
  // }
}
