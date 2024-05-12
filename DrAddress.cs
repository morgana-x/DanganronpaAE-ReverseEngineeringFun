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
        public static long player_genocider_jack_mode = 0xB9972C; // byte that equals 1 or 0, 0xB996F8 alternative address

        public static long gun_ammo_start_offset = 0x7B93C4; // Ammo for the break ammo, add 2 to the offset to go through each ammo amount

        public static long monocoins_offset =  0x7B9434; // or 0x786B64


        public static long entity_list_length_offset = 0x7EC278; // Entity pointer entries (int 64) come after this



        public static long add_entity_func_pointer_offset = 0xBA3A0; //0xBA810; // Probably WRONG!


       // public static long camera_stuff_offset = //0x802CCC; // Z position of camera?

        public static long camera_pos_x_offset = 0x802CC4; // Not certain!
        public static long camera_pos_y_offset = 0x802CC8;
        public static long camera_pos_z_offset = 0x802CCC; // Rotation etc comes after positions, might do later if bothered!



        public static long level_major_id_offset = 0x2FE7E0; // NOT CERTAIN, COULD BE SOMETHING ELSE
        public static long level_minor_id_offset = 0x765F80; // NOT CERTAIN, COULD BE SOMETHING ELSE


        // Spawn entity functions???
        // game.exe+BA810 - 48 85 C9              - test rcx,rcx 

        // Spawn entity?? (Crashes game when called, maybe wrong parameters?)
        // game.exe+108770 - 40 53                 - push rbx


        // Most likely the core function all spawn entity functions use?
        // game.exe+1EBAC0 - 4C 8B D9              - mov r11,rcx

        // Maybe this is the actual spawn entity function?
        // game.exe+BA3A0 - 48 89 5C 24 08        - mov [rsp+08],rbx

        // another possible func
        // game.exe+58AE0 - 40 57                 - push rdi


        // Pretty useless currently
        /* public static long target_posxAddress =  0x827130; // Not player pos, npc pathfinding target(?) towards komaru
         public static long target_posyAddress = 0x827134; // Not player pos, npc pathfinding target(?) towards komaru
         public static long target_poszAddress = 0x834938; // Not player pos, npc pathfinding target(?) towards komaru*/
    }
}
