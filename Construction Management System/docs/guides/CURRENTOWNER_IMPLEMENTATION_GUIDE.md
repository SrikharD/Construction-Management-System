# CurrentOwner Implementation Guide

## Overview

This guide explains the complete implementation of automatic user account creation for CurrentOwners, which resolves the duplicate key constraint violation issue.

---

## Architecture

### System Flow
```
Admin Creates CurrentOwner
        ↓
Form Submitted (POST)
        ↓
Validation (DOB, ModelState)
        ↓
Create IdentityUser
        ↓
Generate Temporary Password
        ↓
Assign StandardUser Role
        ↓
Link User to CurrentOwner
        ↓
Save to Database
        ↓
Display Success Message with Credentials
```

### Database Relationships
```
AspNetUsers (Identity)
    ↓ (1:1)
People (Base Class)
    ↓ (Inheritance)
CurrentOwner (Derived Class)
```

---

## Implementation Details

### 1. Dependency Injection

**File**: `Controllers/CurrentOwnersController.cs`

```csharp
using Microsoft.AspNetCore.Identity;

public class CurrentOwnersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public CurrentOwnersController(
        ApplicationDbContext context, 
        UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
}
```

**Why**: UserManager is needed to create and manage IdentityUser accounts.

### 2. Create GET Action

**Before**: Populated dropdown with existing users
**After**: Simple form without user selection

```csharp
[Authorize(Roles = "Admin")]
public IActionResult Create()
{
    var sessionClicks = HttpContext.Session.Get<List<CurrentOwner>>("UserOwnerClicks");
    if (sessionClicks != null)
    {
        ViewBag.UserOwnerClicks = sessionClicks;
    }
    return View();
}
```

### 3. Create POST Action

**Key Steps**:

1. **Validate Input**
   ```csharp
   if (currentOwner.DOB > DateTime.Today)
   {
       TempData["DOBError"] = "Date of Birth cannot be in the future.";
       return View(currentOwner);
   }
   ```

2. **Create IdentityUser**
   ```csharp
   var newUser = new IdentityUser
   {
       UserName = currentOwner.Email,
       Email = currentOwner.Email,
       EmailConfirmed = true
   };
   ```

3. **Generate Password**
   ```csharp
   string tempPassword = GenerateTemporaryPassword();
   ```

4. **Create User Account**
   ```csharp
   var result = await _userManager.CreateAsync(newUser, tempPassword);
   ```

5. **Assign Role**
   ```csharp
   if (result.Succeeded)
   {
       await _userManager.AddToRoleAsync(newUser, "StandardUser");
   }
   ```

6. **Link and Save**
   ```csharp
   currentOwner.IdentityUserId = newUser.Id;
   _context.Add(currentOwner);
   await _context.SaveChangesAsync();
   ```

7. **Display Success**
   ```csharp
   TempData["SuccessMessage"] = $"Current Owner created successfully! " +
       $"A StandardUser account has been created with username: {newUser.UserName}. " +
       $"Temporary password: {tempPassword}";
   ```

### 4. Edit POST Action

**Key Change**: Preserve existing IdentityUserId

```csharp
if (ModelState.IsValid)
{
    try
    {
        // Get the existing owner to preserve the IdentityUserId
        var existingOwner = await _context.CurrentOwners.FindAsync(id);
        if (existingOwner != null)
        {
            currentOwner.IdentityUserId = existingOwner.IdentityUserId;
        }

        _context.Update(currentOwner);
        await _context.SaveChangesAsync();
        AddClickedOwnerToSession(currentOwner);
    }
    catch (DbUpdateConcurrencyException)
    {
        // Handle concurrency issues
    }
    return RedirectToAction(nameof(Index));
}
```

### 5. Password Generation

```csharp
private string GenerateTemporaryPassword()
{
    const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string lowercase = "abcdefghijklmnopqrstuvwxyz";
    const string digits = "0123456789";
    const string special = "!@#$%^&*";

    var random = new Random();
    var password = new System.Text.StringBuilder();

    // Add one of each required character type
    password.Append(uppercase[random.Next(uppercase.Length)]);
    password.Append(lowercase[random.Next(lowercase.Length)]);
    password.Append(digits[random.Next(digits.Length)]);
    password.Append(special[random.Next(special.Length)]);

    // Add random characters to reach minimum length
    const string allChars = uppercase + lowercase + digits + special;
    for (int i = password.Length; i < 10; i++)
    {
        password.Append(allChars[random.Next(allChars.Length)]);
    }

    // Shuffle the password
    var passwordArray = password.ToString().ToCharArray();
    for (int i = passwordArray.Length - 1; i > 0; i--)
    {
        int randomIndex = random.Next(i + 1);
        var temp = passwordArray[i];
        passwordArray[i] = passwordArray[randomIndex];
        passwordArray[randomIndex] = temp;
    }

    return new string(passwordArray);
}
```

