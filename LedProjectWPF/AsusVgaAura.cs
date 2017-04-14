using System;
using System.Runtime.InteropServices;

namespace AsusVgaAura
{
    public enum LightingMode : UInt32
    {
        None = 0x00,
        Static = 0x01,
        Breath = 0x02,
        Strobe = 0x03,
        CrossfadeColorSpectrum = 0x04
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct LedColor
    {
        [FieldOffset(0)]
        public UInt32 Red;

        [FieldOffset(4)]
        public UInt32 Green;

        [FieldOffset(8)]
        public UInt32 Blue;
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    public struct InteropLedInfo
    {
        [FieldOffset(0)]
        public UInt32 Unknown1;

        [FieldOffset(4)]
        public UInt32 Unknown2;

        [FieldOffset(8)]
        public UInt32 Unknown3;

        /// <summary>
        /// Unsure, Is always 0x01
        /// </summary>
        [FieldOffset(12)]
        public UInt32 Unknown4;

        [FieldOffset(16)]
        public LedColor Color;

        /// <summary>
        /// needs to be 0x01 in order to disable LEDs or change LightingMode
        /// </summary>
        [FieldOffset(28)]
        public UInt32 ChangeState;

        [FieldOffset(32)]
        public LightingMode Mode;
    };

    public class AsusVgaAuraWrapper
    {
        [DllImport(dllName: "VGA_Extra.dll", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void LoadVenderDLL();

        [DllImport(dllName: "VGA_Extra.dll", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Set_LED_ExternalFan_Info(InteropLedInfo ledInfo);

        public AsusVgaAuraWrapper()
        {
            LoadVenderDLL();
        }

        public void SetColor(byte red, byte green, byte blue, LightingMode lightingMode = LightingMode.Static)
        {
            InteropLedInfo data = new InteropLedInfo();
            data.Unknown4 = 0x01;
            data.Color.Red = red;
            data.Color.Green = green;
            data.Color.Blue = blue;
            data.ChangeState = 0x01;
            data.Mode = lightingMode;
            Set_LED_ExternalFan_Info(data);
        }

        //Sets the LEDs to fade across the color spectrum at an undefined rate
        public void SetCrossFade()
        {
            InteropLedInfo data = new InteropLedInfo();
            data.ChangeState = 0x01;
            data.Mode = LightingMode.CrossfadeColorSpectrum;
            Set_LED_ExternalFan_Info(data);
        }

        //Turns the LEDs off
        public void DisableLights()
        {
            InteropLedInfo data = new InteropLedInfo();
            data.Unknown4 = 0x01;
            data.Color.Red = 0x00;
            data.Color.Green = 0x00;
            data.Color.Blue = 0x00;
            data.ChangeState = 0x01;
            data.Mode = LightingMode.Static;
            Set_LED_ExternalFan_Info(data);
        }

        //Freezes the LEDs in their current state (brightness, rgb values, etc.)
        public void FreezeLight()
        {
            InteropLedInfo data = new InteropLedInfo();
            data.Unknown4 = 0x01;
            data.Color.Red = 0x00;
            data.Color.Green = 0x00;
            data.Color.Blue = 0x00;
            data.ChangeState = 0x01;
            data.Mode = LightingMode.None;
            Set_LED_ExternalFan_Info(data);
        }
    }
}