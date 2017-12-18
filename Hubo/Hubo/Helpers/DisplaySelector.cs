// <copyright file="DisplaySelector.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    using Xamarin.Forms;

    public class DisplaySelector : DataTemplateSelector
    {
        public DisplaySelector()
        {
        }

        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate DriveTemplate { get; set; }

        public DataTemplate BreakTemplate { get; set; }

        public DataTemplate NoteTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var template = item as CarouselViewModel;

            switch (template.Page)
            {
                case "BreakView":
                    return BreakTemplate;
                case "DriveView":
                    return DriveTemplate;
                case "NoteView":
                    return NoteTemplate;
                default:
                    return DefaultTemplate;
            }
        }
    }
}
