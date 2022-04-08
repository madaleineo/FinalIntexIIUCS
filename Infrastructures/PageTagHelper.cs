using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using UtahCarSafety.Models.ViewModels;
using System.Collections.Generic;
namespace UtahCarSafety.Infrastructures
{
    [HtmlTargetElement("div", Attributes = "page-blah")]
    public class PageTagHelper : TagHelper
    {
        // Dynamically create page links for us
        private IUrlHelperFactory uhf;
        public PageTagHelper(IUrlHelperFactory temp)
        {
            uhf = temp;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext vc { get; set; }
        public PageInfo PageBlah { get; set; }
        public string PageAction { get; set; }
        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();
        public string PageClass { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }
        public override void Process(TagHelperContext thc, TagHelperOutput tho)
        {
            IUrlHelper uh = uhf.GetUrlHelper(vc);
            TagBuilder final = new TagBuilder("div");
            int current = PageBlah.CurrentPage;
            int total = PageBlah.TotalPages;
            int limit = 0;
            int bottom = 0;
            if (current < 10 && total > 25)
            {
                limit = 25;
                bottom = 1;
            }
            else if (current < 10 && total <= 25)
            {
                limit = total;
                bottom = 1;
            }
            else if (current >= 10 && total <= 25)
            {
                limit = total;
                bottom = 1;
            }
            else if (current >= 10 && total > 25)
            {
                if (total - current < 23)
                {
                    limit = total;
                    bottom = limit - 24;
                }
                else
                {
                    bottom = current - 2;
                    limit = current + 22;
                }
            }
            for (int i = bottom; i <= limit; i++)
            {
                TagBuilder tb = new TagBuilder("a");
                PageUrlValues["pageNum"] = i;
                tb.Attributes["href"] = uh.Action(PageAction, PageUrlValues);
                if (PageClassesEnabled)
                {
                    tb.AddCssClass(PageClass);
                    tb.AddCssClass(i == PageBlah.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                tb.InnerHtml.Append(i.ToString());
                final.InnerHtml.AppendHtml(tb);
            }
            tho.Content.AppendHtml(final.InnerHtml);
        }
    }
}