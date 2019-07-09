using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBoardGame.Model.Factory
{
    class FactoryExercitoIndiano : AbstractFactoryExercito
    {
        public override Arqueiro CriarArqueiro()
        {
            return new ArqueiroIndiano();
        }

        public override Cavaleiro CriarCavalaria()
        {
            return new CavaleiroIndiano();
        }

        public override Guerreiro CriarGuerreiro()
        {
            return new GuerreiroIndiano();
        }
    }
}
