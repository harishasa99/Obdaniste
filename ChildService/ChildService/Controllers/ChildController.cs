using ChildService.Models;
using ChildService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChildService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChildController : ControllerBase
    {
        private readonly ChildDataService _childDataService;

        public ChildController(ChildDataService childDataService)
        {
            _childDataService = childDataService;
        }

        // GET /api/child
        [HttpGet]
        public async Task<ActionResult<List<Child>>> GetAll()
        {
            var children = await _childDataService.GetAllAsync();
            if (children.Count == 0)
                return NoContent(); // ili Ok([]), po izboru

            return Ok(children);
        }

        // GET /api/child/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Child>> Get(int id)
        {
            var child = await _childDataService.GetAsync(id);
            if (child == null)
                return NotFound();

            return Ok(child);
        }

        // POST /api/child
        // Ovde očekujemo JSON sa "id" (int), "name", "age", "group"
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Child newChild)
        {
            // Ako koristite [ApiController], ASP.NET Core već radi ModelState validaciju pre poziva
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Upisujemo novo dete
            await _childDataService.CreateAsync(newChild);

            // Vraćamo 201 Created sa lokacijom GET rute za novo dete
            return CreatedAtAction(nameof(Get), new { id = newChild.Id }, newChild);
        }

        // PUT /api/child/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Child updatedChild)
        {
            var existing = await _childDataService.GetAsync(id);
            if (existing == null)
                return NotFound();

            // obavezno zadržimo isti Id
            updatedChild.Id = id;

            await _childDataService.UpdateAsync(id, updatedChild);
            return NoContent();
        }

        // DELETE /api/child/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _childDataService.GetAsync(id);
            if (existing == null)
                return NotFound();

            await _childDataService.RemoveAsync(id);
            return NoContent();
        }
    }
}
