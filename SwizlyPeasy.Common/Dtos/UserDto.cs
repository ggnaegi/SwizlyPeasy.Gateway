using System.ComponentModel.DataAnnotations;

namespace SwizlyPeasy.Common.Dtos
{
    /// <summary>
    /// Data about the authenticated user.
    /// </summary>
    public class UserDto
    {
        public string? Sub { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
