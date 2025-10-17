# Quick Reference Guide - UI Redesign

## 🚀 Getting Started

### Run the Application
```bash
cd d:\559 - Dotnet2\Dogiparthy_Spring25\Dogiparthy_Spring25
dotnet run
```

### Access the Application
- **URL**: https://localhost:5001
- **Admin Account**: admin@admin.com / Password1!
- **User Account**: user@demo.com / Password1!

---

## 🎨 Color Palette

| Color | Hex | Usage |
|-------|-----|-------|
| Primary Dark | #1a1a1a | Backgrounds, Text |
| Primary Light | #ffffff | Cards, Backgrounds |
| Secondary Dark | #2d2d2d | Hover States |
| Secondary Light | #f5f5f5 | Light Backgrounds |
| Accent Blue | #0066cc | Links, Buttons |
| Success Green | #28a745 | Success, Available |
| Danger Red | #dc3545 | Delete, Danger |
| Warning Yellow | #ffc107 | Warning, Pending |

---

## 📐 Spacing System

| Size | Value | Usage |
|------|-------|-------|
| Small | 0.5rem | Tight spacing |
| Medium | 1rem | Standard spacing |
| Large | 1.5rem | Generous spacing |
| XL | 2rem | Section spacing |

---

## 🔤 Typography

| Element | Size | Weight | Usage |
|---------|------|--------|-------|
| H1 | 2.5rem | 700 | Page titles |
| H2 | 2rem | 700 | Section titles |
| H3 | 1.5rem | 700 | Subsections |
| H4 | 1.25rem | 700 | Card titles |
| Body | 1rem | 400 | Regular text |
| Small | 0.875rem | 400 | Helper text |

---

## 🔘 Button Classes

```html
<!-- Primary Button -->
<a class="btn btn-primary">Action</a>

<!-- Dark Button -->
<a class="btn btn-dark">Secondary</a>

<!-- Success Button -->
<a class="btn btn-success">Confirm</a>

<!-- Danger Button -->
<a class="btn btn-danger">Delete</a>

<!-- Warning Button -->
<a class="btn btn-warning">Caution</a>

<!-- Outline Button -->
<a class="btn btn-outline-dark">Alternative</a>

<!-- Small Button -->
<a class="btn btn-sm btn-primary">Small</a>
```

---

## 🏷️ Badge Classes

```html
<!-- Success Badge -->
<span class="badge bg-success">Available</span>

<!-- Danger Badge -->
<span class="badge bg-danger">Unavailable</span>

<!-- Info Badge -->
<span class="badge bg-info">Info</span>

<!-- Secondary Badge -->
<span class="badge bg-secondary">Secondary</span>

<!-- Warning Badge -->
<span class="badge bg-warning">Pending</span>
```

---

## 📊 Table Structure

```html
<div class="table-responsive">
  <table class="table">
    <thead>
      <tr>
        <th><i class="bi bi-icon"></i> Column</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>Data</td>
      </tr>
    </tbody>
  </table>
</div>
```

---

## 📋 Alert Classes

```html
<!-- Success Alert -->
<div class="alert alert-success">Success message</div>

<!-- Danger Alert -->
<div class="alert alert-danger">Error message</div>

<!-- Warning Alert -->
<div class="alert alert-warning">Warning message</div>

<!-- Info Alert -->
<div class="alert alert-info">Info message</div>
```

---

## 🎯 Card Structure

```html
<div class="card">
  <div class="card-header">
    <h5>Title</h5>
  </div>
  <div class="card-body">
    Content here
  </div>
  <div class="card-footer">
    Footer content
  </div>
</div>
```

---

## 📱 Responsive Breakpoints

| Device | Width | Columns |
|--------|-------|---------|
| Mobile | < 768px | 1 |
| Tablet | 768px - 1024px | 2 |
| Desktop | > 1024px | 4 |

---

## 🔍 Icon Reference

### Navigation Icons
- `bi-house-door` - Home
- `bi-building` - Building/Sites
- `bi-clipboard-check` - Work Orders
- `bi-person-workspace` - Workers
- `bi-person-vcard` - Owners

### Action Icons
- `bi-eye` - View/Details
- `bi-pencil` - Edit
- `bi-trash` - Delete
- `bi-plus-circle` - Add/Create
- `bi-search` - Search

### Status Icons
- `bi-check-circle` - Success/Available
- `bi-x-circle` - Error/Unavailable
- `bi-info-circle` - Information
- `bi-exclamation-circle` - Warning

---

## 🛠️ CSS Variables

```css
:root {
  --primary-dark: #1a1a1a;
  --primary-light: #ffffff;
  --secondary-dark: #2d2d2d;
  --secondary-light: #f5f5f5;
  --accent-color: #0066cc;
  --accent-hover: #0052a3;
  --border-color: #e0e0e0;
  --text-dark: #1a1a1a;
  --text-light: #ffffff;
  --success-color: #28a745;
  --danger-color: #dc3545;
  --warning-color: #ffc107;
}
```

---

## 📝 Form Example

```html
<div class="form-container">
  <div class="form-group">
    <label class="form-label">Label</label>
    <input type="text" class="form-control" />
  </div>
  <button class="btn btn-primary">Submit</button>
</div>
```

---

## 🎬 Animations

| Animation | Duration | Effect |
|-----------|----------|--------|
| Slide In | 0.3s | Fade + Move |
| Hover Lift | 0.3s | Transform |
| Transition | 0.3s | Smooth change |

---

## 📂 File Structure

```
Views/
├── Shared/
│   ├── _Layout.cshtml
│   ├── _SidebarMenu.cshtml
│   ├── _LoginPartial.cshtml
│   └── _ControllerLinksPartial.cshtml
├── Home/
│   └── Index.cshtml
├── ProjectSites/
│   └── Index.cshtml
├── WorkOrders/
│   └── Index.cshtml
├── SiteWorkers/
│   └── Index.cshtml
└── CurrentOwners/
    └── Index.cshtml

wwwroot/
└── css/
    └── site.css
```

---

## 🔗 Useful Links

- **Bootstrap Icons**: https://icons.getbootstrap.com/
- **Bootstrap Docs**: https://getbootstrap.com/docs/
- **CSS Variables**: https://developer.mozilla.org/en-US/docs/Web/CSS/--*

---

## ✅ Deployment Checklist

- [ ] Build successful
- [ ] Database migrated
- [ ] CSS files linked
- [ ] Icons CDN accessible
- [ ] Test on target browsers
- [ ] Clear browser cache
- [ ] Verify seed data
- [ ] Test login functionality

---

**Last Updated**: October 16, 2025
**Version**: 1.0

