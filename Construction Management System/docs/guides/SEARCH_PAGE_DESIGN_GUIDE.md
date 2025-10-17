# 🔍 PROJECT SITES SEARCH PAGE DESIGN GUIDE

## Overview

The ProjectSites Search page has been completely redesigned with a modern, user-friendly interface that makes finding project sites intuitive and efficient.

---

## 📐 **PAGE LAYOUT STRUCTURE**

```
┌─────────────────────────────────────────────────────────┐
│  Header Section (Dark Gradient)                         │
│  🔍 Search Project Sites                               │
│  Find project sites by title                            │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│  Search Form Card (Purple Gradient Header)              │
│  🔎 Search Criteria                                     │
├─────────────────────────────────────────────────────────┤
│  [Site Title Input Field]                               │
│  [Search Button] [Back to List Button]                  │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│  Results Section (if results found)                     │
│  ✓ Search Results (X found)                             │
├─────────────────────────────────────────────────────────┤
│  Results Table:                                         │
│  [Site Title] [Location] [Size] [Type] [Actions]       │
│  [Result 1]   [Result 2]  [...]                         │
└─────────────────────────────────────────────────────────┘

OR

┌─────────────────────────────────────────────────────────┐
│  Empty State (if no results)                            │
│  🔍 No Results Found                                    │
│  Try different keywords or browse all sites             │
│  [View All Sites Button]                                │
└─────────────────────────────────────────────────────────┘
```

---

## 🎨 **COLOR SCHEME**

