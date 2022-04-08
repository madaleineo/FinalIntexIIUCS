using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace UtahCarSafety.Models
{
    public class Prediction
    {
        public float PredictedValue { get; set; }
        public string RoundedValue()
        {
            double pred = Math.Round(PredictedValue);
            string output = "";
            if (pred > 5)
            {
                pred = 5;
            }
            if (pred == 5)
            {
                output = "5 - Fatal";
            }
            else if (pred == 4)
            {
                output = "4 - Suspected Serious Injury";
            }
            else if (pred == 3)
            {
                output = "3 - Suspected Minor Injury";
            }
            else if (pred == 2)
            {
                output = "2 - Possible Injury";
            }
            else
            {
                output = "1 - No Injury";
            }
            return (output);
        }
    }
}





