using Ganss.XSS;
using Markdig;

namespace AlterEgo.Helpers
{
    public static class MarkdownHelper
    {
        public static string Transform(string text)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var transformedText = Markdown.ToHtml(text, pipeline);

            var sanitizer = new HtmlSanitizer();
            var sanitizedText = sanitizer.Sanitize(transformedText);

            return sanitizedText;
        }
    }
}
