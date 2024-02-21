﻿using CitizenFX.Core.Native;
using CitizenFX.Core;
using System.Threading.Tasks;

namespace Client
{
    internal class CommonFunction : BaseScript
    {
        public static void DisplayMessage(string msg, int time)
        {
            API.ClearPrints();
            API.SetTextEntry_2("STRING");
            API.AddTextComponentString(msg);
            API.DrawSubtitleTimed(time, true);
        }
        public static async Task<bool> LoadModel(uint model)
        {
            if (!API.IsModelInCdimage(model))
            {
                Debug.WriteLine($"Invalid model {model} was supplied to LoadModel.");
                return false;
            }
            API.RequestModel(model);
            while (!API.HasModelLoaded(model))
            {
                Debug.WriteLine($"Waiting for model {model} to load");
                await BaseScript.Delay(100);
            }
            return true;
        }
    }
}
