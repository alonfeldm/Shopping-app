namespace SharedLibraries.Payloads;

public sealed partial class ErrorPayload
{
    public string ErrorMessage { get; set; } = string.Empty;// hold the error message to be logged
}
