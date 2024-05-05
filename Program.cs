// See https://aka.ms/new-console-template for more information

using Launcher;
using System.Threading;
public partial class Program
{

    public static void Main(string[] args)
    {
    
        Main main = new Main();
        main.Init();
        
        /*string dllLocation = "C:\\Users\\Lachlan\\source\\repos\\DanganronpaAnotherEpisodeMultiplayer\\x64\\Debug\\DanganronpaMultiplayer.dll";
        string gamePath = "D:\\SteamLibrary\\steamapps\\common\\Danganronpa Another Episode Ultra Despair Girls";
        string dllName = "DanganronpaMultiplayer.dll";
        string gameName = "game";
        if (File.Exists(gamePath + "\\" + dllName))
        {
            File.Delete(gamePath + "\\" + dllName);
        }
        File.Copy(dllLocation, gamePath + "\\" + dllName);
        Injector.Inject( gameName, dllName);*/
    }
}
