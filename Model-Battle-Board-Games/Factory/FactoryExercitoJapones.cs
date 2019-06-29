using BattleBoardGame.Model;
using BattleBoardGame.Model.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleBoardGame.Model.Factory
{
    class FactoryExercitoJapones : AbstractFactoryExercito
    {
        public override Arqueiro CriarArqueiro()
        {
            throw new NotImplementedException();
        }

        public override Cavaleiro CriarCavalaria()
        {
            throw new NotImplementedException();
        }

        public override Guerreiro CriarGuerreiro()
        {
            throw new NotImplementedException();
        }
    }
}
