﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PortfolioAce.Domain.DataObjects.DTOs;
using PortfolioAce.Domain.Models.BackOfficeModels;
using PortfolioAce.Domain.Models.Dimensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioAce.EFCore.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly PortfolioAceDbContextFactory _contextFactory;

        public TransactionService(PortfolioAceDbContextFactory contextFactory)
        {
            this._contextFactory = contextFactory;
        }

        public async Task<TransactionsBO> CreateFXTransaction(ForexDTO fxTransaction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                SecuritiesDIM fxSecurity = context.Securities.AsNoTracking().Where(s => s.Symbol == fxTransaction.Symbol).FirstOrDefault();
                AssetClassDIM assetClass = context.AssetClasses.AsNoTracking().Where(a => a.Name == "FXForward").FirstOrDefault();
                CustodiansDIM custodian = context.Custodians.AsNoTracking().Where(c => c.Name == fxTransaction.Custodian).FirstOrDefault();
                IEnumerable<TransactionTypeDIM> transactionTypes = context.TransactionTypes;
                List<CurrenciesDIM> currencies = context.Currencies.AsNoTracking().ToList();

                CurrenciesDIM buyCurrency = context.Currencies.AsNoTracking().Where(c => c.Symbol == fxTransaction.BuyCurrency).First();
                CurrenciesDIM sellCurrency = context.Currencies.AsNoTracking().Where(c => c.Symbol == fxTransaction.SellCurrency).First();
                if (fxSecurity == null)
                {
                    fxSecurity = new SecuritiesDIM
                    {
                        AssetClassId = assetClass.AssetClassId,
                        CurrencyId = buyCurrency.CurrencyId,
                        SecurityName = fxTransaction.Name,
                        Symbol = fxTransaction.Symbol
                    };
                    context.Securities.Add(fxSecurity);
                }
                // Created LinkedTransactions Here
                LinkedTradesBO linkReference = new LinkedTradesBO();
                TransactionsBO refTransaction = new TransactionsBO
                {
                    Security = fxSecurity,
                    FundId = fxTransaction.FundId,
                    isActive = true,
                    isLocked = false,
                    TradeAmount = 0,
                    Price = fxTransaction.Price,
                    Quantity = fxTransaction.BuyAmount,
                    CurrencyId = buyCurrency.CurrencyId,
                    TransactionTypeId = transactionTypes.Where(tt => tt.TypeName == "FXTrade").First().TransactionTypeId,
                    Comment = "",
                    CustodianId = custodian.CustodianId,
                    Fees = 0,
                    isCashTransaction = false,
                    TradeDate = fxTransaction.TradeDate,
                    SettleDate = fxTransaction.SettleDate,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    LinkedTrades = linkReference
                };
                TransactionsBO refTransactionCollapse = new TransactionsBO
                {
                    Security = fxSecurity,
                    FundId = fxTransaction.FundId,
                    isActive = true,
                    isLocked = false,
                    TradeAmount = 0,
                    Price = fxTransaction.Price,
                    Quantity = fxTransaction.BuyAmount * -1,
                    CurrencyId = buyCurrency.CurrencyId,
                    TransactionTypeId = transactionTypes.Where(tt => tt.TypeName == "FXTradeCollapse").First().TransactionTypeId,
                    Comment = "",
                    CustodianId = custodian.CustodianId,
                    Fees = 0,
                    isCashTransaction = false,
                    TradeDate = fxTransaction.SettleDate,
                    SettleDate = fxTransaction.SettleDate,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    LinkedTrades = linkReference
                };
                EntityEntry<TransactionsBO> res = await context.Transactions.AddAsync(refTransaction);
                context.Transactions.Add(refTransactionCollapse);

                SecuritiesDIM fxBuySecurity = context.Securities.AsNoTracking().Where(s => s.Symbol == $"{fxTransaction.BuyCurrency}c").FirstOrDefault();
                SecuritiesDIM fxSellSecurity = context.Securities.AsNoTracking().Where(s => s.Symbol == $"{fxTransaction.SellCurrency}c").FirstOrDefault();

                TransactionsBO fxBuyLegCash = new TransactionsBO
                {
                    SecurityId = fxBuySecurity.SecurityId,
                    FundId = fxTransaction.FundId,
                    isActive = true,
                    isLocked = false,
                    TradeAmount = fxTransaction.BuyAmount,
                    Price = 1,
                    Quantity = fxTransaction.BuyAmount,
                    CurrencyId = buyCurrency.CurrencyId,
                    TransactionTypeId = transactionTypes.Where(tt => tt.TypeName == "FXBuy").First().TransactionTypeId,
                    Comment = fxTransaction.Description,
                    CustodianId = custodian.CustodianId,
                    Fees = 0,
                    isCashTransaction = false,
                    TradeDate = fxTransaction.SettleDate,
                    SettleDate = fxTransaction.SettleDate,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    LinkedTrades = linkReference
                };
                TransactionsBO fxSellLegCash = new TransactionsBO
                {
                    SecurityId = fxSellSecurity.SecurityId,
                    FundId = fxTransaction.FundId,
                    isActive = true,
                    isLocked = false,
                    TradeAmount = fxTransaction.SellAmount,
                    Price = 1,
                    Quantity = fxTransaction.SellAmount,
                    CurrencyId = sellCurrency.CurrencyId,
                    TransactionTypeId = transactionTypes.Where(tt => tt.TypeName == "FXSell").First().TransactionTypeId,
                    Comment = fxTransaction.Description,
                    CustodianId = custodian.CustodianId,
                    Fees = 0,
                    isCashTransaction = false,
                    TradeDate = fxTransaction.SettleDate,
                    SettleDate = fxTransaction.SettleDate,
                    CreatedDate = DateTime.Now,
                    LastModified = DateTime.Now,
                    LinkedTrades = linkReference
                };
                context.Transactions.Add(fxBuyLegCash);
                context.Transactions.Add(fxSellLegCash);

                await context.SaveChangesAsync();
                return refTransaction;
            }
        }

        public async Task<TransactionsBO> CreateTransaction(TransactionsBO transaction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                EntityEntry<TransactionsBO> res = await context.Transactions.AddAsync(transaction);
                await context.SaveChangesAsync();
                return res.Entity;
            }
        }

        public async Task CreateCashTransfer(List<TransactionsBO> transfers)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                // Created LinkedTransactions Here
                LinkedTradesBO linkReference = new LinkedTradesBO();
                foreach (TransactionsBO transfer in transfers)
                {
                    transfer.LinkedTrades = linkReference;
                }
                context.Transactions.AddRange(transfers);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteFXTransaction(TransactionsBO transaction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                IEnumerable<TransactionsBO> linkedTransactions = context.Transactions.Where(t => t.LinkedTradeId == transaction.LinkedTradeId);
                // To DO im not happy with this it needs to be neater
                foreach (TransactionsBO linkedTransaction in linkedTransactions)
                {
                    TransactionsBO tempTransaction = linkedTransaction;
                    tempTransaction.isActive = false;
                    context.Entry(linkedTransaction).CurrentValues.SetValues(tempTransaction);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteTransaction(TransactionsBO transaction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                TransactionsBO originalTransaction = context.Transactions.First(t => t.TransactionId == transaction.TransactionId);
                transaction.isActive = false;
                context.Entry(originalTransaction).CurrentValues.SetValues(transaction);
                await context.SaveChangesAsync();
            }
        }

        public CustodiansDIM GetCustodian(string name)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.Custodians.AsNoTracking().Where(c => c.Name == name).FirstOrDefault();
            }
        }

        public SecuritiesDIM GetSecurityInfo(string symbol)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.Securities.AsNoTracking().Where(s => s.Symbol == symbol).Include(s => s.Currency).Include(s => s.AssetClass).FirstOrDefault();
            }
        }

        public TransactionTypeDIM GetTradeType(string name)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.TransactionTypes.AsNoTracking().Where(t => (t.TypeName) == name).FirstOrDefault();
            }
        }

        public async Task RestoreFXTransaction(TransactionsBO transaction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                IEnumerable<TransactionsBO> linkedTransactions = context.Transactions.Where(t => t.LinkedTradeId == transaction.LinkedTradeId);
                // To DO im not happy with this it needs to be neater
                foreach (TransactionsBO linkedTransaction in linkedTransactions)
                {
                    TransactionsBO tempTransaction = linkedTransaction;
                    tempTransaction.isActive = true;
                    context.Entry(linkedTransaction).CurrentValues.SetValues(tempTransaction);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task RestoreTransaction(TransactionsBO transaction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                TransactionsBO originalTransaction = context.Transactions.First(t => t.TransactionId == transaction.TransactionId);
                transaction.isActive = true;
                context.Entry(originalTransaction).CurrentValues.SetValues(transaction);
                await context.SaveChangesAsync();
            }
        }

        public bool SecurityExists(string symbol)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return (context.Securities.Where(s => s.Symbol == symbol).FirstOrDefault() != null);
            }
        }

        public async Task UpdateTransaction(TransactionsBO transaction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                TransactionsBO originalTransaction = context.Transactions.First(t => t.TransactionId == transaction.TransactionId);
                context.Entry(originalTransaction).CurrentValues.SetValues(transaction);
                await context.SaveChangesAsync();
            }
        }
    }
}
