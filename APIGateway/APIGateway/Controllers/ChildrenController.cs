using APIGateway.Models;
using APIGateway;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ChildrenController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _childServiceUrl;

    public ChildrenController(HttpClient httpClient, IOptions<Urls> options)
    {
        _httpClient = httpClient;
        _childServiceUrl = options.Value.ChildService;
    }

    [HttpGet]
    public async Task<IActionResult> GetChildren()
    {
        var response = await _httpClient.GetAsync($"{_childServiceUrl}/api/child");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var children = await response.Content.ReadFromJsonAsync<List<Child>>();
        return Ok(children);
    }

    [HttpPost]
    public async Task<IActionResult> AddChild([FromBody] Child child)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_childServiceUrl}/api/child", child);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var createdChild = await response.Content.ReadFromJsonAsync<Child>();
        return Created($"{_childServiceUrl}/api/child/{createdChild.Id}", createdChild);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateChild(int id, [FromBody] Child child)
    {
        if (id != child.Id)
        {
            return BadRequest("Child ID mismatch");
        }

        var response = await _httpClient.PutAsJsonAsync($"{_childServiceUrl}/api/child/{id}", child);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteChild(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_childServiceUrl}/api/child/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        return NoContent();
    }
}
