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
using static IxtlanCalendar.Months;
using static IxtlanCalendar.Helpers;

namespace IxtlanCalendar;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private int SelectedYear { get; set; } 
    private static Months SelectedMonth { get; set; }


    public MainWindow()
    {
        // Hardcoded current month and year
        SelectedMonth = Months.October; 
        SelectedYear = 2026;
        
        InitializeComponent();
        
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

        int currentDay = 1;
        
        for(int i = 0; i < 6; i++) // 6 weeks
        {
            for(int j = 0; j < 7; j++) // 7 days
            {
                if(i == 0 && j < firstDayOfMonth)
                {
                    DatesGrid.Children.Add(new Label());
                }
                else if(currentDay <= daysInMonth)
                {
                    DatesGrid.Children.Add(new Border
                    {
                        BorderBrush = Brushes.LightGray,
                        BorderThickness = new Thickness(0.5),
                        Background = j == 6
                            ? (Brush)new BrushConverter().ConvertFromString("#FF6F61")
                            : (Brush)new BrushConverter().ConvertFromString("#F6F6F6"), // Highlight Sundays
                        
                        Child = new Label
                        {
                            Content = currentDay.ToString(),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            FontSize = 16
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
        e.Handled = !Regex.IsMatch(wholeText, new Regex("^\\d{1,4}$").ToString()); //  Allow only digits and up to 4
    }



    private void UpdateCalendar()
    {
        bool isValidYear = Int32.TryParse(YearTextBox.Text, out int year);
        if (isValidYear) SelectedYear = year;
        else Console.WriteLine($"CAST ERROR: Invalid year input: {YearTextBox.Text}. Please enter a valid year.");
        
        bool isValidMonth = Enum.TryParse((MonthComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString(), out Months month);
        if(isValidMonth) SelectedMonth  = month;
        else Console.WriteLine("CAST ERROR: Invalid month selection. Please select a valid month.");
        
        if(month < Months.January || month > Months.December)
        {
            // TODO: Display an error message to the user in the UI instead of just logging to console.
            Console.WriteLine($"INPUT ERROR: Invalid month value: {month}. Please select a valid month.");
            return;
        }
        
        
        PopulateDatesGrid();
    }

    private void UpdateCalendar(int year, int month)
    {
        try
        {
            // if(month < 1 || month > 12)
            // {
            //     // TODO: Display an error message to the user in the UI instead of just logging to console.
            //     Console.WriteLine($"INPUT ERROR: Invalid month value: {month}. Please select a valid month.");
            //     return;
            // }
            Months monthEnum = (Months)month;
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

        // Clear any previous error
        textBox.ClearValue(TextBox.BorderBrushProperty);
        textBox.ToolTip = null;

        if (string.IsNullOrWhiteSpace(input))
            return;

        // Require full "DD.MM.YYYY" before parsing
        if (input.Length < 10)
            return;

        string[] dayMonthYear = input.Split('.');

        if (dayMonthYear.Length != 3
            || !int.TryParse(dayMonthYear[0], out int day)
            || !int.TryParse(dayMonthYear[1], out int month)
            || !int.TryParse(dayMonthYear[2], out int year)) {
            ShowError(textBox, "Format must be DD.MM.YYYY");
            return;
        }

        if (month < 1 || month > 12) {
            ShowError(textBox, "Month must be between 1 and 12.");
            return;
        }

        if (day < 1 || day > 31) {
            ShowError(textBox, "Day must be between 1 and 31.");
            return;
        }

        if (year < 1 || year > 9999) {
            ShowError(textBox, "Year is out of range.");
            return;
        }
        
        // TODO: Check if day is valid for the given month and year.
        // TODO: Delimiter '.' add automatically while user types 
        

        UpdateCalendar(year, month);
    }

    private static void ShowError(TextBox tb, string message)
    {
        tb.BorderBrush = Brushes.Red;
        tb.BorderThickness = new Thickness(2);
        tb.ToolTip = message;
    }
}