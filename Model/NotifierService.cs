using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TZ.Model
{
    public class NotifierService
    {
        // Can be called from anywhere
        public async Task Update(Tetris tetris)
        {
            if (Notify != null)
            {
                await Notify.Invoke(tetris);
            }
        }

        public event Func<Tetris, Task> Notify;
    }
}
