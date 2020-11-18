﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PortfolioAce.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioAce.EFCore.Services
{
    public class TransferAgencyService : ITransferAgencyService
    {

        private readonly PortfolioAceDbContextFactory _contextFactory;

        public TransferAgencyService(PortfolioAceDbContextFactory contextFactory)
        {
            this._contextFactory = contextFactory;
        }

        public async Task<TransferAgency> CreateInvestorAction(TransferAgency investorAction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                EntityEntry<TransferAgency> res = await context.TransferAgent.AddAsync(investorAction);
                await context.SaveChangesAsync();
                CashBook transaction = TransactionMapper(res.Entity, new CashBook());
                await context.CashBooks.AddAsync(transaction);
                await context.SaveChangesAsync();

                return res.Entity;
            }
        }

        public async Task<TransferAgency> DeleteInvestorAction(int id)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                TransferAgency investorAction = await context.TransferAgent.FindAsync(id);
                if (investorAction == null)
                {
                    return investorAction;
                }
                context.TransferAgent.Remove(investorAction);
                //TODO: raise a warning if there is no transaction to remove. Big issue if this is the case.
                CashBook transaction = await context.CashBooks.Where(ta => ta.TransferAgencyId == id).FirstAsync();
                context.CashBooks.Remove(transaction);
                await context.SaveChangesAsync();

                return investorAction;
            }
        }

        public List<TransferAgency> GetAllFundInvestorActions(int fundId)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return context.TransferAgent.Where(ta => ta.FundId == fundId).OrderBy(ta => ta.TransactionDate).ToList();
            }
        }

        public async Task<TransferAgency> GetInvestorActionById(int id)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                return await context.TransferAgent.FindAsync(id);
            }
        }

        public void InitialiseFundAction(Fund fund, List<TransferAgency> investors, NAVPriceStore initialNav)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                // need to update fund to initialised, add TA to database, add cashbook to DB,
                // Need to update the funds first NAV
                context.Funds.Update(fund);
                context.NavPriceData.Add(initialNav);
                foreach (TransferAgency investor in investors)
                {
                    var res = context.TransferAgent.Add(investor);
                    context.SaveChanges();
                    CashBook transaction = TransactionMapper(res.Entity, new CashBook());
                    context.CashBooks.Add(transaction);

                };
                context.SaveChanges();
            }
        }

        public async Task<TransferAgency> UpdateInvestorAction(TransferAgency investorAction)
        {
            using (PortfolioAceDbContext context = _contextFactory.CreateDbContext())
            {
                context.TransferAgent.Update(investorAction);
                CashBook transaction = await context.CashBooks.Where(ta => ta.TransferAgencyId == investorAction.TransferAgencyId).FirstAsync();
                CashBook newTransaction = TransactionMapper(investorAction, transaction);
                context.CashBooks.Update(newTransaction);
                await context.SaveChangesAsync();

                return investorAction;
            }
        }

        private CashBook TransactionMapper(TransferAgency investorAction, CashBook transaction)
        {
            transaction.TransferAgencyId = investorAction.TransferAgencyId;
            transaction.TransactionType = investorAction.Type;// subscription or redemption
            transaction.TransactionAmount = investorAction.TradeAmount;
            transaction.TransactionDate = investorAction.TransactionSettleDate;
            transaction.Currency = investorAction.Currency;
            transaction.FundId = investorAction.FundId;

            transaction.Comment = (investorAction.Type =="Subscription")?"FUND SUBSCRIPTION":"FUND REDEMPTION";
            return transaction;
        }
    }
}
