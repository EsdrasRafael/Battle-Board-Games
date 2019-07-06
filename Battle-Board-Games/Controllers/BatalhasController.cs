using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGames.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Battle_Board_Games.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class BatalhasController : Controller
    {

        private readonly BatalhasDAO BatalhasDAO;
        public BatalhasController(ModelJogosDeGuerra context)
        {
            BatalhasDAO = new BatalhasDAO(context);
        }

        [Route("Lobby/{batalhaId}")]
        [HttpGet()]
        public ActionResult Lobby(int batalhaId)
        {
            Batalha batalha = BatalhasDAO.BuscarLobby(batalhaId);
            ViewBag.Id = batalha.Id;
            return View(batalha);
        }

        [Route("Tabuleiro/{batalhaId}")]
        [HttpGet()]
        public ActionResult Tabuleiro(int batalhaId)
        {
            Batalha batalha = BatalhasDAO.BuscarBatalha(batalhaId);
            ViewBag.Id = batalha.Id;
            return View(batalha);
        }
    }
}