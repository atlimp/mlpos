using System.Formats.Asn1;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace MLPos.Web.Utils;

public static class HtmlHelperExtension
{
    public static IHtmlContent ImageOrDefault(this IHtmlHelper htmlHelper, string? url, string @class = "")
    {
        string photoUrl = url;
        if (string.IsNullOrEmpty(url))
        {
            photoUrl = Constants.DEFAULT_IMAGE_PATH;
        }
        
        string img = $"<img src=\"{photoUrl}\" class=\"{@class}\" />";

        IHtmlContentBuilder builder = new HtmlContentBuilder();
        builder.AppendHtml(img);

        return builder;
    }
}