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

        public int processHandle;

        public long baseAddress;
        public bool InfHealth = false;

        private long[] GetEntityPointers()
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
            long numberofLivingEntitiesAddress = baseAddress + DrAddress.entity_list_length_offset;
            int numberOfLivingEntities = MemoryRead.ReadInt(processHandle, numberofLivingEntitiesAddress);

            long entityPointer = GetEntityPointers()[numberOfLivingEntities-1];

            byte[] kotoMaruData = new byte[1000];
            MemoryRead.ReadMemory(processHandle, GetEntityPointers()[0], ref kotoMaruData);


            MemoryRead.WriteMemory(processHandle, entityPointer, kotoMaruData);

            MemoryRead.WriteLong(processHandle, numberofLivingEntitiesAddress + (8 * ( (numberOfLivingEntities) + 1)), entityPointer);

            MemoryRead.WriteInt(processHandle, numberofLivingEntitiesAddress, numberOfLivingEntities + 1);
        }
        public Entity GetKomaruEntity()
        {
            return GetEntityList()[0]; // This is not right
        }
        public Entity GetFukawaEntity()
        {
            return GetEntityList()[1]; // This is not right
        }
        public void Main_Loop_Main()
        {

            
            /*float target_pos_x = MemoryRead.ReadFloat(processHandle, target_posxAddress); // Not player pos, npc pathfinding target(?) towards komaru
            float target_pos_y = MemoryRead.ReadFloat(processHandle, target_posyAddress); // Not player pos, npc pathfinding target(?) towards komaru
            float target_pos_z = MemoryRead.ReadFloat(processHandle, target_poszAddress); // Not player pos, npc pathfinding target(?) towards komaru*/

            short player_health = MemoryRead.ReadShort(processHandle, baseAddress + DrAddress.player_health_offset);
            short player_level = MemoryRead.ReadShort(processHandle, baseAddress + DrAddress.player_level_offset);
            short player_selected_ammo = MemoryRead.ReadShort(processHandle, baseAddress + DrAddress.player_selected_ammo_offset);

            byte[] aimBuffer = new byte[1];
            MemoryRead.ReadMemory(processHandle, baseAddress + DrAddress.player_aiming_offset, ref aimBuffer);
            bool player_aiming = (aimBuffer[0] == 1);


            int monocoins = MemoryRead.ReadInt(processHandle, baseAddress + DrAddress.monocoins_offset);

            


            Console.SetCursorPosition(0,0);
            var (cx, cy) = Console.GetCursorPosition();
            Console.Clear();
            Console.WriteLine("Level: " + player_level);
            Console.WriteLine("Health: " + player_health);
            Console.WriteLine("Selected Gun: " + player_selected_ammo);
            Console.WriteLine("Aiming: " + player_aiming);
            for (int i=0; i < 8; i++)
            {
                short amount = MemoryRead.ReadShort(processHandle, baseAddress + DrAddress.gun_ammo_start_offset + (i * 2));

                Console.WriteLine("Ammo[" + i + "]: " + amount + (amount == -2 ? " (Unlimited)" : "") + (amount == -1 ? " (Disabled)" : ""));
            }
            Console.WriteLine("Monocoins: " + monocoins);

            long[] pointers = GetEntityPointers();
            Console.WriteLine("Number of entities (not counting coins): " + pointers.Length);
            Console.WriteLine("Entity Pointers:\n[\t" + string.Join("\n[\t", pointers));
            Console.WriteLine("Entity List: \n" + string.Join("\n", GetEntityList(pointers)));
            Console.WriteLine(new string ('\n', 4) + tempinp);

            if (InfHealth)
            {
                MemoryRead.WriteShort(processHandle, baseAddress + DrAddress.player_health_offset, 3);
            }

        }
        public void Main_Loop()
        {
            while (true)
            {
                Main_Loop_Main();
                Thread.Sleep(10);
            }

        }
        string tempinp = "";

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
                    Thread.Sleep(10);
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
        public void Init()
        {
            Process process = MemoryRead.GetProcess("game");
            processHandle = MemoryRead.GetProcessHandle(process);
            baseAddress = MemoryRead.GetProcessBaseAddress(process);
            main_thread = new Thread(new ThreadStart(Main_Loop));
            main_thread.Start();
       
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