### Header Section
- **Background**: Linear gradient (#1a1a1a → #2d2d2d)
- **Text**: White
- **Icon**: Search icon

### Search Form Card
- **Header Background**: Linear gradient (#667eea → #764ba2)
- **Header Text**: White
- **Header Icon**: Funnel icon

### Results Table
- **Header Background**: Light gray (#f8f9fa)
- **Row Hover**: Light background on hover
- **Badges**: Secondary color (#6c757d)

### Alerts
- **Warning Alert**: Yellow (#ffc107) - for search errors
- **Empty State**: Light blue (#f0f8ff) with blue border

---

## 🔎 **SEARCH FORM SECTION**

### Input Field
- **Type**: Text input
- **Class**: `form-control form-control-lg`
- **Placeholder**: "Enter site title to search..."
- **Label**: "Site Title" with building icon
- **Validation**: Shows validation errors if any

### Buttons
- **Search Button**:
  - Class: `btn btn-primary btn-lg`
  - Icon: Search icon
  - Text: "Search"
  - Action: Submit form

- **Back Button**:
  - Class: `btn btn-secondary btn-lg`
  - Icon: Arrow left icon
  - Text: "Back to List"
  - Action: Navigate to Index

### Layout
- Centered card (8 columns, offset 2)
- Buttons centered with 1rem gap
- Proper spacing and padding

---

## 📊 **RESULTS TABLE**

### Columns
1. **Site Title** (with building icon)
   - Bold text
   - Primary information

2. **Location** (with geo-alt icon)
   - Address information

3. **Size (Sq Ft)** (with rulers icon)
   - Square footage

4. **Type** (with tag icon)
   - Site type badge
   - Secondary color

5. **Actions** (centered)
   - View Details button
   - Eye icon
   - Primary color

### Table Features
- **Responsive**: Scrollable on mobile
- **Hover Effect**: Row highlights on hover
- **Header**: Light gray background
- **Icons**: Bootstrap icons for each column
- **Result Count**: Shows number of results found

### View Details Button
- **Class**: `btn btn-sm btn-primary`
- **Icon**: Eye icon
- **Text**: "View Details"
- **Action**: Navigate to Details page

---

## ⚠️ **ERROR & EMPTY STATES**

### Search Error Alert
```
Condition: Search performed but no results found
Display: Warning alert (yellow)
Message: "Search Notice: [Error message]"
Dismissible: Yes (X button)
```

### Empty State
```
Condition: No results found
Display: Light blue card with icon
Icon: Search icon (large)
Title: "No Results Found"
Message: "Try searching with different keywords or browse all sites."
Button: "View All Sites" (primary button)
```

### No Search Performed
```
Condition: Page loaded without search
Display: Only search form visible
Results: Hidden until search is performed
```

---

## 📱 **RESPONSIVE DESIGN**

### Desktop (≥992px)
- Full width layout
- Search form: 8 columns (centered)
- Results table: Full width
- All columns visible

### Tablet (768px - 991px)
- Full width layout
- Search form: 8 columns (centered)
- Results table: Scrollable
- All columns visible

### Mobile (<768px)
- Full width layout
- Search form: Full width
- Results table: Horizontally scrollable
- Buttons stack if needed

---

## 🔄 **USER FLOW**

### Search Flow
```
1. User lands on Search page
2. Sees search form with input field
3. Enters site title
4. Clicks Search button
5. Form submits to Search action (POST)
6. Results displayed in table OR error message shown
7. User can click View Details to see full site info
8. User can click Back to List to return to Index
```

### Navigation
- **From Index**: Click "Search" button
- **From Search**: Click "Back to List" button
- **From Results**: Click "View Details" to see site
- **From Results**: Click "Back to Project Sites" to return

---

## 🎯 **SEARCH FUNCTIONALITY**

### Search Logic
- **Field**: Site Title
- **Method**: Case-insensitive contains search
- **Results**: All sites matching the search term
- **Single Result**: Auto-redirects to Details page
- **Multiple Results**: Shows table with all matches
- **No Results**: Shows error message

### Validation
- **Empty Search**: Shows error "Please enter a Site Title to search"
- **No Matches**: Shows error "No Project Sites found with the search criteria provided"
- **Valid Search**: Shows results table

---

## 🎨 **DESIGN CONSISTENCY**

### Matches System Design
- ✅ Same header style as other pages
- ✅ Same card layout
- ✅ Same color scheme
- ✅ Same button styling
- ✅ Same responsive behavior
- ✅ Same typography

### Bootstrap Integration
- Uses Bootstrap 5 classes
- Responsive grid system
- Bootstrap icons
- Standard form controls
- Alert components

---

## 📝 **IMPLEMENTATION DETAILS**

### HTML Structure
```html
<div class="container-fluid py-4">
  <!-- Header -->
  <div style="background: linear-gradient(...)">
    <h1>...</h1>
    <p>...</p>
  </div>

  <!-- Search Form -->
  <div class="row mb-4">
    <div class="col-md-8 offset-md-2">
      <div class="card">
        <div style="background: linear-gradient(...)">
          <h5>...</h5>
        </div>
        <div class="card-body">
          <form asp-action="Search" method="post">
            <!-- Form content -->
          </form>
        </div>
      </div>
    </div>
  </div>

  <!-- Results or Empty State -->
  <!-- ... -->
</div>
```

### Key Classes
- `container-fluid`: Full width container
- `py-4`: Vertical padding
- `card`: Bootstrap card
- `form-control form-control-lg`: Large input
- `btn btn-primary btn-lg`: Large button
- `table table-hover`: Hover effect table
- `badge bg-secondary`: Badge styling
- `alert alert-warning`: Warning alert

---

## ✅ **TESTING GUIDELINES**

1. **Search with Results**: Enter valid site title
2. **Search with No Results**: Enter non-existent title
3. **Empty Search**: Click search without entering text
4. **Single Result**: Search for unique site (should redirect)
5. **Multiple Results**: Search for common term
6. **View Details**: Click View Details button
7. **Back Navigation**: Test all back buttons
8. **Responsive**: Test on mobile, tablet, desktop
9. **Error Messages**: Verify error alerts display
10. **Dismissible Alerts**: Test alert close button

---

**Version**: 1.0
**Last Updated**: October 16, 2025


