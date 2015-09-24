
using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using RedRat.RaceTiming.Core.ViewModels;

namespace RedRat.RaceTiming.Core.Util
{
    public class TeamCalc
    {
        public const int TEAM_SIZE = 4;

        public static IList<TeamResult> GetTeamResults( IList<Finisher> finishers )
        {
            // ==== TEAMS ====
            var teamResults = new List<TeamResult>();
            var allTeamMembers = new List<Finisher>();  // Tracks finishers in a team, so can't count as part of a club team.

            var teams = GetTeamNames( finishers );
            foreach ( var team in teams )
            {
                var teamFinishers = GetFinishersInTeam( finishers, team );
                allTeamMembers.AddRange( teamFinishers );
                var teamResult = new TeamResult
                {
                    Name = team,
                    // Get the TEAM_SIZE team members with lowest score.
                    Members = teamFinishers.OrderBy( f => f.Position ).Take( TEAM_SIZE ).ToList(),
                };
                teamResult.CalcScore();
                teamResults.Add( teamResult );
            }

            // ==== CLUBS ====
            var clubTeamFinishers = finishers.Where(f => !allTeamMembers.Contains(f)).ToList();
            var clubs = GetClubTeamNames( clubTeamFinishers );
            foreach ( var club in clubs )
            {
                var clubFinishers = GetFinishersInClub(clubTeamFinishers, club);
                var clubTeamResult = new TeamResult
                {
                    Name = club,
                    // Get the TEAM_SIZE team members with lowest score.
                    Members = clubFinishers.OrderBy(f => f.Position).Take(TEAM_SIZE).ToList(),
                };
                clubTeamResult.CalcScore();
                teamResults.Add( clubTeamResult );
            }

            return teamResults;
        }

        /// <summary>
        /// Gets team names for teams with TEAM_SIZE or more finishers.
        /// </summary>
        public static List<string> GetTeamNames( IList<Finisher> finishers )
        {
            // Get list of distinct team names
            var allTeams = finishers
                .Where(f => !string.IsNullOrEmpty(f.Team))
                .Select(f => f.Team.Trim().ToUpper())
                .Distinct()
                .ToList();

            var teamList = allTeams.Where( t => GetFinishersInTeam(finishers, t).Count() >= TEAM_SIZE );

            return teamList.ToList();
        }

        /// <summary>
        /// Gets club names for teams with TEAM_SIZE or more finishers.
        /// </summary>
        public static List<string> GetClubTeamNames(IList<Finisher> finishers)
        {
            var allClubs = finishers
                .Where( f => !string.IsNullOrEmpty( f.Club ) )
                .Select( f => f.Club.Trim().ToUpper() )
                .Distinct()
                .ToList();

            var clubList = allClubs.Where( c => GetFinishersInClub(finishers, c).Count() >= TEAM_SIZE );

            return clubList.ToList();
        }

        public static List<Finisher> GetFinishersInTeam( IList<Finisher> finishers, string team )
        {
            return
                finishers
                    .Where(
                        f => !string.IsNullOrEmpty( f.Team ) && f.Team.Trim().Equals( team, StringComparison.InvariantCultureIgnoreCase ) )
                    .ToList();
        }

        public static List<Finisher> GetFinishersInClub(IList<Finisher> finishers, string club)
        {
            return
                finishers
                    .Where(
                        f => !string.IsNullOrEmpty(f.Club) && f.Club.Trim().Equals(club, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();
        }
    }
}
