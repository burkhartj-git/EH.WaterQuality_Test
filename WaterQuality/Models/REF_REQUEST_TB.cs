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
    
    public partial class REF_REQUEST_TB
    {
        public int N_REQUEST_SYSID { get; set; }
        public int N_LOCATION_GROUP_SYSID { get; set; }
        public int N_TEST_GROUP_SYSID { get; set; }
        public int N_REQUEST_GROUP_SYSID { get; set; }
        public string SZ_ENTERED_BY { get; set; }
        public System.DateTime DT_ENTERED { get; set; }
        public string SZ_MODIFIED_BY { get; set; }
        public System.DateTime DT_MODIFIED { get; set; }
        public bool B_INACTIVE { get; set; }
        public byte[] SZ_TIMESTAMP { get; set; }
    
        public virtual REF_LOCATION_GROUP_TB REF_LOCATION_GROUP_TB { get; set; }
        public virtual REF_REQUEST_GROUP_TB REF_REQUEST_GROUP_TB { get; set; }
        public virtual REF_TEST_GROUP_TB REF_TEST_GROUP_TB { get; set; }
    }
}
