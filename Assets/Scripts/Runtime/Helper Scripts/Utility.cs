using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Utils
{


    private static int _groundLayerMask = -1;
    public static int GroundLayerMask
    {
        get
        {
            if (_groundLayerMask == -1)
            {
                _groundLayerMask = LayerMask.GetMask("Ground");
            }
            return _groundLayerMask;
        }

    }

    private static int _playerLayerMask = -1;
    public static int PlayerLayerMask
    {
        get
        {
            if (_playerLayerMask == -1)
            {
                _playerLayerMask = LayerMask.GetMask("Player");
            }
            return _playerLayerMask;
        }

    }

    private static int _playerLayer = -1;
    public static int PlayerLayer
    {
        get
        {
            if (_playerLayer == -1)
            {
                _playerLayer = LayerMask.NameToLayer("Player");
            }
            return _playerLayer;
        }

    }

    public static int WorldPositionToTile(float pos)
    {

        if (pos >= 0)
        {
            return Mathf.FloorToInt(pos);
        }
        else
        {
            return -Mathf.CeilToInt(-pos);
        }

    }

    public static Vector2 WorldPositionToTile(Vector2 pos)
    {

        return new Vector2(WorldPositionToTile(pos.x), WorldPositionToTile(pos.y));

    }

    public static Vector2 TileToWorldPosition(int x, int y)
    {
        return new Vector2(0.5f + x, 0.5f + y);

    }

    public static float TileToWorldPosition(int x)
    {
        return 0.5f + x;

    }


    static public T AddArrayElement<T>(ref T[] array, T elToAdd)
    {
        if (array == null)
        {
            array = new T[1];
            array[0] = elToAdd;
            return elToAdd;
        }

        var newArray = new T[array.Length + 1];
        array.CopyTo(newArray, 0);
        newArray[array.Length] = elToAdd;
        array = newArray;
        return elToAdd;
    }

    static public void DeleteArrayElement<T>(ref T[] array, int index)
    {
        if (index >= array.Length || index < 0)
        {
            Debug.LogWarning("invalid index in DeleteArrayElement: " + index);
            return;
        }
        var newArray = new T[array.Length - 1];
        int i;
        for (i = 0; i < index; i++)
        {
            newArray[i] = array[i];
        }
        for (i = index + 1; i < array.Length; i++)
        {
            newArray[i - 1] = array[i];
        }
        array = newArray;
    }

    public static Vector2 Rotate2D(Vector2 vector, float angle)
    {

        angle = Mathf.Deg2Rad*angle;
        return new Vector2(vector.x * Mathf.Cos(angle) - vector.y * Mathf.Sin(angle),
            vector.x * Mathf.Sin(angle) + vector.y * Mathf.Cos(angle));
    }

    public static int Wrap(int value, int minInclusive, int maxExclusive)
    {
        if (value >= maxExclusive) value = minInclusive;
        if (value < minInclusive) value = maxExclusive - 1;

        return value;
    }

}