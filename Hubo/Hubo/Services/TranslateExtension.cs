using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Hubo.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hubo
{
    // You exclude the 'Extension' suffix when using in Xaml markup
    [ContentProperty("Text")]
    public class TranslateExtension : IMarkupExtension
    {
        public string Text { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Text == null)
                return null;

            return Resource.ResourceManager.GetString(Text, CultureInfo.CurrentCulture);
        }
    }
}