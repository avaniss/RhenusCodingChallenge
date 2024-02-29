using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;
public class User : BaseEntity
{
    #region constructors and destructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public User(string name, string username, string password, long balance)
    {
        this.Name = name;
        this.Username = username;
        this.Password = password;
        this.Balance = balance;
    }

    #endregion

    #region properties

    /// <summary>
    /// The username.
    /// </summary>
    [Key]
    public string Username { get; set; } = default!;

    /// <summary>
    /// The password.
    /// </summary>
    [Required]
    public string Password { get; set; } = default!;

    /// <summary>
    /// The Name.
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;

    /// <summary>
    /// The Balance.
    /// </summary>
    [Required]
    public long Balance { get; set; } = default!;

    #endregion
}
