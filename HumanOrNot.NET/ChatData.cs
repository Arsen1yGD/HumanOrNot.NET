using Newtonsoft.Json;

namespace HumanOrNot.NET;

public struct ChatData
{
    /// <summary>
    /// Chat ID.
    /// </summary>
    [JsonProperty("chat_id")] public string ChatId;
    /// <summary>
    /// How much time is given to chat
    /// </summary>
    [JsonProperty("chat_time")] public int ChatTime;
    /// <summary>
    /// Bot's user ID
    /// </summary>
    [JsonProperty("user_id")] public string BotUserId;
    [JsonProperty("created_at")] private long _chatCreatedAtUnixTime;
    
    /// <summary>
    /// When chat was created
    /// </summary>
    public DateTime ChatCreatedAt => DateTimeOffset.FromUnixTimeMilliseconds(_chatCreatedAtUnixTime).UtcDateTime;
    /// <summary>
    /// How many participants are in the chat. Always equals to 2
    /// </summary>
    [JsonProperty("num_participants")] public int ParticipantsCount;
    /// <summary>
    /// Partner's user ID. Can be used to do very (un)funny things
    /// </summary>
    [JsonProperty("partner_id")] public string PartnerId;
    /// <summary>
    /// Group of the partner. It is unknown how partners are sorted, but it can be used to determine whenever is partner is a bot or a human 
    /// </summary>
    [JsonProperty("partner_group")] public string PartnerGroup;
    /// <summary>
    /// Is it time to bot to make a message
    /// </summary>
    [JsonProperty("is_my_turn")] public bool BotsTurn;
    /// <summary>
    /// Is chat active for bot
    /// </summary>
    [JsonProperty("is_active")] public bool IsActive;
    /// <summary>
    /// How much time is given to send a message
    /// 20 if it's bot's turn
    /// 0 otherwise
    /// </summary>
    [JsonProperty("turn_time")] public int MessageTime;

    // Available after sending /guess/ request:
    
    /// <summary>
    /// *AVAILABLE AFTER GUESSING*: How much chats this bot had in total.
    /// </summary>
    [JsonProperty("chat_counter")] public int ChatCounter;
    
    /// <summary>
    /// *AVAILABLE AFTER GUESSING*: How much chats this bot finished.
    /// </summary>
    [JsonProperty("finished_chat_counter")]
    public int FinishedChatCounter;
    
    /// <summary>
    /// *AVAILABLE AFTER GUESSING*: How much time bot had guessed correctly.
    /// </summary>
    [JsonProperty("spot_on_guess_counter")]
    public int SpotOnCounter;

    [JsonProperty("partner_type")] private string _partnerType;

    /// <summary>
    /// *AVAILABLE AFTER GUESSING*: Who was the partner.
    /// </summary>
    /// <exception cref="InvalidDataException"></exception>
    public PartnerType PartnerType => _partnerType switch
    {
        "bot" => PartnerType.Ai,
        "human" => PartnerType.Human,
        _ => throw new InvalidDataException("Please, use types that are defined in enum.")
    };
    
    /// <summary>
    /// *AVAILABLE AFTER GUESSING*: Error message if there was some failed requests.
    /// </summary>
    [JsonProperty("message")] public string ErrorMessage;

    // Except this:
    
    /// <summary>
    /// Contains all the messages from chat.
    /// </summary>
    [JsonProperty("messages")] public List<Message> Messages;
}