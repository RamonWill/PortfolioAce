﻿using Microsoft.EntityFrameworkCore;
using PortfolioAce.DataCentre.APIConnections;
using PortfolioAce.DataCentre.DeserialisedObjects;
using PortfolioAce.Domain.Models;
using PortfolioAce.Domain.Models.Dimensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioAce.EFCore.Services.PriceServices
{
    public class PriceService : IPriceService
    {
        private readonly PortfolioAceDbContextFactory _contextFactory;
        private readonly DataConnectionFactory _dataFactory;
        public PriceService(PortfolioAceDbContextFactory contextFactory, DataConnectionFactory dataFactory)
        {
            this._contextFactory = contextFactory;
            this._dataFactory = dataFactory;
        }


        public async Task<IEnumerable<AVSecurityPriceData>> AddDailyPrices(SecuritiesDIM security)
        {
            // This is for Equity Prices
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                string avKey = context.AppSettings.Where(ap => ap.SettingName == "AlphaVantageAPI").First().SettingValue;
                AlphaVantageConnection avConn = _dataFactory.CreateAlphaVantageClient(avKey);
                IEnumerable<AVSecurityPriceData> allPrices = await avConn.GetPricesAsync(security);
                HashSet<DateTime> existingDates = context.SecurityPriceData.Where(spd => spd.Security.Symbol == security.Symbol).Select(spd => spd.Date).ToHashSet();

                foreach(AVSecurityPriceData price in allPrices)
                {
                    if (!existingDates.Contains(price.TimeStamp))
                    {
                        SecurityPriceStore newPrice = new SecurityPriceStore { Date = price.TimeStamp, ClosePrice = price.Close, SecurityId = security.SecurityId, PriceSource=price.PriceSource};
                        context.SecurityPriceData.Add(newPrice);
                    }
                }
                await context.SaveChangesAsync();

                return allPrices;
            }
        }


        public SecuritiesDIM GetSecurityInfo(string symbol)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.Securities.Where(s => s.Symbol == symbol).Include(s => s.Currency).Include(s => s.AssetClass).FirstOrDefault();
            }
        }

        public List<string> GetAllSecuritySymbols()
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.Securities.Where(s=> s.AssetClass.Name !="Cash" && s.AssetClass.Name!="FX").OrderBy(s=>s.Symbol).Select(s=>s.Symbol).Distinct().ToList();
            }
        }

        public List<SecurityPriceStore> GetSecurityPrices(string symbol)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.SecurityPriceData.Where(spd => spd.Security.Symbol == symbol).OrderBy(spd=>spd.Date).ToList();
            }
        }
    }
}
