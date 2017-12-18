// <copyright file="CustomTableViewRenderer.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Hubo;
using Hubo.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomTableView), typeof(CustomTableViewRenderer))]

namespace Hubo.Droid
{
    public class CustomTableViewRenderer : TableViewRenderer
    {
        protected override TableViewModelRenderer GetModelRenderer(Android.Widget.ListView listView, TableView view)
        {
            return new CustomTableViewModelRenderer(this.Context, listView, view);
        }

        private class CustomTableViewModelRenderer : TableViewModelRenderer
        {
            private readonly CustomTableView customTableView;

            public CustomTableViewModelRenderer(Context context, Android.Widget.ListView listView, TableView view)
                : base(context, listView, view)
            {
                this.customTableView = view as CustomTableView;
            }

            public override Android.Views.View GetView(int position, Android.Views.View convertView, ViewGroup parent)
            {
                var view = base.GetView(position, convertView, parent);

                var element = this.GetCellForPosition(position);

                if (element.GetType() == typeof(TextCell))
                {
                    try
                    {
                        var textView = (((view as LinearLayout).GetChildAt(0) as LinearLayout).GetChildAt(1) as LinearLayout).GetChildAt(0) as TextView;

                        var divider = (view as LinearLayout).GetChildAt(1);

                        textView.SetTextColor(this.customTableView.HeaderTextColor.ToAndroid());
                        textView.TextAlignment = Android.Views.TextAlignment.TextStart;
                        textView.Gravity = GravityFlags.Start;
                        divider.SetBackgroundColor(this.customTableView.SectionSeparatorColor.ToAndroid());
                    }
                    catch (Exception)
                    {
                    }
                }

                if (element.GetType() == typeof(SwitchCell))
                {
                    try
                    {
                        var switchCell = (((view as LinearLayout).GetChildAt(0) as LinearLayout).GetChildAt(1) as LinearLayout).GetChildAt(0) as TextView;

                        var divider = (view as LinearLayout).GetChildAt(1);

                        switchCell.SetTextColor(this.customTableView.TextColor.ToAndroid());
                        switchCell.TextAlignment = Android.Views.TextAlignment.TextStart;
                        switchCell.Gravity = GravityFlags.Start;
                        divider.SetBackgroundColor(this.customTableView.SeparatorColor.ToAndroid());
                    }
                    catch (Exception)
                    {
                    }
                }

                return view;
            }
        }
    }
}