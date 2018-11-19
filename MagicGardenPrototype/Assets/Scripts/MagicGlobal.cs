using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace MagicGlobal
{
    public static class Elements
    {
        public enum elementTypes { noElement, fire, ice, air, earth, water, energy, summer, autumn, winter, spring, life, death };
    }

    public static class GameStates
    {
        public enum gameScreens
        { mainGame, emails, inventory, laptop, spellGame, settings, transactions, selling };
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
        public enum itemTypes { pot, plant, potWithPlant, potion, decor };
        public itemTypes itemType;
        public Elements.elementTypes baseElement;
        public Elements.elementTypes elementNeeded;
        public string itemDescription;
        public enum itemStage { normal, seed, planted, germ1, germ2, special, sick, dead}
        public itemStage currentStage;
        [Header ("Stage Ends must be Cumulative and in Minutes")]
        public int ageStartGerm1Stage; 
        public int ageStartGerm2Stage; 
        public int ageStartBloomStage;  // At end, dies.
        public int ageLifeTotal;  // At end, dies.
        public int buyPriceFlorets;
        public int sellPriceFlorets;
        public int buyPriceCrystals;
    }

    public class MagicTools
    {

        /// <summary>
        /// Makes a Deep Copy of a Serializable Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        /// https://www.infoworld.com/article/3109870/application-development/my-two-cents-on-deep-copy-vs-shallow-copy-in-net.html
        public static T DeepCopy<T>(T obj)
        {
            if (!typeof(T).IsSerializable)

            {
                throw new Exception("The source object must be serializable");
            }
            if (System.Object.ReferenceEquals(obj, null))

            {
                throw new Exception("The source object must not be null");
            }
            T result = default(T);

            using (var memoryStream = new MemoryStream())
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(memoryStream, obj);
                memoryStream.Seek(0, SeekOrigin.Begin);
                result = (T)formatter.Deserialize(memoryStream);
                memoryStream.Close();
            }

            return result;
        }
    }

}

// Extensions and Tools

//https://answers.unity.com/questions/799429/transformfindstring-no-longer-finds-grandchild.html
public static class TransformDeepChildExtension
{
    //Breadth-first search
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        var result = aParent.Find(aName);
        if (result != null)
            return result;
        foreach (Transform child in aParent)
        {
            result = child.FindDeepChild(aName);
            if (result != null)
                return result;
        }
        return null;
    }
}


