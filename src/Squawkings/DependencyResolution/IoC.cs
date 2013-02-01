using System.Data;
using System.Data.Common;
using FluentValidation;
using NPoco;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using StructureMap;
namespace Squawkings {
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
                        {
                            x.Scan(scan =>
                                    {
                                        scan.TheCallingAssembly();
                                        scan.WithDefaultConventions();
                                        scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>));
                                        
                                    });

                            x.For<IDatabase>().HybridHttpOrThreadLocalScoped().Use(new DatabaseFactory("Squawkings"));
                     
                        });


            return ObjectFactory.Container;
        }
    }

    public class DatabaseFactory : Database
    {
        public DatabaseFactory(string connectionStringName) : base(connectionStringName)
        {
        }

        internal static IDatabase GetDatabase()
        {
            return new Database("Squawkings");
        }

        public override IDbConnection OnConnectionOpened(IDbConnection connection)
        {
            // wrap the connection with a profiling connection that tracks timings
            return new ProfiledDbConnection((DbConnection)connection, MiniProfiler.Current);
        }
    }

}