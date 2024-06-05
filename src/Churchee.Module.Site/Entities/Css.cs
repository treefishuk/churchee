﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Churchee.Common.Data;

namespace Churchee.Module.Site.Entities
{
    public class Css : Entity
    {
        private Css()
        {

        }

        public Css(Guid applicationTenantId) : base(applicationTenantId)
        {
        }

        public string Styles { get; private set; }

        public string MinifiedStyles { get; private set; }

        public void SetStyles(string styles)
        {
            Styles = styles;
        }

        public void SetMinifiedStyles(string minStyles)
        {
            MinifiedStyles = minStyles;

        }

    }
}
