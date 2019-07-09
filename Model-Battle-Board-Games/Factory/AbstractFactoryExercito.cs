using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleBoardGame.Model.Factory
{
    public abstract class AbstractFactoryExercito
    {
        public abstract Arqueiro CriarArqueiro();

        public abstract Cavaleiro CriarCavalaria();

        public abstract Guerreiro CriarGuerreiro();

        public enum Nacao { India=1, Persia=2, Egito=3, Japones=4};

        /// <summary>
        /// Este método é uma factory para a Abstract Factory.
        /// Deste modo, não existirá dependência do sistema com as 
        /// Factories concretas.
        /// </summary>
        /// <param name="nacionalidade"></param>
        /// <returns></returns>
        public static AbstractFactoryExercito CriarFactoryExercito(Nacao nacionalidade)
        {
            AbstractFactoryExercito factory = null;
            if (nacionalidade == Nacao.Persia)
            {
                factory = new Factory.FactoryExercitoPersa();
            }
            else if (Nacao.Egito == nacionalidade)
            {
                factory = new Factory.FactoryExercitoEgipcio();
            }
            else if (Nacao.India == nacionalidade)
            {
                factory = new Factory.FactoryExercitoIndiano();
            }
            else if (Nacao.Japones == nacionalidade)
            {
                factory = new Factory.FactoryExercitoJapones();
            }
            return factory;
        }
    }
}
