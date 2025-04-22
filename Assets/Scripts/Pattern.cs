
using UnityEngine;

//skapar menyval f�r att skapa ett pattern
[CreateAssetMenu(menuName = "Game of Life/Pattern")]
public class Pattern : ScriptableObject
{
    //"koordinat"
    public Vector2Int [] cells;

    //centrera
    public Vector2Int GetCenter()
    {
        if (cells == null || cells.Length == 0)
        {
            return Vector2Int.zero;
        }

        Vector2Int min = Vector2Int.zero;
        Vector2Int max = Vector2Int.zero;

        for (int i = 0; i < cells.Length; i++)
        {
            //lagra current cell
            Vector2Int cell = cells[i];
            //j�mf�r, tar l�gsta v�rdet
            min.x = Mathf.Min(cell.x, min.x);
            min.y = Mathf.Min(cell.y, min.y);
            //j�mf�r, tar h�gsta v�rdet
            max.x = Mathf.Max(cell.x, max.x);
            max.y = Mathf.Max(cell.y, max.y);
        }
        //mitten
        return (min + max / 2);

        //flytta upp lite?

    }

}
