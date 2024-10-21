using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AISystem.Menu
{
    public static class Validators
    {
        static public bool IsWithinBand(float value, float[] band)
        {
            if (value >= band[0] && value <= band[1])
            {
                return true;
            }

            return false;
        }

        static public bool IsWithinBand(float value, float min, float max)
        {
            if (value >= min && value <= max)
            {
                return true;
            }

            return false;
        }
    }
}