using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Core.Entities.Base
{
    public abstract class EntityBase<TId> : IEntityBase<TId>
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public virtual TId Id { get; set; }
        public virtual DateTime? CreatedAt { get; set; }
        public virtual string CreatedBy { get; set; }
        public virtual DateTime? UpdatedAt { get; set; }
        public virtual string UpdatedBy { get; set; }
        [DefaultValue(false)]
        public virtual bool IsDelete { get; set; }
    }
}
