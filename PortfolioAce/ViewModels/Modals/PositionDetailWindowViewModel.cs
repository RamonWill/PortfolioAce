﻿using LiveCharts;
using PortfolioAce.Domain.DataObjects;
using PortfolioAce.Domain.Models;
using PortfolioAce.Domain.Models.FactTables;
using PortfolioAce.EFCore.Services.DimensionServices;
using PortfolioAce.EFCore.Services.FactTableServices;
using PortfolioAce.EFCore.Services.PriceServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortfolioAce.ViewModels.Modals
{
    public class PositionDetailWindowViewModel: ViewModelWindowBase
    {

        // I need the actual position, take the static references)
        private IFactTableService _factTableService;
        private IPriceService _priceService;
        
        public PositionDetailWindowViewModel(IPriceService priceService, IFactTableService factTableService,
           SecurityPositionValuation valuedPosition, Fund fund)
        {
            _factTableService = factTableService;
            _priceService = priceService;
            _valuedPosition = valuedPosition;
            PositionOpenLots = _valuedPosition.Position.GetOpenLots();
            Title = $"{_valuedPosition.Position.security.SecurityName} ({_valuedPosition.Position.security.Symbol})";
            FundName = fund.FundName;

            List<PositionFACT> positionHistory = _factTableService.GetAllFundStoredPositions(fund.FundId, valuedPosition.Position.security.SecurityId);
            PositionPriceLineChartYAxis = new ChartValues<decimal>(positionHistory.Select(ph=> ph.RealisedPnl+ph.UnrealisedPnl));
            PositionPriceLineChartXAxis = positionHistory.Select(ph => ph.PositionDate.ToString("dd/MM/yyyy")).ToArray();
        }

        public ChartValues<decimal> PositionPriceLineChartYAxis { get; set; }
        public string[] PositionPriceLineChartXAxis { get; set; }


        private SecurityPositionValuation _valuedPosition;
        public SecurityPositionValuation TargetPosition
        {
            get
            {
                return _valuedPosition;
            }
        }
        public List<OpenLots> PositionOpenLots { get; set; }

        public string Title { get; set; }
        public string FundName { get; set; }
    }
}