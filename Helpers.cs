namespace IxtlanCalendar;

public class Helpers
{
    public static int GetNumberOfDaysInMonth(int month, int year)
    {
        bool isLeapYear  = year % 4 == 0 && (year % 100 != 0 || year % 400 == 0); 
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
}