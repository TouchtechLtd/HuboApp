using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace Hubo.Droid
{
    public class PageController : IPageController
    {
        private readonly ReflectedProxy<Page> _proxy;

        public static IPageController Create(Page page)
        {
            return new PageController(page);
        }

        PageController(Page page)
        {
            _proxy = new ReflectedProxy<Page>(page);
        }

        public Rectangle ContainerArea
        {
            get
            {
                return _proxy.GetPropertyValue<Rectangle>();
            }

            set
            {
                _proxy.SetPropertyValue(value);
            }
        }

        public bool IgnoresContainerArea
        {
            get
            {
                return _proxy.GetPropertyValue<bool>();
            }

            set
            {
                _proxy.SetPropertyValue(value);
            }
        }

        public ObservableCollection<Element> InternalChildren
        {
            get
            {
                return _proxy.GetPropertyValue<ObservableCollection<Element>>();
            }

            set
            {
                _proxy.SetPropertyValue(value);
            }
        }

        public void SendAppearing()
        {
            _proxy.Call();
        }

        public void SendDisappearing()
        {
            _proxy.Call();
        }
    }
}