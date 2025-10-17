# 🔄 SiteWorker Availability Auto-Update Implementation Guide

## Overview

This document explains how the SiteWorker availability system works and automatically updates the `IsAvailable` status based on work order assignments.

---

## 📊 **System Architecture**

### Database Relationships
```
SiteWorker (1) ──── (Many) WorkAllocation (Many) ──── (1) WorkOrder
    ↓
  IsAvailable (bool)
```

### Status Flow
```
SiteWorker Created
    ↓ (IsAvailable = true)
Worker Assigned to WorkOrder
    ↓ (IsAvailable = false)
Worker Removed from All WorkOrders
    ↓ (IsAvailable = true)
```

---

## 🔧 **Implementation Details**

### 1. Model Properties

**SiteWorker.cs**
```csharp
public class SiteWorker : Person
{
    [Display(Name = "Is Available")]
    public bool IsAvailable { get; set; }  // ✅ Tracks availability

    public virtual ICollection<WorkAllocation> WorkAllocations { get; set; }
}
```

### 2. Assignment Flow

#### When Admin Assigns Workers to WorkOrder
**File**: `Controllers/WorkOrdersController.cs` - `AssignSiteWorkers` (POST)

```csharp
[HttpPost]
public IActionResult AssignSiteWorkers(int? id, List<WorkOrder_AddSiteWorker_ViewModel> AssignSiteWorkers)
{
    foreach (var item in AssignSiteWorkers)
    {
        if (item.Selected)
        {
            // Create work allocation
            WorkAllocation workAlloc = new WorkAllocation();
            workAlloc.WorkOrderID = id;
            workAlloc.SiteWorkerPersonID = item.SiteWorker.PersonID;
            workAlloc.AssignedDate = DateTime.Now;
            _context.WorkAllocation.Add(workAlloc);

            // ✅ Update SiteWorker's IsAvailable status to false
            var siteWorker = _context.SiteWorkers.FirstOrDefault(
                sw => sw.PersonID == item.SiteWorker.PersonID);
            if (siteWorker != null)
            {
                siteWorker.IsAvailable = false;
                _context.Update(siteWorker);
            }
        }
    }
    _context.SaveChanges();
    return RedirectToAction("ManageSiteWorkers", new { id = id });
}
```

#### When Worker Adds WorkOrders to Themselves
**File**: `Controllers/SiteWorkersController.cs` - `AddWorkOrders` (POST)

```csharp
[HttpPost]
public IActionResult AddWorkOrders(int? id, List<SiteWorker_AddWorkOrder_ViewModel> AddWorkOrders)
{
    bool hasAssignments = false;

    foreach (var item in AddWorkOrders)
    {
        if (item.Selected)
        {
            WorkAllocation workAlloc = new WorkAllocation();
            workAlloc.SiteWorkerPersonID = id;
            workAlloc.WorkOrderID = item.WorkOrder.WorkOrderID;
            workAlloc.AssignedDate = DateTime.Now;
            _context.WorkAllocation.Add(workAlloc);
            hasAssignments = true;
        }
    }

    // ✅ If any work orders were assigned, update IsAvailable to false
    if (hasAssignments)
    {
        var siteWorker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == id);
        if (siteWorker != null)
        {
            siteWorker.IsAvailable = false;
            _context.Update(siteWorker);
        }
    }

    _context.SaveChanges();
    return RedirectToAction("ManageWorkOrders", new { id = id });
}
```

### 3. Removal Flow

#### When Removing WorkOrder from Worker
**File**: `Controllers/SiteWorkersController.cs` - `DeleteWorkOrder` (POST)

```csharp
[HttpPost, ActionName("DeleteWorkOrder")]
public IActionResult DeleteWorkOrderConfirmation(int? id)
{
    var workAlloc = _context.WorkAllocation
        .Include(wa => wa.SiteWorker)
        .SingleOrDefault(wa => wa.WorkID == id);

    int siteWorkerPersonID = workAlloc.SiteWorkerPersonID ?? 0;

    _context.WorkAllocation.Remove(workAlloc);
    _context.SaveChanges();

    // ✅ Check if worker has remaining allocations
    var remainingAllocations = _context.WorkAllocation
        .Where(wa => wa.SiteWorkerPersonID == siteWorkerPersonID)
        .Any();

    // ✅ If no remaining allocations, set IsAvailable back to true
    if (!remainingAllocations)
    {
        var siteWorker = _context.SiteWorkers.FirstOrDefault(
            sw => sw.PersonID == siteWorkerPersonID);
        if (siteWorker != null)
        {
            siteWorker.IsAvailable = true;
            _context.Update(siteWorker);
            _context.SaveChanges();
        }
    }

    return RedirectToAction("ManageWorkOrders", new { id = siteWorkerPersonID });
}
```

