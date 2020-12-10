using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace FullStack.Svc.Http.Tests.Models
{
    public interface ITextQuery
    {
        string Text { get; set; }
    }

    public interface IHitsResult
    {
        public IList<string> Hits { get; set; }
    }

    public class TextQuery : ITextQuery
    {
        public string Text { get; set; }
    }

    public class TextPatternQuery : ITextQuery
    {
        [Required]
        [RegularExpression(".{6,}")]
        public string Text { get; set; }
    }

    public class HitsResult : IHitsResult
    {
        public IList<string> Hits { get; set; }
    }

    public class MockHttpResponse
    {
        public string Body { get; set; }

        public HttpStatusCode Status { get; set; }
    }
}
