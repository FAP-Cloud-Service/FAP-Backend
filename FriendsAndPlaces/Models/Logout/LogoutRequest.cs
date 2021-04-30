using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Logout
{

	public class LogoutRequest
	{
		[JsonProperty(PropertyName = "loginName")]
		public string LoginName { get; set; }

		[JsonProperty(PropertyName = "sitzung")]
		public string SessionId{ get; set; }

	}
}