﻿using PortfolioAce.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PortfolioAce.EFCore.Services
{
    public interface IFundService : IBaseService
    {
        Task<Fund> CreateFund(Fund fund);
        Task<Fund> UpdateFund(Fund fund);
        Task<Fund> DeleteFund(int id);
        bool FundExists(string fundSymbol);
        List<Fund> GetAllFunds();
        List<string> GetAllFundSymbols();
        Fund GetFund(string fundSymbol);
    }
}
