using EFCore.Model;
using EFCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace TuyenDanQuan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitizensController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CitizensController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Citizen>>> GetCitizens()
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

        [HttpPost]
        public async Task<ActionResult<Citizen>> CreateCitizen(Citizen citizen)
        {
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

            await _unitOfWork.Citizens.UpdateAsync(citizen);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCitizen(int id)
        {
            await _unitOfWork.Citizens.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("search/{fullName}")]
        public async Task<ActionResult<IEnumerable<Citizen>>> SearchByFullName(string fullName)
        {
            var citizens = await _unitOfWork.Citizens.GetByFullNameAsync(fullName);
            return Ok(citizens);
        }
    }
}