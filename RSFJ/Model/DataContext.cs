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
            #region Calculations
            double param1 = Math.Round(Entry.Param1, 2);
            double param2 = Math.Round(Entry.Param2 ?? 0, 2);
            double result = Math.Round(Entry.Result, 2);

            double waste = Entry.Waste ?? 0;
            double labour = Entry.Labour ?? 0;

            if (Entry.Type == RojmelEntryType.ItemExchangeFine)
            {
                Entry.Result = param1 * param2 / 100;
            }
            else if (Entry.Type == RojmelEntryType.ItemExchangeCash)
            {
                if (Entry.Account.Type == AccountType.Boolean)
                {
                    Entry.Result = param1 * param2;
                }
                else if(Entry.Account.Type == AccountType.Customer)
                {
                    if (Entry.IsLabourAsAmount)
                        Entry.Result = ((param1 + waste) * param2) + labour;
                    else
                        Entry.Result = ((param1 + waste) * (param2 + labour));
                }
                else
                {
                    Entry.Result = param1 / param2;
                }
            }
            else if (Entry.Type == RojmelEntryType.SimpleCashExchange)
            {
                Entry.Result = param1;
            }
            else if (Entry.Type == RojmelEntryType.UseCash)
            {
                Entry.Result = param1 / param2;
            }
            else
            {
                Entry.Result = 0;
            }
            #endregion

            #region Update balances
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
            #endregion

            Entry.Id = RojmelEntries.Count + 1;
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
                    if (Entry.StockItem == StockItem.None)
                    {
                        Entry.Account.FineInMoney -= Entry.IsLeftSide ? Entry.Param1 : -Entry.Param1;
                    }
                    break;
            }

            RojmelEntries.Remove(Entry);
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
