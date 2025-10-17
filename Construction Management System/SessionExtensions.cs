// Created By: Srikhar Dogiparthy
#nullable enable
using Microsoft.Extensions.Options;
using Newtonsoft.Json; // may need to update to this library
namespace Dogiparthy_Spring25
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value, new
            JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
        }
        public static T? Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default :
            System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
    }
}