//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EHWaterQuality.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetPermission
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AspNetPermission()
        {
            this.AspNetApplicationRolePermissions = new HashSet<AspNetApplicationRolePermission>();
        }
    
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool ViewPublished { get; set; }
        public bool ViewOwn { get; set; }
        public bool ViewAll { get; set; }
        public bool AddNew { get; set; }
        public bool EditOwn { get; set; }
        public bool EditAll { get; set; }
        public bool DeleteOwn { get; set; }
        public bool DeleteAll { get; set; }
        public bool ExpungeOwn { get; set; }
        public bool ExpungeAll { get; set; }
        public bool AdministerOwn { get; set; }
        public bool AdministerAll { get; set; }
        public int Sequence { get; set; }
        public bool Inactive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetApplicationRolePermission> AspNetApplicationRolePermissions { get; set; }
    }
}