using BattleBoardGame.Model.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service_Battle_Board_Games
{
    public class Service : IDisposable
    {
        protected readonly ModelJogosDeGuerra _context;

        public Service(ModelJogosDeGuerra context)
        {
            _context = context;
        }

        public void Dispose()
            => _context.Dispose();
    }
}
