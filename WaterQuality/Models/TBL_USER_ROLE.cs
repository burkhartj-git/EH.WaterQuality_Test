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
    
    public partial class TBL_USER_ROLE
    {
        public decimal USER_ID { get; set; }
        public string PWD { get; set; }
        public string USER_ROLE { get; set; }
    
        public virtual TBL_USER TBL_USER { get; set; }
    }
}
