﻿using PortfolioAce.Domain.Models.Dimensions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortfolioAce.Domain.Models.BackOfficeModels
{
    // This will encompass Trades and CashTrade entries.  TransferAgency tasks will flow in as subs/reds (without the price and units)
    [Table("bo_Transactions")]
    public class TransactionsBO
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        public decimal Quantity { get; set; }
        [Required, Column(TypeName = "decimal(18,4)"), Range(0, long.MaxValue, ErrorMessage = "Prices can not be negative numbers.")]
        public decimal Price { get; set; }
        [Required]
        public decimal TradeAmount { get; set; } // this should be autocalculated in wpf when created and edit. The sum of this will determine our cash

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TradeDate { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime SettleDate { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime CreatedDate { get; set; } // This is not set by the user

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime LastModified { get; set; } // This is not set by the user

        public decimal Fees { get; set; } // Or fees
        [Required]
        public bool isActive { get; set; } // for audit purposes entries cannot be deleted, only deactivated
        [Required]
        public bool isLocked { get; set; } // if true then this trade cannot be edited or deleted unless the trade is unlocked

        [Required]
        public bool isCashTransaction { get; set; } // I use this to determine whether cashbook trades can be edited or not.

        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [ForeignKey("Security")]
        public int SecurityId { get; set; }
        public SecuritiesDIM Security { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public CurrenciesDIM Currency { get; set; }

        [ForeignKey("Custodian")]
        public int CustodianId { get; set; }
        public CustodiansDIM Custodian { get; set; }

        [ForeignKey("Fund")]
        public int FundId { get; set; }
        public Fund Fund { get; set; }

        [ForeignKey("TransactionType")]
        public int TransactionTypeId { get; set; }
        public TransactionTypeDIM TransactionType { get; set; }

        [ForeignKey("LinkedTrades")]
        public int? LinkedTradeId { get; set; }
        public LinkedTradesBO LinkedTrades { get; set; }
    }
}
