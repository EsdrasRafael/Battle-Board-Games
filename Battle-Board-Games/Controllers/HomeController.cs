using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BattleBoardGame.Models;
using Microsoft.AspNetCore.Authorization;
using BattleBoardGame.Model;
using BattleBoardGames.DAL;

namespace BattleBoardGame.Controllers
{
    public class HomeController : Controller
    {


        public IActionResult About()
        {
            ViewData["Message"] = "Aplicação desenvolvida para o curso de Análise" +
            	" e Desenvolvimento de Sistemas da Universidade Positivo.";

            return View();
        }

        [Authorize]
        public IActionResult Contact()
        {
            ViewData["Message"] = "Contatos do Professor.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            bool usuarioAutenticado = true;
/*                Utils.Utils.ObterUsuarioLogado(
                    ctx
                    ) != null;
*/
            if (!usuarioAutenticado)
            {
                return RedirectToAction("Login");
            }

            return View();
        }




        public ActionResult Tabuleiro(int BatalhaId = -1)
        {
            ViewBag.Title = "Tabuleiro";
            Batalha batalha = HomeDAO.BuscarBatalha(BatalhaId);
            return (batalha != null) ? View(batalha) : View();
        }

        public ActionResult Login(string usuario, string password, string rememberme, string returnurl)
        {
            return View();
        }
    }
}
