using NHibernate;
using NHibernate.Exceptions;
using NHibernateBugTest.Entity;
using NHibernateBugTest.Session;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NHibernateBugTest
{
    [TestFixture]
    public class TxnDefTest
    {
        [SetUp]
        public void SetUpBase()
        {
            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    //just init connection and ISessionFactory
                    transaction.Commit();
                }
            }
        }

        [Test, Order(1)]
        public void Saves_MultiTenantTxnDefs_Success()
        {
            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    for (int i = 0; i < 4; i++)
                    {
                        TxnDef txnDef = new TxnDef()
                        {
                            Description = "txnDef " + i.ToString()
                        };
                        session.Save(txnDef);

                        txnDef.TxnMemberDefs = new List<TxnDefMember>();
                        txnDef.TxnMemberDefs.Add(new TxnDefMember 
                        { 
                                IsIncludeTax= true,
                                TaxType ="1",
                                BucketName = "Bucket " + i.ToString(),
                            Id = new TxnDefMemberKey { MbrId=1, TxnDefGuid = txnDef.Guid }
                        });
                        txnDef.TxnMemberDefs.Add(new TxnDefMember
                        {
                            IsIncludeTax = true,
                            TaxType = "2",
                            BucketName = "Bucket "+ i.ToString(), 
                            Id = new TxnDefMemberKey { MbrId = 2, TxnDefGuid = txnDef.Guid }
                        });

                        session.Merge(txnDef);
                    }
                    transaction.Commit();
                }
            }

            using (ISession session = SessionProvider.ISessionFactory.OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {

                    Assert.That(session.Query<TxnDef>().ToList().Count == 4);
                }
            }
        }

        [TestCase(1), Order(2)]
        [TestCase(2)]
        public void Retrieve_MultiTenantTxnDefs_Success(short mbrId)
        {
            CurrentSession.MbrId = mbrId;
            //Sets Current Session
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .Interceptor(new ContextInterceptor())
                                        .OpenSession())
            {
                using (ITransaction transaction = session.BeginTransaction())
                {
                    var txnDefinitions = (from txndef in session.Query<TxnDef>()
                                          join txnDefMember in session.Query<TxnDefMember>() on txndef.Guid equals txnDefMember.Id.TxnDefGuid
                                          select txnDefMember.BucketName);

                    Assert.That(txnDefinitions.ToList().Count == 4);
                }
            }
        }

       
    }
}
