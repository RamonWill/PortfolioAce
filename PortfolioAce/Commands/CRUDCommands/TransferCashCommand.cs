﻿using PortfolioAce.Domain.Models.BackOfficeModels;
using PortfolioAce.Domain.Models.Dimensions;
using PortfolioAce.EFCore.Services;
using PortfolioAce.ViewModels.Modals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PortfolioAce.Commands
{
    public class TransferCashCommand : AsyncCommandBase
    {
        private AddCashTradeWindowViewModel _addCashTradeWindowVM;
        private ITransactionService _transactionService;

        public TransferCashCommand(
            AddCashTradeWindowViewModel addCashTradeWindowVM,
            ITransactionService transactionService)
        {
            _addCashTradeWindowVM = addCashTradeWindowVM;
            _transactionService = transactionService;
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            try
            {
                // cash symbol would be something like EURc name = EUR CASH
                SecuritiesDIM security = _transactionService.GetSecurityInfo(_addCashTradeWindowVM.Symbol);
                TransactionTypeDIM tradeType = _transactionService.GetTradeType(_addCashTradeWindowVM.CashType);
                CustodiansDIM payeeCustodian = _transactionService.GetCustodian(_addCashTradeWindowVM.PayeeCustodian);
                CustodiansDIM recipientCustodian = _transactionService.GetCustodian(_addCashTradeWindowVM.RecipientCustodian);
                TransactionsBO payeeTrade = new TransactionsBO
                {
                    SecurityId = security.SecurityId,
                    Quantity = _addCashTradeWindowVM.PayeeAmount,
                    TradeAmount = _addCashTradeWindowVM.PayeeAmount,
                    TradeDate = _addCashTradeWindowVM.TradeDate,
                    SettleDate = _addCashTradeWindowVM.SettleDate,
                    CreatedDate = _addCashTradeWindowVM.CreatedDate,
                    LastModified = _addCashTradeWindowVM.LastModifiedDate,
                    Fees = _addCashTradeWindowVM.PayeeFee,
                    isActive = _addCashTradeWindowVM.isActive,
                    isLocked = _addCashTradeWindowVM.isLocked,
                    isCashTransaction = true,
                    FundId = _addCashTradeWindowVM.FundId,
                    TransactionTypeId = tradeType.TransactionTypeId,
                    CurrencyId = security.CurrencyId,
                    Comment = _addCashTradeWindowVM.PayeesNotes,
                    CustodianId = payeeCustodian.CustodianId
                };

                TransactionsBO recipientTrade = new TransactionsBO
                {
                    SecurityId = security.SecurityId,
                    Quantity = _addCashTradeWindowVM.RecipientAmount,
                    Price = _addCashTradeWindowVM.Price,
                    TradeAmount = _addCashTradeWindowVM.RecipientAmount,
                    TradeDate = _addCashTradeWindowVM.TradeDate,
                    SettleDate = _addCashTradeWindowVM.SettleDate,
                    CreatedDate = _addCashTradeWindowVM.CreatedDate,
                    LastModified = _addCashTradeWindowVM.LastModifiedDate,
                    Fees = _addCashTradeWindowVM.RecipientFee,
                    isActive = _addCashTradeWindowVM.isActive,
                    isLocked = _addCashTradeWindowVM.isLocked,
                    isCashTransaction = true,
                    FundId = _addCashTradeWindowVM.FundId,
                    TransactionTypeId = tradeType.TransactionTypeId,
                    CurrencyId = security.CurrencyId,
                    Comment = _addCashTradeWindowVM.RecipientNotes,
                    CustodianId = recipientCustodian.CustodianId
                };
                List<TransactionsBO> transfer = new List<TransactionsBO> { payeeTrade, recipientTrade };
                await _transactionService.CreateCashTransfer(transfer);
                _addCashTradeWindowVM.CloseAction();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
