using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Partner.BrasilConnect.Did.Api.Models;

namespace Partner.BrasilConnect.Did.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Partner.BrasilConnect.Did.Api.Models.DidActivation> DidActivation { get; set; } = default!;
    }
}
