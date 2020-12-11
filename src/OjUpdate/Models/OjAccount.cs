using SatelliteSite.OjUpdateModule.Entities;
using System;

namespace SatelliteSite.OjUpdateModule.Models
{
    /// <summary>
    /// The account model for result display.
    /// </summary>
    public class OjAccount : IComparable<OjAccount>
    {
        /// <summary>
        /// The grade of student
        /// </summary>
        public int Grade { get; set; }
        
        /// <summary>
        /// The account name
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// The display name
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// The solved count
        /// </summary>
        public int? Solved { get; set; }

        /// <summary>
        /// Instantiate an <see cref="OjAccount"/> via <see cref="SolveRecord"/>.
        /// </summary>
        /// <param name="p">The solve record.</param>
        public OjAccount(SolveRecord p)
        {
            Account = p.Account;
            NickName = p.NickName;
            Solved = p.Result;
            Grade = p.Grade;
        }

        /// <inheritdoc />
        public int CompareTo(OjAccount other)
        {
            if (other == null) return 1;
            if (Solved == other.Solved)
                return -Grade.CompareTo(other.Grade);
            if (Solved.HasValue && other.Solved.HasValue)
                return -Solved.Value.CompareTo(other.Solved.Value);
            if (!Solved.HasValue && !other.Solved.HasValue)
                return 0;
            return Solved.HasValue ? -1 : 1;
        }
    }
}
