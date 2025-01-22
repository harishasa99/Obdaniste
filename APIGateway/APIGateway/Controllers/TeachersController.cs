using APIGateway.Models;
using APIGateway;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TeachersController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _teacherServiceUrl;

    public TeachersController(HttpClient httpClient, IOptions<Urls> options)
    {
        _httpClient = httpClient;
        _teacherServiceUrl = options.Value.TeacherService;
    }

    
    [HttpGet]
    public async Task<IActionResult> GetTeachers()
    {
        var response = await _httpClient.GetAsync($"{_teacherServiceUrl}/users");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var teachers = await response.Content.ReadFromJsonAsync<List<Teacher>>();
        return Ok(teachers);
    }

    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeacherById(int id)
    {
        var response = await _httpClient.GetAsync($"{_teacherServiceUrl}/users/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var teacher = await response.Content.ReadFromJsonAsync<Teacher>();
        return Ok(teacher);
    }

    
    [HttpPost]
    public async Task<IActionResult> AddTeacher([FromBody] Teacher teacher)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_teacherServiceUrl}/users", teacher);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        var createdTeacher = await response.Content.ReadFromJsonAsync<Teacher>();
        return Created($"{_teacherServiceUrl}/users/{createdTeacher.Id}", createdTeacher);
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeacher(int id, [FromBody] Teacher teacher)
    {
        if (id != teacher.Id)
        {
            return BadRequest("Teacher ID mismatch");
        }

        var response = await _httpClient.PutAsJsonAsync($"{_teacherServiceUrl}/users/{id}", teacher);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        return NoContent();
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        var response = await _httpClient.DeleteAsync($"{_teacherServiceUrl}/users/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        return NoContent();
    }
}
