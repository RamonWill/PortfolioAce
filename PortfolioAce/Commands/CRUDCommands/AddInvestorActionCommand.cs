﻿using PortfolioAce.Domain.Models.BackOfficeModels;
using PortfolioAce.EFCore.Services;
using PortfolioAce.ViewModels.Modals;
using System;
using System.Windows;
using System.Windows.Input;

namespace PortfolioAce.Commands
{
    public class AddInvestorActionCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private InvestorActionViewModel _investorActionVM;
        private ITransferAgencyService _investorService;

        public AddInvestorActionCommand(InvestorActionViewModel investorActionVM,
            ITransferAgencyService investorService)
        {
            _investorActionVM = investorActionVM;
            _investorService = investorService;
        }

        public bool CanExecute(object parameter)
        {
            return true; // true for now
        }

        public async void Execute(object parameter)
        {
            try
            {
                if (_investorActionVM.TradeAmount <= _investorActionVM.TargetFundMinimumInvestment && _investorActionVM.TAType == "Subscription")
                {
                    throw new ArgumentException("The Subscription amount must be greater than the Funds minimum investment");
                }
                TransferAgencyBO newInvestorAction = new TransferAgencyBO
                {
                    TransactionDate = _investorActionVM.TradeDate,
                    TransactionSettleDate = _investorActionVM.SettleDate,
                    IssueType = _investorActionVM.TAType,
                    Units = _investorActionVM.Units,
                    NAVPrice = _investorActionVM.Price,
                    TradeAmount = _investorActionVM.TradeAmount,
                    Currency = _investorActionVM.Currency,
                    Fees = _investorActionVM.Fee,
                    FundId = _investorActionVM.FundId,
                    IsNavFinal = _investorActionVM.isNavFinal
                };
                FundInvestorBO fundInvestor = _investorService.GetFundInvestor(_investorActionVM.FundId, _investorActionVM.SelectedInvestor.InvestorId);
                if (fundInvestor == null)
                {
                    //this means the investor is new to the fund.
                    fundInvestor = new FundInvestorBO
                    {
                        InceptionDate = _investorActionVM.TradeDate,
                        FundId = _investorActionVM.FundId,
                        InvestorId = _investorActionVM.SelectedInvestor.InvestorId,
                    };
                    fundInvestor.HighWaterMark = (_investorActionVM.TargetFundWaterMark && _investorActionVM.isNavFinal) ? _investorActionVM.Price : (decimal?)null;
                    newInvestorAction.FundInvestor = fundInvestor;
                }
                else
                {
                    newInvestorAction.FundInvestorId = fundInvestor.FundInvestorId;
                }

                // Cash should hit the transactions on SettleDate not trade date...

                await _investorService.CreateInvestorAction(newInvestorAction);
                _investorActionVM.CloseAction();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}