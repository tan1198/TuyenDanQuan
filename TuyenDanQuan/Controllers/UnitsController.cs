using EFCore.Model;
using EFCore.UnitOfWork;
using Microsoft.AspNetCore.Mvc;

namespace TuyenDanQuan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Unit>>> GetUnits()
        {
            var units = await _unitOfWork.Units.GetAllAsync();
            return Ok(units);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Unit>> GetUnit(int id)
        {
            var unit = await _unitOfWork.Units.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> CreateUnit(Unit unit)
        {
            await _unitOfWork.Units.AddAsync(unit);
            await _unitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUnit), new { id = unit.Id }, unit);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUnit(int id, Unit unit)
        {
            if (id != unit.Id)
            {
                return BadRequest();
            }

            await _unitOfWork.Units.UpdateAsync(unit);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            await _unitOfWork.Units.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<Unit>> GetByCode(string code)
        {
            var unit = await _unitOfWork.Units.GetByCodeAsync(code);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpGet("type/{unitType}")]
        public async Task<ActionResult<IEnumerable<Unit>>> GetByType(string unitType)
        {
            var units = await _unitOfWork.Units.GetByTypeAsync(unitType);
            return Ok(units);
        }
    }
}