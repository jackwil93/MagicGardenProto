using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicGlobal
{
    public static class Elements
    {
        public enum elementTypes { noElement, fire, ice, air, force, water, energy, summer, autumn, winter, spring, life, death };
    }

    public static class GameStates
    {
        public enum gameScreens
        { mainGame, emails, inventory, laptop, spellGame, settings, transactions };
    }

    public static class GameEvents
    {
        public enum eventStatus { notReady, readyOnLoad, readyNextDay, startImmediately, active, passed };
        public static Dictionary<string, eventStatus> generalEvents;
        /// <summary>
        /// Add events that occur after a specific amount of time. Float = minutes
        /// </summary>
        public static Dictionary<string, float> timedEvents;

        // NOTE: How to do email and mail delay? Every minute game checks dictionary to see what has transpired?
    }

    public static class GameDateTime
    {
        public static int loggedMinuteOfYear;
        public static int loggedDayOfYear; // For tracking Leap Years
        public static int realTimeSinceLastPlay;

        public static float DayProgressionPercent()
        {
            return ((float)DateTime.Now.Second + ((float)DateTime.Now.Minute * 60) + ((float)DateTime.Now.Hour * 60 * 60)) / 86400; // Divide now by how many seconds in the day
        }

        public static Vector2 LogCurrentDateTime() // In future, maybe can return a Tuple<int,int> 
        {
            // DayOfYear - 1 because we then add the hours of the current day
            loggedMinuteOfYear = ((DateTime.Now.DayOfYear - 1) * 24 * 60) + (DateTime.Now.Hour * 60) + DateTime.Now.Minute;
            loggedDayOfYear = DateTime.Now.DayOfYear;
            Debug.Log("Minute Of Year (recorded) = " + loggedMinuteOfYear);
            Debug.Log("Day Of Year (recorded) = " + loggedDayOfYear);
            return new Vector2 (loggedMinuteOfYear, loggedDayOfYear);
        }


        public static void SetRealTimeSinceLastPlay(bool newGame, int previousMinuteOfYear, int previousDayOfYear)
        {
            int _prevMinuteOfYear = previousMinuteOfYear;
            int _prevDayOfYear = previousDayOfYear;


            Vector2 dateTime = LogCurrentDateTime();
            int currentMinute = (int)dateTime.x;
            Debug.Log("current minute = " + currentMinute);
            int currentDay = (int)dateTime.y;

            if (newGame)
            {
                _prevMinuteOfYear = currentMinute;
                _prevDayOfYear = currentDay;
            }


            int result;

            // If played over December 31 - Jan 1, just add the previous year leftover minutes to the current minute. No Subtraction
            if (previousDayOfYear > currentDay)
                result = currentMinute += MinutesPassedToEndOfYear(_prevMinuteOfYear, _prevDayOfYear);
            else // If same year, use subtraction
                result = currentMinute - _prevMinuteOfYear;

           
            Debug.Log("minute difference since last play(current - prev) = " + result);
            realTimeSinceLastPlay = result;
            Debug.Log("Time since last play updated");
        }

        static int MinutesPassedToEndOfYear(int previousMinuteOfYear, int previousDayOfYear)
        {
            int minutesOfPrevYear; // minutes from previous logged number to the last minute of the year

            // if a Leap Year
            if (previousDayOfYear == 366)
                minutesOfPrevYear = 527040 - previousMinuteOfYear;
            else
                minutesOfPrevYear = 525600 - previousMinuteOfYear;

            return minutesOfPrevYear;
        }
    }

    [System.Serializable]
    public class ItemProperties
    {
        public string itemID;
        public string displayedName;
        public enum itemTypes { pot, plant, potWithPlant, seed, potion, decor };
        public itemTypes itemType;
        public Elements.elementTypes baseElement;
        public Elements.elementTypes elementNeeded;
        public string itemDescription;
        public enum itemStage { normal, seed, germ1, germ2, special, sick, dead}
        public itemStage currentStage;
        public int buyPriceFlorets;
        public int sellPriceFlorets;
        public int buyPriceCrystals;
    }
}
