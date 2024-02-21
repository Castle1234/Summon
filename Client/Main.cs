using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Threading.Tasks;

namespace Client
{
    public class Main : BaseScript
    {

        private static int gTimer;
        public Main()
        {
            API.RegisterCommand("coroner", new Action(SummonCoroner), false);

            Tick += OnTick;

        }

        private async Task OnTick()
        {
            if(API.GetGameTimer() -gTimer >= 1000)
            {
                gTimer = API.GetGameTimer();

                API.EnableDispatchService(5, false);

                Coroner.Loop();
            }
        }

        private void SummonCoroner()
        {
            Coroner.Summon();
        }
    }
}
