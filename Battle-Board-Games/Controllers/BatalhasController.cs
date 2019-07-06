using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BattleBoardGame.Model;
using BattleBoardGame.Model.DAL;
using BattleBoardGames.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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