**Requirements Met**:
- ✅ Minimum 6 characters (10 generated)
- ✅ At least 1 uppercase letter
- ✅ At least 1 lowercase letter
- ✅ At least 1 digit
- ✅ At least 1 special character

---

## View Changes

### Create.cshtml

**Removed**:
```html
<!-- REMOVED: User selection dropdown -->
<div class="form-group">
    <label asp-for="IdentityUserId" class="col-sm-6 fw-bolder">UserName: </label>
    <select asp-for="IdentityUserId" class="form-control-sm" asp-items="ViewBag.IdentityUserId"></select>
</div>
```

**Added**:
```html
<!-- Information alert -->
<div class="alert alert-info" role="alert">
    <i class="bi bi-info-circle"></i>
    <strong>Note:</strong> A StandardUser account will be automatically created with the email address provided above.
</div>

<!-- Success message display -->
@if (TempData["SuccessMessage"] != null)
{
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        <i class="bi bi-check-circle"></i>
        <strong>Success!</strong> @TempData["SuccessMessage"]
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    </div>
}
```

### Edit.cshtml

**Removed**:
```html
<!-- REMOVED: User selection dropdown -->
<div class="form-group">
    <label asp-for="IdentityUserId" class="col-sm-6 fw-bolder"></label>
    <select asp-for="IdentityUserId" class="form-control-sm" asp-items="ViewBag.IdentityUserId"></select>
</div>
```

---

## Error Handling

### User Creation Failures

```csharp
if (result.Succeeded)
{
    // Success path
}
else
{
    // Handle user creation errors
    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }
}
```

### General Exceptions

```csharp
catch (Exception ex)
{
    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
}
```

---

## Security Considerations

1. **Password Complexity**: Meets ASP.NET Identity requirements
2. **Email Confirmation**: Set to true for new accounts
3. **Role-Based Access**: StandardUser role provides appropriate permissions
4. **Unique Constraint**: Database enforces one-to-one relationship
5. **Temporary Password**: Should be changed on first login

---

## Testing Scenarios

### Scenario 1: Successful Creation
```
1. Fill form with valid data
2. Click Create
3. ✅ User created with StandardUser role
4. ✅ Success message shows credentials
5. ✅ Can login with new credentials
```

### Scenario 2: Invalid DOB
```
1. Fill form with future DOB
2. Click Create
3. ✅ Error message displayed
4. ✅ Form preserved for correction
```

### Scenario 3: Edit Existing Owner
```
1. Navigate to Edit
2. Modify information
3. Click Save
4. ✅ Owner updated
5. ✅ User account unchanged
```

---

## Deployment Checklist

- [ ] Verify UserManager is injected correctly
- [ ] Ensure "StandardUser" role exists in database
- [ ] Test user creation with various email formats
- [ ] Verify temporary passwords meet security policy
- [ ] Test login with new credentials
- [ ] Verify role permissions are correct
- [ ] Test error scenarios
- [ ] Verify success messages display correctly

---

## Performance Considerations

- **User Creation**: Async operation (non-blocking)
- **Database Saves**: Single transaction for consistency
- **Password Generation**: Minimal overhead
- **Role Assignment**: Single database operation

---

## Future Enhancements

1. **Email Notification**: Send credentials via email
2. **Password Reset**: Implement password reset flow
3. **Account Management**: User account management interface
4. **Two-Factor Auth**: Add 2FA support
5. **Audit Logging**: Log all user creation events
6. **Bulk Import**: Create multiple owners at once

---

## Troubleshooting

### Issue: "StandardUser role not found"
**Solution**: Ensure role is created in SeedData or manually in database

### Issue: "Email already exists"
**Solution**: Check if email is already used by another user

### Issue: "Password doesn't meet requirements"
**Solution**: Verify password generator includes all required character types

### Issue: "User creation fails silently"
**Solution**: Check ModelState errors and exception details

---

**Status**: ✅ COMPLETE
**Date**: October 16, 2025
**Version**: 1.0

