//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Skywalker.Ddd.Infrastructure.DbContextConfiguration;
//using System;
//using System.Data.Common;
//using System.Diagnostics.CodeAnalysis;

//namespace Skywalker.EntityFrameworkCore.DependencyInjection
//{
//    public class SkywalkerEntityFrameworkCoreDbContextConfigurationContext : SkywalkerDbContextConfigurationContext
//    {

//        public DbConnection ExistingConnection { get; }

//        public DbContextOptionsBuilder DbContextOptions { get; protected set; }

//        public SkywalkerEntityFrameworkCoreDbContextConfigurationContext(
//            [NotNull] string connectionString,
//            [NotNull] IServiceProvider serviceProvider,
//            [MaybeNull] string connectionStringName,
//            [MaybeNull]DbConnection existingConnection):base(connectionString,serviceProvider,connectionStringName)
//        {
//            ExistingConnection = existingConnection;

//            DbContextOptions = new DbContextOptionsBuilder()
//                .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());
//        }
//    }

//    public class SkywalkerEntityFrameworkCoreDbContextConfigurationContext<TDbContext> : SkywalkerEntityFrameworkCoreDbContextConfigurationContext
//        where TDbContext : EntityFrameworkCoreDbContext<TDbContext>
//    {
//        public new DbContextOptionsBuilder<TDbContext> DbContextOptions => (DbContextOptionsBuilder<TDbContext>)base.DbContextOptions;

//        public SkywalkerEntityFrameworkCoreDbContextConfigurationContext(
//            string connectionString,
//            [NotNull] IServiceProvider serviceProvider,
//            [MaybeNull] string connectionStringName,
//            [MaybeNull] DbConnection existingConnection)
//            : base(
//                  connectionString, 
//                  serviceProvider, 
//                  connectionStringName, 
//                  existingConnection)
//        {
//            base.DbContextOptions = new DbContextOptionsBuilder<TDbContext>()
//                .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());
//        }
//    }
//}