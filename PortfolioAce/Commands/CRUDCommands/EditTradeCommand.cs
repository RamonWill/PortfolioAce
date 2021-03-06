﻿using PortfolioAce.Domain.Models.BackOfficeModels;
using PortfolioAce.Domain.Models.Dimensions;
using PortfolioAce.EFCore.Services;
using PortfolioAce.ViewModels.Modals;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PortfolioAce.Commands
{
    public class EditTradeCommand : AsyncCommandBase
    {

        private EditTradeWindowViewModel _editTradeWindowVM;
        private ITransactionService _transactionService;
        private TransactionsBO _transaction;

        public EditTradeCommand(EditTradeWindowViewModel editTradeWindowVM,
            ITransactionService transactionService, TransactionsBO transactions)
        {
            _editTradeWindowVM = editTradeWindowVM;
            _transactionService = transactionService;
            _transaction = transactions;
        }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter);
        }

        public override async Task ExecuteAsync(object parameter)
        {
            try
            {
                CustodiansDIM custodian = _transactionService.GetCustodian(_editTradeWindowVM.Custodian);
                _transaction.LastModified = DateTime.Now;
                _transaction.CustodianId = custodian.CustodianId;
                _transaction.Custodian = custodian;
                _transaction.Quantity = _editTradeWindowVM.Quantity;
                _transaction.Price = _editTradeWindowVM.Price;
                _transaction.TradeAmount = _editTradeWindowVM.TradeAmount;
                _transaction.Fees = _editTradeWindowVM.Commission;
                _transaction.TradeDate = _editTradeWindowVM.TradeDate;
                _transaction.SettleDate = _editTradeWindowVM.SettleDate;
                await _transactionService.UpdateTransaction(_transaction);
                _editTradeWindowVM.CloseAction();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
