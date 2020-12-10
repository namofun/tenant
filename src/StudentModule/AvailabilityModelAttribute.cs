using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;

namespace SatelliteSite.StudentModule
{
    internal sealed class AvailabilityModelAttribute : Attribute, IControllerModelConvention
    {
        public static bool Enabled { get; set; }

        public void Apply(ControllerModel controller)
        {
            if (!Enabled) controller.Actions.Clear();
        }
    }
}
