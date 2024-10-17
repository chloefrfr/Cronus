using System.Web;

namespace Larry.Source.Utilities.Parsers
{
    public class BodyParser
    {
        public async Task<Dictionary<string, string>> Parse(HttpRequest request)
        {
            request.EnableBuffering();

            using (var reader = new StreamReader(request.Body))
            {
                var body = await reader.ReadToEndAsync();
                request.Body.Position = 0;

                var formValues = System.Web.HttpUtility.ParseQueryString(body);
                var dictionary = new Dictionary<string, string>();

                foreach (string key in formValues.AllKeys)
                {
                    dictionary[key] = formValues[key];
                }

                return dictionary;
            }
        }
    }
}
