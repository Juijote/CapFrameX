﻿namespace CapFrameX.PMD
{
    public struct PmdChannel
    {
        public string Name;
        public PmdChannelType PmdChannelType;
        public float Current;
        public float Voltage;
        public float Power;
        public long TimeStamp;
    }

    public struct PmdChannelArrayPosition
    {
        public int Index;
        public string Name;
        public PmdChannelType PmdChannelType;
    }

    public static class PmdChannelExtensions
    {
        public static PmdChannelArrayPosition[] PmdChannelIndexMapping
            = new PmdChannelArrayPosition[]
            {
                new PmdChannelArrayPosition(){ Index= 0, Name= "PCIe_Slot_12V_Voltage", PmdChannelType= PmdChannelType.PCIeSlot},
                new PmdChannelArrayPosition(){ Index= 1, Name= "PCIe_Slot_12V_Current", PmdChannelType= PmdChannelType.PCIeSlot},
                new PmdChannelArrayPosition(){ Index= 2, Name= "PCIe_Slot_12V_Power", PmdChannelType= PmdChannelType.PCIeSlot},

                new PmdChannelArrayPosition(){ Index= 3, Name= "PCIe_Slot_33V_Voltage", PmdChannelType= PmdChannelType.PCIeSlot},
                new PmdChannelArrayPosition(){ Index= 4, Name= "PCIe_Slot_33V_Current", PmdChannelType= PmdChannelType.PCIeSlot},
                new PmdChannelArrayPosition(){ Index= 5, Name= "PCIe_Slot_33V_Power", PmdChannelType= PmdChannelType.PCIeSlot},

                new PmdChannelArrayPosition(){ Index= 6, Name= "PCIe_12V_Voltage1", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 7, Name= "PCIe_12V_Voltage2", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 8, Name= "PCIe_12V_Voltage3", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 9, Name= "PCIe_12V_Voltage4", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 10, Name= "PCIe_12V_Voltage5", PmdChannelType= PmdChannelType.PCIe},

                new PmdChannelArrayPosition(){ Index= 11, Name= "PCIe_12V_Current1", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 12, Name= "PCIe_12V_Current2", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 13, Name= "PCIe_12V_Current3", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 14, Name= "PCIe_12V_Current4", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 15, Name= "PCIe_12V_Current5", PmdChannelType= PmdChannelType.PCIe},

                new PmdChannelArrayPosition(){ Index= 16, Name= "PCIe_12V_Power1", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 17, Name= "PCIe_12V_Power2", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 18, Name= "PCIe_12V_Power3", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 19, Name= "PCIe_12V_Power4", PmdChannelType= PmdChannelType.PCIe},
                new PmdChannelArrayPosition(){ Index= 20, Name= "PCIe_12V_Power5", PmdChannelType= PmdChannelType.PCIe},

                new PmdChannelArrayPosition(){ Index= 21, Name= "EPS_Voltage1", PmdChannelType= PmdChannelType.EPS},
                new PmdChannelArrayPosition(){ Index= 22, Name= "EPS_Voltage2", PmdChannelType= PmdChannelType.EPS},
                new PmdChannelArrayPosition(){ Index= 23, Name= "EPS_Voltage3", PmdChannelType= PmdChannelType.EPS},

                new PmdChannelArrayPosition(){ Index= 24, Name= "EPS_Current1", PmdChannelType= PmdChannelType.EPS},
                new PmdChannelArrayPosition(){ Index= 25, Name= "EPS_Current2", PmdChannelType= PmdChannelType.EPS},
                new PmdChannelArrayPosition(){ Index= 26, Name= "EPS_Current3", PmdChannelType= PmdChannelType.EPS},

                new PmdChannelArrayPosition(){ Index= 27, Name= "EPS_Power1", PmdChannelType= PmdChannelType.EPS},
                new PmdChannelArrayPosition(){ Index= 28, Name= "EPS_Power2", PmdChannelType= PmdChannelType.EPS},
                new PmdChannelArrayPosition(){ Index= 29, Name= "EPS_Power3", PmdChannelType= PmdChannelType.EPS},

                new PmdChannelArrayPosition(){ Index= 30, Name= "ATX_12V_Voltage", PmdChannelType= PmdChannelType.ATX},
                new PmdChannelArrayPosition(){ Index= 31, Name= "ATX_12V_Current", PmdChannelType= PmdChannelType.ATX},
                new PmdChannelArrayPosition(){ Index= 32, Name= "ATX_12V_Power", PmdChannelType= PmdChannelType.ATX},

                new PmdChannelArrayPosition(){ Index= 33, Name= "ATX_5V_Voltage", PmdChannelType= PmdChannelType.ATX},
                new PmdChannelArrayPosition(){ Index= 34, Name= "ATX_5V_Current", PmdChannelType= PmdChannelType.ATX},
                new PmdChannelArrayPosition(){ Index= 35, Name= "ATX_5V_Power", PmdChannelType= PmdChannelType.ATX},

                new PmdChannelArrayPosition(){ Index= 36, Name= "ATX_33V_Voltage", PmdChannelType= PmdChannelType.ATX},
                new PmdChannelArrayPosition(){ Index= 37, Name= "ATX_33V_Current", PmdChannelType= PmdChannelType.ATX},
                new PmdChannelArrayPosition(){ Index= 38, Name= "ATX_33V_Power", PmdChannelType= PmdChannelType.ATX},

                new PmdChannelArrayPosition(){ Index= 39, Name= "ATX_STB_Voltage", PmdChannelType= PmdChannelType.ATX},
                new PmdChannelArrayPosition(){ Index= 40, Name= "ATX_STB_Current", PmdChannelType= PmdChannelType.ATX},
                new PmdChannelArrayPosition(){ Index= 41, Name= "ATX_STB_Power", PmdChannelType= PmdChannelType.ATX},
            };
    }
}
