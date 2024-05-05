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

        public void Main_Loop_Main()
        {

            long player_levelAddress = baseAddress + 0x7B93BC;
            long healthAddress = baseAddress + 0x7B93BE;
            long selectedGunAddress = baseAddress + 0x7B93C2; // or 0x786658
            long monocoinsAddress = baseAddress + 0x7B9434; // or 0x786B64

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
            //Console.WriteLine("NPC Target Position: " + "\n \t" + "x: " + target_pos_x + "\n \t" + "y: " + target_pos_y + "\n \t" + "z: " + target_pos_z);
            Console.SetCursorPosition(0,0);
            var (cx, cy) = Console.GetCursorPosition();
            Console.WriteLine("Level: " + player_level);
            Console.WriteLine("Health: " + health);
            Console.WriteLine("Selected Gun: " + selected_gun_type);
            Console.WriteLine("Ammo (Break): " + gun_ammo_break);
            Console.WriteLine("Ammo (Dance): " + gun_ammo_dance);
            Console.WriteLine("Ammo (Knockback): " + gun_ammo_knockback);
            Console.WriteLine("Ammo (Paralyze): " + gun_ammo_paralyze);
            Console.WriteLine("Ammo (burn): " + gun_ammo_burn);
            Console.WriteLine("Ammo (Link): " + gun_ammo_link);
            Console.WriteLine("Monocoins: " + monocoins);
            Console.SetCursorPosition(cx, (cy > 10) ? cy : 11);

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

        public void Init()
        {
            Process process = MemoryRead.GetProcess("game");
            processHandle = MemoryRead.GetProcessHandle(process);
            baseAddress = MemoryRead.GetProcessBaseAddress(process);
            main_thread = new Thread(new ThreadStart(Main_Loop));
            main_thread.Start();
            while (true)
            {
                string inp = Console.ReadLine();
                if (inp == "quit")
                {
                    break;
                }
                if (inp == "infhp")
                {
                    InfHealth = !InfHealth;
                }
                if (inp == "infammo")
                {
                    InfAmmo = !InfAmmo;
                }
                Thread.Sleep(10);
            }
            main_thread.Suspend();
           
            
        }
    }
}
