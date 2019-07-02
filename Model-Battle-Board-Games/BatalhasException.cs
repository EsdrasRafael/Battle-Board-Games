using System;
using System.Collections.Generic;
using System.Text;

namespace ModelBattleBoardGames
{
    public class BatalhasExeception : Exception
    {
        public BatalhasExeception(string mensagem)
            : base(mensagem) { }
    }
}