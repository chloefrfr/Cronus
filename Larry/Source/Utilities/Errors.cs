using Larry.Source.Interfaces;
using Larry.Source.Utilities.Managers;
using System.Text.RegularExpressions;

namespace Larry.Source.Utilities
{

    public class ErrorData : IErrorData
    {
        public int errorCode { get; set; }
        public string errorMessage { get; set; }
        public List<string?> messageVars { get; set; } = new List<string?>();
        public int numericErrorCode { get; set; }
        public string originatingService { get; set; }
        public string intent { get; set; }
        public string createdAt { get; set; }
    }


    public static class Errors
    {
        private static List<IErrorData> errors = new List<IErrorData>();

        /// <summary>
        /// Creates a new error with the specified details.
        /// </summary>
        /// <param name="code">The error code.</param>
        /// <param name="route">The route that caused the error, which can be null.</param>
        /// <param name="message">The error message.</param>
        /// <param name="timestamp">The timestamp of the error creation.</param>
        /// <returns>The created error data.</returns>
        public static IErrorData CreateError(int code, string? route, string message, string timestamp)
        {
            string? sanitizedRoute = null;

            if (route != null)
            {
                sanitizedRoute = Regex.Replace(route, @"^(https?:\/\/)?([a-zA-Z0-9.-]+)(:\d+)?", "");
                sanitizedRoute = Regex.Replace(sanitizedRoute, @".*\/fortnite", "");
            }

            if (string.IsNullOrEmpty(sanitizedRoute))
            {
                var errorManager = new ErrorManager(new ErrorData
                {
                    errorCode = code,
                    errorMessage = message,
                    messageVars = new List<string?>(),
                    numericErrorCode = code,
                    originatingService = "Larry",
                    intent = "prod-live",
                    createdAt = timestamp
                });

                return errorManager.GetError();
            }

            var errorData = new ErrorData
            {
                errorCode = code,
                errorMessage = message,
                messageVars = new List<string?> { sanitizedRoute },
                numericErrorCode = code,
                originatingService = "Larry",
                intent = "prod-live",
                createdAt = timestamp
            };
            errors.Add(errorData);

            return new ErrorManager(errorData).GetError();
        }

        /// <summary>
        /// Gets a list of all recorded errors.
        /// </summary>
        /// <returns>A list of error data.</returns>
        public static List<IErrorData> GetErrors()
        {
            return errors;
        }

        /// <summary>
        /// Clears all recorded errors.
        /// </summary>
        public static void ClearErrors()
        {
            errors.Clear();
        }
    }
}
