using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VotingIrregularities.Domain.Models
{
    public partial class VotingContext : DbContext
    {
        public VotingContext(DbContextOptions<VotingContext> options)
            :base(options)
        {

        }
    }
}