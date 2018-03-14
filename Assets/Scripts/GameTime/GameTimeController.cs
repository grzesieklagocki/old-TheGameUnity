using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class GameTimeController : MonoBehaviour
{
    public GameTime GameTime
    {
        get { return gameTime; }
        set { SetTime(value); }
    }

    private GameTime gameTime;
    private Coroutine measureTimeCoroutine;


    private void Start()
    {
        GameTime = SerializationHelper.LoadGame("1").gameTime;
        //SerializationHelper.SaveGame(new GameStatus(), "1");

        foreach(var n in SerializationHelper.GetSavesList())
        {
            //Debug.Log(n);
        }
        //System.Diagnostics.Process.Start("https://drive.google.com/open?id=1Zu-OYgTJ7SNtFk79SVUQFruYlhhpWOD3");
    }

    private void Update()
    {

    }


    private void SetTime(GameTime time)
    {
        if (measureTimeCoroutine != null)
            StopCoroutine(measureTimeCoroutine);

        gameTime = time;

        measureTimeCoroutine = StartCoroutine(MeasureTime());
    }

    private IEnumerator MeasureTime()
    {
        while (true)
            yield return new WaitForSeconds(1);
    }
}