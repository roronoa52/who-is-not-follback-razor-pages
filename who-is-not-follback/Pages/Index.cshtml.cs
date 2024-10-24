using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using System.Text.Json;

namespace who_is_not_follback.Pages;
public class IndexModel : PageModel
{
	public List<(string Username, string ProfileLink, DateTime Date)> NotFollowingBack { get; set; } = new List<(string Username, string ProfileLink, DateTime Date)>();

	public async Task OnGetAsync()
	{
		var followingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "following.json");
		var followersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "followers.json");

		var followingJson = await System.IO.File.ReadAllTextAsync(followingFilePath);
		var followersJson = await System.IO.File.ReadAllTextAsync(followersFilePath);

		var followingList = JsonSerializer.Deserialize<FollowingRoot>(followingJson);
		var followersList = JsonSerializer.Deserialize<List<FollowerRoot>>(followersJson);

		var followingUsers = followingList.relationships_following.Select(f => (
		Username: f.string_list_data[0].value,
		ProfileLink: f.string_list_data[0].href,
		Date: DateTimeOffset.FromUnixTimeSeconds(f.string_list_data[0].timestamp).UtcDateTime)).ToList();


		var followersUsernames = followersList.Select(f => f.string_list_data[0].value).ToList();

		NotFollowingBack = followingUsers
			.Where(f => !followersUsernames.Contains(f.Username))
			.ToList();
	}

	public class FollowingRoot
	{
		public List<Relationship> relationships_following { get; set; }
	}

	public class FollowerRoot
	{
		public List<StringListData> string_list_data { get; set; }
	}

	public class Relationship
	{
		public List<StringListData> string_list_data { get; set; }
	}

	public class StringListData
	{
		public string href { get; set; }
		public string value { get; set; }
		public long timestamp { get; set; }
	}
}