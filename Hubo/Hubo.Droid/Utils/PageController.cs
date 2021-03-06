// <copyright file="PageController.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo.Droid
{
    using System.Collections.ObjectModel;
    using Xamarin.Forms;

    public class PageController : IPageController
    {
        private ReflectedProxy<Page> proxy;

        private PageController(Page page)
        {
            this.proxy = new ReflectedProxy<Page>(page);
        }

        public Rectangle ContainerArea
        {
            get
            {
                return this.proxy.GetPropertyValue<Rectangle>();
            }

            set
            {
                this.proxy.SetPropertyValue(value);
            }
        }

        public bool IgnoresContainerArea
        {
            get
            {
                return this.proxy.GetPropertyValue<bool>();
            }

            set
            {
                this.proxy.SetPropertyValue(value);
            }
        }

        public ObservableCollection<Element> InternalChildren
        {
            get
            {
                return this.proxy.GetPropertyValue<ObservableCollection<Element>>();
            }

            set
            {
                this.proxy.SetPropertyValue(value);
            }
        }

        public static IPageController Create(Page page)
        {
            return new PageController(page);
        }

        public void SendAppearing()
        {
            this.proxy.Call();
        }

        public void SendDisappearing()
        {
           this.proxy.Call();
        }
    }
}