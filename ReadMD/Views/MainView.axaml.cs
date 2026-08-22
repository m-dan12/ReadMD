using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MarkView.Avalonia;
using ReadMD.ViewModels;
using System;

namespace ReadMD.Views;

public partial class MainView : UserControl
{
    private Point? _middleClickOrigin;
    private ScrollViewer? _scroller;
    private DispatcherTimer? _scrollTimer;
    private Point _lastKnownPointerPosition;
    private bool _isMiddleButtonScrollActive;

    private const double ScrollSpeedMultiplier = 0.75;

    public MainView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _scroller = this.FindControl<ScrollViewer>("MainScroller");

        if (_scroller != null)
        {
            // Подписываемся на события ScrollViewer
            _scroller.PointerPressed += Scroller_PointerPressed;
            _scroller.PointerMoved += Scroller_PointerMoved;
            _scroller.PointerReleased += Scroller_PointerReleased;
        }
    }

    private void Scroller_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Игнорируем клики на ScrollBar
        if (IsPointerOverScrollBar(e))
            return;

        var point = e.GetCurrentPoint(_scroller);

        if (point.Properties.IsMiddleButtonPressed && !point.Properties.IsLeftButtonPressed)
        {
            _isMiddleButtonScrollActive = true;
            _middleClickOrigin = e.GetPosition(_scroller);
            _lastKnownPointerPosition = _middleClickOrigin.Value;

            StartScrollTimer();
            e.Handled = true;
        }
    }

    private bool IsPointerOverScrollBar(PointerEventArgs e)
    {
        var source = e.Source as Control;

        while (source != null)
        {
            if (source is ScrollBar)
                return true;

            source = source.Parent as Control;
        }

        return false;
    }

    private void Scroller_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isMiddleButtonScrollActive && _middleClickOrigin.HasValue && _scroller != null)
        {
            _lastKnownPointerPosition = e.GetPosition(_scroller);
        }
    }

    private void StartScrollTimer()
    {
        if (_scrollTimer == null)
        {
            _scrollTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16)
            };
            _scrollTimer.Tick += ScrollTimer_Tick;
        }
        _scrollTimer.Start();
    }

    private void ScrollTimer_Tick(object? sender, EventArgs e)
    {
        if (!_isMiddleButtonScrollActive || !_middleClickOrigin.HasValue || _scroller == null)
        {
            StopScrolling();
            return;
        }

        var delta = _lastKnownPointerPosition - _middleClickOrigin.Value;

        double speedY = delta.Y * ScrollSpeedMultiplier;
        double speedX = delta.X * ScrollSpeedMultiplier;

        _scroller.Offset = new Vector(
            _scroller.Offset.X + speedX,
            _scroller.Offset.Y + speedY);
    }

    private void Scroller_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Middle && _isMiddleButtonScrollActive)
        {
            StopScrolling();
            e.Handled = true;
        }
    }

    private void StopScrolling()
    {
        _isMiddleButtonScrollActive = false;
        _middleClickOrigin = null;
        _scrollTimer?.Stop();
    }
}