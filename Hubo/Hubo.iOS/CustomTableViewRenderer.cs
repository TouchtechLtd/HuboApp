using System;
using System.Collections.Generic;
using System.Text;
using Hubo;
using Hubo.iOS;

[assembly: ExportRenderer(typeof(CustomTableView), typeof(CustomTableViewRenderer))]

namespace Hubo.iOS
{
    public class CustomTableViewRenderer : TableViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TableView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                return;
            }

            var customTableView = Element as customTableView;
            tableView.WeakDelegate = new CustomTableViewModelRenderer(customTableView);
        }

        private class CustomTableViewModelRenderer :UnEvenTableViewModelRenderer
        {
            private readonly CustomTableView customTableView;

            public CustomTableViewModelRenderer(TableView model) : base(model)
            {
                customTableView = model as CustomTableView;
            }

            public override UIView GetViewForHeader(UITableView tableView, nint section)
            {
                return new UILabel()
                {
                    Text = TitleForHeader(tableView, section),
                    TextColor = customTableView.HeaderTextColor.ToUIColor(),
                    TextAlignment = UITextAlignment.Center
                };
            }
        }
    }
}
