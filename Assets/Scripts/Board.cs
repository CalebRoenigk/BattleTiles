using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public Domino Root;
    public List<Domino> OpenDominos = new List<Domino>();
    public List<DominoInterface> OpenInterfaces = new List<DominoInterface>();
    public List<int> OpenValues = new List<int>();

    public void PlaceRoot(Domino domino)
    {
        Root = domino;
        OpenDominos.Add(domino);
    }

    public void AddDomino(Domino domino, DominoSide placementSide, Domino connection, DominoSide connectionSide)
    {
        OpenDominos.Add(domino);
        domino.ConnectSide(placementSide);
        connection.ConnectSide(connectionSide);

        if (!connection.HasOpenConnections())
        {
            OpenDominos.Remove(connection);
        }
    }

    public void CacheOpenInterfaces()
    {
        OpenInterfaces.Clear();
        foreach (var openDomino in OpenDominos)
        {
            OpenInterfaces.AddRange(openDomino.GetAllOpenInterfaces());
        }

        OpenValues = GetOpenValues();
    }

    public List<DominoInterface> GetMatchedOpenInterfaces(int matchValueA = -1, int matchValueB = -1)
    {
        List<DominoInterface> openInterfaces = new List<DominoInterface>();
        if (matchValueA != -1 || matchValueB != -1)
        {
            List<DominoInterface> matchesCulled = new List<DominoInterface>();
            List<DominoInterface> matchedA = OpenInterfaces.Where(i => i.Value == matchValueA).ToList();
            if (matchValueA != -1)
            {
                matchesCulled.AddRange(matchedA);
            }
        
            List<DominoInterface> matchedB = OpenInterfaces.Where(i => i.Value == matchValueB).ToList();
            if (matchValueB != -1)
            {
                matchesCulled.AddRange(matchedB);
            }

            openInterfaces = matchesCulled.Distinct().ToList();
        }
        
        return openInterfaces;
    }

    public List<int> GetOpenValues()
    {
        List<int> openValues = new List<int>();
        foreach (var openInterface in OpenInterfaces)
        {
            if (!openValues.Contains(openInterface.Value))
            {
                openValues.Add(openInterface.Value);
            }
        }

        return openValues;
    }
}
