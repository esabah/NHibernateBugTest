
using Newtonsoft.Json;
using NHibernateBugTest.Session;

namespace NHibernateBugTest.Entity
{

    /// <summary>
    /// Txn Member Key
    /// </summary>
    public class TxnDefMemberKey : Tupple<TxnDefMemberKey>
    {
        public virtual long TxnDefGuid { get; set; }
        public virtual short MbrId { get; set; }

    }

    public class TxnDefMember
    {
        /// <summary>
        /// Primary Key
        /// </summary>
        public virtual TxnDefMemberKey Id { get; set; }

        /// <summary>
        /// <inheritdoc cref="Entity.TxnDef"/>
        /// </summary>
        [JsonIgnore]
        public virtual TxnDef TxnDef { get; set; }

        public virtual string TaxType { get; set; }

        public virtual bool IsIncludeTax { get; set; }

        public virtual string BucketName { get; set; }
    }
}