namespace GestaoTarefas.Domain.Helper;

public static class DateTimeHelper
{
    private static readonly TimeZoneInfo BrasilTz = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
    public static DateTime ToBrazilTime(this DateTime utcDate)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDate, BrasilTz);
    }
}
