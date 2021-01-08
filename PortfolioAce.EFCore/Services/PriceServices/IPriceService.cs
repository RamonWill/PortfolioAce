﻿using PortfolioAce.DataCentre.DeserialisedObjects;
using PortfolioAce.Domain.Models;
using PortfolioAce.Domain.Models.Dimensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioAce.EFCore.Services.PriceServices
{
    public interface IPriceService:IBaseService
    {
        Task<List<AVSecurityPriceData>> AddDailyPrices(SecuritiesDIM security);
        List<string> GetAllSecuritySymbols();
        SecuritiesDIM GetSecurityInfo(string symbol);

        List<SecurityPriceStore> GetSecurityPrices(string symbol);
    }
}
