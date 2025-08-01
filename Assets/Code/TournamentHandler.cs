using System.Collections.Generic;

namespace Code
{
	public class TournamentHandler
	{
		private List<TournamentUser> _tournamentUsers = new();

		public TournamentHandler()
		{
			CreateTournamentUsers();
		}
		
		private void CreateTournamentUsers()
		{
			var amount = 100;

			for (int i = 0; i < amount; i++)
			{
				var user = new TournamentUser
				{
					Name = $"User {i}",
					Rank = i,
					Score = i * 100
				};
				
				_tournamentUsers.Add(user);
			}
		}

		public List<TournamentUser> GetTournamentUsers() => _tournamentUsers;
	}
}