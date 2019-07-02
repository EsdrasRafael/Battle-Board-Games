using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGames.Areas.Identity.Data;
using BattleBoardGames.Controllers;
using BattleBoardGames.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ModelBattleBoardGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace Battle_Board_Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BatalhasAPIController : ControllerBase
    {
        private readonly IBatalha _batalha;

        UsuarioService _usuarioService;
        UserManager<BattleBoardGamesUser> _userManager;

        public BatalhasAPIController(IBatalha batalha, UserManager<BattleBoardGamesUser> userManager, ModelJogosDeGuerra context)
        {
            _batalha = batalha;
            _userManager = userManager;
            _usuarioService = new UsuarioService(context, _userManager);
        }

        [HttpGet("QtdBatalhas")]
        public async Task<IActionResult> ObterQuantidadeBatalhas()
            => Ok(await _batalha.BuscarQuantidadeAsync());

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Batalha>> GetBatalhas(bool Finalizada = false)
            => await _batalha.BuscarAsync(Finalizada);

        [HttpGet("QtdBatalhasJogador")]
        [Authorize]
        public async Task<IActionResult> GetBatalhasJogador()
        {
            var usuario = _usuarioService.ObterUsuarioEmail(User);
            return Ok(await _batalha.ContarPorJogador(usuario.Id));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EscolherNacao(Nacao nacao, int exercitoId)
            => Ok(await _batalha.EscolherNacaoAsync(nacao, exercitoId));

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBatalha(int id)
        {
            try
            {
                var batalha = await _batalha.BuscarAsync(id);

                if (batalha is null)
                    return NotFound();

                return Ok(batalha);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("IniciarBatalha/{id}")]
        [Authorize]
        public async Task<IActionResult> IniciarBatalha(int id, IniciarBatalha iniciarBatalhaRequest)
        {
            try
            {
                var usuario = _usuarioService.ObterUsuarioEmail(User);

                var batalha = await _batalha.IniciarAsync(
                    new IniciarBatalhaParams
                    {
                        Id = id,
                        Jogador = usuario.Id,
                        NacaoExercitoBranco = iniciarBatalhaRequest.NacaoExercitoBranco,
                        NacaoExercitoPreto = iniciarBatalhaRequest.NacaoExercitoPreto

                    });

                return Ok(batalha);
            }
            catch (BatalhasExeception exception)
            {
                return BadRequest(exception.Message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

        }

        [HttpPost("Jogar")]
        [Authorize]
        public async Task<IActionResult> Jogar([FromBody]Movimento movimento)
        {
            try
            {
                var usuario = _usuarioService.ObterUsuarioEmail(User);
                return Ok(await _batalha.JogarAsync(movimento, usuario.Id));
            }
            catch (BatalhasExeception exception)
            {
                return BadRequest(exception.Message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBatalha([FromRoute] int id, [FromBody] Batalha batalha)
        {
            try
            {
                return Ok(await _batalha.AtualizarAsync(id, batalha));
            }
            catch (BatalhasExeception exception)
            {
                return BadRequest(exception.Message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostBatalha([FromBody] Batalha batalha)
        {
            try
            {
                batalha = await _batalha.EnviarAsync(batalha);
                return CreatedAtAction
                    (
                        actionName: nameof(GetBatalha),
                        routeValues: new { id = batalha.Id },
                        value: batalha
                    );
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [Route("CriarBatalha")]
        [Authorize]
        public async Task<IActionResult> CriarBatalha()
        {
            try
            {
                var usuario = _usuarioService.ObterUsuarioEmail(User);
                var batalha = await _batalha.CriarAsync(usuario);
                return Ok(batalha);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBatalha(int id)
        {
            try
            {
                return Ok(await _batalha.DeletarAsync(id));
            }
            catch (BatalhasExeception exception)
            {
                return BadRequest(exception.Message);
            }
            catch
            {
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}
