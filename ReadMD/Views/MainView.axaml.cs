using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MarkView.Avalonia;
using ReadMD.ViewModels;
using System;

namespace ReadMD.Views;

public partial class MainView : UserControl
{
    private Point? _middleClickOrigin;
    private ScrollViewer? _scroller;
    private DispatcherTimer? _scrollTimer;
    private Point _lastKnownPointerPosition;   // ← сохраняем позицию здесь

    private const double ScrollSpeedMultiplier = 0.75; // подбирай под себя (0.5..1.2)

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
            _scroller.PointerPressed += Scroller_PointerPressed;
            _scroller.PointerMoved += Scroller_PointerMoved;   // важно!
            _scroller.PointerReleased += Scroller_PointerReleased;
        }
    }

    private void Scroller_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(_scroller).Properties.IsMiddleButtonPressed)
        {
            e.Handled = true;

            // Запоминаем точку, где зажали колесико (относительно ScrollViewer)
            _middleClickOrigin = e.GetPosition(_scroller);
            _lastKnownPointerPosition = _middleClickOrigin.Value;

            StartScrollTimer();
        }
    }

    private void Scroller_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_middleClickOrigin.HasValue && _scroller != null)
        {
            // Обновляем последнюю известную позицию курсора
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
        if (!_middleClickOrigin.HasValue || _scroller == null)
        {
            StopScrolling();
            return;
        }

        var delta = _lastKnownPointerPosition - _middleClickOrigin.Value;

        // Чем дальше от точки нажатия — тем быстрее
        double speedY = delta.Y * ScrollSpeedMultiplier;
        double speedX = delta.X * ScrollSpeedMultiplier;

        _scroller.Offset = new Vector(
            _scroller.Offset.X + speedX,
            _scroller.Offset.Y + speedY);
    }

    private void Scroller_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Middle)
        {
            e.Handled = true;
            StopScrolling();
        }
    }

    private void StopScrolling()
    {
        _middleClickOrigin = null;
        _scrollTimer?.Stop();
    }
}