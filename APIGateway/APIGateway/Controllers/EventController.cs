using APIGateway.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace APIGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly Urls _urls;
        private readonly HttpClient _httpClient;

        public EventsController(IOptions<Urls> urls, HttpClient httpClient)
        {
            _urls = urls.Value;
            _httpClient = httpClient;
        }

        // GET /api/events
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var response = await _httpClient.GetAsync($"{_urls.ActivityService}/api/events");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<List<Event>>(content);

            return Ok(events);
        }

        // GET /api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var response = await _httpClient.GetAsync($"{_urls.ActivityService}/api/events/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            var content = await response.Content.ReadAsStringAsync();
            var eventItem = JsonConvert.DeserializeObject<Event>(content);

            return Ok(eventItem);
        }

        // POST /api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            var response = await _httpClient.PostAsJsonAsync($"{_urls.ActivityService}/api/events", newEvent);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdEvent = JsonConvert.DeserializeObject<Event>(content);

            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
        }

        // PUT /api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_urls.ActivityService}/api/events/{id}", updatedEvent);
            if (!response.IsSuccessStatusCode) return NotFound();

            return NoContent();
        }

        // DELETE /api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var response = await _httpClient.DeleteAsync($"{_urls.ActivityService}/api/events/{id}");
            if (!response.IsSuccessStatusCode) return NotFound();

            return NoContent();
        }
    }
}
