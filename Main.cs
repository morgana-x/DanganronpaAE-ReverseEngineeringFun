using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Launcher
{
    internal class Main
    {
        private Thread main_thread;

        public Process process;

        public int processHandle;

        public int processId;

        public long baseAddress;
        public bool InfHealth = false;

        public int GetNumberOfEntities()
        {
            long numberofLivingEntitiesAddress = baseAddress + DrAddress.entity_list_length_offset;
            return  MemoryRead.ReadInt(processHandle, numberofLivingEntitiesAddress);
        }
        public long[] GetEntityPointers()
        {
            long numberofLivingEntitiesAddress = baseAddress + DrAddress.entity_list_length_offset;
            int numberOfLivingEntities = MemoryRead.ReadInt(processHandle, numberofLivingEntitiesAddress);

            long[] entityOffsets = new long[numberOfLivingEntities];

            for (int i=0; i < numberOfLivingEntities; i++)
            {
                entityOffsets[i] = MemoryRead.ReadLong(processHandle, numberofLivingEntitiesAddress + (8 * (i+1)));
            }

            return entityOffsets;
        }

        public Entity GetEntityFromPointer(long pointer)
        {
            return new Entity(pointer, processHandle);
        }
        public Entity GetEntityFromId(int id)
        {
            return GetEntityFromPointer(GetEntityPointers()[id]);
        }
        public List<Entity> GetEntityList()
        {
            return GetEntityList(GetEntityPointers());
        }
        public List<Entity> GetEntityList(long[] entityPointers)
        {
            List<Entity> entityList = new List<Entity>();


            foreach (var x in entityPointers)
            {
                entityList.Add(new Entity(x, processHandle));
            }

            return entityList;

        }
        public void SpawnEntity() // Todo: find ingame function for spawning entities
        {
            RemoteFunction.ExecuteFunction(processHandle, baseAddress + DrAddress.add_entity_func_pointer_offset);
   
        }
        public Entity GetKomaruEntity()
        {
            return GetEntityList()[0]; // This is not right
        }
        public Entity GetFukawaEntity()
        {
            return GetEntityList()[1]; // This is not right
        }

        long nextDrawConsole = 0;
        public void Main_Loop_Main(float curTime)
        {
            noclip.Tick(curTime);
            if (InfHealth)
            {
                MemoryRead.WriteShort(processHandle, baseAddress + DrAddress.player_health_offset, 3);
            }

            if (Runtime.CurrentRuntime < nextDrawConsole)
            {
                return;
            }
            nextDrawConsole = Runtime.CurrentRuntime + 100;

            short player_health = MemoryRead.ReadShort(processHandle, baseAddress + DrAddress.player_health_offset);
            short player_level = MemoryRead.ReadShort(processHandle, baseAddress + DrAddress.player_level_offset);
            short player_selected_ammo = MemoryRead.ReadShort(processHandle, baseAddress + DrAddress.player_selected_ammo_offset);

            bool player_aiming = MemoryRead.ReadByte(processHandle, baseAddress + DrAddress.player_aiming_offset) == 1;
            bool player_genocider_jack = MemoryRead.ReadByte(processHandle, baseAddress + DrAddress.player_genocider_jack_mode) == 1;

            int monocoins = MemoryRead.ReadInt(processHandle, baseAddress + DrAddress.monocoins_offset);

            int level_id_major = MemoryRead.ReadInt(processHandle, baseAddress + DrAddress.level_major_id_offset);
            int level_id_minor = MemoryRead.ReadInt(processHandle, baseAddress + DrAddress.level_minor_id_offset);


          
            var (cx, cy) = Console.GetCursorPosition();
            //Console.Clear();
      
            Console.SetCursorPosition(0, 0);
            Console.Write(clearConsole);
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Current Level / Map id? " + level_id_major + " : " + level_id_minor);
            Console.WriteLine("Player Level: " + player_level);
            Console.WriteLine("Monocoins: " + monocoins);
            Console.WriteLine("Health: " + player_health);

            Console.WriteLine("Is Genocider Jill : " + player_genocider_jack);
            Console.WriteLine("Noclip active: " + noclip.Active);
            Console.WriteLine("Selected Gun: " + player_selected_ammo);
            Console.WriteLine("Aiming: " + player_aiming);

            for (int i=0; i < 8; i++)
            {
                short amount = MemoryRead.ReadShort(processHandle, baseAddress + DrAddress.gun_ammo_start_offset + (i * 2));

                Console.WriteLine("Ammo[" + i + "]: " + amount + (amount == -2 ? " (Unlimited)" : "") + (amount == -1 ? " (Disabled)" : ""));
            }


            long[] pointers = GetEntityPointers();

            Console.WriteLine("Number of entities (not counting coins): " + pointers.Length);
            Console.WriteLine("Entity Pointers:\n[\t" + string.Join("\n[\t", pointers));
            if (pointers.Length < 10) // Change if you wish, just that the way console is refreshed is bad! and will mess up your view!
            {
                Console.WriteLine("Entity List: \n" + string.Join("\n", GetEntityList(pointers)));
            }

  
           
            Console.WriteLine("\nInput: " + tempinp);
            Console.SetCursorPosition(cx, cy);
   

        }
        public void Main_Loop()
        {
            float curTime = 0;
            Stopwatch sw = new Stopwatch();
            while (true)
            {
                sw.Restart();
                Main_Loop_Main(curTime);
                sw.Stop();
                curTime = (float)sw.Elapsed.TotalSeconds;
  
            }

        }
        string tempinp = "";
        string clearConsole = "";
        private bool ProcessCommand(string inp)
        {
            if (inp != "")
            {
                tempinp = "";
            }
            if (inp == "quit")
            {
                return true;
            }
            if (inp == "infhp")
            {
                InfHealth = !InfHealth;
            }
            if (inp == "infammo")
            {
                for (int i = 0; i < 8; i++)
                {
                    long addr = baseAddress + DrAddress.gun_ammo_start_offset + (i * 2);
                    MemoryRead.WriteShort(processHandle, addr, -2);
                   // Thread.Sleep(10);
                }
            }
            if (inp.StartsWith("setammo"))
            {
                short ammotype = short.Parse(inp.Split(' ')[1]);

                short ammoamount = short.Parse(inp.Split(' ')[2]);

                long addr = baseAddress + DrAddress.gun_ammo_start_offset + (ammotype*2);
                MemoryRead.WriteShort(processHandle, addr, ammoamount);

            }
            if (inp.StartsWith("setcoins"))
            {
                short amount = short.Parse(inp.Split(' ')[1]);


                MemoryRead.WriteShort(processHandle, baseAddress + DrAddress.monocoins_offset, amount);

            }
            if (inp.StartsWith("sethp"))
            {
                short amount = short.Parse(inp.Split(' ')[1]);


                MemoryRead.WriteShort(processHandle, baseAddress + DrAddress.player_health_offset, amount);

            }
            if (inp.StartsWith("up"))
            {
                List<Entity> entities = GetEntityList();
                foreach (Entity ent in entities) 
                {
                    ent.UpdatePosition(processHandle);

                    ent.SetPosition(processHandle, ent.x, ent.y, ent.z + 2.0f);
                }
            }
            if (inp.StartsWith("spawn"))
            {
                SpawnEntity();
            }
            if (inp.StartsWith("model"))
            {
                var model = GetKomaruEntity().GetId(processHandle);

                foreach (var ent in GetEntityList())
                {
                    ent.SetId(processHandle, model);
                }

            }
            if (inp.StartsWith("tp"))
            {
                var kom = GetKomaruEntity();

                foreach (var ent in GetEntityList())
                {
                    ent.SetPosition(processHandle, kom.x, kom.y, kom.z);
                }

            }
            return false;
        }

        Noclip noclip;
        public void Init()
        {
            process = MemoryRead.GetProcess("game");
            processId = process.Id;
            processHandle = MemoryRead.GetProcessHandle(process);
            baseAddress = MemoryRead.GetProcessBaseAddress(process);


            clearConsole = "";
            for (int i = 0; i < 200; i++)
            {
                clearConsole += new string('\t', 30) + "\n";
            }


            main_thread = new Thread(new ThreadStart(Main_Loop));
            main_thread.Start();
            noclip = new Noclip(this);
           


            while (true)
            {
                var key = Console.ReadKey();
            
                if (key.Key == ConsoleKey.Backspace && tempinp.Length > 0)
                {
                    tempinp = tempinp.Substring(0, tempinp.Length - 1);
                }
                else
                {
                    tempinp += key.KeyChar;
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    if (ProcessCommand(tempinp))
                    {
                        break;
                    }
                }



                Thread.Sleep(10);
            }
            main_thread.Suspend();
           
            
        }
    }
}
