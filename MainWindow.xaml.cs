using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IxtlanCalendar;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<string> WeekDays { get; set; } = new List<string> { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

    public MainWindow()
    {
        InitializeComponent();
        SetWeekDays();
        PopulateDatesGrid();
    }
    
    private void SetWeekDays()
    {
        foreach (string day in WeekDays)
        {
            WeekDaysGrid.Children.Add(new Label 
            { 
                Content = day,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                FontWeight = FontWeights.Bold
            });
        }
    }

    private void PopulateDatesGrid()
    {
        // TODO: Create custom function for these
        int firstDayOfMonth = (int)new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).DayOfWeek - 1; 
        int daysInMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);

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
                        Background = j == 6 ? Brushes.LightCoral : Brushes.White, // Highlight Sundays
                        
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
    
}