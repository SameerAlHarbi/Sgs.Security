﻿using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Security.Idp.Extensions
{
    public abstract class ActiveTagHelper : TagHelper
    {
        protected string tagHelperName= "";

        private IDictionary<string, string> _routeValues;

        /// <summary>The name of the action method.</summary>
        /// <remarks>Must be <c>null</c> if <see cref="P:Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper.Route" /> is non-<c>null</c>.</remarks>
        [HtmlAttributeName("asp-action")]
        public string Action { get; set; }

        /// <summary>The name of the controller.</summary>
        /// <remarks>Must be <c>null</c> if <see cref="P:Microsoft.AspNetCore.Mvc.TagHelpers.AnchorTagHelper.Route" /> is non-<c>null</c>.</remarks>
        [HtmlAttributeName("asp-controller")]
        public string Controller { get; set; }

        /// <summary>Additional parameters for the route.</summary>
        [HtmlAttributeName("asp-all-route-data", DictionaryAttributePrefix = "asp-route-")]
        public IDictionary<string, string> RouteValues
        {
            get
            {
                if (this._routeValues == null)
                    this._routeValues = (IDictionary<string, string>)new Dictionary<string, string>((IEqualityComparer<string>)StringComparer.OrdinalIgnoreCase);
                return this._routeValues;
            }
            set
            {
                this._routeValues = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.ViewContext" /> for the current request.
        /// </summary>
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            if (ShouldBeActive())
            {
                MakeActive(output);
            }

            output.Attributes.RemoveAll(tagHelperName);
        }

        protected abstract bool ShouldBeActive();

        private void MakeActive(TagHelperOutput output)
        {
            var classAttr = output.Attributes.FirstOrDefault(a => a.Name == "class");
            if (classAttr == null)
            {
                classAttr = new TagHelperAttribute("class", "active");
                output.Attributes.Add(classAttr);
            }
            else if (classAttr.Value == null || classAttr.Value.ToString().IndexOf("active") < 0)
            {
                output.Attributes.SetAttribute("class", classAttr.Value == null
                    ? "active"
                    : classAttr.Value.ToString() + " active");
            }
        }
    }
}
