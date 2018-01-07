using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace CodeFirstForum.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("mark-textarea", Attributes = "mark-textarea")]
    public class TextAreaTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";

        /// <summary>
        /// Creates a new <see cref="TextAreaTagHelper"/>.
        /// </summary>
        /// <param name="generator">The <see cref="IHtmlGenerator"/>.</param>
        public TextAreaTagHelper(IHtmlGenerator generator)
        {
            Generator = generator;
        }

        /// <inheritdoc />
        public override int Order => -1000;

        protected IHtmlGenerator Generator { get; }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        /// <summary>
        /// An expression to be evaluated against the current model.
        /// </summary>
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }

        /// <inheritdoc />
        /// <remarks>Does nothing if <see cref="For"/> is <c>null</c>.</remarks>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var tagBuilder = Generator.GenerateTextArea(
                ViewContext,
                For.ModelExplorer,
                For.Name,
                rows: 0,
                columns: 0,
                htmlAttributes: null);

            if (tagBuilder != null)
            {
                output.MergeAttributes(tagBuilder);
                if (tagBuilder.HasInnerHtml)
                {
                    // Overwrite current Content to ensure expression result round-trips correctly.
                    output.Content.SetHtmlContent(tagBuilder.InnerHtml);
                }
            }
        }
    }
}
