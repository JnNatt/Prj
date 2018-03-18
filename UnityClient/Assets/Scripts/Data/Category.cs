using System.Collections.Generic;
using Newtonsoft.Json;

public class Category
{
    [JsonProperty("category_id")]
    public int id;
    public string name;
    
    public List<TimepointData> timeline;
}
