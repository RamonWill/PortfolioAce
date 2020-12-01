﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PortfolioAce.Domain.Models.Dimensions
{
    [Table("dim_TransactionType")]
    public class TransactionTypeDIM
    {
        [Key]
        public int TransactionTypeId { get; set; }
        [Required]
        public TranTypes TypeName { get; set; } 
        [Required]
        public TranClasses TypeClass { get; set; } 
    }

    public enum TranTypes
    {
        Purchase,
        Sell,
        Coupon,
        Dividends,
        Income,
        Expense,
        Deposit,
        Withdrawal,
        Interest,
        ManagementFee,
        PerformanceFee,
        Miscellaneous
    }

    public enum TranClasses
    {
        SecurityTrade,
        CashTrade
    }
}
