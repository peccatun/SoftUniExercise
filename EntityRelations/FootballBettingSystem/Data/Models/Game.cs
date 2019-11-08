using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace P03_FootballBetting.Data.Models
{
    public class Game
    {
        public int GameId { get; set; }

        [Required]
        public double AwayTeamBetRate { get; set; }

        [Required]
        public int AwayTeamGoals { get; set; }

        public int AwayTeamId { get; set; }

        public Team AwayTeam { get; set; }

        public double DrawBetRate { get; set; }

        [Required]
        public double HomeTeamBetRate { get; set; }

        [Required]
        public int HomeTeamGoals { get; set; }

        public int HomeTeamId { get; set; }

        public Team HomeTeam { get; set; }

        public string Result { get; set; }

        public DateTime DateTime { get; set; }

        public ICollection<Bet> Bets { get; set; } = new HashSet<Bet>();

        public ICollection<PlayerStatistic> PlayerStatistics { get; set; }
            = new HashSet<PlayerStatistic>();


    }
}
