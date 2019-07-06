using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BattleBoardGames.DAL
{
    public class BatalhasDAO
    {
        private static ModelJogosDeGuerra _context;
        public BatalhasDAO(ModelJogosDeGuerra context)
        {
            _context = context;
        }

        public static Batalha BuscarLobby(int batalhaId)
        {
            return _context.Batalhas
                .Where(x => x.Id.Equals(batalhaId))
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoPreto.Usuario)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario)
                .Include(b => b.Vencedor)
                .Include(b => b.Vencedor.Usuario)
                .FirstOrDefault();
        }

        public Batalha BuscarBatalha(int batalhaId)
        {
            return _context.Batalhas
                .Where(x => x.Id.Equals(batalhaId))
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoBranco.Elementos)
                .Include(b => b.ExercitoBranco.ElementosVivos)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoPreto.Elementos)
                .Include(b => b.ExercitoPreto.ElementosVivos)
                .Include(b => b.ExercitoPreto.Usuario)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario)
                .Include(b => b.Vencedor)
                .Include(b => b.Vencedor.Usuario)
                .FirstOrDefault();
        }
    }
}
