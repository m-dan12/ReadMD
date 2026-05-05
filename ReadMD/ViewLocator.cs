using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;
using ReadMD.ViewModels;
using System;

namespace ReadMD;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null) return null;

        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type is not null)
            return App.Services!.GetRequiredService(type) as Control;

        return new TextBlock { Text = $"View Not Found: {name}" };
    }

    public bool Match(object? data) => data is ViewModelBase;
}