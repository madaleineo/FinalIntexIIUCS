using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UtahCarSafety.Models
{
    public class CrashData
    {
  
        public float CITY_OGDEN { get; set; }
  
        public float CITY_PROVO { get; set; }
 
        public float CITY_WEST_JORDAN { get; set; }
 
        public float PEDESTRIAN_INVOLVED_True { get; set; }

        public float BICYCLIST_INVOLVED_True { get; set; }
   
        public float MOTORCYCLE_INVOLVED_True { get; set; }
        
        public float IMPROPER_RESTRAINT_True { get; set; }
       
        public float UNRESTRAINED_True { get; set; }
     
        public float DUI_True { get; set; }
       
        public float INTERSECTION_RELATED_True { get; set; }
   
        public float OVERTURN_ROLLOVER_True { get; set; }
    
        public float SINGLE_VEHICLE_True { get; set; }
    
        public float DISTRACTED_DRIVING_True { get; set; }
    
        public float DROWSY_DRIVING_True { get; set; }
    
        public float ROADWAY_DEPARTURE_True { get; set; }


        public Tensor<float> AsTensor()
        {
            float[] data = new float[]
            {
                CITY_OGDEN, CITY_PROVO, CITY_WEST_JORDAN, PEDESTRIAN_INVOLVED_True, BICYCLIST_INVOLVED_True,
                IMPROPER_RESTRAINT_True, MOTORCYCLE_INVOLVED_True, IMPROPER_RESTRAINT_True, UNRESTRAINED_True, DUI_True,
                INTERSECTION_RELATED_True, OVERTURN_ROLLOVER_True, SINGLE_VEHICLE_True, DISTRACTED_DRIVING_True,
                DROWSY_DRIVING_True, ROADWAY_DEPARTURE_True
            };

            int[] dimension = new int[] { 1, 16 };
            return new DenseTensor<float>(data, dimension);
        }


        public Prediction prediction { get; set; }

    }
}
