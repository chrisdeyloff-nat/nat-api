using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using nat_api.data.configurations;
using nat_api.data.configurations.amount;
using nat_api.data.core.audit;
using nat_api.data.models;
using nat_api.data.models.amount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace nat_api.data.context
{
    public class DataContext : DbContext
    {
        // private readonly IHttpContextAccessor httpContextAccessor;

        public DataContext(
            DbContextOptions<DataContext> options
            // , IHttpContextAccessor httpContextAccessor
        ) : base(options)
        {
            // this.httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AuditModelConfiguration());
            builder.ApplyConfiguration(new AmountConfiguration());

        }

        public DbSet<Audit> Audits { get; set; }
        public DbSet<Amount> Amounts { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            int result = 0;
            try
            {
                // var userName = RegexHelper.GetBaseUserName(httpContextAccessor?.HttpContext?.User?.Identity?.Name);
                await this.AddAuditLogs(/*userName*/);
                result = await base.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw e;
            }
            return result;
        }
        protected virtual async Task AddAuditLogs(/*string userName*/)
        {
            // long? userId = null;
            // if (!string.IsNullOrEmpty(userName))
            // {
            //     userId = (await this.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower()))?.Id;
            // }
            this.ChangeTracker.DetectChanges();
            List<AuditEntry> auditEntries = new List<AuditEntry>();
            foreach (EntityEntry entry in this.ChangeTracker.Entries())
            {
                if ((entry.Entity is Audit)
                    || (entry.State == EntityState.Detached)
                    || (entry.State == EntityState.Unchanged))
                {
                    continue;
                }
                // if (userId.HasValue)
                // {
                    var baseModel = entry.Entity as BaseModel;
                    if (baseModel != null)
                    {
                        // if (entry.State == EntityState.Added)
                        // { baseModel.CreatedBy = userId.Value; }

                        if ((entry.State == EntityState.Modified)
                            || (entry.State == EntityState.Added))
                        {
                            // baseModel.UpdatedBy = userId.Value;
                            baseModel.Updated = DateTime.UtcNow;
                        }
                    }
                // }
                var auditEntry = new AuditEntry(entry/*, userName*/);
                auditEntries.Add(auditEntry);
            }

            if (auditEntries.Any())
            {
                var logs = auditEntries.Select(x => x.ToAudit());
                this.Audits.AddRange(logs);
            }
        }

    }
}