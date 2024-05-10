using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher
{
    internal class Noclip
    {
        public bool Active = false;

        public float pos_x = 0;
        public float pos_y = 0;
        public float pos_z = 0;

        public int procHandle;

        public Entity entity;



        public int input_forward = 0;
        public int input_right = 0;
        public int input_up = 0;

        public Main main;


        // reference https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
     
        long lastNoclip = 0;

        int speed = 10;
        private void input()
        {

            if ( lastNoclip < Runtime.CurrentRuntime && Input.IsKeydown(0x56))
            {

                lastNoclip = Runtime.CurrentRuntime + 1000;
                if (!Active)
                {
                    Activate();
                }
                else
                {
                    Deactivate();
                }
                return;
            }
            if (!Active) return;

            input_forward = Input.IsKeydown(0x57) ? 1 : Input.IsKeydown(0x53) ? -1 : 0;
            input_right = Input.IsKeydown(0x44) ? 1 : Input.IsKeydown(0x41) ? -1 : 0;
            input_up = Input.IsKeydown(0x20) ? 1 : Input.IsKeydown(0xA0) ? -1 : 0;
        }
        public void Activate()
        {
            if (main.GetNumberOfEntities() < 1) return;
            Active = true;

            procHandle = main.processHandle;
            input_forward = 0;
            input_right = 0;

          

        

            entity = main.GetKomaruEntity();

            pos_x = entity.x;
            pos_y = entity.y;
            pos_z = entity.z;
        }

        public void Deactivate() 
        {
            Active = false ;
            input_forward = 0;
            input_right = 0;

        }
        public void Tick(float curTime)
        {
            input();

            if (!Active)
            {
                return;
            }


            if (main.GetNumberOfEntities() < 1) return;
            entity.UpdatePosition(procHandle);

            pos_x += input_forward * curTime * speed;// * (float)Math.Sin(entity.rotation);
            pos_y += input_right * curTime * speed;// * (float)Math.Cos(entity.rotation);
            pos_z += input_up * curTime * speed;

      



            entity.SetPosition(procHandle, pos_x, pos_y, pos_z);
        }

        public Noclip(Main main)
        {
            this.main = main;
        }
    }
}
