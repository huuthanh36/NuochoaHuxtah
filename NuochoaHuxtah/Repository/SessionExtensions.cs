using System.Runtime.CompilerServices;
using Newtonsoft.Json;
namespace NuochoaHuxtah.Repository
{
	public static class SessionExtensions
	{
		public static void SetJson(this ISession session, string key, object value)
		{
			session.SetString(key, JsonConvert.SerializeObject(value));
		}
		public static T GetJson<T> (this ISession session, string key)
		{
			var sessiondata = session.GetString(key);
			return sessiondata == null ? default(T) : JsonConvert.DeserializeObject<T>(sessiondata);
		}
	}
}
