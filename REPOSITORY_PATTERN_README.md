# Repository and Unit of Work Pattern Implementation

This project implements the Repository and Unit of Work patterns to provide a clean separation of concerns and centralized data access logic.

## Architecture Overview

### Repository Pattern
The Repository pattern encapsulates the logic needed to access data sources. It centralizes common data access functionality, providing better maintainability and decoupling the infrastructure or technology used to access databases from the domain model layer.

### Unit of Work Pattern
The Unit of Work pattern maintains a list of objects affected by a business transaction and coordinates writing out changes and the resolution of concurrency problems.

## Implementation Structure

```
EFCore/
├── Repositories/
│   ├── Interfaces/
│   │   ├── IRepository.cs              # Generic repository interface
│   │   ├── ICitizenRepository.cs       # Citizen-specific repository interface
│   │   ├── IUnitRepository.cs          # Unit-specific repository interface
│   │   └── IUnitOfWork.cs              # Unit of Work interface
│   └── Implementations/
│       ├── Repository.cs               # Generic repository implementation
│       ├── CitizenRepository.cs        # Citizen-specific repository implementation
│       ├── UnitRepository.cs           # Unit-specific repository implementation
│       └── UnitOfWork.cs               # Unit of Work implementation
```

## Key Features

### Generic Repository (IRepository<T>)
- **GetByIdAsync(int id)**: Retrieve entity by ID
- **GetAllAsync()**: Get all entities
- **FindAsync(Expression<Func<T, bool>> predicate)**: Find entities by predicate
- **FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)**: Get first entity matching predicate
- **AnyAsync(Expression<Func<T, bool>> predicate)**: Check if any entity exists matching predicate
- **CountAsync(Expression<Func<T, bool>>? predicate)**: Count entities with optional predicate
- **AddAsync(T entity)**: Add single entity
- **AddRangeAsync(IEnumerable<T> entities)**: Add multiple entities
- **Update(T entity)**: Update single entity
- **UpdateRange(IEnumerable<T> entities)**: Update multiple entities
- **Remove(T entity)**: Remove single entity
- **RemoveRange(IEnumerable<T> entities)**: Remove multiple entities

### Citizen Repository (ICitizenRepository)
Extends IRepository<Citizen> with specific methods:
- **GetCitizensByNameAsync(string name)**: Find citizens by name
- **GetCitizenByIdentificationNumberAsync(int identificationNumber)**: Find citizen by ID number
- **GetCitizensByAgeRangeAsync(int minAge, int maxAge)**: Find citizens within age range

### Unit Repository (IUnitRepository)
Extends IRepository<Unit> with specific methods:
- **GetUnitByCodeAsync(string code)**: Find unit by code
- **GetUnitsByTypeAsync(string unitType)**: Find units by type
- **GetUnitsByNameAsync(string unitName)**: Find units by name

### Unit of Work (IUnitOfWork)
- **Citizens**: Access to citizen repository
- **Units**: Access to unit repository
- **SaveChangesAsync()**: Save all changes to database
- **BeginTransactionAsync()**: Start a database transaction
- **CommitTransactionAsync()**: Commit current transaction
- **RollbackTransactionAsync()**: Rollback current transaction

## Usage Examples

### Basic CRUD Operations

```csharp
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

    [HttpPost]
    public async Task<ActionResult<Citizen>> CreateCitizen(Citizen citizen)
    {
        citizen.CreatedTime = DateTime.UtcNow;
        citizen.UpdatedTime = DateTime.UtcNow;
        
        await _unitOfWork.Citizens.AddAsync(citizen);
        await _unitOfWork.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetCitizen), new { id = citizen.Id }, citizen);
    }
}
```

### Transaction Management

```csharp
[HttpPost("bulk-create")]
public async Task<IActionResult> BulkCreateData(BulkDataRequest request)
{
    try
    {
        await _unitOfWork.BeginTransactionAsync();

        if (request.Citizens?.Any() == true)
        {
            await _unitOfWork.Citizens.AddRangeAsync(request.Citizens);
        }

        if (request.Units?.Any() == true)
        {
            await _unitOfWork.Units.AddRangeAsync(request.Units);
        }

        await _unitOfWork.CommitTransactionAsync();
        
        return Ok(new { Message = "Bulk data created successfully" });
    }
    catch (Exception ex)
    {
        await _unitOfWork.RollbackTransactionAsync();
        return StatusCode(500, new { Message = "Error creating bulk data", Error = ex.Message });
    }
}
```

## Dependency Injection Registration

In `Program.cs`:

```csharp
// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<ICitizenRepository, CitizenRepository>();
builder.Services.AddScoped<IUnitRepository, UnitRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

## Benefits

1. **Separation of Concerns**: Data access logic is separated from business logic
2. **Testability**: Repositories can be easily mocked for unit testing
3. **Maintainability**: Changes to data access logic are centralized
4. **Consistency**: Uniform interface for data operations across entities
5. **Transaction Management**: Coordinated transactions across multiple repositories
6. **Async Support**: All operations support async/await pattern for better performance

## API Endpoints

The implementation includes the following controllers:

### CitizenController
- `GET /api/citizen` - Get all citizens
- `GET /api/citizen/{id}` - Get citizen by ID
- `GET /api/citizen/by-name/{name}` - Get citizens by name
- `GET /api/citizen/by-identification/{identificationNumber}` - Get citizen by identification number
- `GET /api/citizen/by-age-range?minAge={minAge}&maxAge={maxAge}` - Get citizens by age range
- `POST /api/citizen` - Create new citizen
- `PUT /api/citizen/{id}` - Update citizen
- `DELETE /api/citizen/{id}` - Delete citizen

### UnitController
- `GET /api/unit` - Get all units
- `GET /api/unit/{id}` - Get unit by ID
- `GET /api/unit/by-code/{code}` - Get unit by code
- `GET /api/unit/by-type/{unitType}` - Get units by type
- `GET /api/unit/by-name/{unitName}` - Get units by name
- `POST /api/unit` - Create new unit
- `PUT /api/unit/{id}` - Update unit
- `DELETE /api/unit/{id}` - Delete unit

### DataManagementController
- `POST /api/datamanagement/bulk-create` - Create multiple entities in a single transaction
- `GET /api/datamanagement/statistics` - Get database statistics
- `DELETE /api/datamanagement/cleanup?daysOld={daysOld}` - Clean up old data

## Best Practices Implemented

1. **Generic Repository**: Provides common CRUD operations for all entities
2. **Specific Repositories**: Extend generic repository with entity-specific methods
3. **Unit of Work**: Manages transactions and coordinates multiple repositories
4. **Dependency Injection**: All dependencies are registered and injected properly
5. **Async/Await**: All database operations use async pattern for better performance
6. **Error Handling**: Proper exception handling with transaction rollback
7. **Expression Trees**: Support for LINQ expressions in repository methods