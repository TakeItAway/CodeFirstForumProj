using System;
using System.Linq;
using System.Threading.Tasks;
using CommonMark;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeFirstForum.TagHelpers
{
    [HtmlTargetElement("markdown", Attributes = "markdown")]
    public class MarkdownTagHelper : TagHelper
    {

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var childContent = await output.GetChildContentAsync();
            var lines = childContent.GetContent()
                .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim());
            var content = string.Join(" ", lines);
            var transformedContent = CommonMarkConverter.Convert(content);

            output.Content.SetHtmlContent(transformedContent);

            output.Attributes.RemoveAll("markdown");
        }
    }
}
