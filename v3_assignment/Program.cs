using System;

namespace v3_assignment
{


    public static class Program
    {
        static bool restartgame = false;
        [STAThread]
        static void Main()
        {

            var game = new Game1();
            game.Run();

            if (restartgame)
            {
                game.Exit();
                game = new Game1();
                restartgame = false;
            }

        }

        static void torestart(bool restart) {
            restartgame = true;
        }
    }
}
