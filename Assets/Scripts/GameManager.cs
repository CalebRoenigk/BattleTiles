using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Game Setup Variables")]
    public int PlayerCount = 4;
    [SerializeField] private int _startingHealth = 100;
    [SerializeField] private int _startingHandSize = 5;

    [Header("Scoring and Health")]
    [SerializeField] private int _healthCardValue = 5;
    [SerializeField] private int _damageCardValue = 8;

    [Header("State")]
    public int PlayerTurn = -1;
    public int TurnCount = -1;
    
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("Scenes/SampleScene");
        }
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
            Player player = new Player(i, _startingHealth);
            PlayerHandVisual playerHandVisual = Instantiate(_playerHandPrefab, Vector3.zero, Quaternion.identity, _playerHandsParent).GetComponent<PlayerHandVisual>();
            playerHandVisual.SetHand(player.Hand);
            HandViewManager.Instance.AddHand(playerHandVisual);
            Players.Add(i, player);
        }
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

        foreach (Player player in Players.Values)
        {
            player.Hand.ForceHideHand();
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

    // Places the highest tile currently in a hand and returns the index of the player that owned the tile
    private int PlaceHighestTileFromHands()
    {
        Tile highestTile = GetHighestTileFromHands();

        Board.PlaceTile(highestTile, true);
        return highestTile.Owner.Index;
    }
    
    private void NextPlayer()
    {
        TurnCount++;
        PlayerTurn++;
        PlayerTurn %= PlayerCount;

        PreTurnChecks();
    }

    private int LastPlayerIndex()
    {
        int lastPlayerIndex = PlayerTurn - 1;
        if (lastPlayerIndex < 0)
        {
            lastPlayerIndex = PlayerCount - 1;
        }

        return lastPlayerIndex;
    }
    
    private int NextPlayerIndex()
    {
        int nextPlayerIndex = PlayerTurn + 1;
        nextPlayerIndex %= PlayerCount;

        return nextPlayerIndex;
    }

    // Checks to do before the turn starts, things like checks for hands that cannot place a tile, etc.
    private void PreTurnChecks()
    {
        // Update the Temp UI
        TemporaryUI.Instance.SetPlayerStats("Player " + PlayerTurn, Players[PlayerTurn].Health, Players[PlayerTurn].Score);
        // Debug.Log("Preturn Checks!");
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
        
        // At the end of preturn checks, trigger the start of the turn
        Players[PlayerTurn].StartTurn();
    }

    private void PostTurnChecks()
    {
        // TODO: Do any post turn checks
        // Update the board cache
        Board.UpdateSelection(null);
        Board.UpdateCache();
        
        // Collect all open interfaces to tally up the score
        Dictionary<int, List<Interface>> groupedOpenInterfaces = Board.GetGroupedOpenInterfaces();
        
        // Filter the dictonary so that only interfaces with 3 or more of a value remain
        groupedOpenInterfaces.Where(kvp => kvp.Value.Count >= 3).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        // For each remaining group tally up the total damage for that group
        List<DamageTally> damageTallies = new List<DamageTally>();
        foreach (var groupedInterfaces in groupedOpenInterfaces)
        {
            int interfaceValue = groupedInterfaces.Key;
            int sourcePlayer = PlayerTurn;
            List<Interface> sources = groupedInterfaces.Value;
            int damage;
            List<int> effected = new List<int>();

            if (interfaceValue == 5)
            {
                // Heal the player
                effected.Add(PlayerTurn);
                damage = sources.Count * -_healthCardValue;
            }
            else
            {
                // Do damage to all other players
                effected.AddRange(GetAllPlayerIndices(true));
                damage = sources.Count * _damageCardValue;
            }
            
            damageTallies.Add(new DamageTally(damage, sourcePlayer, effected, sources));
        }
        
        // Apply that damage to all effected players (outlined by the damage values class)
        ApplyDamages(damageTallies);
        
        // Update the player UI
        TemporaryUI.Instance.SetPlayerStats("Player " + PlayerTurn, Players[PlayerTurn].Health, Players[PlayerTurn].Score);

        // End the turn of the current player
        Players[PlayerTurn].EndTurn();

        // Start the next turn
        NextPlayer();
    }

    // Called after a tile placement is finished
    public void EndTurn()
    {
        // TODO: Score/Dmg/etc? Or do I put all of those in post turn checks?
        PostTurnChecks();
    }

    public void TryPlaceTile(Tile tile)
    {
        if (Board.PrimaryMatch != null)
        {
            Board.PlaceTile(tile);
        }
        
        // TODO: Maybe add some kinda error state to show the player they cant place that tile
    }

    // Updates the ghost selection on the board
    public void UpdateSelection(Tile tile)
    {
        Board.UpdateSelection(tile);
    }
    
    // Returns a list of the turn indices on either side of the current player turn
    public List<int> GetPlayerTurnsReference()
    {
        List<int> playerTurns = new List<int>();
        playerTurns.Add(LastPlayerIndex());
        playerTurns.Add(PlayerTurn);
        playerTurns.Add(NextPlayerIndex());

        return playerTurns;
    }

    private List<int> GetAllPlayerIndices(bool excludeCurrentPlayer = false)
    {
        List<int> playerIndices = new List<int>();
        foreach (var playerDictEntry in Players)
        {
            if (!(excludeCurrentPlayer && playerDictEntry.Key == PlayerTurn))
            {
                playerIndices.Add(playerDictEntry.Key);
            }
        }

        return playerIndices;
    }

    private void ApplyDamages(List<DamageTally> damageTallies)
    {
        foreach (DamageTally damageTally in damageTallies)
        {
            foreach (int effectedPlayerIndex in damageTally.PlayersEffected)
            {
                Players[effectedPlayerIndex].DoDamage(damageTally.Damage);
            }
        }
    }
}