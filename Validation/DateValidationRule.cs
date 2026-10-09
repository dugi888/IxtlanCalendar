using System.Globalization;
using System.Windows.Controls;

namespace IxtlanCalendar;
using static IxtlanCalendar.Helpers;

public class DateValidationRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        string input = value as string;
        
        if (string.IsNullOrWhiteSpace(input))
            return ValidationResult.ValidResult;

        
        string[] dayMonthYear = input.Split('.');
        if (dayMonthYear.Length != 3
            || !int.TryParse(dayMonthYear[0], out int day)
            || !int.TryParse(dayMonthYear[1], out int month)
            || !int.TryParse(dayMonthYear[2], out int year))
        {
            return new ValidationResult(false, "Format must be DD.MM.YYYY");
        }

        //  Range
        if (month < 1 || month > 12)
            return new ValidationResult(false, "Month must be between 1 and 12.");

        if (day < 1 || day > 31)
            return new ValidationResult(false, "Day must be between 1 and 31.");

        if (year < 1 || year > 9999)
            return new ValidationResult(false, "Year is out of range. (1-9999)");

        // Day validation based on month and year 
        bool isLeap = IsLeapYear(year);

        if (month == 2 && isLeap && day > 29)
            return new ValidationResult(false, "February has only 29 days in a leap year.");

        if (month == 2 && !isLeap && day > 28)
            return new ValidationResult(false, "February has only 28 days in a non-leap year.");

        if ((month == 4 || month == 6 || month == 9 || month == 11) && day > 30)
            return new ValidationResult(false, $"{month} has only 30 days.");

        return ValidationResult.ValidResult;
    }
}