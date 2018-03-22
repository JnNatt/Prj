using Newtonsoft.Json;

public class TimepointData
{
    public int id;
    public int category;
    public TimelineType type;
    public int order;
    [JsonProperty("year_title")]
    public string title;
    public string description;
}

public enum TimelineType
{
    Th, W
}
