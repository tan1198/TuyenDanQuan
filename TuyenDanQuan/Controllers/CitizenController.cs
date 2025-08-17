using EFCore.Model;
using EFCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TuyenDanQuan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitizenController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CitizenController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Citizen>>> GetAllCitizens()
        {
            var citizens = await _unitOfWork.Citizens.GetAllAsync();
            return Ok(citizens);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Citizen>> GetCitizen(int id)
        {
            var citizen = await _unitOfWork.Citizens.GetByIdAsync(id);
            if (citizen == null)
            {
                return NotFound();
            }
            return Ok(citizen);
        }

        [HttpGet("by-name/{name}")]
        public async Task<ActionResult<IEnumerable<Citizen>>> GetCitizensByName(string name)
        {
            var citizens = await _unitOfWork.Citizens.GetCitizensByNameAsync(name);
            return Ok(citizens);
        }

        [HttpGet("by-identification/{identificationNumber}")]
        public async Task<ActionResult<Citizen>> GetCitizenByIdentificationNumber(int identificationNumber)
        {
            var citizen = await _unitOfWork.Citizens.GetCitizenByIdentificationNumberAsync(identificationNumber);
            if (citizen == null)
            {
                return NotFound();
            }
            return Ok(citizen);
        }

        [HttpGet("by-age-range")]
        public async Task<ActionResult<IEnumerable<Citizen>>> GetCitizensByAgeRange([FromQuery] int minAge, [FromQuery] int maxAge)
        {
            var citizens = await _unitOfWork.Citizens.GetCitizensByAgeRangeAsync(minAge, maxAge);
            return Ok(citizens);
        }

        [HttpPost]
        public async Task<ActionResult<Citizen>> CreateCitizen(Citizen citizen)
        {
            citizen.CreatedTime = DateTime.UtcNow;
            citizen.UpdatedTime = DateTime.UtcNow;
            
            await _unitOfWork.Citizens.AddAsync(citizen);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetCitizen), new { id = citizen.Id }, citizen);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCitizen(int id, Citizen citizen)
        {
            if (id != citizen.Id)
            {
                return BadRequest();
            }

            var existingCitizen = await _unitOfWork.Citizens.GetByIdAsync(id);
            if (existingCitizen == null)
            {
                return NotFound();
            }

            citizen.UpdatedTime = DateTime.UtcNow;
            _unitOfWork.Citizens.Update(citizen);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCitizen(int id)
        {
            var citizen = await _unitOfWork.Citizens.GetByIdAsync(id);
            if (citizen == null)
            {
                return NotFound();
            }

            _unitOfWork.Citizens.Remove(citizen);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}