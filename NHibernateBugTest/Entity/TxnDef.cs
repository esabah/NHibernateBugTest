
using Newtonsoft.Json;
using NHibernate.Util;
using System.Collections.Generic;
using System.Linq;

namespace NHibernateBugTest.Entity
{
    public class TxnDef 
    {
        public TxnDef()
        {
            TxnMemberDefs = new List<TxnDefMember>();
        }

        public virtual long Guid { get; set; }

        public virtual string Description { get; set; }

        [JsonIgnore]
        public virtual TxnDef Tax1TxnDef { get; set; }

        public virtual long? Tax1TxnDefGuid { get; set; }

       
        public virtual IList<TxnDefMember> TxnMemberDefs { get; set; }

    }
}