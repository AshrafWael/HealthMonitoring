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
        public enum HeartRateCategory
        {
            Low,
            Normal,
            High, 
        }
        public enum HeartDiseaseCategory
        {
            Normal,
            Supraventricular_premature,
            Premature_ventricular_contraction,
            Fusion_of_ventricular_and_normal,
            Unclassifiable
        }
    }
}

