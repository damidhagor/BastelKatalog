﻿using System;
using BastelKatalog.Views;
using Xamarin.Forms;

namespace BastelKatalog
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(BrowseItemsPage), typeof(BrowseItemsPage));
            Routing.RegisterRoute(nameof(ShowItemPage), typeof(ShowItemPage));
            Routing.RegisterRoute(nameof(EditItemPage), typeof(EditItemPage));
            Routing.RegisterRoute(nameof(ShowProjectPage), typeof(ShowProjectPage));
            Routing.RegisterRoute(nameof(EditProjectPage), typeof(EditProjectPage));

            CurrentItem = searchItem;
        }
    }
}
