namespace ReadMD.Models;

/// <summary>Структура строк для десериализации из JSON.</summary>
public sealed class UiStringsData
{
    public string AppName { get; set; } = string.Empty;
    public string FileNamePlaceholder { get; set; } = string.Empty;
    public string Menu { get; set; } = string.Empty;
    public string OpenFile { get; set; } = string.Empty;
    public string CloseFile { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string LanguageSystem { get; set; } = string.Empty;
    public string LanguageRussian { get; set; } = string.Empty;
    public string LanguageEnglish { get; set; } = string.Empty;
    public string Reading { get; set; } = string.Empty;
    public string Font { get; set; } = string.Empty;
    public string FontSerif { get; set; } = string.Empty;
    public string FontSansSerif { get; set; } = string.Empty;
    public string FontSize { get; set; } = string.Empty;
    public string LineSpacing { get; set; } = string.Empty;
    public string LineWidth { get; set; } = string.Empty;
}
