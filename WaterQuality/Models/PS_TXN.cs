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
    
    public partial class PS_TXN
    {
        public decimal ID { get; set; }
        public Nullable<decimal> PARENTID { get; set; }
        public decimal COLLID { get; set; }
        public byte[] CONTENT { get; set; }
        public Nullable<System.DateTime> CREATION_DATE { get; set; }
    }
}