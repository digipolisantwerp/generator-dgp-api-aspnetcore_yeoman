using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Digipolis.DataAccess.Context;

namespace StarterKit
{
    public class EntityContext : EntityContextBase<EntityContext>
    {
        public EntityContext(DbContextOptions<EntityContext> options) : base(options)
        { }

        // Add your DbSets here
    }
}