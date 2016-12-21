using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Hubo
{
    public class BaseEntry : Entry
    {
        public new event EventHandler Completed;

        public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create("ReturnType", typeof(ReturnType), typeof(BaseEntry), ReturnType.Done);

        public Entry Next { get; set; }

        public BaseEntry()
        {
            Completed += Goto;
        }

        public ReturnType ReturnType
        {
            get { return (ReturnType)GetValue(ReturnTypeProperty); }
            set { SetValue(ReturnTypeProperty, value); }
        }

        public void InvokeCompleted()
        {
            if (this.Completed != null)
                this.Completed.Invoke(this, null);
        }

        private void Goto(object sender, EventArgs e)
        {
            if (sender != null && ((BaseEntry)sender).Next != null)
                ((BaseEntry)sender).Next.Focus();
        }
    }

    public enum ReturnType
    {
        Go, Next, Done, Send, Search
    }
}
