using System.IO;
using System.Text.Json;
using System.Windows;

namespace IxtlanCalendar;

using static IxtlanCalendar.Holiday;

public class Helpers
{
    public static int GetNumberOfDaysInMonth(int month, int year)
    {
        bool isLeapYear  = IsLeapYear(year); 
        switch (month)
        {
            case 1: 
            case 3: 
            case 5: 
            case 7: 
            case 8: 
            case 10: 
            case 12: 
                return 31;
            case 4:
            case 6: 
            case 9: 
            case 11: 
                return 30;
            case 2: 
                if (isLeapYear)
                    return 29;
                return 28;
            default:
                throw new ArgumentOutOfRangeException("Invalid month value");
        }
    }
    // More about algorithm on https://en.wikipedia.org/wiki/Zeller%27s_congruence
    public static int GetDayOfWeekZeller(int day, int month, int year)
    {
        if (month < 3)
        {
            month += 12;
            year -= 1;
        }
        
        int yearOfCentury = year % 100;
        int centuryZero = year / 100;
        int d = (day + (13 * (month + 1) / 5) + yearOfCentury + (yearOfCentury / 4) + (centuryZero / 4) - 2 * centuryZero) % 7;
        return (d + 5) % 7; // Adjusting to make Monday = 0, Sunday = 6 for easier calculation onwards
        
    }
    public static bool IsLeapYear(int year) => year % 4 == 0 && (year % 100 != 0 || year % 400 == 0);
 
    public static Dictionary<string, List<Holiday>> LoadHolidays()
    {
        try
        {
            string json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "holidays.json"));
            return JsonSerializer.Deserialize<Dictionary<string, List<Holiday>>>(json);
        }catch (Exception ex)
        {
            var messageBoxText =
                "Can't find 'holidays.json' file. The holidays won't be displayed.\n" +
                "Make sure to provide this file in the same directory as the application.";
            var caption = "Warning";
            var button = MessageBoxButton.OK;
            var icon = MessageBoxImage.Warning;
            var messageBoxResult = MessageBox.Show(messageBoxText, caption, button, icon);

            
            Console.WriteLine($"Error loading holidays: {ex.Message}");
            return new Dictionary<string, List<Holiday>>();
        }
        
    }
}