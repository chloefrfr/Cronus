namespace Larry.Source.Interfaces
{
    public interface IErrorData
    {
        int errorCode { get; set; }
        string errorMessage { get; set; }
        List<string?> messageVars { get; set; }
        int numericErrorCode { get; set; }
        string originatingService { get; set; }
        string intent { get; set; }
        string createdAt { get; set; }
    }
}
