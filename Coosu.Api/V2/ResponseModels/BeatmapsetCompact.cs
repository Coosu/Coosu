namespace Coosu.Api.V2.ResponseModels;

public class BeatmapsetCompact
{
    public string artist { get; set; }
    public string artist_unicode { get; set; }
    public BeatmapCovers covers { get; set; }
    public string creator { get; set; }
    public int favourite_count { get; set; }
    public int id { get; set; }
    public int play_count { get; set; }
    public string preview_url { get; set; }
    public string source { get; set; }
    public string status { get; set; }
    public string title { get; set; }
    public string title_unicode { get; set; }
    public long user_id { get; set; }
    public string video { get; set; }

}