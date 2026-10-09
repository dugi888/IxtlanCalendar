using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static IxtlanCalendar.Month;
using static IxtlanCalendar.Helpers;
using static IxtlanCalendar.DateValidationRule;

namespace IxtlanCalendar;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int SelectedYear { get; set; } 
    private static Month SelectedMonth { get; set; }

    private static Dictionary<string, List<Holiday>> Holidays { get; set; } = new Dictionary<string, List<Holiday>>();

    private string _fullDateInput;
    public string FullDateInput
    {
        get => _fullDateInput;
        set { _fullDateInput = value; }
    }
    ValidationRule DateValidationRule { get; set; }

    public MainWindow()
    {
        // Hardcoded current month and year
        SelectedMonth = Month.October; 
        SelectedYear = 2026;
        
        InitializeComponent();
        
        DataContext = this;
        DateValidationRule = new DateValidationRule();
        
        Holidays = LoadHolidays();
        
        PopulateDatesGrid();

    }
    

    private void PopulateDatesGrid()
    {
        // Refresh the grid
        DatesGrid.Children.Clear();
        
        // Write month and year to label
        MonthYearLabel.Content = $"{SelectedMonth} {SelectedYear}";
        
        // Get first day of month and number of days
        int firstDayOfMonth = GetDayOfWeekZeller(1,(int) SelectedMonth, SelectedYear);
        int daysInMonth = GetNumberOfDaysInMonth((int) SelectedMonth, SelectedYear);

        
        
        bool hasHoliday = Holidays.ContainsKey(SelectedMonth.ToString().ToLowerInvariant());
        List<Holiday>? holidaysInCurrentMonth = new List<Holiday>();
        if (hasHoliday) Holidays.TryGetValue(SelectedMonth.ToString().ToLowerInvariant(), out holidaysInCurrentMonth);
        
        
        int currentDay = 1;
        
        for(int i = 0; i < 6; i++) // 6 weeks
        {
            for(int j = 0; j < 7; j++) // 7 days
            {
                if(i == 0 && j < firstDayOfMonth) // Skip weekdays before the 1st of the month
                {
                    DatesGrid.Children.Add(new Label());
                }
                else if(currentDay <= daysInMonth)
                {
                    // Check if current day is a holiday. If it's reapeatable, show them only for 2026 (current year).
                    bool isHoliday =  holidaysInCurrentMonth != null ? holidaysInCurrentMonth
                        .Any(h => h.Day == currentDay && (h.Repeating || SelectedYear == 2026)) : false;
                    
                    DatesGrid.Children.Add(new Border
                    {
                        BorderBrush = Brushes.LightGray,
                        BorderThickness = new Thickness(0.5),
                        Background =  isHoliday
                                ? new LinearGradientBrush // Holiday color
                                {
                                    StartPoint = new Point(0, 0),
                                    EndPoint = new Point(1, 1),
                                    GradientStops = new GradientStopCollection
                                    {
                                        new GradientStop((Color)ColorConverter.ConvertFromString("#9B1B30"), 0),
                                        new GradientStop((Color) ColorConverter.ConvertFromString("#F6F6F6"), 0.4),
                                        new GradientStop((Color) ColorConverter.ConvertFromString("#F6F6F6"), 0.6),
                                        new GradientStop((Color)ColorConverter.ConvertFromString("#9B1B30"), 1)
                                    }
                                        
                                }
                                : j == 6
                                    ? (Brush)new BrushConverter().ConvertFromString("#FF6F61") // Highlight Sundays 
                            : (Brush)new BrushConverter().ConvertFromString("#F6F6F6"),  // Default
                        
                        Child = new Label
                        {
                            Content = currentDay.ToString(),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            FontSize = 16,
                            ToolTip = isHoliday
                                ? holidaysInCurrentMonth.First(h => h.Day == currentDay).Name
                                : null
                            
                        }
                    });
                    currentDay++;
                }
                
            }
        }

    }
    
    private void YearTextBox_OnTextChanged(object sender, TextChangedEventArgs e) => UpdateCalendar();
    private void MonthComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateCalendar();
    
    
    
    private void YearTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        // Get whole input field text as this event only returns the new input character. 
        // Append the new char to text before.
        TextBox textBox = (TextBox)sender;
        string wholeText = textBox.Text + e.Text;
        e.Handled = !Regex.IsMatch(wholeText, "^\\d{1,4}$"); //  Allow only digits and up to 4
    }



    private void UpdateCalendar()
    {
        if (Int32.TryParse(YearTextBox.Text, out int year)) SelectedYear = year;
        
        if(Enum.TryParse((MonthComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString(), out Month month)) SelectedMonth  = month;
        
        
        PopulateDatesGrid();
    }

    private void UpdateCalendar(int year, int month)
    {
        try
        {
            Month monthEnum = (Month)month;
            SelectedYear = year;
            SelectedMonth = monthEnum;
            PopulateDatesGrid();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating calendar with year {year} and month {month}: {ex.Message}");
        }

    }
    private void FullDateTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender;
        string input = textBox.Text;
        
        // Require full "DD.MM.YYYY" before parsing
        if (input.Length < 10)
            return;

        ValidationResult result = DateValidationRule.Validate(input, System.Globalization.CultureInfo.CurrentCulture);
        var be = BindingOperations.GetBindingExpressionBase(textBox, TextBox.TextProperty);
        var error = new ValidationError(DateValidationRule, be)
        {
            ErrorContent = result.ErrorContent
        };
        
        if (!result.IsValid)
        {
            Validation.MarkInvalid(be, error);
            return;
        }
        
        string[] dayMonthYear = input.Split('.');
        int.TryParse(dayMonthYear[2], out int year);
        int.TryParse(dayMonthYear[1], out int month);
        
        UpdateCalendar(year, month);
    }

    private void FullDateTextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;
        string wholeText = textBox.Text + e.Text;
        if(wholeText.Length > 10) // Limit to 10 characters (DD.MM.YYYY)
        {
            e.Handled = true;
            return;
        }  
        e.Handled = !Regex.IsMatch(wholeText, @"^[\d.]+$"); // Only digits and '.'
        
    }
}