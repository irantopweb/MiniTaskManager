namespace MiniTaskManager.ViewModels;

public class ErrorViewModel
{
    public int StatusCode { get; init; } = 500;

    public string? RequestId { get; init; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string Headline => StatusCode == 404
        ? "صفحه مورد نظر پیدا نشد."
        : "خطای غیرمنتظره‌ای رخ داد.";

    public string Description => StatusCode == 404
        ? "آدرسی که وارد کرده‌اید وجود ندارد یا حذف شده است."
        : "در پردازش درخواست خطایی رخ داد. لطفاً دوباره تلاش کنید.";
}
