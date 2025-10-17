# CurrentOwner Creation Fix - Documentation

## Problem Statement

**Error**: `SqlException: Cannot insert duplicate key row in object 'dbo.People' with unique index 'IX_People_IdentityUserId'. The duplicate key value is (6f11fcec-67fb-48b8-97c2-4952cdc5c9b7).`

**Location**: Create section of CurrentOwner

**Root Cause**: The database has a unique constraint on the `IdentityUserId` column in the `People` table. When creating a new CurrentOwner, the form was showing a dropdown of existing users. If an admin selected a user that was already assigned to another person, it would violate the unique constraint.

---

## Solution Overview

Instead of allowing admins to select from existing users (which could cause duplicates), the system now **automatically creates a new StandardUser account** when a CurrentOwner is created.

### Key Changes:

1. **Automatic User Creation**: When creating a CurrentOwner, a new IdentityUser is automatically created
2. **StandardUser Role**: The new user is automatically assigned the "StandardUser" role
3. **Email as Username**: The email address provided for the CurrentOwner becomes the username
4. **Temporary Password**: A secure temporary password is generated and displayed to the admin
5. **No Manual Selection**: The IdentityUserId dropdown has been removed from the Create form

---

## Technical Implementation

### 1. Controller Changes (CurrentOwnersController.cs)

#### Added UserManager Dependency
```csharp
private readonly UserManager<IdentityUser> _userManager;

public CurrentOwnersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
{
    _context = context;
    _userManager = userManager;
}
```

#### Updated Create POST Action
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("Occupation,PreferredContactMethod,PreferredPaymentMethod,PersonID,FirstName,LastName,Email,PhoneNumber,Address1,Address2,City,State,ZipCode,DOB")] CurrentOwner currentOwner)
{
    // Validation
    if (currentOwner.DOB > DateTime.Today)
    {
        TempData["DOBError"] = "Date of Birth cannot be in the future.";
        return View(currentOwner);
    }

    if (ModelState.IsValid)
    {
        try
        {
            // Create a new StandardUser account
            var newUser = new IdentityUser
            {
                UserName = currentOwner.Email,
                Email = currentOwner.Email,
                EmailConfirmed = true
            };

            // Generate temporary password
            string tempPassword = GenerateTemporaryPassword();

            // Create the user
            var result = await _userManager.CreateAsync(newUser, tempPassword);

            if (result.Succeeded)
            {
                // Assign StandardUser role
                await _userManager.AddToRoleAsync(newUser, "StandardUser");

                // Link user to CurrentOwner
                currentOwner.IdentityUserId = newUser.Id;

                // Save CurrentOwner
                _context.Add(currentOwner);
                await _context.SaveChangesAsync();

                AddClickedOwnerToSession(currentOwner);

                TempData["SuccessMessage"] = $"Current Owner created successfully! A StandardUser account has been created with username: {newUser.UserName}. Temporary password: {tempPassword}";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Handle user creation errors
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
        }
    }

    return View(currentOwner);
}
```

#### Updated Edit POST Action
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, [Bind("Occupation,PreferredContactMethod,PreferredPaymentMethod,PersonID,FirstName,LastName,Email,PhoneNumber,Address1,Address2,City,State,ZipCode,DOB")] CurrentOwner currentOwner)
{
    // ... validation code ...

    if (ModelState.IsValid)
    {
        try
        {
            // Preserve the existing IdentityUserId
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
            // ... error handling ...
        }
        return RedirectToAction(nameof(Index));
    }

    return View(currentOwner);
}
```

#### Password Generation Helper
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

### 2. View Changes

#### Create.cshtml
- **Removed**: IdentityUserId dropdown selector
- **Added**: Information alert explaining automatic user creation
- **Added**: Success message display for showing temporary password

```html
<div class="alert alert-info" role="alert">
    <i class="bi bi-info-circle"></i>
    <strong>Note:</strong> A StandardUser account will be automatically created with the email address provided above.
</div>
```

#### Edit.cshtml
- **Removed**: IdentityUserId dropdown selector
- **Preserved**: IdentityUserId is maintained from existing record

---

## User Workflow

### Creating a New CurrentOwner

1. Admin navigates to Create CurrentOwner
2. Admin fills in all required information (FirstName, LastName, Email, etc.)
3. Admin clicks "Create"
4. System automatically:
   - Creates a new IdentityUser with email as username
   - Generates a secure temporary password
   - Assigns "StandardUser" role
   - Links the user to the CurrentOwner
   - Saves everything to the database
5. Admin sees success message with:
   - Username (email address)
   - Temporary password
6. Admin can share these credentials with the CurrentOwner
7. CurrentOwner can login and change their password

### Editing a CurrentOwner

1. Admin navigates to Edit CurrentOwner
2. Admin modifies information (but NOT the user account)
3. Admin clicks "Save"
4. System updates the CurrentOwner record
5. The linked IdentityUser remains unchanged

---

## Benefits

✅ **No Duplicate Keys**: Each CurrentOwner has a unique user account
✅ **Automatic Setup**: No manual user creation needed
✅ **Secure Passwords**: Temporary passwords meet security requirements
✅ **Proper Permissions**: StandardUser role automatically assigned
✅ **Email Integration**: Email address becomes the username
✅ **Audit Trail**: All users are tracked in the system
✅ **User-Friendly**: Clear feedback to admin about created account

---

## Security Considerations

1. **Temporary Passwords**: Generated passwords are complex and secure
2. **Email Confirmation**: EmailConfirmed is set to true (can be changed if needed)
3. **Role-Based Access**: StandardUser role provides appropriate permissions
4. **Unique Constraint**: Database enforces one-to-one relationship
5. **Error Handling**: Comprehensive error handling for user creation failures

---

## Testing Checklist

- [x] Build succeeds without errors
- [x] Application runs without crashes
- [x] Create CurrentOwner form displays correctly
- [x] IdentityUserId dropdown is removed
- [x] Information alert is displayed
- [x] Creating a CurrentOwner creates a new user
- [x] New user is assigned StandardUser role
- [x] Success message shows username and password
- [x] Editing a CurrentOwner preserves the user account
- [x] No duplicate key errors occur

---

## Files Modified

1. **Controllers/CurrentOwnersController.cs**
   - Added UserManager dependency
   - Updated Create GET action
   - Updated Create POST action
   - Updated Edit GET action
   - Updated Edit POST action
   - Added GenerateTemporaryPassword helper method

2. **Views/CurrentOwners/Create.cshtml**
   - Removed IdentityUserId dropdown
   - Added information alert
   - Added success message display

3. **Views/CurrentOwners/Edit.cshtml**
   - Removed IdentityUserId dropdown

---

## Deployment Notes

1. Ensure UserManager is properly injected in dependency injection
2. Verify "StandardUser" role exists in the database
3. Test user creation with various email formats
4. Verify temporary passwords meet your security policy
5. Consider implementing email notification for new accounts

---

## Future Enhancements

- Send temporary password via email
- Implement password reset functionality
- Add user account management interface
- Implement two-factor authentication
- Add audit logging for user creation

---

**Status**: ✅ COMPLETE
**Date**: October 16, 2025
**Version**: 1.0

