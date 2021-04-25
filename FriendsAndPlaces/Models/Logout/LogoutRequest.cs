using System;
using Newtonsoft.Json;

namespace FriendsAndPlaces.Models.Logout
{

	public class LogoutRequest
	{
		[JsonProperty(PropertyName = "loginName")]
		public string LoginName { get; set; }

		[JsonProperty(PropertyName = "sessionId")]
		public string SessionId{ get; set; }

	}
}