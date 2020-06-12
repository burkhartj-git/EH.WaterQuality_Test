//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TRN_SEWER_OVERFLOW_TB
    {
        public int N_SEWER_OVERFLOW_SYSID { get; set; }
        public Nullable<System.DateTime> DT_ACTIVITY_START { get; set; }
        public Nullable<System.DateTime> DT_ACTIVITY_END { get; set; }
        public Nullable<decimal> N_DISCHARGE_GALLONS { get; set; }
        public Nullable<int> N_WATER_BODY_SYSID { get; set; }
        public Nullable<bool> B_CHLORINATE { get; set; }
        public Nullable<bool> B_NPDESCOMP { get; set; }
        public string SZ_ACTIVITY_TYPE { get; set; }
        public Nullable<decimal> N_DURATION_MINUTES { get; set; }
        public Nullable<int> N_FACILITY_SYSID { get; set; }
        public string SZ_ACTIVITY_START_TIME { get; set; }
        public string SZ_ACTIVITY_END_TIME { get; set; }
        public Nullable<double> N_PRECIPITATION { get; set; }
        public string SZ_ENTERED_BY { get; set; }
        public System.DateTime DT_ENTERED { get; set; }
        public string SZ_MODIFIED_BY { get; set; }
        public System.DateTime DT_MODIFIED { get; set; }
        public bool B_INACTIVE { get; set; }
        public byte[] SZ_TIMESTAMP { get; set; }
        public string SZ_CHLORINATE { get; set; }
        public string SZ_NPDESCOMP { get; set; }
    
        public virtual REF_FACILITY_TB REF_FACILITY_TB { get; set; }
        public virtual REF_WATER_BODY_TB REF_WATER_BODY_TB { get; set; }
    }
}
