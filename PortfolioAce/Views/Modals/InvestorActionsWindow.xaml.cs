﻿using PortfolioAce.HelperObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PortfolioAce.Views.Modals
{
    /// <summary>
    /// Interaction logic for InvestorActionsWindow.xaml
    /// </summary>
    public partial class InvestorActionsWindow : Window
    {
        public InvestorActionsWindow()
        {
            InitializeComponent();
            cmbType.ItemsSource = StaticDataObjects.IssueTypes;
        }
    }
}