using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Launcher
{
    public class Entity
    {
        public static int IdOffset = 2; // Could actually be Entity Type But maybe model (Changing makes camera offset weird, but no visual update)

        public static int posxOffset = 12;
        public static int posyOffset = posxOffset + 4;
        public static int poszOffset = posyOffset + 4;

        public static int rotXOffset = 60;

        public long OffsetAddress;

        public float x;
        public float y;
        public float z;
        public float rotation;
        public short iD; // Could actually be Entity Type
        // 60

        public void UpdatePosition(int ProcHandle)
        {
            x = MemoryRead.ReadFloat(ProcHandle, OffsetAddress + posxOffset);
            y = MemoryRead.ReadFloat(ProcHandle, OffsetAddress + posyOffset);
            z = MemoryRead.ReadFloat(ProcHandle, OffsetAddress + poszOffset);
            rotation = MemoryRead.ReadFloat(ProcHandle, OffsetAddress + rotXOffset);
            //Console.WriteLine(x);
        }
        public void SetPosition(int ProcHandle, float new_x, float new_y, float new_z)
        {
            MemoryRead.WriteFloat(ProcHandle, OffsetAddress + posxOffset, new_x);
            MemoryRead.WriteFloat(ProcHandle, OffsetAddress + posyOffset, new_y);
            MemoryRead.WriteFloat(ProcHandle, OffsetAddress + poszOffset, new_z);
            UpdatePosition(ProcHandle);
        }
        public void SetRotation(int ProcHandle, float new_rot)
        {
            MemoryRead.WriteFloat(ProcHandle, OffsetAddress + rotXOffset, new_rot);
            UpdatePosition(ProcHandle);
        }
        public short GetId(int ProcHandle)
        {
            return MemoryRead.ReadShort(ProcHandle, OffsetAddress + IdOffset);
        }
        public void SetId(int ProcHandle, short Model)
        {
            MemoryRead.WriteShort(ProcHandle, OffsetAddress + IdOffset, Model);
            iD = GetId(ProcHandle);
        }
        public bool IsMainCharacter()
        {
            return (this.iD == -31708);
        }
        /*public bool IsKomaru() to do!
        {

        }*/
        public override string ToString()
        {
            return "{\n\tx: " + x.ToString() + "\n\ty: " + y.ToString() + "\n\tz: " + z.ToString() + "\n\trot: " + rotation.ToString() + "\n\tid?: " + iD.ToString() + "\n}";
        }
        public Entity() 
        {

        }
        public Entity(long offsetAddress, int procHandle = -1)
        {
            OffsetAddress = offsetAddress;
            if (procHandle != -1) 
            {
                UpdatePosition(procHandle);
                iD = GetId(procHandle);
            }
        }
    }
}
