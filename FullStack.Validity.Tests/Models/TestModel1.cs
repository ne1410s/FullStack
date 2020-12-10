using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FullStack.Validity.Tests.Models
{
    public class TestModel1
    {
        [Required]
        public string MyRequiredString { get; set; }

        [Range(20, 100)]
        public int MyInt32 { get; set; }

        [RegularExpression(@"\d{6}")]
        public string MyPatternString { get; set; } = "hi";
    }

    public class TestModel2
    {
        [Required]
        public System.Text.NormalizationForm? MyNullEnum { get; set; }

        public TestModel1 TestSubModel => new TestModel1
        {
            MyInt32 = 22,
            MyPatternString = null,
        };
    }

    public class TestModel3 : TestModel1
    {
        [Required]
        [MinLength(2)]
        public IEnumerable<string> MyItems { get; set; }

        public TestModel1[] MyModelArray { get; set; } = new[]
        {
            new TestModel1 { MyRequiredString = "hi" }, 
            new TestModel1 { MyPatternString = "fail" },
        };
    }
}
