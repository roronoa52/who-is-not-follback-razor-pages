using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Text.Json;
using who_is_not_follback.Models;

namespace who_is_not_follback.Pages;
public class IndexModel : PageModel
{
	private readonly ILogger logger;

	public IndexModel(ILogger<IndexModel> logger)
    {
		this.logger = logger;
	}
    public List<(string Username, string ProfileLink, DateTime Date)> NotFollowingBack { get; set; } = new List<(string Username, string ProfileLink, DateTime Date)>();

	public async Task OnGetAsync()
	{
		var followingFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "following.json");
		var followersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "followers.json");

		var followingJson = await System.IO.File.ReadAllTextAsync(followingFilePath);
		var followersJson = await System.IO.File.ReadAllTextAsync(followersFilePath);


		var followingList = JsonSerializer.Deserialize<FollowingRoot>(followingJson);
		var followersList = JsonSerializer.Deserialize<List<FollowersRoot>>(followersJson);

		logger.LogInformation(JsonSerializer.Serialize(followingList));

		var followingUsers = followingList.relationships_following.Select(x => (
		Username: x.string_list_data[0].value,
		ProfileLink: x.string_list_data[0].href,
		Date: DateTimeOffset.FromUnixTimeSeconds(x.string_list_data[0].timestamp).UtcDateTime)).ToList();


		var followersUsers = followersList.Select(f => f.string_list_data[0].value).ToList();

		NotFollowingBack = followingUsers
			.Where(f => !followersUsers.Contains(f.Username))
			.ToList();
	}
}