using FluentNHibernate.Mapping;
using NHibernate.Engine;
using NHibernate.Id;
using NHibernateBugTest.Session;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernateBugTest.Entity
{


    public class TxnDefMemberMap : ClassMap<TxnDefMember>
    {
        public TxnDefMemberMap()
        {
            Schema("OCN_TRASH");
            Table("TXN_DEF_MEMBER");

            CompositeId(x => x.Id).KeyProperty(y => y.TxnDefGuid, "TXN_DEF_GUID")
                                  .KeyProperty(y => y.MbrId, "MBR_ID");

            References(x => x.TxnDef).Column("TXN_DEF_GUID").ReadOnly().LazyLoad(); ;
            Map(x => x.IsIncludeTax).Column("IS_INCLUDE_TAX").CustomType<SqlBoolean>().Precision(1);
            Map(x => x.BucketName).Column("BUCKET_NAME").Length(30);
            Map(x => x.TaxType).Column("TAX_TYPE").Not.Nullable().Length(1);

            ApplyFilter<MemberConditionFilter>();
        }
    }
}
