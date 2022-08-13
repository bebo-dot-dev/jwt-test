using System.ComponentModel.DataAnnotations;

namespace jwt_test
{
    public class RequestModel
    {
        [Required]
        public int SomeValue { get; set; }
        
        public string? SomeString { get; set; }
    }
}