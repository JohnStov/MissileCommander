using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;

namespace MissileCommander
{
    public class Launcher
    {
        public static async Task<Launcher> CreateAsync()
        {
            var selector = HidDevice.GetDeviceSelector(0x01, 0x10, 0x2123, 0x1010);

            var devices = await DeviceInformation.FindAllAsync(selector);
            if (devices.Count > 0)
            {
                var device = await HidDevice.FromIdAsync(devices[0].Id, FileAccessMode.ReadWrite);
                return new Launcher(device);
            }

            return null;
        }

        private HidDevice device;

        private Launcher(HidDevice device)
        {
            this.device = device;
        }

        private async Task SendOutputMessage(byte[] message)
        {
            if (device != null)
            {
                var report = device.CreateOutputReport();
                report.Data = message.AsBuffer();
                await device.SendOutputReportAsync(report);
            }
        }

        private static readonly byte[] CMD = { 0, 0, 0, 0, 0, 0, 0, 0, 2 };
        private static readonly byte[] Up = { 0, 2, 2, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] DOWN = { 0, 2, 1, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] LEFT = { 0, 2, 4, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] RIGHT = { 0, 2, 8, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] FIRE = { 0, 2, 16, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] Stop = { 0, 2, 32, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] GetStatus = { 0, 1, 0, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] LedOn = { 0, 3, 1, 0, 0, 0, 0, 0, 0 };
        private static readonly byte[] LedOff = { 0, 3, 0, 0, 0, 0, 0, 0, 0 };

        public async Task SetLightAsync(bool switchOn)
        {
            await SendOutputMessage(switchOn?LedOn:LedOff);

        }

        public async Task MoveUpAsync(int millisecs)
        {
            await SendOutputMessage(Up);
            await Task.Delay(millisecs);
            await SendOutputMessage(Stop);
        }

        public async Task MoveDownAsync(int millisecs)
        {
            await SendOutputMessage(DOWN);
            await Task.Delay(millisecs);
            await SendOutputMessage(Stop);
        }

        public async Task MoveLeftAsync(int millisecs)
        {
            await SendOutputMessage(LEFT);
            await Task.Delay(millisecs);
            await SendOutputMessage(Stop);
        }

        public async Task MoveRightAsync(int millisecs)
        {
            await SendOutputMessage(RIGHT);
            await Task.Delay(millisecs);
            await SendOutputMessage(Stop);
        }

        public async Task FireAsync()
        {
            await SendOutputMessage(FIRE);
        }
    }
}