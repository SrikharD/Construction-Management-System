// Created By: Srikhar Dogiparthy
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogiparthy_Spring25.Models
{
    public class WorkAllocation
    {
        [Key]
        public int WorkID { get; set; }

        [Display(Name = "Assigned Date")]
        [DataType(DataType.Date)]
        public DateTime AssignedDate { get; set; }

        // Foreign Key: SiteWorkerPersonID
        [Display(Name = "Site Worker")]
        public int? SiteWorkerPersonID { get; set; }

        [ForeignKey("SiteWorkerPersonID")]
        public virtual SiteWorker SiteWorker { get; set; }

        // Foreign Key: WorkOrderID
        [Display(Name = "Work Order")]
        public int? WorkOrderID { get; set; }

        [ForeignKey("WorkOrderID")]
        public virtual WorkOrder WorkOrder { get; set; }
    }
}
