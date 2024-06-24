using System.Globalization;

namespace EShop_Site.Components;

public static class DateTimeFormatter
{
    private static readonly DateTimeFormatInfo DateTimeFormat = new();

    static DateTimeFormatter()
    {
        DateTimeFormat.ShortDatePattern = "dd.MM.yyyy";
        DateTimeFormat.LongTimePattern = "HH:mm";
    }
    
    public static string GeneralFormat(DateTime dateTime)
    {
        return dateTime.ToString("G", DateTimeFormat);
    }
}