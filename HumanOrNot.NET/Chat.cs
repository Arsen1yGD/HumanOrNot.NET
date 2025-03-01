using System.Text;
using Newtonsoft.Json;

namespace HumanOrNot.NET;

public class Chat(Bot bot, ChatData data)
{
    public ChatData ChatData { get; private set; } = data;


    /// <summary>
    /// Waits a message, whenever it's a bot turn or other side.
    /// </summary>
    public async Task WaitMessage()
    {
        Dictionary<string, string> data = new()
        {
            { "user_id", bot.UserId }
        };

        string json = JsonConvert.SerializeObject(data);

        HttpResponseMessage response =
            await bot.Client.PutAsync(new Uri(Bot.AppUri + $"/chat/{ChatData.ChatId}/wait-message"),
                new StringContent(json, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        await UpdateChatData(response);
    }

    /// <summary>
    /// Waits a message if needed. Still you need to call it once at the start of the game
    /// </summary>
    /// <returns>
    /// Whenever it was needed to wait a message or not.
    /// </returns>
    public async Task<bool> WaitMessageIfNeeded()
    {
        if (ChatData.BotsTurn) return false;
        await WaitMessage();
        return true;
    }

    /// <summary>
    /// Gets the message that partner had sent.
    /// </summary>
    /// <returns>A message.</returns>
    public Message? GetPartnerMessage()
    {
        try
        {
            Message msg = ChatData.Messages.Last();

            if (msg.Sender is Message.MessageFrom.Me)
                return null;

            return msg;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Sends a message to partner. Limits to 200 chars
    /// </summary>
    /// <param name="text">What to send</param>
    /// <exception cref="ArgumentOutOfRangeException">The <see cref="text"/> was too long</exception>
    public async Task SendMessage(string text)
    {
        if (text.Length > 200)
            throw new ArgumentOutOfRangeException(nameof(text));

        Dictionary<string, string> data = new()
        {
            { "user_id", bot.UserId },
            { "text", text }
        };

        string json = JsonConvert.SerializeObject(data);

        HttpResponseMessage response = await bot.Client.PutAsync(
            new Uri(Bot.AppUri + $"/chat/{ChatData.ChatId}/send-message"),
            new StringContent(json, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        await UpdateChatData(response);
    }

    /// <summary>
    /// Guesses who was the partner.
    /// </summary>
    /// <param name="who">Type of the partner</param>
    /// <exception cref="InvalidDataException">You did (PartnerType)2 or smth like that. Please don't do that.</exception>
    public async Task GuessPartner(PartnerType who)
    {
        Dictionary<string, string> data = new()
        {
            { "user_id", bot.UserId },
            {
                "partner_type", who switch
                {
                    PartnerType.Human => "human",
                    PartnerType.Ai => "bot",
                    _ => throw new InvalidDataException("Please, use types that are defined in enum.")
                }
            }
        };

        string json = JsonConvert.SerializeObject(data);

        HttpResponseMessage response = await bot.Client.PutAsync(new Uri(Bot.AppUri + $"/chat/{ChatData.ChatId}/guess"),
            new StringContent(json, Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        await UpdateChatData(response);
    }

    /// <summary>
    /// Predicts whenever partner is.
    /// </summary>
    /// <seealso cref="GuessPartner"/>
    public PartnerType PredictPartner => "EHKdfiz".Contains(ChatData.PartnerGroup) ? PartnerType.Ai : PartnerType.Human;

    private async Task UpdateChatData(HttpResponseMessage response)
    {
        ChatData = JsonConvert.DeserializeObject<ChatData>(await response.Content.ReadAsStringAsync(),
            new JsonSerializerSettings { DateParseHandling = DateParseHandling.None });
    }
}