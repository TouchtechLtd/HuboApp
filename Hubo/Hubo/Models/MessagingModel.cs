// <copyright file="MessagingModel.cs" company="TrioTech">
// Copyright (c) TrioTech. All rights reserved.
// </copyright>

namespace Hubo
{
    internal class MessagingModel
    {
        public string PropertyName { get; set; }

        public double PropertyValue { get; set; } = -1;

        public bool PropertyBool { get; set; } = false;
    }

    internal class QuestionModel
    {
        public string Question { get; set; }

        public bool YesCorrect { get; set; }
    }
}
