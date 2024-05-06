using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    public static class DrAddress
    {
        public static long player_level_offset = 0x7B93BC;
        public static long player_health_offset = 0x7B93BE;
        public static long player_selected_ammo_offset = 0x7B93C2; // or 0x786658
        public static long player_aiming_offset = 0x2E4498; // byte that equals 1 or 0

        public static long gun_ammo_start_offset = 0x7B93C4; // Ammo for the break ammo, add 2 to the offset to go through each ammo amount

        public static long monocoins_offset =  0x7B9434; // or 0x786B64


        public static long entity_list_length_offset = 0x7EC278; // Entity pointer entries (int 64) come after this


        // Pretty useless currently
       /* public static long target_posxAddress =  0x827130; // Not player pos, npc pathfinding target(?) towards komaru
        public static long target_posyAddress = 0x827134; // Not player pos, npc pathfinding target(?) towards komaru
        public static long target_poszAddress = 0x834938; // Not player pos, npc pathfinding target(?) towards komaru*/
    }
}
