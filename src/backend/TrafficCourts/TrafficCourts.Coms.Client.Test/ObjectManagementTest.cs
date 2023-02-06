using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrafficCourts.Coms.Client.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Npgsql;

namespace TrafficCourts.Coms.Client.Test
{
    public class ObjectManagementTest
    {
        public static Guid NewId => Guid.NewGuid();

        //[Fact]
        public void X()
        {
            NpgsqlConnectionStringBuilder connectionString = new NpgsqlConnectionStringBuilder();
            connectionString.Host = "localhost";
            connectionString.Database = "coms";
            connectionString.Username = "postgres";
            connectionString.Password = "password";

            DbContextOptionsBuilder<ObjectManagementContext> builder = new DbContextOptionsBuilder<ObjectManagementContext>();
            builder.UseNpgsql(connectionString.ConnectionString);

            ObjectManagementContext context = new ObjectManagementContext(builder.Options);
            
            for (int i = 0; i < 100000; i++)
            {
                Guid versionId = NewId;
                Guid objectId = NewId;
                string createdBy = NewId.ToString("d");
                string updatedBy = NewId.ToString("d");

                var obj = new Data.Models.Object { Id = objectId, Path = "/", Public = false, Active = true, CreatedBy = createdBy, UpdatedBy = updatedBy };

                var ver = new Data.Models.Version { Id = versionId, ObjectId = objectId, MimeType = "application/pdf", DeleteMarker = false, CreatedBy = createdBy, UpdatedBy = updatedBy };

                context.Objects.Add(obj);
                context.Versions.Add(ver);
            }

            context.SaveChanges();

        }
    }
}
