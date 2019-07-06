using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGames.Areas.Identity.Data;
using BattleBoardGames.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace BattleBoardGames.DAL
{
    public class BatalhasAPIDAO
    {
        private static ModelJogosDeGuerra _context;
        readonly UserManager<BattleBoardGamesUser> _userManager;
        UsuarioService _usuarioService;

        public BatalhasAPIDAO(ModelJogosDeGuerra context, UserManager<BattleBoardGamesUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _usuarioService = new UsuarioService(_context, _userManager);
        }

        public int ObterQuantidadeBatalhas()
        {
            return (_context.Batalhas.Count());
        }

        public IEnumerable<Batalha> GetBatalhasFinalizadas()
        {
            return _context.Batalhas.Where(b => b.Vencedor != null).ToList();
        }

        public IEnumerable<Batalha> GetTodasBatalhas()
        {
            return _context.Batalhas.ToList();
        }

        public async Task<int> GetBatalhasJogador(string name)
        {
            return await _context.Batalhas.Where(b => (b.ExercitoBranco != null
                   && b.ExercitoBranco.UsuarioId == name)
                   || (b.ExercitoPreto != null && b.ExercitoPreto.UsuarioId == name)).CountAsync();
        }

        public async Task<object> EscolherNacao(Nacao nacao, int ExercitoId)
        {
            Exercito exercito = _context.Exercitos.Where(e => e.Id == ExercitoId).FirstOrDefault();
            exercito.Nacao = nacao;
            await SalvarDadosAsync();
            return exercito;
        }

        public async Task<Batalha> GetBatalha(int id)
        {
            return await _context.Batalhas.Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoPreto.Usuario)
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Tabuleiro.ElementosDoExercito)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario).Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public Usuario BuscarUsuario(ClaimsPrincipal name)
        {
            return _usuarioService.ObterUsuarioEmail(name);
        }

        public Batalha BuscarBatalha(int id, Usuario usuario)
        {
            return _context.Batalhas
                .Include(b => b.ExercitoPreto)
                .Include(b => b.ExercitoBranco)
                .Include(b => b.Tabuleiro)
                .Include(b => b.Turno)
                .Include(b => b.Turno.Usuario)
                .Where(b =>
                (b.ExercitoBranco.Usuario.Id == usuario.Id
                || b.ExercitoPreto.Usuario.Id == usuario.Id)
                && (b.ExercitoBranco != null && b.ExercitoPreto != null)
                && b.Id == id).FirstOrDefault();
        }

        public void SalvarDados()
        {
            _context.SaveChanges();
        }

        public async Task SalvarDadosAsync()
        {
            await _context.SaveChangesAsync();
        }

        public ElementoDoExercito Mover(Movimento movimento)
        {
            return _context.ElementosDoExercitos.Include(el => el.Exercito).FirstOrDefault(el => el.Id == movimento.ElementoId);
        }

        public Batalha BuscarBatalhaPorMovimento(Movimento movimento)
        {
            return _context.Batalhas
                .Include(b => b.Tabuleiro)
                .Include(b => b.Tabuleiro.ElementosDoExercito)
                .Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoPreto)
                .Include(b => b.Turno)
                .Include(b => b.Vencedor)
                .Include(b => b.ExercitoBranco.Usuario)
                .Include(b => b.ExercitoPreto.Usuario)
                .FirstOrDefault(
                    m => m.Id == movimento.BatalhaId);
        }

        public async Task<Batalha> BuscarBatalhaPorID(int id)
        {
            return await _context.Batalhas.FindAsync(id);
        }

        public async Task RemoverBatalha(Batalha batalha)
        {
            _context.Batalhas.Remove(batalha);
            await SalvarDadosAsync();
        }

        public void AtualizarBatalha(Batalha batalha)
        {
            _context.Entry(batalha).State = EntityState.Modified;
        }

        public async Task SalvarBatalha(Batalha batalha)
        {
            _context.Batalhas.Add(batalha);
            await SalvarDadosAsync();
        }

        public Batalha BuscarBatalhaUsuario(Usuario usuario)
        {
            return _context.Batalhas.Include(b => b.ExercitoBranco)
                .Include(b => b.ExercitoPreto)
                .FirstOrDefault(b =>
            (b.ExercitoBrancoId == null
            || b.ExercitoPretoId == null) &&
            (b.ExercitoBranco.UsuarioId != usuario.Id
            && b.ExercitoPreto.UsuarioId != usuario.Id));
        }

        public void AdicionarBatalha(Batalha batalha)
        {
            _context.Add(batalha);
        }
    }
}