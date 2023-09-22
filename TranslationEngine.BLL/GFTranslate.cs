using System.Net.Http.Headers;
using System.Web;

namespace TranslationEngine.BLL
{
    public class GFTranslate
    {
        public static void UploadFiles()
        {
            var translateArrayResponses = UploadFile().GetAwaiter().GetResult();
        }

        private static void CheckStatus()
        {
            var translateArrayResponses = CheckStatus("rQWWIo2mfHPSqNgFiLh8ksu88MrS06CxFj3yLFxC25X0WmVW8ApJtSpEE_q0ABIDqVfgVmKm59uIwsjNT58TILFxphZYGbhhSd2uOh8nxo0").GetAwaiter().GetResult();
        }

        public static async Task<string> UploadFile(string fileName)
        {

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["from"] = "en-us";
            query["to"] = "fr-fr";
            query["options.timeout"] = "3000";
            query["options.application"] = "TMS";
            query["options.transactionId"] = "1281_fr-fr_EvalLock";

            MultipartFormDataContent form = new MultipartFormDataContent();
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"C:\\Temp\23S32 - Ford Owner Letter Interim2.xlz");
            var byteContent = new ByteArrayContent(fileBytes);

            // byteContent.Headers.ContentType =  MediaTypeHeaderValue.Parse("application/octet-stream");
            // byteContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            //form.Add(byteContent, "Xliff", "test.xlz");
            form.Add(byteContent, "\"Xliff\"", @"C:\\Temp\23S32 - Ford Owner Letter Interim2.xlz");

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://lionbridge-geofluent-api-qa.azurewebsites.net");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());
            // client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsync("translation/v3/translate/translatexliff?" + query.ToString(), form);

            //return new TranslateResponse { ContinuationToken = response.StatusCode.ToString() };

            if (response.IsSuccessStatusCode)
            {
                string httpResponseResult = response.Content.ReadAsStringAsync().ContinueWith(task => task.Result).Result;
            }
            //throw new Exception(response.StatusCode.ToString());
        }

        public static async Task<string> CheckStatus(string continuationToken)
        {

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["continuationToken"] = continuationToken;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://lionbridge-geofluent-api-qa.azurewebsites.net");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            var response = await client.GetAsync("Translation/v3/Translate/TranslateXliff?" + query.ToString());
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().ContinueWith(task => task.Result).Result;
            }
            throw new Exception(response.StatusCode.ToString());
        }

        protected static Guid _accountKey = new Guid("a8f34f46-96b2-474c-ac9a-91aa652d1424");

        protected static string _accountSecret = "169Z4MS+Bcb33wgVqLNT+6ehZsllefPksXAgq5Oi0yo=";

        protected static string _token;

        protected static DateTime _expiration;

        protected static TimeSpan TokenTimeout = TimeSpan.FromMinutes(10.0);

        protected static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        protected static bool TokenIsAboutToExpire()
        {
            return _expiration < DateTime.UtcNow.AddMinutes(1.0);
        }

        protected static long ConvertToJavaScriptTime(DateTime dateTime)
        {
            return (long)Math.Round((dateTime - UnixEpoch).TotalSeconds);
        }

        public static string GetToken()
        {
            if (string.IsNullOrEmpty(_token) || TokenIsAboutToExpire())
            {
                DateTime utcNow = DateTime.UtcNow;
                DateTime dateTime = utcNow.Add(TokenTimeout);
                Dictionary<string, object> payload = new Dictionary<string, object>
                {
                    { "sub", _accountKey },
                    {
                        "iat",
                        ConvertToJavaScriptTime(utcNow)
                    },
                    {
                        "exp",
                        ConvertToJavaScriptTime(dateTime)
                    },
                    { "iss", "GeoFluent SDK v3" }
                };
                byte[] key = JsonWebToken.Base64UrlDecode(_accountSecret);
                _token = JsonWebToken.Encode(payload, key, JwtHashAlgorithm.HS256);
                _expiration = dateTime;
            }

            return _token;
        }
    }
}
