using System;

namespace SharedLibraries.Payloads;

public partial class Message
{
    public string MessageId {get; set;} = string.Empty;
    public string SentBy { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}

public sealed partial class ChatPostPayload
{
    public Message Message { get; set; } = new Message();
}

public sealed partial class ChatBroadcastPayload
{
    public Message Message { get; set; } = new Message();
}
