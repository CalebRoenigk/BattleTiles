using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game Setup Variables")]
    public int PlayerCount = 4;
    [SerializeField] private int _startingHealth = 100;
    [SerializeField] private int _startingHandSize = 5;

    [Header("State")]
    public int PlayerTurn = -1;
    
    [Header("Players")]
    public Dictionary<int, Player> Players = new Dictionary<int, Player>();
    [SerializeField] private Transform _playerHandsParent;
    [SerializeField] private GameObject _playerHandPrefab;
    
    [Header("Boneyard")]
    public Boneyard Boneyard;
    
    [Header("Board")]
    public Board Board;

    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        Boneyard = new Boneyard();
        Board = new Board();
        
        CreatePlayers(PlayerCount);

        DealTiles(_startingHandSize);
        
        PlayerTurn = PlaceHighestTileFromHands();
        
        NextPlayer();
    }

    private void CreatePlayers(int playerCount)
    {
        for (int i = 0; i < playerCount; i++) 
        {
            Vector3 rotation = Quaternion.LookRotation(GetPlayerHandPosition(i)*1.5f - GetPlayerHandPosition(i), Vector3.up).eulerAngles;
            Player player = new Player(i, _startingHealth);
            PlayerHandVisual playerHandVisual = Instantiate(_playerHandPrefab, GetPlayerHandPosition(i), Quaternion.Euler(0f, rotation.y, 0f), _playerHandsParent).GetComponent<PlayerHandVisual>();
            playerHandVisual.SetHand(player.Hand);
            Players.Add(i, player);
        }
    }
    
    private Vector3 GetPlayerHandPosition(int i)
    {
        float playerDegreeSeparation = Mathf.Deg2Rad * (360f / (float)PlayerCount);
        float playerPositionOffset = 12f;
        
        Vector3 playerPosition = new Vector3(Mathf.Cos(playerDegreeSeparation * i) * playerPositionOffset, 0f, Mathf.Sin(playerDegreeSeparation * i) * playerPositionOffset);
        
        return playerPosition;
    }
    
    private void DealTiles(int handSize)
    {
        for (int i = 0; i < handSize; i++)
        {
            for (int p = 0; p < PlayerCount; p++)
            {
                Players[p].Hand.DrawTile();
            }
        }
    }

    public Tile DrawTile()
    {
        return Boneyard.DrawTile();
    }
    
    // Returns the highest tile from all hands
    private Tile GetHighestTileFromHands()
    {
        // Look for the highest double and non double
        Tile highestTile = null;
        Tile highestNonDouble = null;
        for (int i = 0; i < PlayerCount; i++)
        {
            // Get all double value pairs
            List<Tile> doubles = Players[i].Hand.Tiles.FindAll(t => t.IsDouble());
            List<Tile> nonDoubles = Players[i].Hand.Tiles.FindAll(t => !t.IsDouble());
            foreach (var doubleTile in doubles)
            {
                if (highestTile == null || doubleTile.IsGreater(highestTile))
                {
                    highestTile = doubleTile;
                }
            }
            
            foreach (var nonDoubleTile in nonDoubles)
            {
                if (highestNonDouble == null || nonDoubleTile.IsGreater(highestNonDouble))
                {
                    highestNonDouble = nonDoubleTile;
                }
            }
        }

        if (highestTile != null)
        {
            // Place the highest Tile
            return highestTile;
        }
        else
        {
            return highestNonDouble;
        }
    }

    private int PlaceHighestTileFromHands()
    {
        Tile highestTile = GetHighestTileFromHands();
        
        Board.PlaceRoot(highestTile);
        return highestTile.Owner.Index;
    }
    
    private void NextPlayer()
    {
        PlayerTurn++;
        PlayerTurn %= PlayerCount;
        Board.UpdateCache();
        CameraManager.Instance.NextTurn();
        for (int i = 0; i < PlayerCount; i++)
        {
            Players[i].Hand.HandVisual.SetViewingState(i != PlayerTurn);
        }

        PreTurnChecks();
    }

    // Checks to do before the turn starts, things like checks for hands that cannot place a tile, etc.
    private void PreTurnChecks()
    {
        // TODO: FIX THIS
        // if (!Players[PlayerTurn].PlayerHand.CheckMatchingDominos())
        // {
        //     // There are no matches in this hand, draw one domino then next turn
        //     Camera mainCamera = Camera.main;
        //     Vector3 offscreenBoneyardPosition = mainCamera.ScreenToWorldPoint(new Vector3(-mainCamera.pixelWidth/4f, mainCamera.pixelHeight/2f, 10f));
        //     offscreenBoneyardPosition.y = 0f;
        //     Domino domino = Instantiate(_dominoPrefab, offscreenBoneyardPosition, Quaternion.identity, Players[PlayerTurn].PlayerHand.transform).GetComponent<Domino>();
        //     domino.gameObject.name = domino.Values.ToString();
        //     ValuePair pair = Boneyard.DrawDomino();
        //     domino.SetDomino(pair);
        //     Players[PlayerTurn].AddToHand(domino);
        //     Invoke("NextPlayer", 2f);
        // }
    }
}