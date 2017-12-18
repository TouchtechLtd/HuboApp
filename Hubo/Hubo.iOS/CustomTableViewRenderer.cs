// <copyright file="CustomTableViewRenderer.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using System;
using Foundation;
using Hubo;
using Hubo.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomTableView), typeof(CustomTableViewRenderer))]

namespace Hubo.iOS
{
    public class CustomTableViewRenderer : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null)
            {
                return;
            }

            var customTableView = this.Element as CustomTableView;

            this.Control.WeakDelegate = new CustomTableViewModelRenderer(customTableView);
        }

        private class CustomTableViewModelRenderer : TableViewModelRenderer
        {
            private readonly CustomTableView customTableView;

            public CustomTableViewModelRenderer(TableView model)
                : base(model)
            {
                this.customTableView = model as CustomTableView;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                return new UILabel()
                {
                    Text = this.TitleForHeader(tableView, section),
                    TextColor = this.customTableView.HeaderTextColor.ToUIColor(),
                    TextAlignment = UITextAlignment.Left
                };
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = base.GetCell(tableView, indexPath);

                cell.TextLabel.TextColor = this.customTableView.TextColor.ToUIColor();

                return cell;
            }
        }
    }
}
