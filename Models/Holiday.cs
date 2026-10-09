using System.Text.Json.Serialization;

namespace IxtlanCalendar;

public class Holiday
{
    [JsonPropertyName("day")]
    public int Day { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("repeating")]
    public bool Repeating { get; set; }
}