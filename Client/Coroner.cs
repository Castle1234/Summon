using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client
{
    internal class Coroner : BaseScript
    {
        private static Vehicle cVanEntity;
        private static int cVanBlip;
        private static Ped cVanPed1;
        private static Ped cVanPed2;
        private static bool eventSpawned;
        private static bool eventOnScene;

        private static Vector3 targetLocation = new Vector3();

        private static readonly Random random = new Random();

        public static List<string> ped1Messages = new List<string>
        {
            "~b~Coroner~s~: Hey! Oh man! Its a bad one!", "~b~Coroner~s~: Why can't we get an easy one?"
        };

        public static List<string> ped2Messages = new List<string>
        {
            "~b~Assistant~s~: Did you grab any bodybags?", "~bAssistant~s~: I'll get the stretcher ready."
        };

        public static List<string> progressMessages = new List<string>
        {
            "~b~Coroner~s~: You should've bought the bigger stretcher", "~b~Coroner~s~: He's got a lot of holes"
        };

        public static List<string> leaveMessages = new List<string>
        {
            "~b~Assistant~s~: I need a drink. Lets stop by the bar after we've dropped this one off", "~b~Assistant~s~: I'm going to barf!"
        };

        public static async void Loop()
        {
            if(eventSpawned)
            {
                Ped player = Game.Player.Character;

                if(!eventOnScene && API.GetDistanceBetweenCoords(cVanEntity.Position.X, cVanEntity.Position.Y, cVanEntity.Position.Z, targetLocation.X, targetLocation.Y, targetLocation.Z, true) < 10F)
                {
                    eventOnScene = true;
                    Screen.ShowNotification("Coroner has arrived at the scene. Please direct him to the body");
                    cVanPed1.Task.ClearAllImmediately();
                    API.SetVehicleForwardSpeed(cVanEntity.Handle, 0F);
                    API.TaskLeaveVehicle(cVanPed1.Handle, cVanEntity.Handle, 0);
                    API.TaskLeaveVehicle(cVanPed2.Handle, cVanEntity.Handle, 0);

                    //Communications
                    CommonFunction.DisplayMessage(ped1Messages[random.Next(ped1Messages.Count)], 2500);
                    await BaseScript.Delay(2500);
                    CommonFunction.DisplayMessage(ped1Messages[random.Next(ped2Messages.Count)], 2500);

                    API.TaskGoToEntity(cVanPed1.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);
                    API.TaskGoToEntity(cVanPed2.Handle, player.Handle, -1, 5F, 3F, 1073741824, 0);

                    while (API.GetDistanceBetweenCoords(cVanPed2.Position.X, cVanPed2.Position.Y, cVanPed2.Position.Z, player.Position.X, player.Position.Y, player.Position.Z, true) > 5F)
                    {
                        Debug.WriteLine("Assistant Pathfinding to player");
                        await BaseScript.Delay(500);
                    }
                    API.TaskStartScenarioAtPosition(cVanPed2.Handle, "CODE_HUMAN_MEDIC_TIME_OF_DEATH", cVanPed2.Position.X, cVanPed2.Position.Y, cVanPed2.Position.Z, cVanPed2.Heading, -1, false, false);

                }
            }
        }

        public async static void Summon()
        {
            Ped player = Game.Player.Character; 
            Screen.ShowNotification("Coroner has been summoned. He's on the waay to collect the dead");

            //Van model spawn
            Vector3 spawnLocation = new Vector3();
            float spawnHeading = 0f;
            int unused = 0;
            API.GetNthClosestVehicleNodeWithHeading(player.Position.X, player.Position.Y, player.Position.Z, 80, ref spawnLocation, ref spawnHeading, ref unused, 9, 3.0f, 2.5f);
            await CommonFunction.LoadModel((uint)VehicleHash.Burrito3);
            cVanEntity = await World.CreateVehicle(VehicleHash.Burrito3, spawnLocation, spawnHeading);
            cVanEntity.Mods.PrimaryColor = VehicleColor.MetallicBlack;
            cVanEntity.Mods.LicensePlate = $"SA C {random.Next(10)}";
            cVanEntity.Mods.LicensePlateStyle = LicensePlateStyle.BlueOnWhite1;

            //Van Blip
            cVanBlip = API.AddBlipForEntity(cVanEntity.Handle);
            API.SetBlipColour(cVanBlip, 40);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString("Coroner");
            API.EndTextCommandSetBlipName(cVanBlip);

            //Driver
            await CommonFunction.LoadModel((uint)PedHash.Doctor01SMM);
            cVanPed1 = await World.CreatePed(PedHash.Doctor01SMM, spawnLocation);
            cVanPed1.SetIntoVehicle(cVanEntity, VehicleSeat.Driver);
            cVanPed1.CanBeTargetted = false;

            //Passenger
            await CommonFunction.LoadModel((uint)PedHash.Scientist01SMM);
            cVanPed2 = await World.CreatePed(PedHash.Doctor01SMM, spawnLocation);
            cVanPed2.SetIntoVehicle(cVanEntity, VehicleSeat.Passenger);
            cVanPed2.CanBeTargetted = false;

            // Configurations
            float targetHeading = 0F;
            API.GetClosestVehicleNodeWithHeading(player.Position.X, player.Position.Y, player.Position.Z, ref targetLocation, ref targetHeading, 1, 3.0F, 0);
            cVanPed1.Task.DriveTo(cVanEntity, targetLocation, 10F, 20F, 262972);
            eventSpawned = true;
        }
    }
}
