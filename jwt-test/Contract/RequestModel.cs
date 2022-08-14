using System.ComponentModel.DataAnnotations;

namespace jwt_test.Contract
{
    public class RequestModel
    {
        [Required] public int SomeValue { get; set; }

        public string? SomeString { get; set; }
    }
}