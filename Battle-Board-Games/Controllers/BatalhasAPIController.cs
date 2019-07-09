using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGames.Areas.Identity.Data;
using BattleBoardGames.DAL;
using BattleBoardGames.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace Battle_Board_Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatalhasAPIController : ControllerBase
    {
        BancoDAO BancoDAO;
        UsuarioService _usuarioService;

        public BatalhasAPIController(ModelJogosDeGuerra context, UserManager<BattleBoardGamesUser> userManager)
        {
            BancoDAO = new BancoDAO(context);
            _usuarioService = new UsuarioService(context, userManager);
        }

        [HttpGet]
        [Route("QtdBatalhas")]
        public async Task<IActionResult> ObterQuantidadeBatalhas()
        {
            return Ok(await BancoDAO.RetornarQuantidadeBatalhas());
        }

        // GET: api/BatalhasAPI
        [Authorize]
        [HttpGet]
        public IEnumerable<Batalha> GetBatalhas(bool Finalizada = false)
        {
            IEnumerable<Batalha> batalhas;
            if (Finalizada)
            {
                batalhas = BancoDAO.RetornarBatalhasFinalizadas();
            }
            else
            {
                batalhas = BancoDAO.RetornarTodasBatalhas();
            }
            return batalhas;
        }

        [Authorize]
        [HttpGet]
        [Route("QtdBatalhasJogador")]
        public async Task<IActionResult> GetBatalhasJogador()
        {
            int batalhas = BancoDAO.RetornarBatalhasJogador(User.Identity.Name);
            return Ok(batalhas);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EscolherNacao(Nacao nacao, int ExercitoId)
        {

            Exercito exercito = BancoDAO.BuscarExercitoPorID(ExercitoId);
            exercito.Nacao = nacao;

            await BancoDAO.AlterarDadosAsync();

            return Ok(exercito);
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

            Batalha batalha = await BancoDAO.BatalhaPorID(id);

            if (batalha == null)
            {
                return NotFound();
            }

            return Ok(batalha);
        }

        [Route("IniciarBatalha/{id}")]
        [Authorize]
        public async Task<IActionResult> IniciarBatalha(int id)
        {
            Usuario usuario = _usuarioService.ObterUsuarioEmail(this.User);


            //Get batalha
            Batalha batalha = BancoDAO.RetornarBatalhaPorUsuario(usuario, id);

            if (batalha == null)
            {
                return NotFound();
            }

            if (batalha.Tabuleiro == null)
            {
                batalha.Tabuleiro = new Tabuleiro();
                batalha.Tabuleiro.Altura = 8;
                batalha.Tabuleiro.Largura = 8;
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
            catch (ArgumentException arg)
            {
                BadRequest("Não foi escolhido uma nação.");
            }

            BancoDAO.AlterarDados();

            return Ok(batalha);
        }

        [Authorize]
        [Route("Jogar")]
        [HttpPost]
        public async Task<IActionResult> Jogar([FromBody]Movimento movimento)
        {
            movimento.Elemento = BancoDAO.BuscarElementoPorId(movimento.ElementoId);

            if (movimento.Elemento == null)
            {
                return NotFound();
            }

            movimento.Batalha = BancoDAO.RetornarBatalhaPorId(movimento.BatalhaId);

            var usuario = this._usuarioService.ObterUsuarioEmail(this.User);


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
                await BancoDAO.AlterarDadosAsync();
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

            BancoDAO.AlterarBatalha(batalha);


            try
            {
                await BancoDAO.AlterarDadosAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (BancoDAO.BuscarBatalha(id) == null)
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

            BancoDAO.AdicionarBatalha(batalha);
            await BancoDAO.AlterarDadosAsync();

            return CreatedAtAction("GetBatalha", new { id = batalha.Id }, batalha);
        }

        [HttpGet]
        [Route("CriarBatalha/{idNacao}")]
        //[Route("CriarBatalha")]
        [Authorize]
        public async Task<IActionResult> CriarBatalha(int idNacao)
        //public async Task<IActionResult> CriarBatalha()
        {
            Usuario usuario = _usuarioService.ObterUsuarioEmail(this.User);

            Batalha batalha = BancoDAO.BuscarBatalhaPendente(usuario);

            if (batalha == null)
            {
                batalha = new Batalha();
                BancoDAO.AdicionarBatalha(batalha);
            }

            Exercito e = new Exercito();
            e.Usuario = usuario;

            switch (idNacao)
            {
                case 1:
                    e.Nacao = Nacao.India;
                    break;
                case 2:
                    e.Nacao = Nacao.Persia;
                    break;
                case 3:
                    e.Nacao = Nacao.Egito;
                    break;
                case 4:
                    e.Nacao = Nacao.Japones;
                    break;
                default:
                    break;
            }


            if(batalha.ExercitoBrancoId == null)
            {
                batalha.ExercitoBranco = e;
            }
            else if (batalha.ExercitoPretoId == null)
            {
                batalha.ExercitoPreto = e;
            }
            BancoDAO.AlterarDados();
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

            Batalha batalha = await BancoDAO.BuscarBatalha(id);

            if (batalha == null)
            {
                return NotFound();
            }

            BancoDAO.RemoverBatalha(batalha);

            await BancoDAO.AlterarDadosAsync();

            return Ok(batalha);
        }

    }
}