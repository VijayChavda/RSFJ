using System;
using System.Collections.Generic;

namespace RSFJ.Model
{
    public class DataContext
    {
        public HashSet<RojmelEntry> RojmelEntries { get; set; }
        public HashSet<StockItem> StockItems { get; set; }
        public HashSet<Account> Accounts { get; set; }

        public DataContext()
        {
            RojmelEntries = new HashSet<RojmelEntry>();
            StockItems = new HashSet<StockItem>() { StockItem.Cash, StockItem.None };
            Accounts = new HashSet<Account>();
        }

        public event EventHandler<StockItem> StockItemAdded;
        public event EventHandler<StockItem> StockItemRemoved;

        public event EventHandler<Account> AccountAdded;
        public event EventHandler<Account> AccountRemoved;

        public void AddRojmelEntry(RojmelEntry Entry)
        {
            if (Entry.Type == RojmelEntryType.Invalid || Entry.StockItem == null || Entry.Account == null)
            {
                return;
            }

            Entry.StockItem.InStock += Entry.IsLeftSide ? Entry.Param1 : -Entry.Param1;

            switch (Entry.Type)
            {
                case RojmelEntryType.ItemExchangeFine:
                    Entry.Account.FineInGold += Entry.IsLeftSide ? -Entry.Result : Entry.Result;
                    break;
                case RojmelEntryType.ItemExchangeCash:
                    Entry.Account.FineInMoney += Entry.IsLeftSide ? -Entry.Result : Entry.Result;
                    break;
                case RojmelEntryType.SimpleCashExchange:
                    Entry.Account.FineInMoney += Entry.IsLeftSide ? -Entry.Result : Entry.Result;
                    break;
                case RojmelEntryType.UseCash:
                    Entry.Account.FineInGold += Entry.IsLeftSide ? -Entry.Result : Entry.Result;
                    //Use account money balance if Cash is not provided.
                    if (Entry.StockItem == StockItem.None)
                    {
                        Entry.Account.FineInMoney += Entry.IsLeftSide ? Entry.Param1 : -Entry.Param1;
                    }
                    break;
            }

            RojmelEntries.Add(Entry);
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
