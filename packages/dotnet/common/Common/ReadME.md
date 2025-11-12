# Waggle.Common

A lightweight shared library for consistent patterns and utilities across Waggle services, including type-safe error handling with the Result pattern.

## Installation
```bash
dotnet add package Waggle.Common
```

## Creating Results

**Success:**
```csharp
var result = Result<User>.Ok(user);
```

**Success with message:**
```csharp
var result = Result<User>.Ok(user, "User created successfully");
```

**Failure:**
```csharp
var result = Result<User>.Fail("User not found", ErrorCodes.NotFound);
```

**Validation errors:**
```csharp
var errors = new Dictionary<string, string[]>
{
    ["Email"] = new[] { "Email is required", "Invalid format" }
};
var result = Result<User>.ValidationFail(errors);
```

## Working with Results

**Map** - Transform the data if successful:
```csharp
Result<string> userId = GetUser(42).Map(user => user.Id.ToString());
```

**MapAsync** - Transform the data asynchronously:
```csharp
Result<UserDetails> details = await GetUser(42)
    .MapAsync(id => _repository.GetDetailsAsync(id));
```

**Ensure** - Add validation:
```csharp
var result = GetUser(42).Ensure(user => user.IsActive, "User is inactive");
```

**Tap** - Perform side effects without changing the result:
```csharp
var result = GetUser(42).Tap(user => _logger.LogInfo($"Retrieved user {user.Id}"));
```

**Match** - Handle success or failure:
```csharp
var message = result.Match(
    onSuccess: user => $"Hello {user.Name}",
    onFailure: error => $"Error: {error}"
);
```

**ToResult** - Convert nullable values to Result:
```csharp
User? user = _repository.Find(id);
Result<User> result = user.ToResult("User not found");
```

## HTTP Integration

**Basic action result:**
```csharp
[HttpGet("{id}")]
public async Task<IActionResult> GetUser(int id)
{
    var result = await _service.GetUserAsync(id);
    return result.ToActionResult(); // Returns 200 or appropriate error status
}
```

**Created result (201):**
```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateUserDto dto)
{
    var result = await _service.CreateUserAsync(dto);
    return result.ToCreatedResult($"/api/users/{result.Data?.Id}");
}
```

**No content result (204):**
```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var result = await _service.DeleteUserAsync(id);
    return result.ToNoContentResult();
}
```

### Response Format

All responses use JSend format:

**Success:**
```json
{
  "status": "success",
  "data": {
    "id": 42,
    "name": "John Doe"
  }
}
```

**Error:**
```json
{
  "status": "fail",
  "message": "User not found",
  "code": "NOT_FOUND"
}
```

## Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `UNAUTHORIZED` | 401 | Authentication required |
| `FORBIDDEN` | 403 | Insufficient permissions |
| `VALIDATION_FAILED` | 400 | Validation errors |
| `INVALID_INPUT` | 400 | Invalid request data |
| `NOT_FOUND` | 404 | Resource not found |
| `ALREADY_EXISTS` | 409 | Resource conflict |
| `SERVICE_FAILED` | 502 | External service error |
| `SERVICE_UNAVAILABLE` | 503 | Service unavailable |

## Example Service
```csharp
public class UserService
{
    public async Task<Result<User>> GetUserAsync(int id)
    {
        var user = await _repository.FindAsync(id);
        return user.ToResult("User not found");
    }

    public async Task<Result<User>> CreateUserAsync(CreateUserDto dto)
    {
        if (await _repository.EmailExistsAsync(dto.Email))
            return Result<User>.Fail("Email already in use", ErrorCodes.AlreadyExists);

        var user = new User { Email = dto.Email, Name = dto.Name };
        await _repository.AddAsync(user);
        
        return Result<User>.Ok(user, "User created");
    }
}
```

## Chaining Example
```csharp
public async Task<Result<Order>> PlaceOrderAsync(CreateOrderDto dto)
{
    return await _userService.GetUserAsync(dto.UserId)
        .Ensure(user => user.IsActive, "Account is inactive")
        .Ensure(user => user.HasPaymentMethod, "No payment method configured")
        .MapAsync(async user =>
        {
            var order = new Order { UserId = user.Id, Items = dto.Items };
            await _repository.AddAsync(order);
            return order;
        });
}
```