using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Digipolis.DataAccess.Context;
using StarterKit.DataAccess;

namespace StarterKit
{
    public class EntityContext : EntityContextBase<EntityContext>
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        { }

        // Add your DbSets here

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(DataAccessDefaults.SchemaName);       // Remove this if you are not using schema's
        
            // Your modelBuilder code here
        }
    }
}