using EFCore.Model;
using EFCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TuyenDanQuan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Unit>>> GetAllUnits()
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

        [HttpGet("by-code/{code}")]
        public async Task<ActionResult<Unit>> GetUnitByCode(string code)
        {
            var unit = await _unitOfWork.Units.GetUnitByCodeAsync(code);
            if (unit == null)
            {
                return NotFound();
            }
            return Ok(unit);
        }

        [HttpGet("by-type/{unitType}")]
        public async Task<ActionResult<IEnumerable<Unit>>> GetUnitsByType(string unitType)
        {
            var units = await _unitOfWork.Units.GetUnitsByTypeAsync(unitType);
            return Ok(units);
        }

        [HttpGet("by-name/{unitName}")]
        public async Task<ActionResult<IEnumerable<Unit>>> GetUnitsByName(string unitName)
        {
            var units = await _unitOfWork.Units.GetUnitsByNameAsync(unitName);
            return Ok(units);
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> CreateUnit(Unit unit)
        {
            unit.CreatedTime = DateTime.UtcNow;
            unit.UpdatedTime = DateTime.UtcNow;
            
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

            var existingUnit = await _unitOfWork.Units.GetByIdAsync(id);
            if (existingUnit == null)
            {
                return NotFound();
            }

            unit.UpdatedTime = DateTime.UtcNow;
            _unitOfWork.Units.Update(unit);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            var unit = await _unitOfWork.Units.GetByIdAsync(id);
            if (unit == null)
            {
                return NotFound();
            }

            _unitOfWork.Units.Remove(unit);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}