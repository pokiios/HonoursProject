using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EmpaticaAndBioharness
{
    public class SensorTimeConverter
    {
        public static Stopwatch applicationTime; // Keep a reference of the app time

        public double currentTime;
        public double lastReference;

        public SensorTimeConverter()
        {
            currentTime = 0;
            lastReference = 0;
        }

        //public double UpdateTime(double timeOfWriting, double newReference)
        public double UpdateTime(double newReference)
        {
            // If there are no past references, it means that this stream has started now (we assume).
            // Kickoff by using the current game time and saving the reference for later use.
            if (lastReference == 0)
            {
                currentTime = Convert.ToDouble(CurrentTimeInSeconds());
                lastReference = newReference;
                return currentTime;
            }
            else
            {
                double diff = newReference - lastReference;
                currentTime += diff;
                lastReference = newReference;
                return currentTime;
            }
        }

        public void ResetTime()
        {
            currentTime = 0;
            lastReference = 0;
        }

        public double CurrentTimeInSeconds()
        {
            return applicationTime.ElapsedMilliseconds / 1000.0;
        }
    }
}