#### When Removing Worker from WorkOrder
**File**: `Controllers/WorkOrdersController.cs` - `DeleteSiteWorker` (POST)

```csharp
[HttpPost, ActionName("DeleteSiteWorker")]
public IActionResult DeleteSiteWorkerConfirmed(int? id)
{
    var workAlloc = _context.WorkAllocation
        .Include(wa => wa.SiteWorker)
        .SingleOrDefault(wa => wa.WorkID == id);

    int? siteWorkerPersonID = workAlloc?.SiteWorkerPersonID;
    int? workOrderID = workAlloc?.WorkOrderID;

    _context.WorkAllocation.Remove(workAlloc);
    _context.SaveChanges();

    // ✅ Check if worker has remaining allocations
    if (siteWorkerPersonID.HasValue)
    {
        var remainingAllocations = _context.WorkAllocation
            .Where(wa => wa.SiteWorkerPersonID == siteWorkerPersonID)
            .Any();

        // ✅ If no remaining allocations, set IsAvailable back to true
        if (!remainingAllocations)
        {
            var siteWorker = _context.SiteWorkers.FirstOrDefault(
                sw => sw.PersonID == siteWorkerPersonID);
            if (siteWorker != null)
            {
                siteWorker.IsAvailable = true;
                _context.Update(siteWorker);
                _context.SaveChanges();
            }
        }
    }

    return RedirectToAction("ManageSiteWorkers", new { id = workOrderID });
}
```

---

## 🎯 **Key Features**

1. **Automatic Status Update**
   - No manual intervention needed
   - Status updates immediately when assignments change

2. **Accurate Availability**
   - Only available workers shown in assignment lists
   - Prevents over-allocation

3. **Smart Restoration**
   - IsAvailable restored to true only when ALL work orders removed
   - Handles multiple assignments correctly

4. **Data Integrity**
   - Includes proper null checks
   - Handles edge cases

---

## 📋 **Usage Scenarios**

### Scenario 1: Assign Worker to WorkOrder
1. Admin goes to WorkOrder > Manage Workers
2. Selects available workers
3. Clicks "Assign"
4. ✅ Selected workers' IsAvailable set to false
5. Workers no longer appear in available list

### Scenario 2: Remove Worker from WorkOrder
1. Admin goes to WorkOrder > Manage Workers
2. Clicks "Remove" on a worker
3. ✅ If worker has no other assignments, IsAvailable set to true
4. Worker reappears in available list

### Scenario 3: Worker Adds Multiple WorkOrders
1. Worker goes to SiteWorkers > Add Work Orders
2. Selects multiple work orders
3. Clicks "Add"
4. ✅ IsAvailable set to false (once for all assignments)
5. Worker can remove individual orders later

---

## ✅ **Testing Guide**

### Test Case 1: Single Assignment
1. Create SiteWorker with IsAvailable = true
2. Assign to WorkOrder
3. Verify IsAvailable = false
4. Remove from WorkOrder
5. Verify IsAvailable = true

### Test Case 2: Multiple Assignments
1. Create SiteWorker with IsAvailable = true
2. Assign to WorkOrder 1
3. Verify IsAvailable = false
4. Assign to WorkOrder 2
5. Remove from WorkOrder 1
6. Verify IsAvailable = false (still has WorkOrder 2)
7. Remove from WorkOrder 2
8. Verify IsAvailable = true

### Test Case 3: Availability Filter
1. Create 2 SiteWorkers (both available)
2. Assign Worker 1 to WorkOrder
3. Go to Assign Workers page
4. Verify only Worker 2 appears in list
5. Remove Worker 1 from WorkOrder
6. Verify both workers appear in list

---

## 🔍 **Troubleshooting**

### Issue: IsAvailable not updating
**Solution**: Ensure `_context.SaveChanges()` is called after `_context.Update()`

### Issue: Worker still appears in available list
**Solution**: Check if worker has other active allocations

### Issue: IsAvailable set to true but worker still unavailable
**Solution**: Verify all work allocations were removed

---

**Version**: 1.0
**Last Updated**: October 16, 2025


