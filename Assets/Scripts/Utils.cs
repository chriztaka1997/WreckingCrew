using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils
{
    /// <summary>
    /// Takes an int and maps it into range: [low, high)
    /// </summary>
    /// <param name="num">input number</param>
    /// <param name="low">low bound inclusive</param>
    /// <param name="high">high bound exclusive</param>
    /// <returns></returns>
    public static int BindRange(int num, int low, int high)
    {
        int range = high - low;
        return ((((num - low) % range) + range) % range) + low;
    }

    /// <summary>
    /// Takes a float and maps it into range: [low, high]
    /// </summary>
    /// <param name="num">input number</param>
    /// <param name="low">low bound inclusive</param>
    /// <param name="high">high bound inclusive</param>
    /// <returns></returns>
    public static float BindRange(float num, float low, float high)
    {
        float range = high - low;
        if (num < low) return num + (range * ((int)((low - num) / range) + 1));
        if (num > high) return num - (range * ((int)((num - high) / range) + 1));
        return num;
    }

    /// <summary>
    /// linear interpolation from source to dest by ratio amount
    /// </summary>
    /// <param name="source">starting point of interpolation</param>
    /// <param name="dest">ending point of interpolation</param>
    /// <param name="ratio">ratio of dest/start, between 0 and 1</param>
    /// <returns></returns>
    public static int Interpolate(int source, int dest, float ratio)
    {
        return source + (int)((dest - source) * ratio);
    }
    /// <summary>
    /// linear interpolation from source to dest by ratio amount
    /// </summary>
    /// <param name="source">starting point of interpolation</param>
    /// <param name="dest">ending point of interpolation</param>
    /// <param name="ratio">ratio of dest/start, between 0 and 1</param>
    /// <returns></returns>
    public static float Interpolate(float source, float dest, float ratio)
    {
        return source + ((dest - source) * ratio);
    }
    /// <summary>
    /// linear interpolation from source to dest by ratio amount
    /// </summary>
    /// <param name="source">starting point of interpolation</param>
    /// <param name="dest">ending point of interpolation</param>
    /// <param name="ratio">ratio of dest/start, between 0 and 1</param>
    /// <returns></returns>
    public static Vector3 Interpolate(Vector3 source, Vector3 dest, float ratio)
    {
        return new Vector3
            (
            Interpolate(source.x, dest.x, ratio),
            Interpolate(source.y, dest.y, ratio),
            Interpolate(source.z, dest.z, ratio)
            );
    }

    /// <summary>
    /// Get the mouse position as it intersects with this board
    /// </summary>
    /// <returns></returns>
    public static Vector3 MousePosPlane()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -Camera.main.transform.position.z; // z is distance from camera to plane
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    /// <summary>
    /// Destroys an object if it isnt null, works in both runtime and edit mode
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool TryDestroy(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            if (Application.isEditor)
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            else
            {
                UnityEngine.Object.Destroy(obj);
            }
            return true;
        }
        return false;
    }

}
