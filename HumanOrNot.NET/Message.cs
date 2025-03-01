using Newtonsoft.Json;

namespace HumanOrNot.NET;

public struct Message
{
    public enum MessageFrom
    {
        Me,
        Other
    }
    
    /// <summary>
    /// Message content.
    /// </summary>
    [JsonProperty("text")] public string Text;
    [JsonProperty("user")] private string _sender;

    /// <summary>
    /// Who sent the message.
    /// </summary>
    /// <exception cref="InvalidDataException">Server sent something wrong</exception>
    public MessageFrom Sender => _sender switch
    {
        "me" => MessageFrom.Me,
        "other" => MessageFrom.Other,
        _ => throw new InvalidDataException("Please, use types that are defined in enum.")
    };

    [JsonProperty("created_at")] private long _createdAtUnixTime;
    /// <summary>
    /// When was the message sent.
    /// </summary>
    public DateTime SendTime => DateTimeOffset.FromUnixTimeMilliseconds(_createdAtUnixTime).UtcDateTime;
    
    /// <summary>
    /// ID of the message
    /// </summary>
    [JsonProperty("id")] public string Uuid;
}