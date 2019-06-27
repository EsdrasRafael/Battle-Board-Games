using BattleBoardGame.Model;
using BattleBoardGame.Model.Factory;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModelBattleBoardGames.Factory
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
