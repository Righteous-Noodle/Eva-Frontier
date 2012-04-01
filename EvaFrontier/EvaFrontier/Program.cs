using System;

namespace EvaFrontier
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (EvaFrontier game = new EvaFrontier())
            {
                game.Run();
            }
        }
    }
}

