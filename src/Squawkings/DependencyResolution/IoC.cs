using FluentValidation;
using NPoco;
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
            //                x.For<IExample>().Use<Example>();
                            x.For<IDatabase>().HybridHttpOrThreadLocalScoped().Use(DatabaseFactory.GetDatabase());
                        });


            return ObjectFactory.Container;
        }
    }

    public class DatabaseFactory
    {
        internal static IDatabase GetDatabase()
        {
            return new Database("Squawkings");
        }
    }
}