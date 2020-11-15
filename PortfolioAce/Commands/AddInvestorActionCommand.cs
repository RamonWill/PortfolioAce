﻿using PortfolioAce.Domain.Models;
using PortfolioAce.EFCore.Repository;
using PortfolioAce.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace PortfolioAce.Commands
{
    public class AddInvestorActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private InvestorActionViewModel _investorActionVM;
        private ITransferAgencyRepository _investorRepo;

        public AddInvestorActionCommand(InvestorActionViewModel investorActionVM, 
            ITransferAgencyRepository investorRepo)
        {
            _investorActionVM = investorActionVM;
            _investorRepo = investorRepo;
        }

        public bool CanExecute(object parameter)
        {
            return true; // true for now
        }

        public async void Execute(object parameter)
        {
            try
            {
                TransferAgency newInvestorAction = new TransferAgency
                {
                    TransactionDate = _investorActionVM.TradeDate,
                    TransactionSettleDate = _investorActionVM.SettleDate,
                    InvestorName = _investorActionVM.InvestorName,
                    Type = _investorActionVM.TAType,
                    Units = _investorActionVM.Units,
                    NAVPrice = _investorActionVM.Price,
                    TradeAmount = _investorActionVM.TradeAmount,
                    Currency = _investorActionVM.Currency,
                    Fees = _investorActionVM.Fee,
                    FundId = _investorActionVM.FundId
                };

                // why cant i use await here? might be redundant once i refactor everything.
                await _investorRepo.CreateInvestorAction(newInvestorAction);
                _investorActionVM.CloseAction();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}