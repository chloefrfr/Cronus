using Larry.Source.Interfaces;

namespace Larry.Source.Utilities.Managers
{
    public class ErrorManager
    {
        private IErrorData errorData;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorManager"/> class
        /// with the specified error data.
        /// </summary>
        /// <param name="errorData">The error data to manage.</param>
        public ErrorManager(IErrorData errorData)
        {
            this.errorData = errorData;
        }

        /// <summary>
        /// Adds a message variable to the error data.
        /// </summary>
        /// <param name="messageVar">The message variable to add, which can be null.</param>
        /// <returns>The current instance of <see cref="ErrorManager"/> for method chaining.</returns>
        public ErrorManager SetMessageVar(string? messageVar)
        {
            if (messageVar != null)
            {
                errorData.messageVars.Add(messageVar);
            }
            return this;
        }

        /// <summary>
        /// Gets the managed error data.
        /// </summary>
        /// <returns>The error data managed by this instance.</returns>
        public IErrorData GetError()
        {
            return errorData;
        }
    }
}
