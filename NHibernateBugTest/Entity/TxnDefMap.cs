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
    public class TxnDefMap : ClassMap<TxnDef>
    {
        public TxnDefMap()
        {
            Table("TXN_DEF_1");
            Id(x => x.Guid).Column("GUID").GeneratedBy.Sequence("DefaultSequence");
            Map(x => x.Description).Column("DESCRIPTION").Length(4000);
            References(x => x.Tax1TxnDef).Column("TAX1_TXN_DEF_GUID").ReadOnly();
            Map(x => x.Tax1TxnDefGuid).Column("TAX1_TXN_DEF_GUID");
            HasMany(x => x.TxnMemberDefs).KeyColumn("TXN_DEF_GUID").Cascade.All().ApplyFilter<MemberConditionFilter>();
        }
    }
}
