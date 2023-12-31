﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.WinUI.UI.Controls;

namespace SnpApp.Views
{
    /// <summary>
    /// Extension methods used with the DataGrid control to support sorting.
    /// </summary>
    public static class DataGridHelper
    {
        /// <summary>
        /// Sorts the DataGrid by the specified column, updating the column header to reflect the current sort direction.
        /// </summary>
        /// <param name="dataGrid">The DataGrid to sort.</param>
        /// <param name="columnToSort">The column to sort by. If this column is already sorted, the data will be sorted descending order.</param>
        /// <param name="sort">A method that sorts the actual data source that the DataGrid is bound to.</param>
        public static void Sort(this DataGrid dataGrid, DataGridColumn columnToSort, Action<string, bool> sort)
        {
            var lastSortedColumn = dataGrid.Columns.FirstOrDefault(column => column.SortDirection.HasValue);
            var isSortColumnDifferentThanLast = columnToSort != lastSortedColumn;
            var isAscending = isSortColumnDifferentThanLast ||
                              columnToSort.SortDirection == DataGridSortDirection.Descending;

            columnToSort.SortDirection = isAscending ?
                DataGridSortDirection.Ascending : DataGridSortDirection.Descending;
            if (isSortColumnDifferentThanLast && lastSortedColumn != null)
            {
                lastSortedColumn.SortDirection = null;
            }

            var propertyName = columnToSort.Tag as string ?? columnToSort.Header.ToString();
#pragma warning disable CS8604 // Possible null reference argument.
            sort(propertyName, isAscending);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        /// Sorts the data in an ObservableCollection by the specified property and in the specified sort direction.
        /// </summary>
        public static void Sort<T>(this ObservableCollection<T> collection, string propertyName, bool isAscending)
        {
            var sortedCollection = isAscending ?
                collection.OrderBy(SortFunc).ToList() :
                collection.OrderByDescending(SortFunc).ToList();
            collection.Clear();
            foreach (var obj in sortedCollection)
            {
                collection.Add(obj);
            }

            return;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8603 // Possible null reference return.
            object SortFunc(T obj) => obj.GetType().GetProperty(propertyName).GetValue(obj);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
