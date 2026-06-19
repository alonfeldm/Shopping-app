using System;

namespace SharedLibraries.Payloads;

public partial class Message
{
    public string MessageId {get; set;} = string.Empty;// used for differentiating messages
    public string SentBy { get; set; } = string.Empty;// used to let users know who sent the message
    public string Text { get; set; } = string.Empty;// the message
    public DateTime Timestamp { get; set; }// might be used for more info to the users
}

public sealed partial class ChatPostPayload
{
    public Message Message { get; set; } = new Message();// contains all data needed for a message payload to be sent
}

public sealed partial class ChatBroadcastPayload
{
    public Message Message { get; set; } = new Message();// contains all data needed for a message payload to be sent, maintains differentiation with a regular chat post
}
