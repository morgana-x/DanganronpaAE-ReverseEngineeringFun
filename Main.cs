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
        public bool InfAmmo = false;

        private long[] GetEntityPointers()
        {
            long numberofLivingEntitiesAddress = baseAddress + 0x7EC278;
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
            List<Entity> entityList = new List<Entity>();

            long[] entityPointers = GetEntityPointers();

            foreach (var x in entityPointers)
            {
                entityList.Add(new Entity( x, processHandle));
            }

            return entityList;

        }
        public void SpawnEntity()
        {
            long numberofLivingEntitiesAddress = baseAddress + 0x7EC278;
            int numberOfLivingEntities = MemoryRead.ReadInt(processHandle, numberofLivingEntitiesAddress);

            long entityPointer = GetEntityPointers()[numberOfLivingEntities-1] + 11200;

            byte[] kotoMaruData = new byte[11200];
            MemoryRead.ReadMemory(processHandle, GetEntityPointers()[0], ref kotoMaruData);


            MemoryRead.WriteMemory(processHandle, entityPointer, kotoMaruData);

            MemoryRead.WriteLong(processHandle, numberofLivingEntitiesAddress + (8 * ( (numberOfLivingEntities) + 1)), entityPointer);

            MemoryRead.WriteInt(processHandle, numberofLivingEntitiesAddress, numberOfLivingEntities + 1);
        }
        public Entity GetKomaruEntity()
        {
            return GetEntityList()[0];
        }
        public Entity GetFukawaEntity()
        {
            return GetEntityList()[1];
        }
        public void Main_Loop_Main()
        {

            long player_levelAddress = baseAddress + 0x7B93BC;
            long healthAddress = baseAddress + 0x7B93BE;
            long selectedGunAddress = baseAddress + 0x7B93C2; // or 0x786658
            long monocoinsAddress = baseAddress + 0x7B9434; // or 0x786B64
            long playerAimingAddress = baseAddress + 0x2E4498;

            long numberofLivingEntitiesAddress = baseAddress + 0x7EC278;

            long gunAmmo_type_break_address = baseAddress + 0x7B93C4;
            long gunAmmo_type_knockback_address = baseAddress + 0x7B93CA;
            long gunAmmo_type_dance_address = baseAddress + 0x7B93CC;
            long gunAmmo_type_burn_address = baseAddress + 0x7B93C6;
            long gunAmmo_type_paralyze_address = baseAddress + 0x7B93C8;
            long gunAmmo_type_link_address = baseAddress + 0x7B93CE;

            long target_posxAddress = baseAddress + 0x827130; // Not player pos, npc pathfinding target(?) towards komaru
            long target_posyAddress = baseAddress + 0x827134; // Not player pos, npc pathfinding target(?) towards komaru
            long target_poszAddress = baseAddress + 0x834938; // Not player pos, npc pathfinding target(?) towards komaru
           // Console.WriteLine("Base Address: " + baseAddress);
           // Console.WriteLine("Health Address: " + healthAddress);
            float target_pos_x = MemoryRead.ReadFloat(processHandle, target_posxAddress); // Not player pos, npc pathfinding target(?) towards komaru
            float target_pos_y = MemoryRead.ReadFloat(processHandle, target_posyAddress); // Not player pos, npc pathfinding target(?) towards komaru
            float target_pos_z = MemoryRead.ReadFloat(processHandle, target_poszAddress); // Not player pos, npc pathfinding target(?) towards komaru

            short health = MemoryRead.ReadShort(processHandle, healthAddress);
            short player_level = MemoryRead.ReadShort(processHandle, player_levelAddress);
            short selected_gun_type = MemoryRead.ReadShort(processHandle, selectedGunAddress);
            short gun_ammo_break = MemoryRead.ReadShort(processHandle, gunAmmo_type_break_address);
            short gun_ammo_dance = MemoryRead.ReadShort(processHandle, gunAmmo_type_dance_address);
            short gun_ammo_knockback = MemoryRead.ReadShort(processHandle, gunAmmo_type_knockback_address);
            short gun_ammo_paralyze = MemoryRead.ReadShort(processHandle, gunAmmo_type_paralyze_address);
            short gun_ammo_burn = MemoryRead.ReadShort(processHandle, gunAmmo_type_burn_address);
            short gun_ammo_link = MemoryRead.ReadShort(processHandle, gunAmmo_type_link_address);
            int monocoins = MemoryRead.ReadInt(processHandle, monocoinsAddress);

            byte[] aimBuffer = new byte[1];
            MemoryRead.ReadMemory(processHandle, playerAimingAddress, ref aimBuffer);
            bool player_aiming = (aimBuffer[0] == 1);

            int numberOfLivingEntities = MemoryRead.ReadInt(processHandle, numberofLivingEntitiesAddress);
            //Console.WriteLine("NPC Target Position: " + "\n \t" + "x: " + target_pos_x + "\n \t" + "y: " + target_pos_y + "\n \t" + "z: " + target_pos_z);
            Console.SetCursorPosition(0,0);
            var (cx, cy) = Console.GetCursorPosition();
            Console.Clear();
            Console.WriteLine("Level: " + player_level);
            Console.WriteLine("Health: " + health);
            Console.WriteLine("Selected Gun: " + selected_gun_type);
            Console.WriteLine("Aiming: " + player_aiming);
            for (int i=0; i < 8; i++)
            {
                short amount = MemoryRead.ReadShort(processHandle, gunAmmo_type_break_address + (i * 2));

                Console.WriteLine("Ammo[" + i + "]: " + amount + (amount == -2 ? " (Unlimited)" : "") + (amount == -1 ? " (Disabled)" : ""));
            }
            Console.WriteLine("Monocoins: " + monocoins);
            Console.WriteLine("Number of entities (not counting coins): " + numberOfLivingEntities);
            Console.WriteLine("Entity Pointers:\n[\t" + string.Join("\n[\t", GetEntityPointers()));
            Console.WriteLine("Entity List: \n" + string.Join("\n", GetEntityList()));
            Console.WriteLine(new string ('\n', 4) + tempinp);

            if (InfHealth)
            {
                MemoryRead.WriteShort(processHandle, healthAddress, 3);
            }
            if (InfAmmo)
            {
                MemoryRead.WriteShort(processHandle, gunAmmo_type_break_address, 99);
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
                InfAmmo = !InfAmmo;
            }
            if (inp.StartsWith("setammo"))
            {
                short ammotype = short.Parse(inp.Split(' ')[1]);

                short ammoamount = short.Parse(inp.Split(' ')[2]);

                long addr = baseAddress + 0x7B93C4 + (ammotype*2);
                MemoryRead.WriteShort(processHandle, addr, ammoamount);

            }
            if (inp.StartsWith("setcoins"))
            {
                short amount = short.Parse(inp.Split(' ')[1]);


                MemoryRead.WriteShort(processHandle, baseAddress + 0x7B9434, amount);

            }
            if (inp.StartsWith("sethp"))
            {
                short amount = short.Parse(inp.Split(' ')[1]);


                MemoryRead.WriteShort(processHandle, baseAddress + 0x7B93BE, amount);

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
