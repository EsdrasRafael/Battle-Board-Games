using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBoardGame.Model.Factory
{
    class FactoryExercitoEgipcio : AbstractFactoryExercito
    {
        public override Arqueiro CriarArqueiro()
        {
            return new ArqueiroEgipcio();
        }

        public override Cavaleiro CriarCavalaria()
        {
            return new CavaleiroEgipcio();
        }

        public override Guerreiro CriarGuerreiro()
        {
            return new GuerreiroEgipcio();
        }
    }
}
