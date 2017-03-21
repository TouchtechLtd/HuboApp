// <copyright file="BaseEntry.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using System;
    using Xamarin.Forms;

    public enum ReturnType
    {
        Go, Next, Done, Send, Search
    }

    public class BaseEntry : Entry
    {
        public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create("ReturnType", typeof(ReturnType), typeof(BaseEntry), ReturnType.Done);

        public BaseEntry()
        {
            Completed += Goto;
        }

        public new event EventHandler Completed;

        public Entry Next { get; set; }

        public ReturnType ReturnType
        {
            get { return (ReturnType)GetValue(ReturnTypeProperty); }
            set { SetValue(ReturnTypeProperty, value); }
        }

        public void InvokeCompleted()
        {
            if (this.Completed != null)
            {
                this.Completed.Invoke(this, null);
            }
        }

        private void Goto(object sender, EventArgs e)
        {
            if (sender != null && ((BaseEntry)sender).Next != null)
            {
                ((BaseEntry)sender).Next.Focus();
            }
        }
    }
}
