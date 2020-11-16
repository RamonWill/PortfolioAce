﻿using PortfolioAce.Commands;
using PortfolioAce.Domain.Models;
using PortfolioAce.EFCore.Services;
using PortfolioAce.Navigation;
using PortfolioAce.ViewModels.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PortfolioAce.ViewModels.Modals
{
    public class AddTradeWindowViewModel: ViewModelWindowBase
    {
        private Fund _fund;
        private ITradeService _tradeService;
        public AddTradeWindowViewModel(ITradeService tradeService, Fund fund)
        {
            AddTradeCommand = new AddTradeCommand(this, tradeService);
            _tradeService = tradeService;
            _fund = fund;
            _tradeDate = DateTime.Today;
            _settleDate = DateTime.Today;
        }

        public int FundId
        {
            get
            {
                return _fund.FundId;
            }
            private set
            {

            }
        }

        private string _tradeType;
        public string TradeType
        {
            get
            {
                return _tradeType;
            }
            set
            {
                _tradeType = value;
                OnPropertyChanged(nameof(TradeType));
            }
        }

        private string _symbol;
        public string Symbol
        {
            get
            {
                return _symbol;
            }
            set
            {
                _symbol = value;
                // I can check the Database for the value,
                // and if it exists prefill the currency field
                // if not raise exception
                OnPropertyChanged(nameof(Symbol));
                OnPropertyChanged(nameof(TradeCurrency));
            }
        }

        private decimal _quantity;
        public decimal Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(TradeAmount));
            }
        }

        private decimal _price;
        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price= value;
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(TradeAmount)); 
            }
        }

        private decimal _tradeAmount;
        public decimal TradeAmount
        {
            
            get
            {
                if (TradeType != "Corporate Action")
                {
                    int multiplier = -1;
                    _tradeAmount = Math.Round((Quantity * Price * multiplier) - Commission, 2);
                }

                return _tradeAmount;
            }
            set
            {
                if(TradeType == "Corporate Action")
                {
                    _tradeAmount = value;
                }
                OnPropertyChanged(nameof(TradeAmount));
            }
        }

        //private string _tradeCurrency;
        public string TradeCurrency
        {
            get
            {
                //this should be based on the security symbol
                if (Symbol == null)
                {
                    return null;
                }
                else
                {
                    var res = _tradeService.GetSecurityInfo(Symbol);
                    return (res != null) ? res.Currency : null;
                }
            }
            private set
            {

            }
        }

        private decimal _commission;
        public decimal Commission
        {
            get
            {
                return _commission;
            }
            set
            {
                _commission = value;
                OnPropertyChanged(nameof(Commission));
                OnPropertyChanged(nameof(TradeAmount)); 
            }
        }

        private DateTime _tradeDate;
        public DateTime TradeDate
        {
            get
            {
                return _tradeDate;
            }
            set
            {
                _tradeDate = value;
                OnPropertyChanged(nameof(TradeDate));
            }
        }

        private DateTime _settleDate;
        public DateTime SettleDate
        {
            get
            {
                return _settleDate;
            }
            set
            {
                _settleDate = value;
                OnPropertyChanged(nameof(SettleDate));
            }
        }



        public ICommand AddTradeCommand { get; set; }
    }
}
