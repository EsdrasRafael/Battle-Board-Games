using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGames.Areas.Identity.Data;
using BattleBoardGames.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace Battle_Board_Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatalhasAPIController : ControllerBase
    {
        static BatalhasAPIDAO BatalhasAPIDAO;

        public BatalhasAPIController(ModelJogosDeGuerra context, UserManager<BattleBoardGamesUser> userManager)
        {
            BatalhasAPIDAO = new BatalhasAPIDAO(context, userManager);
        }

        [HttpGet]
        [Route("QtdBatalhas")]
        public IActionResult ObterQuantidadeBatalhas()
        {
            return Ok(BatalhasAPIDAO.ObterQuantidadeBatalhas());
        }

        // GET: api/BatalhasAPI
        [Authorize]
        [HttpGet]
        public IEnumerable<Batalha> GetBatalhas(bool Finalizada = false)
        {
            IEnumerable<Batalha> batalhas;
            if (Finalizada)
            {
                batalhas = BatalhasAPIDAO.GetBatalhasFinalizadas();
            }
            else
            {
                batalhas = BatalhasAPIDAO.GetTodasBatalhas();
            }
            return batalhas;
        }

        [Authorize]
        [HttpGet]
        [Route("QtdBatalhasJogador")]
        public async Task<IActionResult> GetBatalhasJogador()
        {
            int batalhas = await BatalhasAPIDAO.GetBatalhasJogador(User.Identity.Name);
            return Ok(batalhas);
        }

        [HttpGet]
        [Authorize]
        public IActionResult EscolherNacao(Nacao nacao, int ExercitoId)
        {
            return Ok(BatalhasAPIDAO.EscolherNacao(nacao, ExercitoId));
        }

        // GET: api/BatalhasAPI?id=5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBatalha([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Batalha batalha = await BatalhasAPIDAO.GetBatalha(id);

            if (batalha == null)
            {
                return NotFound();
            }

            return Ok(batalha);
        }

        [Route("IniciarBatalha/{id}")]
        [Authorize]
        public IActionResult IniciarBatalha(int id)
        {
            Usuario usuario = BatalhasAPIDAO.BuscarUsuario(this.User);


            //Get batalha
            Batalha batalha = BatalhasAPIDAO.BuscarBatalha(id, usuario);
            if (batalha == null)
            {
                return NotFound();
            }

            if (batalha.Tabuleiro == null)
            {
                batalha.Tabuleiro = new Tabuleiro
                {
                    Altura = 8,
                    Largura = 8
                };
            }
            try
            {
                if (batalha.Estado == Batalha.EstadoBatalhaEnum.NaoIniciado)
                {
                    batalha.Tabuleiro.IniciarJogo(batalha.ExercitoBranco, batalha.ExercitoPreto);
                    Random r = new Random();
                    batalha.Turno = r.Next(100) < 50
                        ? batalha.ExercitoPreto :
                        batalha.ExercitoBranco;
                    batalha.Estado = Batalha.EstadoBatalhaEnum.Iniciado;
                }
            }
            catch (ArgumentException)
            {
                BadRequest("Não foi escolhido uma nação.");
            }
            BatalhasAPIDAO.SalvarDados();
            return Ok(batalha);
        }

        [Authorize]
        [Route("Jogar")]
        [HttpPost]
        public async Task<IActionResult> Jogar([FromBody]Movimento movimento)
        {
            movimento.Elemento = BatalhasAPIDAO.Mover(movimento);

            if (movimento.Elemento == null)
            {
                return NotFound();
            }

            movimento.Batalha = BatalhasAPIDAO.BuscarBatalhaPorMovimento(movimento);


            var usuario = BatalhasAPIDAO.BuscarUsuario(this.User);

            if (usuario.Id != movimento.AutorId)
            {
                return Forbid("O usuário autenticado não é o autor da jogada");
            }

            var batalha = movimento.Batalha;
            if (movimento.AutorId != movimento.Elemento.Exercito.UsuarioId)
            {
                //Usuário não é o dono do exercito.
                return Forbid("O jogador não é dono do exercito");
            }
            if (movimento.AutorId == batalha.Turno.UsuarioId)
            {
                if (!batalha.Jogar(movimento))
                {
                    return BadRequest("A jogada é invalida");
                }
                batalha.Turno = null;
                batalha.TurnoId = batalha.TurnoId == batalha.ExercitoBrancoId ?
                    batalha.ExercitoPretoId : batalha.ExercitoBrancoId;
                await BatalhasAPIDAO.SalvarDadosAsync();
                return Ok(batalha);
            }
            return BadRequest("Operação não realizada");

        }

        // PUT: api/BatalhasAPI/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBatalha([FromRoute] int id, [FromBody] Batalha batalha)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != batalha.Id)
            {
                return BadRequest();
            }

            BatalhasAPIDAO.AtualizarBatalha(batalha);

            try
            {
                await BatalhasAPIDAO.SalvarDadosAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (BatalhasAPIDAO.BuscarBatalhaPorID(id) == null)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BatalhasAPI
        [HttpPost]
        public async Task<IActionResult> PostBatalha([FromBody] Batalha batalha)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await BatalhasAPIDAO.SalvarBatalha(batalha);

            return CreatedAtAction("GetBatalha", new { id = batalha.Id }, batalha);
        }

        [HttpGet]
        [Route("CriarBatalha")]
        [Authorize]
        public IActionResult CriarBatalha()
        {

            Usuario usuario = BatalhasAPIDAO.BuscarUsuario(this.User);

            Batalha batalha = BatalhasAPIDAO.BuscarBatalhaUsuario(usuario);

            if (batalha == null)
            {
                batalha = new Batalha();
                BatalhasAPIDAO.AdicionarBatalha(batalha);
            }
            Exercito e = new Exercito
            {
                Usuario = usuario,
                Nacao = Nacao.Egito
            };
            if (batalha.ExercitoBrancoId == null)
            {
                batalha.ExercitoBranco = e;
            }
            else
            {
                batalha.ExercitoPreto = e;
            }
            BatalhasAPIDAO.SalvarDados();
            return Ok(batalha);
        }



        // DELETE: api/BatalhasAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatalha([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Batalha batalha = await BatalhasAPIDAO.BuscarBatalhaPorID(id);
            if (batalha == null)
            {
                return NotFound();
            }

            await BatalhasAPIDAO.RemoverBatalha(batalha);

            return Ok(batalha);
        }
    }
}