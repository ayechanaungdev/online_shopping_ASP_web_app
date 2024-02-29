using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Entities.Base
{
    public interface IEntityBase<TId>
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        TId Id { get; }
        DateTime? CreatedAt { get; set; }   
        string CreatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        string UpdatedBy { get; set; }
        bool IsDelete { get; set; }
    }
}
