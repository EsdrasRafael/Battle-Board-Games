using System;
using System.Collections.Generic;
using System.Text;

namespace ModelBattleBoardGames
{
    public class BatalhaExeception : Exception
    {
        public BatalhaExeception(string mensagem)
            : base(mensagem) { }
    }
}