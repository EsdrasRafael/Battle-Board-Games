using System.Linq;
using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;

namespace BattleBoardGames.DAL
{
    public class HomeDAO
    {
        private static ModelJogosDeGuerra _context;

        public HomeDAO(ModelJogosDeGuerra context)
        {
            _context = context;
        }
        public static Batalha BuscarBatalha(int batalhaId)
        {
            return _context.Batalhas.Where(b => b.Id == batalhaId).FirstOrDefault();
        }
    }
}
