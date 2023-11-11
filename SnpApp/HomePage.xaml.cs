//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System.Linq;
using Microsoft.UI.Xaml.Navigation;
using Snp.App.Navigation;

namespace Snp.App
{
    public sealed partial class HomePage : ItemsPageBase
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationRootPageArgs args = (NavigationRootPageArgs)e.Parameter;
            var menuItem = (Microsoft.UI.Xaml.Controls.NavigationViewItem)args.NavigationRootPage.NavigationView.MenuItems.First();
            menuItem.IsSelected = true;
        }

        protected override bool GetIsNarrowLayoutState()
        {
            return LayoutVisualStates.CurrentState == NarrowLayout;
        }
    }
}
