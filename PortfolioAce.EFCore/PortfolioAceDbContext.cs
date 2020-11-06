﻿using Microsoft.EntityFrameworkCore;
using PortfolioAce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioAce.EFCore
{
    // this class will manage our interaction with the database.
    public class PortfolioAceDbContext:DbContext
    {
        public DbSet<Fund> Funds { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<CashTrade> CashTrades { get; set; }
        public DbSet<CashAccount> CashAccounts { get; set; }
        public DbSet<Security> Securities { get; set; }

        public PortfolioAceDbContext(DbContextOptions options) : base(options)
        { 
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Security>().HasNoKey();
        }


    }
}