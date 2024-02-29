using Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

/// <summary>
/// Bet domain entity.
/// </summary>
public class Bet : BaseEntity
{
    #region constructors and destructors

    /// <summary>
    /// Default constructor.
    /// </summary>
    public Bet(long userId, int number, long points)
    {
        this.UserId = userId;
        this.Number = number;
        this.Points = points;
        this.Status = (int)BetStatus.UNDECIDED;
    }

    #endregion

    #region properties

    /// <summary>
    /// The user id.
    /// </summary>
    [Required]
    public long UserId { get; set; } = default!;

    /// <summary>
    /// The number.
    /// </summary>
    [Required]
    public int Number { get; set; } = default!;

    /// <summary>
    /// The points.
    /// </summary>
    [Required]
    public long Points { get; set; } = default!;

    /// <summary>
    /// The status.
    /// </summary>
    [Required]
    public int Status { get; set; } = default!;

    #endregion

    public enum BetStatus
    {
        UNDECIDED = -1,
        LOST = 0,
        WON = 1
    }
}
