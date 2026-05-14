using System;

namespace ReadMD.Services;

public interface IMarkdownDocumentService
{
    string Markdown { get; set; }
    event Action? MarkdownChanged;
}

public class MarkdownDocumentService : IMarkdownDocumentService
{
    private string _markdown = "# Заголовок";

    public string Markdown
    {
        get => _markdown;
        set
        {
            if (_markdown == value)
                return;

            _markdown = value;
            MarkdownChanged?.Invoke();
        }
    }

    public event Action? MarkdownChanged;
}
