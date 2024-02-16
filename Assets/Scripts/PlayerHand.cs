using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEditor;

public class PlayerHand : MonoBehaviour
{
    public Player Player;
    public List<Domino> Hand = new List<Domino>();

    [SerializeField] private bool isClosed = true;
    [SerializeField] private float _openMaxLength = 6f;
    [SerializeField] private float _closeMaxLength = 3f;
    [SerializeField] private float _openMaxSpacing = 2.1f;
    [SerializeField] private float _closeMaxSpacing = 1f;
    
    [SerializeField] private int _handIndex;

    public void SetPlayer(Player player)
    {
        this.Player = player;
        _handIndex = player.Index;
    }

    public void SetState(bool state, bool forceReorg = false)
    {
        if (state != isClosed || forceReorg)
        {
            // Iterate over points in reorg positions
            List<Vector3> handPositions = GetHandPositions();
            for (int i = 0; i < Hand.Count; i++)
            {
                // Hand[i].transform.position = handPositions[i];
                Hand[i].transform.DOKill();
                Hand[i].transform.DOMove(handPositions[i], 0.2f).SetEase(Ease.InOutSine);
            }
        }

        foreach (Domino domino in Hand)
        {
            domino.CanInteract = !state;
            if (state)
            {
                domino.MouseOn = false;
            }
            
            // Test if each domino can be interacted with given the open interfaces
            if (domino.CanInteract)
            {
                if (!domino.Values.HasMatchingValue(GameManager.Instance.Board.OpenValues))
                {
                    domino.CanInteract = false;
                }
            }
        }

        isClosed = state;
    }

    public void AddDomino(Domino domino, float delay = 0f)
    {
        Hand.Add(domino);
        domino.ParentHand = this;
        domino.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        // List<Vector3> handPositions = GetHandPositions();

        // Vector3 handPos = handPositions[handPositions.Count - 1] + new Vector3(0f, 2.17647f, 0f);
        // if (handPos.x != Single.NaN)
        // {
        //     // Grab a free Sequence to use
        //     Sequence dominoToHandTween = DOTween.Sequence();
        //     // Add a movement tween at the beginning
        //     dominoToHandTween.Append(domino.transform.DOMove(handPos, 0.25f).SetEase(Ease.OutCubic).SetDelay(delay));
        //     dominoToHandTween.Insert(0f,domino.transform.DOLocalRotate(new Vector3(90f, 0f, 0f), 0.25f).SetEase(Ease.OutCubic).SetDelay(delay));
        // }
        SetState(isClosed, true);
    }

    public Domino RemoveDomino(Domino domino)
    {
        Hand.Remove(domino);
        domino.ParentHand = null;
        SetState(isClosed, true);
        return domino;
    }

    public List<Vector3> GetHandPositions()
    {
        List<Vector3> handPositions = new List<Vector3>();
        float maxLength = isClosed ? _openMaxLength : _closeMaxLength;
        float maxLengthHalf = maxLength / 2f;
        Vector3 startPoint = new Vector3(-maxLengthHalf, 0f, 0f);
        Vector3 endPoint = new Vector3(maxLengthHalf, 0f, 0f);
        float stepSize = Mathf.Min(isClosed ? _openMaxSpacing : _closeMaxSpacing, Vector3.Distance(startPoint, endPoint) / (Hand.Count - 1));
        bool mustStagger = stepSize < 2.1f;
        Vector3 centeringOffset = Vector3.zero;
        if (stepSize * Hand.Count < (maxLength))
        {
            centeringOffset.x = (maxLength - (stepSize * Hand.Count))/2f;
        }
        
        for (int i = 0; i < Hand.Count; i++)
        {
            float t = i / (float)(Hand.Count - 1);
            Vector3 point = Vector3.Lerp(startPoint, endPoint, t) + centeringOffset;

            if (mustStagger && i % 2 != 0)
            {
                point.z += 0.8f;
            }
            
            handPositions.Add(transform.TransformPoint(point));
        }

        return handPositions;
    }

    private void OnDrawGizmos()
    {
        // List<Vector3> handPositions = GetHandPositions();
        //
        // Gizmos.color = Color.cyan;
        // Handles.color = Color.black;
        // int i = 0;
        // foreach (Vector3 handPosition in handPositions)
        // {
        //     Gizmos.DrawSphere(handPosition, 0.125f);
        //     Handles.Label(handPosition, i.ToString());
        //     i++;
        // }
    }

    public Domino DominoSelected()
    {
        int dominoIndex = Hand.FindIndex(d => d.MouseOn);

        if (dominoIndex == -1)
        {
            return null;
        }

        return Hand[dominoIndex];
    }

    public void ClearMouseSelection(Domino excludeDomino)
    {
        foreach (var domino in Hand)
        {
            if (domino != excludeDomino)
            {
                domino.ResetMouseState();
            }
        }
    }

    public bool CheckMatchingDominos()
    {
        return Hand.Where(d => d.CanInteract).ToList().Count != 0;
    }
}
