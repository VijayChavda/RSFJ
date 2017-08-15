using RSFJ.Services;
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

        public StockItem Cash { get; set; }
        public StockItem Fine999 { get; set; }
        public StockItem None { get; set; }

        public Account Self { get; set; }

        public DataContext()
        {
            Cash = new StockItem() { Name = "Cash", Rate_Purity = 2900 };   //TODO: Change 2900 to user defined.
            Fine999 = new StockItem() { Name = "Fine999" };
            None = new StockItem() { Name = "None" };

            Self = new Account() { Name = "Self", Group = "Others", Note = "Application generated.", Type = AccountType.Self };

            RojmelEntries = new List<RojmelEntry>();
            StockItems = new HashSet<StockItem>();
            Accounts = new HashSet<Account>();
        }

        public event EventHandler<StockItem> StockItemAdded;
        public event EventHandler<StockItem> StockItemRemoved;

        public event EventHandler<Account> AccountAdded;
        public event EventHandler<Account> AccountRemoved;

        public void AddRojmelEntry(RojmelEntry Entry)
        {
            #region Calculations
            if (Entry.Type == RojmelEntryType.ItemExchangeFine)
            {
                Entry.Result = Entry.Param1 * (double)Entry.Param2 / 100;
            }
            else if (Entry.Type == RojmelEntryType.ItemExchangeCash)
            {
                if (Entry.Account.Type == AccountType.Boolean)
                {
                    Entry.Result = Entry.Param1 * (double)Entry.Param2;
                }
                else if (Entry.Account.Type == AccountType.Customer)
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

            double result = Entry.Result = Math.Round(Entry.Result, 2);
            #endregion

            #region Update balances
            Entry.StockItem.InStock += Entry.IsLeftSide ? Entry.Param1 : -Entry.Param1;
            switch (Entry.Type)
            {
                case RojmelEntryType.ItemExchangeFine:
                    Entry.Account.FineInGold += Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.ItemExchangeCash:
                    Entry.Account.FineInMoney += Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.SimpleCashExchange:
                    Entry.Account.FineInMoney += Entry.IsLeftSide ? -result : result;
                    break;
                case RojmelEntryType.UseCash:
                    Entry.Account.FineInGold += Entry.IsLeftSide ? -result : result;
                    //Use account money balance if Cash is not provided.
                    if (Entry.StockItem == DataContextService.Instance.DataContext.None)
                    {
                        Entry.Account.FineInMoney += Entry.IsLeftSide ? result : -result;
                    }
                    break;
            }
            #endregion

            Entry.Id = RojmelEntries.Count == 0 ? 1 : RojmelEntries.Last().Id + 1;
            RojmelEntries.Add(Entry);
        }

        public void RemoveRojmelEntry(RojmelEntry Entry)
        {
            if (RojmelEntries.Contains(Entry) == false)
                return;

            Entry.StockItem.InStock -= Entry.IsLeftSide ? Entry.Param1 : -Entry.Param1;

            switch (Entry.Type)
            {
                case RojmelEntryType.ItemExchangeFine:
                    Entry.Account.FineInGold -= Entry.IsLeftSide ? -Entry.Result : Entry.Result;
                    break;
                case RojmelEntryType.ItemExchangeCash:
                    Entry.Account.FineInMoney -= Entry.IsLeftSide ? -Entry.Result : Entry.Result;
                    break;
                case RojmelEntryType.SimpleCashExchange:
                    Entry.Account.FineInMoney -= Entry.IsLeftSide ? -Entry.Result : Entry.Result;
                    break;
                case RojmelEntryType.UseCash:
                    Entry.Account.FineInGold -= Entry.IsLeftSide ? -Entry.Result : Entry.Result;
                    //Use account money balance if Cash is not provided.
                    if (Entry.StockItem == DataContextService.Instance.DataContext.None)
                    {
                        Entry.Account.FineInMoney -= Entry.IsLeftSide ? Entry.Param1 : -Entry.Param1;
                    }
                    break;
            }

            RojmelEntries.Remove(Entry);
        }

        internal void Load()
        {
            if (StockItems.Contains(Cash)) StockItems.Remove(Cash);
            if (StockItems.Contains(None)) StockItems.Remove(None);
            if (StockItems.Contains(Fine999)) StockItems.Remove(Fine999);

            if (Accounts.Contains(Self)) Accounts.Remove(Self);

            StockItems.Add(Cash);
            StockItems.Add(None);
            StockItems.Add(Fine999);

            Accounts.Add(Self);
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
