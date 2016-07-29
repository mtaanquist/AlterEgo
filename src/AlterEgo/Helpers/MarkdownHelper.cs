using Markdig;

namespace AlterEgo.Helpers
{
    public static class MarkdownHelper
    {
        public static string Transform(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var pipeline = new MarkdownPipelineBuilder()
                .DisableHtml()
                .UseMediaLinks()
                .UseAdvancedExtensions()
                .Build();
            
            var transformedText = Markdown.ToHtml(text, pipeline);

            return transformedText;
        }
    }
}
