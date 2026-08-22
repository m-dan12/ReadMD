using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ReadMD.Services;

public interface IErrorHandlingService
{
    void ShowError(string title, string message);
    void ShowWarning(string title, string message);
    Task<bool> ShowConfirmation(string title, string message);
}

public class ErrorHandlingService : IErrorHandlingService
{
    public void ShowError(string title, string message)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var messageBox = MessageBoxManager
                .GetMessageBoxStandard(title, message, ButtonEnum.Ok, Icon.Error);
            await messageBox.ShowAsync();
        });
    }

    public void ShowWarning(string title, string message)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var messageBox = MessageBoxManager
                .GetMessageBoxStandard(title, message, ButtonEnum.Ok, Icon.Warning);
            await messageBox.ShowAsync();
        });
    }

    public async Task<bool> ShowConfirmation(string title, string message)
    {
        var messageBox = MessageBoxManager
            .GetMessageBoxStandard(title, message, ButtonEnum.YesNo, Icon.Question);
        var result = await messageBox.ShowAsync();
        return result == ButtonResult.Yes;
    }
}
