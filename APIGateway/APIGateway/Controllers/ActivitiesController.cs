using APIGateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace APIGateway.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ActivitiesController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _activityServiceUrl;

    public ActivitiesController(HttpClient httpClient, IOptions<Urls> options)
    {
        _httpClient = httpClient;
        _activityServiceUrl = options.Value.ActivityService;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetActivities()
    {
        var response = await _httpClient.GetAsync($"{_activityServiceUrl}/api/activities");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var activities = await response.Content.ReadFromJsonAsync<List<Activity>>();
        return Ok(activities);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetActivityById(int id)
    {
        var response = await _httpClient.GetAsync($"{_activityServiceUrl}/api/activities/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var activity = await response.Content.ReadFromJsonAsync<Activity>();
        return Ok(activity);
    }

    
    [HttpPost]
    public async Task<IActionResult> AddActivity([FromBody] Activity activity)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_activityServiceUrl}/api/activities", activity);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var createdActivity = await response.Content.ReadFromJsonAsync<Activity>();
        return Created($"{_activityServiceUrl}/api/activities/{createdActivity.Id}", createdActivity);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateActivity(int id, [FromBody] Activity activity)
    {
        if (id != activity.Id)
        {
            return BadRequest("Activity ID mismatch");
        }

        var response = await _httpClient.PutAsJsonAsync($"{_activityServiceUrl}/api/activities/{id}", activity);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteActivity(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_activityServiceUrl}/api/activities/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        return NoContent();
    }
}
