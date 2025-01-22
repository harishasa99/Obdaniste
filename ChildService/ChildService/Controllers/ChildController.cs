using ChildService.Models;
using ChildService.Services;
using ChildService.Broker;
using ChildService.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ChildService.Controllers
{
    [ApiController]
    [Route("api/child")]
    public class ChildController : ControllerBase
    {
        private readonly ChildDataService _childDataService;
        private readonly IMessageBroker _messageBroker;

        public ChildController(ChildDataService childDataService, IMessageBroker messageBroker)
        {
            _childDataService = childDataService;
            _messageBroker = messageBroker;
        }

      
        [HttpGet]
        public async Task<ActionResult<List<ChildReadDto>>> GetAll()
        {
            var children = await _childDataService.GetAllAsync();
            var childrenDto = children.Select(c => new ChildReadDto
            {
                Id = c.Id,
                Name = c.Name,
                Age = c.Age,
                Group = c.Group
            }).ToList();

            return Ok(childrenDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChildReadDto>> Get(int id)
        {
            var child = await _childDataService.GetAsync(id);
            if (child == null)
                return NotFound();

            var childDto = new ChildReadDto
            {
                Id = child.Id,
                Name = child.Name,
                Age = child.Age,
                Group = child.Group
            };

            return Ok(childDto);
        }

       
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChildCreateDto childDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newChild = new Child
            {
                Name = childDto.Name,
                Age = childDto.Age,
                Group = childDto.Group
            };

            await _childDataService.CreateAsync(newChild);

            var childEvent = new ChildEventDto
            {
                Id = newChild.Id,
                Name = newChild.Name,
                Age = newChild.Age,
                Group = newChild.Group,
                EventType = "ChildCreated"
            };
            _messageBroker.Publish(childEvent);

            return CreatedAtAction(nameof(Get), new { id = newChild.Id }, newChild);
        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ChildUpdateDto childDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingChild = await _childDataService.GetAsync(id);
            if (existingChild == null)
                return NotFound();

            existingChild.Name = childDto.Name;
            existingChild.Age = childDto.Age;
            existingChild.Group = childDto.Group;

            await _childDataService.UpdateAsync(id, existingChild);

            
            var childEvent = new ChildEventDto
            {
                Id = existingChild.Id,
                Name = existingChild.Name,
                Age = existingChild.Age,
                Group = existingChild.Group,
                EventType = "ChildUpdated"
            };
            _messageBroker.Publish(childEvent);

            return NoContent();
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingChild = await _childDataService.GetAsync(id);
            if (existingChild == null)
                return NotFound();

            await _childDataService.RemoveAsync(id);

            
            var childEvent = new ChildEventDto
            {
                Id = id,
                Name = existingChild.Name,
                Age = existingChild.Age,
                Group = existingChild.Group,
                EventType = "ChildDeleted"
            };
            _messageBroker.Publish(childEvent);

            return NoContent();
        }
    }
}
