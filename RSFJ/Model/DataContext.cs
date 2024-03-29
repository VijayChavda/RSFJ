﻿using RSFJ.Services;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RSFJ.Model
{
    public class DataContext
    {
        public List<RojmelEntry> RojmelEntries { get; set; }
        public HashSet<StockItem> StockItems { get; set; }
        public HashSet<Account> Accounts { get; set; }

        public StockItem Cash { get => StockItems.Single(x => x.Name == "Cash"); }
        public StockItem Fine999 { get => StockItems.Single(x => x.Name == "Fine999"); }
        public StockItem None { get => StockItems.Single(x => x.Name == "None"); }

        public Account Self { get => Accounts.Single(x => x.Name == "Self"); }

        public DataContext()
        {
            RojmelEntries = new List<RojmelEntry>();
            StockItems = new HashSet<StockItem>();
            Accounts = new HashSet<Account>();
        }

        public event EventHandler<StockItem> StockItemAdded;
        public event EventHandler<StockItem> StockItemRemoved;

        public event EventHandler<Account> AccountAdded;
        public event EventHandler<Account> AccountRemoved;

        public void AddRojmelEntry(RojmelEntry Entry, bool AssignNewId = true)
        {
            var account = Accounts.Single(x => x.Name == Entry.AccountName);
            var stockItem = StockItems.Single(x => x.Name == Entry.StockItemName);

            #region Calculations
            if (Entry.Type == RojmelEntryType.ItemExchangeFine)
            {
                Entry.Result = Entry.Param1 * (double)Entry.Param2 / 100;
            }
            else if (Entry.Type == RojmelEntryType.ItemExchangeCash)
            {
                if (account.Type == AccountType.Boolean)
                {
                    Entry.Result = Entry.Param1 * (double)Entry.Param2;
                }
                else if (account.Type == AccountType.Customer)
                {
                    if (Entry.IsLabourAsAmount)
                        Entry.Result = ((Entry.Param1 + (double)Entry.Waste) * (double)Entry.Param2) + (double)Entry.Labour;
                    else
                        Entry.Result = ((Entry.Param1 + (double)Entry.Waste) * ((double)Entry.Param2 + (double)Entry.Labour));
                }
                else
                {
                    Entry.Result = Entry.Param1 / (double)Entry.Param2;
                }
            }
            else if (Entry.Type == RojmelEntryType.SimpleCashExchange)
            {
                Entry.Result = Entry.Param1;
            }
            else if (Entry.Type == RojmelEntryType.UseCash)
            {
                Entry.Result = Entry.Param1 / (double)Entry.Param2;
            }
            else if (Entry.Type == RojmelEntryType.Initial)
            {
                Entry.Result = Entry.Param1;
            }
            else
            {
                Entry.Result = 0;
            }
            #endregion

            #region Update balances
            double result = Entry.Result = Math.Round(Entry.Result, 2);
            double param1 = Entry.Param1 = Math.Round(Entry.Param1, 2);

            stockItem.InStock += Entry.IsLeftSide ? Entry.Param1 : -Entry.Param1;

            switch (Entry.Type)
            {
                case RojmelEntryType.ItemExchangeFine:
                    account.FineInGold += Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.ItemExchangeCash:
                    account.FineInMoney += Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.SimpleCashExchange:
                    account.FineInMoney += Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.UseCash:
                    account.FineInGold += Entry.IsLeftSide ? -result : result;
                    account.FineInMoney += Entry.IsLeftSide ? param1 : -param1;
                    break;
            }
            #endregion

            if (AssignNewId)
                Entry.Id = RojmelEntries.Count == 0 ? 1 : RojmelEntries.OrderBy(x => x.Id).Last().Id + 1;

            RojmelEntries.Add(Entry);
        }

        public void RemoveRojmelEntry(RojmelEntry Entry)
        {
            if (RojmelEntries.Contains(Entry) == false)
                return;

            var account = Accounts.Single(x => x.Name == Entry.AccountName);
            var stockItem = StockItems.Single(x => x.Name == Entry.StockItemName);

            double result = Entry.Result = Math.Round(Entry.Result, 2);
            double param1 = Entry.Result = Math.Round(Entry.Param1, 2);

            stockItem.InStock -= Entry.IsLeftSide ? Entry.Param1 : -Entry.Param1;

            switch (Entry.Type)
            {
                case RojmelEntryType.ItemExchangeFine:
                    account.FineInGold -= Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.ItemExchangeCash:
                    account.FineInMoney -= Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.SimpleCashExchange:
                    account.FineInMoney -= Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.UseCash:
                    account.FineInGold -= Entry.IsLeftSide ? -result : result;
                    account.FineInMoney -= Entry.IsLeftSide ? param1 : -param1;
                    break;
            }

            RojmelEntries.Remove(Entry);
        }

        public void Load()
        {
            StockItems.Add(new StockItem() { Name = "Cash" });
            StockItems.Add(new StockItem() { Name = "Fine999" });
            StockItems.Add(new StockItem() { Name = "None" });
            Accounts.Add(new Account() { Name = "Self", Type = AccountType.Self });
        }

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
