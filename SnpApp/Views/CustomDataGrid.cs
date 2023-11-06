﻿using System.Collections;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Snp.App.Views;

public class CustomDataGrid : DataGrid
{
    public CustomDataGrid ()
    {
        this.SelectionChanged += CustomDataGrid_SelectionChanged;
    }

    void CustomDataGrid_SelectionChanged (object sender, SelectionChangedEventArgs e)
    {
        this.SelectedItemsList = this.SelectedItems;
    }
    #region SelectedItemsList

    public IList SelectedItemsList
    {
        get { return (IList)GetValue (SelectedItemsListProperty); }
        set { SetValue (SelectedItemsListProperty, value); }
    }

    public static readonly DependencyProperty SelectedItemsListProperty =
        DependencyProperty.Register ("SelectedItemsList", typeof (IList), typeof (CustomDataGrid), new PropertyMetadata (null));

    #endregion
}