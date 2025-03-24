using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitoring.DAL.Consts
{
    public static  class StaticData
    {
        // Blood pressure categories
        public enum BloodPressureCategory
        {
            Low,
            Normal,
            Elevated,
            HypertensionStage1,
            HypertensionStage2,
            HypertensiveCrisis
        }
    }
}

