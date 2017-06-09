﻿using System;

namespace RSFJ.Model
{
    /// <summary>
    /// Represents a Rojmel entry.
    /// </summary>
    public class RojmelEntry
    {
        /// <summary>
        /// Identifier of current entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Date of entry.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Account holder's name.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Stock item's name.
        /// </summary>
        public string StockItem { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if uplak is clear.
        /// </summary>
        public bool UplakClear { get; set; }

        /// <summary>
        /// Parameter 1 of rojmel entry.
        /// </summary>
        public double Param1 { get; set; }

        /// <summary>
        /// Parameter 2 of rojmel entry.
        /// </summary>
        public double Param2 { get; set; }

        /// <summary>
        /// Result of rojmel entry.
        /// </summary>
        public double Result { get; set; }

        /// <summary>
        /// Direction of rojmel entry. Represents credit/debit.
        /// </summary>
        public bool IsLeftSide { get; set; }
    }
}