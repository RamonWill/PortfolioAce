﻿using Microsoft.EntityFrameworkCore;
using PortfolioAce.Domain.Models.FactTables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortfolioAce.EFCore.Services.FactTableServices
{
    public class FactTableService : IFactTableService
    {
        private readonly PortfolioAceDbContextFactory _contextFactory;


        public FactTableService(PortfolioAceDbContextFactory contextFactory)
        {
            this._contextFactory = contextFactory;
        }

        public List<PositionFACT> GetAllStoredPositions(DateTime date, int FundId, bool onlyActive = false)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                if (onlyActive)
                {
                    return context.Positions.Where(p => p.Quantity != 0 && p.FundId == FundId && p.PositionDate == date)
                        .AsNoTracking()
                        .Include(p => p.AssetClass)
                        .Include(p => p.Security)
                        .ToList();
                }
                else
                {
                    return context.Positions.Where(p => p.FundId == FundId && p.PositionDate == date)
                        .AsNoTracking()
                        .Include(p => p.AssetClass)
                        .Include(p => p.Security)
                        .ToList();
                }
            }
        }

        public List<NAVPriceStoreFACT> GetAllNAVPrices()
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.NavPriceData
                        .AsNoTracking()
                        .OrderBy(np => np.FinalisedDate)
                        .Include(np => np.Fund)
                        .ToList();
            }
        }

        public List<NAVPriceStoreFACT> GetAllFundNAVPrices(int fundId)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.NavPriceData
                        .AsNoTracking()
                        .Where(np => np.FundId == fundId)
                        .OrderBy(np => np.FinalisedDate)
                        .ToList();
            }
        }

        public List<PositionFACT> GetAllFundStoredPositions(int fundId, int securityId)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.Positions.AsNoTracking()
                            .Where(p => p.FundId == fundId && p.SecurityId == securityId)
                            .ToList();
            }

        }
    }
}
