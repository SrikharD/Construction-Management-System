# 🚀 Quick Reference: Account Creation & Login

## 📋 How to Create a Site Worker Account

### Step 1: Login as Admin
- Go to: `https://localhost:5001/Identity/Account/Login`
- Email: `admin@example.com`
- Password: `Admin@123`

### Step 2: Navigate to Site Workers
- Click "Site Workers" in the navigation menu
- Click "Add New Site Worker" button

### Step 3: Fill in Details
- **First Name**: Alice
- **Last Name**: Testing
- **Email**: alicetesting@gmail.com
- **Phone**: (555) 123-4567
- **Date of Birth**: 01/15/1990
- **Address**: 123 Main St
- **City**: Springfield
- **State**: IL
- **Zip Code**: 62701
- **Start Date**: 10/16/2025
- **Hourly Wage**: 25.00
- **Is Available**: ✓ Check

### Step 4: Click Create
- System automatically creates IdentityUser account
- Success message displays credentials

### Step 5: Share Credentials with Worker
- **Username**: alicetesting@gmail.com
- **Password**: Password1!

---

## 🔐 How to Login as Site Worker

### Step 1: Go to Login Page
- URL: `https://localhost:5001/Identity/Account/Login`

### Step 2: Enter Credentials
- **Email**: alicetesting@gmail.com
- **Password**: Password1!

### Step 3: Click Sign In
- Redirected to dashboard
- See assigned work orders
- See assigned project sites

---

## 📊 Site Worker Dashboard Features

### Statistics Cards
- **Total Work Orders**: Count of all assigned work orders
- **In Progress**: Count of work orders in progress
- **Completed**: Count of completed work orders
- **Project Sites**: Count of assigned project sites

### Work Orders Table
- Work Order ID
- Project Site Name
- Status (Completed, In Progress, Not Started, On Hold)
- Description
- View button

### Project Sites Grid
- Site Name
- Location
- Property Owner
- View Details button

---

## 👨‍💼 How to Create a Current Owner Account

### Step 1: Login as Admin
- Go to: `https://localhost:5001/Identity/Account/Login`
- Email: `admin@example.com`
- Password: `Admin@123`

### Step 2: Navigate to Current Owners
- Click "Property Owners" in the navigation menu
- Click "Add New Owner" button

### Step 3: Fill in Details
- **First Name**: John
- **Last Name**: Doe
- **Email**: john@example.com
- **Phone**: (555) 987-6543
- **Date of Birth**: 05/20/1985
- **Address**: 456 Oak Ave
- **City**: Springfield
- **State**: IL
- **Zip Code**: 62701

### Step 4: Click Create
- System automatically creates IdentityUser account
- Success message displays credentials

### Step 5: Share Credentials
- **Username**: john@example.com
- **Password**: Password1!

---

## ❌ Public Registration Disabled

**Important**: The public registration page has been removed.

### To Create New Accounts:
1. ✅ Contact the system administrator
2. ✅ Admin creates account in admin panel
3. ✅ Admin shares credentials with user
4. ✅ User logs in with provided credentials

### Why?
- 🔐 Security: Controlled access
- 👨‍💼 Admin oversight: All accounts created by admin
- 📋 Consistency: Proper role assignment
- 🛡️ Protection: Prevents unauthorized access

---

## 🔑 Default Credentials

### Admin Account
- **Email**: admin@example.com
- **Password**: Admin@123
- **Role**: Admin

### Test Site Worker (Alice)
- **Email**: alicetesting@gmail.com
- **Password**: Password1!
- **Role**: StandardUser (SiteWorker)

### Test Current Owner
- **Email**: john@example.com
- **Password**: Password1!
- **Role**: StandardUser (CurrentOwner)

---

## 🆘 Troubleshooting

### Login Not Working
- ✓ Verify email is correct
- ✓ Verify password is correct (case-sensitive)
- ✓ Check if account exists in admin panel
- ✓ Verify user has StandardUser role

### Can't Create Account
- ✓ Only admin can create accounts
- ✓ Contact system administrator
- ✓ Admin creates account in admin panel

### Dashboard Not Showing Work Orders
- ✓ Verify work orders are assigned to you
- ✓ Check with admin to assign work orders
- ✓ Refresh the page

### Can't See Project Sites
- ✓ Verify you have work orders assigned
- ✓ Project sites appear based on assigned work orders
- ✓ Contact admin to assign work orders

---

## 📞 Support

For account creation or access issues:
1. Contact the system administrator
2. Provide your name and email
3. Admin will create account and share credentials
4. Login with provided credentials

---

**Version**: 1.0
**Last Updated**: October 16, 2025

