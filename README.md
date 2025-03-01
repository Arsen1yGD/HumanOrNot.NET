Simple library for making bots in humanornot.ai written in C# 13/.NET 9
---

# How to install

Download the .dll package from release (Don't worry if you're on linux)<br/>
Add it as a refrence in your project<br/>
Install Newthonsoft.JSON using NuGet<br/>
Profit!

---

# How to use

Before doing this, make sure you running it in async mehod.

Firstly, you need to create a bot instance: `Bot bot = new(userAgent);`
userAgent - your user agent. If you're wondering how to get it, just google it.

Also, if you have the bot ID, you'd probably want it to be in your bot: `Bot bot = new(userAgent, botId)`

To actually make bot useful, you need to find the chat: `Chat chat = await bot.FindChat();`

After the bot could find the chat, it will return a Chat object, which contains all of the methods to do chatting.<br/>
Some parts of the chat are contained in separate object `chat.ChatData`, which contains useful data too work with.

Then, you need to call `chat.WaitMessageIfNeeded()` method to wait a message, if needed.

Main logic after waiting a message is:

```
while (chat.ChatData.IsActive)
{
  if (!chat.ChatData.BotsTurn)
    break;

  string text = "A message";

  await chat.SendMessage(text);
}
```

Also, if you need to consider using partner's (other side) reply, just call `Message msg = chat.GetPartnerMessage() ?? break;`, which will return a Message object, and now you can extract the real message by doing `msg.Text`.

But after the conversation is ended, you need to guess whenever the partner was a bot or a human. You can do that by calling `chat.GuessPartner(chat.PredictPartner)`, which will always guess the right option.

---

# Special thanks
[NotFrants](https://github.com/NotFrants) - helped to make this library by providing API documentation of humanornot.ai
