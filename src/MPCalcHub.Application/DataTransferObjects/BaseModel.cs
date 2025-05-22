using System.Text.Json.Serialization;

namespace MPCalcHub.Application.DataTransferObjects;

public class BaseModel : Identifier
{
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("created_by")]
    public Guid CreatedBy { get; set; }

    [JsonPropertyName("removed")] 
    public bool Removed { get; set; } 

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("updated_by")]
    public Guid? UpdatedBy { get; set; }

    [JsonPropertyName("removed_at")]
    public DateTime? RemovedAt { get; set; }

    [JsonPropertyName("removed_by")]
    public Guid? RemovedBy { get; set; }

    public BaseModel() : base() { }
}