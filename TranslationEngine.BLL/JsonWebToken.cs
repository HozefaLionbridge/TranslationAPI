using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace TranslationEngine.BLL
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);

        T Deserialize<T>(string json);
    }
    public class DefaultJsonSerializer : IJsonSerializer
    {

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public enum JwtHashAlgorithm
    {
        HS256,
        HS384,
        HS512
    }
    public static class JsonWebToken
    {
        private static readonly IDictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>> HashAlgorithms;

        public static IJsonSerializer JsonSerializer;

        private static readonly DateTime UnixEpoch;

        static JsonWebToken()
        {
            JsonSerializer = new DefaultJsonSerializer();
            UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            HashAlgorithms = new Dictionary<JwtHashAlgorithm, Func<byte[], byte[], byte[]>>
            {
                {
                    JwtHashAlgorithm.HS256,
                    delegate(byte[] key, byte[] value)
                    {
                        using HMACSHA256 hMACSHA3 = new HMACSHA256(key);
                        return hMACSHA3.ComputeHash(value);
                    }
                },
                {
                    JwtHashAlgorithm.HS384,
                    delegate(byte[] key, byte[] value)
                    {
                        using HMACSHA384 hMACSHA2 = new HMACSHA384(key);
                        return hMACSHA2.ComputeHash(value);
                    }
                },
                {
                    JwtHashAlgorithm.HS512,
                    delegate(byte[] key, byte[] value)
                    {
                        using HMACSHA512 hMACSHA = new HMACSHA512(key);
                        return hMACSHA.ComputeHash(value);
                    }
                }
            };
        }

        public static string Encode(object payload, string key, JwtHashAlgorithm algorithm)
        {
            return Encode(new Dictionary<string, object>(), payload, Encoding.UTF8.GetBytes(key), algorithm);
        }

        public static string Encode(object payload, byte[] key, JwtHashAlgorithm algorithm)
        {
            return Encode(new Dictionary<string, object>(), payload, key, algorithm);
        }

        public static string Encode(IDictionary<string, object> extraHeaders, object payload, string key, JwtHashAlgorithm algorithm)
        {
            return Encode(extraHeaders, payload, Encoding.UTF8.GetBytes(key), algorithm);
        }

        public static string Encode(IDictionary<string, object> extraHeaders, object payload, byte[] key, JwtHashAlgorithm algorithm)
        {
            List<string> list = new List<string>();
            Dictionary<string, object> obj = new Dictionary<string, object>(extraHeaders)
            {
                { "typ", "JWT" },
                {
                    "alg",
                    algorithm.ToString()
                }
            };
            byte[] bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));
            byte[] bytes2 = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            list.Add(Base64UrlEncode(bytes));
            list.Add(Base64UrlEncode(bytes2));
            string s = string.Join(".", list.ToArray());
            byte[] bytes3 = Encoding.UTF8.GetBytes(s);
            byte[] input = HashAlgorithms[algorithm](key, bytes3);
            list.Add(Base64UrlEncode(input));
            return string.Join(".", list.ToArray());
        }

        public static string Decode(string token, string key, bool verify = true)
        {
            return Decode(token, Encoding.UTF8.GetBytes(key), verify);
        }

        public static string Decode(string token, byte[] key, bool verify = true)
        {
            string[] array = token.Split('.');
            if (array.Length != 3)
            {
                throw new ArgumentException("Token must consist from 3 delimited by dot parts");
            }

            string text = array[0];
            string text2 = array[1];
            byte[] inArray = Base64UrlDecode(array[2]);
            string @string = Encoding.UTF8.GetString(Base64UrlDecode(text));
            string string2 = Encoding.UTF8.GetString(Base64UrlDecode(text2));
            Dictionary<string, object> dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(@string);
            if (verify)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text + "." + text2);
                string algorithm = (string)dictionary["alg"];
                byte[] inArray2 = HashAlgorithms[GetHashAlgorithm(algorithm)](key, bytes);
                string decodedCrypto = Convert.ToBase64String(inArray);
                string decodedSignature = Convert.ToBase64String(inArray2);
                Verify(decodedCrypto, decodedSignature, string2);
            }

            return string2;
        }

        public static object DecodeToObject(string token, string key, bool verify = true)
        {
            return DecodeToObject(token, Encoding.UTF8.GetBytes(key), verify);
        }

        public static object DecodeToObject(string token, byte[] key, bool verify = true)
        {
            string json = Decode(token, key, verify);
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json);
        }

        public static T DecodeToObject<T>(string token, string key, bool verify = true)
        {
            return DecodeToObject<T>(token, Encoding.UTF8.GetBytes(key), verify);
        }

        public static T DecodeToObject<T>(string token, byte[] key, bool verify = true)
        {
            string json = Decode(token, key, verify);
            return JsonSerializer.Deserialize<T>(json);
        }

        public static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input).Split('=')[0].Replace('+', '-').Replace('/', '_');
        }

        public static byte[] Base64UrlDecode(string input)
        {
            string text = input;
            text = text.Replace('-', '+');
            text = text.Replace('_', '/');
            switch (text.Length % 4)
            {
                case 2:
                    text += "==";
                    break;
                case 3:
                    text += "=";
                    break;
                default:
                    throw new FormatException("Illegal base64url string!");
                case 0:
                    break;
            }

            return Convert.FromBase64String(text);
        }

        private static JwtHashAlgorithm GetHashAlgorithm(string algorithm)
        {
            return algorithm switch
            {
                "HS256" => JwtHashAlgorithm.HS256,
                "HS384" => JwtHashAlgorithm.HS384,
                "HS512" => JwtHashAlgorithm.HS512,
                _ => throw new ArgumentException("Algorithm not supported."),
            };
        }

        private static void Verify(string decodedCrypto, string decodedSignature, string payloadJson)
        {
            if (decodedCrypto != decodedSignature)
            {
                throw new ArgumentException($"Invalid signature. Expected {decodedCrypto} got {decodedSignature}");
            }

            if (JsonSerializer.Deserialize<Dictionary<string, object>>(payloadJson).TryGetValue("exp", out var value) && value != null)
            {
                int num;
                try
                {
                    num = Convert.ToInt32(value);
                }
                catch (FormatException)
                {
                    throw new ArgumentException("Claim 'exp' must be an integer.");
                }

                if (Math.Round((DateTime.UtcNow - UnixEpoch).TotalSeconds) >= (double)num)
                {
                    throw new ArgumentException("Token has expired.");
                }
            }
        }
    }

   
}
