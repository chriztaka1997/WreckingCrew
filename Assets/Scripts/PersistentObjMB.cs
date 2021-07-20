using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObjMB : MonoBehaviour
{
    public static PersistentObjMB instance;

    public bool startInTutorial;

    public void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        startInTutorial = false;
    }

    public static bool IsStartInTutorial()
    {
        if (instance == null) return false;
        return instance.startInTutorial;
    }

    public void SetStartInTutorial(bool set)
    {
        startInTutorial = set;
    }

    public void DoQuit()
    {
        Application.Quit();
    }

}
