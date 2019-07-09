using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBoardGame.Model.Factory
{
    class FactoryExercitoJapones : AbstractFactoryExercito
    {
        public override Arqueiro CriarArqueiro()
        {
            return new ArqueiroJapones();
        }

        public override Cavaleiro CriarCavalaria()
        {
            return new CavaleiroJapones();
        }

        public override Guerreiro CriarGuerreiro()
        {
            return new GuerreiroJapones();
        }
    }
}
