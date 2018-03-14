using System;
using UnityEngine;

[Serializable]
public class GameTime
{
    public int Minutes { get { return minutes; } }
    public int Hours { get { return hours; } }
    public int Days { get { return days; } }


    [SerializeField]
    private int minutes;

    [SerializeField]
    private int hours;

    [SerializeField]
    private int days;


    public void NextMinute()
    {
        if (++minutes == 60)
        {
            minutes = 0;

            if (++hours == 25)
            {
                hours = 1;

                ++days;
            }
        }
    }
}