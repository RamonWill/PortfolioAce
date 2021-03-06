﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioAce.Domain.Models.BackOfficeModels
{
    [Table("bo_TransferAgency")]
    public class TransferAgencyBO
    {
        [Key]
        public int TransferAgencyId { get; set; }

        [Required]
        public bool IsNavFinal { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } // must be on or after the funds launch date!!!

        [Required]
        public DateTime TransactionSettleDate { get; set; } // this is the subscription and redemption dates.

        [Required]
        public decimal Units { get; set; }

        [Required]
        public decimal NAVPrice { get; set; }

        [Required]
        public decimal TradeAmount { get; set; } // fees should be taken into account here.

        [Required]
        public string Currency { get; set; } // hardcoded to funds base currency... no support for share classes atm.

        [Required]
        public decimal Fees { get; set; }

        [Required]
        public string IssueType { get; set; } //subscription and redemption date

        [ForeignKey("Fund")]
        public int FundId { get; set; }
        public Fund Fund { get; set; }

        [ForeignKey("FundInvestor")]
        public int FundInvestorId { get; set; }
        public FundInvestorBO FundInvestor { get; set; }

    }
}
