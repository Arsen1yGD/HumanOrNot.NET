using System.Text;
using Newtonsoft.Json;

namespace HumanOrNot.NET;

public class Bot
{
    public HttpClient Client;

    public const string AppUri = "https://api.humanornot.ai/human-or-not";

    public string UserId;
    
    /// <summary>
    /// Generate a UUID 4 for bot.
    /// </summary>
    /// <returns>UUID 4</returns>
    public static string GenerateBotId()
    {
        const string template = "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx";

        const string xTemplate = "0123456789abcdef";
        const string yTemplate = "89ab";

        using StringWriter writer = new();
        
        Random random = new();
        
        foreach (char elem in template)
        {
            writer.Write(elem switch
            {
                'x' => xTemplate[random.Next(xTemplate.Length)],
                'y' => yTemplate[random.Next(yTemplate.Length)],
                _ => elem
            });
        }

        return writer.ToString();
    }

    /// <summary>
    /// Creates a bot that can search for chats.
    /// </summary>
    /// <param name="userAgent">
    /// Your User-Agent.
    /// Required to pass CloudFlare protection.
    /// How to get: go to https://duckduckgo.com and search for 'user agent'. Then copy text from 'Your user agent:' (excluding) to the 'Other HTTP headers' (also excluding).
    /// </param>
    public Bot(string userAgent)
    {
        Client = new HttpClient();

        Client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

        UserId = GenerateBotId();
    }

    /// <summary>
    /// Creates a bot that can search for chats.
    /// </summary>
    /// <param name="userAgent">
    /// Your User-Agent.
    /// Required to pass CloudFlare protection.
    /// How to get: go to https://duckduckgo.com and search for 'user agent'. Then copy text from 'Your user agent:' (excluding) to the 'Other HTTP headers' (also excluding).
    /// </param>
    /// <param name="userId">
    /// User ID. Used for storing your win streak on the server.
    /// </param>
    public Bot(string userAgent, string userId)
    {
        Client = new HttpClient();

        Client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

        UserId = userId;
    }

    /// <summary>
    /// Tests if your bot can connect to HON servers.
    /// </summary>
    public async Task TestConnection()
    {
        HttpResponseMessage response = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, AppUri));

        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Tries to start the conversation. 
    /// </summary>
    /// <param name="origin">Whenever you clicked 'New Game' button. Defaults to <see cref="ChatStartOrigin.LandingPage"/></param>
    /// <returns>New conversation in a dedicated class, if it's found.</returns>
    /// <exception cref="InvalidDataException">You tried to cast some integer that's not 0, 1, 2 or 3 into an enum object. Please, don't.</exception>
    public async Task<Chat> FindChat(ChatStartOrigin origin = ChatStartOrigin.LandingPage)
    {
        Dictionary<string, string> data = new()
        {
            { "user_id", UserId },
            {
                "origin", origin switch
                {
                    ChatStartOrigin.LandingPage => "honLandPage",
                    ChatStartOrigin.DecisionBox => "decisionBox",
                    ChatStartOrigin.ConversationEnded => "conversationEndedNewGameButton",
                    ChatStartOrigin.RepresentativeLogin => "honRepresentativeLogin",
                    _ => throw new InvalidDataException("Please, use types that are defined in enum.")
                }
            }
        };

        string json = JsonConvert.SerializeObject(data);

        HttpResponseMessage response = await Client.PostAsync(new Uri(AppUri + "/chat/"),
            new StringContent(json, Encoding.UTF8, "application/json"));


        response.EnsureSuccessStatusCode();

        return new Chat(this,
            JsonConvert.DeserializeObject<ChatData>(await response.Content.ReadAsStringAsync(),
                new JsonSerializerSettings { DateParseHandling = DateParseHandling.None }));
    }
}