using System.ComponentModel.DataAnnotations;

namespace SwizlyPeasy.Common.Dtos
{
    public class UserDto
    {
        public string? Sub { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
