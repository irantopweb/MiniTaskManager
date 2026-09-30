namespace MiniTaskManager.Extensions;

public static class StringExtensions
{
    /// <summary>رشته خالی یا فقط فاصله را به null تبدیل می‌کند و در غیر این صورت آن را Trim می‌کند.</summary>
    public static string? NullIfWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
