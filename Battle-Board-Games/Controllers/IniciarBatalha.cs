using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BattleBoardGame.Model.Factory.AbstractFactoryExercito;

namespace BattleBoardGames.Controllers
{
    public class IniciarBatalha
    {
        public Nacao NacaoExercitoBranco { get; set; }
        public Nacao NacaoExercitoPreto { get; set; }
    }
}
