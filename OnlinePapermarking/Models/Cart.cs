//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OnlinePapermarking.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Cart
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Cart()
        {
            this.CartItems = new HashSet<CartItem>();
        }
    
        public long CartId { get; set; }
        public long LoginId { get; set; }
        public Nullable<decimal> Total { get; set; }
        public Nullable<decimal> SubTotal { get; set; }
        public bool IsCheckOut { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
        public bool IsPromoApplied { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
