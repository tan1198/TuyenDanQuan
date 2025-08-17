using EFCore.Model;
using EFCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace TuyenDanQuan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataManagementController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public DataManagementController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("bulk-create")]
        public async Task<IActionResult> BulkCreateData(BulkDataRequest request)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Add citizens
                if (request.Citizens?.Any() == true)
                {
                    foreach (var citizen in request.Citizens)
                    {
                        citizen.CreatedTime = DateTime.UtcNow;
                        citizen.UpdatedTime = DateTime.UtcNow;
                    }
                    await _unitOfWork.Citizens.AddRangeAsync(request.Citizens);
                }

                // Add units
                if (request.Units?.Any() == true)
                {
                    foreach (var unit in request.Units)
                    {
                        unit.CreatedTime = DateTime.UtcNow;
                        unit.UpdatedTime = DateTime.UtcNow;
                    }
                    await _unitOfWork.Units.AddRangeAsync(request.Units);
                }

                await _unitOfWork.CommitTransactionAsync();
                
                return Ok(new { Message = "Bulk data created successfully", 
                               CitizensCreated = request.Citizens?.Count() ?? 0,
                               UnitsCreated = request.Units?.Count() ?? 0 });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(500, new { Message = "Error creating bulk data", Error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var citizenCount = await _unitOfWork.Citizens.CountAsync();
            var unitCount = await _unitOfWork.Units.CountAsync();
            
            // Get citizens with email addresses (assuming it should be string, but model has int)
            var citizensWithEmails = await _unitOfWork.Citizens.CountAsync(c => c.EmailAddress > 0);
            
            // Get units by type
            var governmentUnits = await _unitOfWork.Units.CountAsync(u => u.UnitType == "Government");
            var privateUnits = await _unitOfWork.Units.CountAsync(u => u.UnitType == "Private");

            return Ok(new
            {
                TotalCitizens = citizenCount,
                TotalUnits = unitCount,
                CitizensWithEmails = citizensWithEmails,
                GovernmentUnits = governmentUnits,
                PrivateUnits = privateUnits
            });
        }

        [HttpDelete("cleanup")]
        public async Task<IActionResult> CleanupOldData([FromQuery] int daysOld = 30)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();

                var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                
                var oldCitizens = await _unitOfWork.Citizens.FindAsync(c => c.CreatedTime < cutoffDate);
                var oldUnits = await _unitOfWork.Units.FindAsync(u => u.CreatedTime < cutoffDate);

                if (oldCitizens.Any())
                {
                    _unitOfWork.Citizens.RemoveRange(oldCitizens);
                }

                if (oldUnits.Any())
                {
                    _unitOfWork.Units.RemoveRange(oldUnits);
                }

                await _unitOfWork.CommitTransactionAsync();

                return Ok(new { 
                    Message = "Cleanup completed successfully", 
                    CitizensRemoved = oldCitizens.Count(),
                    UnitsRemoved = oldUnits.Count()
                });
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return StatusCode(500, new { Message = "Error during cleanup", Error = ex.Message });
            }
        }
    }

    public class BulkDataRequest
    {
        public IEnumerable<Citizen>? Citizens { get; set; }
        public IEnumerable<Unit>? Units { get; set; }
    }
}