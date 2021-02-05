using NHibernate;
using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Linq;
using NHibernateBugTest.Entity;
using NHibernateBugTest.Session;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NHibernateBugTest
{
    [TestFixture]
    public class CacheNativeSqlTest
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

        [Test]
        public void RetrieveFromCacheUpdate()
        {

            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .OpenSession())
            {
                // First Select
                var user = session.Query<User>().Where(x => x.UserCode == "user1")
                   .WithOptions(x => x.SetCacheable(true)).FirstOrDefault();

                // Second Select from cache
                var user2 = session.Query<User>().Where(x => x.UserCode == "user1")
                    .WithOptions(x => x.SetCacheable(true)).FirstOrDefault();


               // native Sql Update
                    ISQLQuery query = session.CreateSQLQuery("UPDATE CUSTOMER SET NAME=:Name where GUID= :Guid");

                    query.SetParameter("Name", "Test");
                    query.SetParameter("Guid", 100);

                    query.ExecuteUpdate();


                // third select :  User entity not changed should retrieve from cache. But hits Db 
                var user3 = session.Query<User>().Where(x => x.UserCode == "user1")
                        .WithOptions(x => x.SetCacheable(true)).FirstOrDefault();

                Assert.That(1 == 1);
            }
        }

        [Test]
        public void RetrieveFromCacheUpdate2()
        {
            using (ISession session = SessionProvider.ISessionFactory
                                        .WithOptions()
                                        .OpenSession())
            {
                // First Select
                var user = session.Query<User>().Where(x => x.UserCode == "user1")
                        .WithOptions(x => x.SetCacheable(true)).FirstOrDefault();

                // Second Select from cache
                var user2 = session.Query<User>().Where(x => x.UserCode == "user1")
                        .WithOptions(x => x.SetCacheable(true)).FirstOrDefault();



                var query = session.CreateQuery($@"update {typeof(CustomerMap)} 
                                                       set Name= 'test'
                                                       where Guid = 100");
                query.ExecuteUpdate();


                // user entity not changed retrieves from cache. Not Hit Db
                var user3 = session.Query<User>().Where(x => x.UserCode == "user1")
                        .WithOptions(x => x.SetCacheable(true)).FirstOrDefault();

                Assert.That(1 == 1);
            }
        }
    }
}
