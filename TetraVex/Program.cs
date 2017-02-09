using System;

namespace TetraVex
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string dllName = (args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "")).Replace(".", "_");

                return dllName.EndsWith("_resources", StringComparison.Ordinal) ? null : System.Reflection.Assembly.Load((byte[])new System.Resources.ResourceManager("Tetravex.Resources", System.Reflection.Assembly.GetExecutingAssembly()).GetObject(dllName));
            };

            RunGame();
        }

        public static void RunGame()
        {
            using (TetraVex game = new TetraVex())
            {
                game.Run();
            }
        }
    }
}
