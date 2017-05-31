using System;

namespace Sharper
{
    public class ModelBase : IModel
    {
        public int Id { get; set; }
        public bool? Valid { get; set; }
        public int? CreatorId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? ModifierId { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
