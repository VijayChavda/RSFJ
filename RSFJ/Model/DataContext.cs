﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace RSFJ.Model
{
    public class DataContext
    {
        public HashSet<RojmelEntry> RojmelEntries { get; set; }
        public HashSet<StockItem> StockItems { get; set; }
        public HashSet<Account> Accounts { get; set; }

        public static HashSet<string> RojmelEntryTypes { get; set; }

        public static readonly StockItem Cash;
        public static readonly StockItem None;

        static DataContext()
        {
            RojmelEntryTypes = new HashSet<string>()
            {
                RojmelEntryType.Exchange,
                RojmelEntryType.Customer,
                RojmelEntryType.Bullion,
                RojmelEntryType.Initital,
                RojmelEntryType.Uplak,
                RojmelEntryType.UplakClear,
                RojmelEntryType.InstantCash,
            };

            Cash = new StockItem()
            {
                Name = "Cash",
                AppliesToType = new List<string>()
                {
                    RojmelEntryType.Exchange,
                    RojmelEntryType.Bullion,
                    RojmelEntryType.Uplak,
                }
            };

            None = new StockItem()
            {
                Name = "None",
                AppliesToType = new List<string>()
                {
                    RojmelEntryType.Exchange
                }
            };
        }

        public DataContext()
        {
            RojmelEntries = new HashSet<RojmelEntry>();
            StockItems = new HashSet<StockItem>() { Cash, None };
            Accounts = new HashSet<Account>();
        }

        public event EventHandler<StockItem> StockItemAdded;
        public event EventHandler<StockItem> StockItemRemoved;

        public event EventHandler<Account> AccountAdded;
        public event EventHandler<Account> AccountRemoved;

        internal void FireStockItemAdded(StockItem Item)
        {
            StockItemAdded?.Invoke(this, Item);
        }

        internal void FireStockItemRemoved(StockItem Item)
        {
            StockItemRemoved?.Invoke(this, Item);
        }

        internal void FireAccountAdded(Account Account)
        {
            AccountAdded?.Invoke(this, Account);
        }

        internal void FireAccountRemoved(Account Account)
        {
            AccountRemoved?.Invoke(this, Account);
        }
    }
}
