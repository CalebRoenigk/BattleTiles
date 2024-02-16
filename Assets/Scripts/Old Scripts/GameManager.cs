using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    // public static GameManager Instance;
    //
    // public int PlayerTurn = -1;
    // public Dictionary<int, Player> Players = new Dictionary<int, Player>();
    //
    // [SerializeField] private GameObject _dominoPrefab;
    // [SerializeField] private GameObject _playerHandPrefab;
    // [SerializeField] private GameObject _dominoGhostPrefab;
    //
    // [Header("Camera")]
    // [SerializeField] private Transform _cameraGroup;
    // [SerializeField] private CinemachineTargetGroup _boardTargetGroup;
    //
    // [Header("Game Setup Variables")]
    // [SerializeField] private int _playerCount = 2;
    // [SerializeField] private int _startingHealth = 100;
    // [SerializeField] private int _startingHandSize = 5;
    //
    // [Header("Players")]
    // [SerializeField] private Transform _handsParent;
    //
    // [Header("Boneyard")]
    // public Boneyard Boneyard;
    //
    // [Header("Board")]
    // public Board Board;
    // [SerializeField] private Transform _boardParent;
    // [SerializeField] private float _rotateBoard = 0f;
    // [SerializeField] private float _rotationSpeedRamp = 20f;
    // [SerializeField] private float _rotationSpeedMax = 125f;
    // [SerializeField] private float _rotationSpeedSlow = 0.05f;
    // [SerializeField] private List<GameObject> _ghosts = new List<GameObject>();
    //
    // private void Awake() 
    // { 
    //     // If there is an instance, and it's not me, delete myself.
    //
    //     if (Instance != null && Instance != this) 
    //     { 
    //         Destroy(this); 
    //     } 
    //     else 
    //     { 
    //         Instance = this; 
    //     } 
    // }
    //
    // private void Start()
    // {
    //     // Create players
    //     Boneyard = new Boneyard();
    //     Board = new Board();
    //     CreatePlayers(_playerCount);
    //     DealDominos(_startingHandSize);
    //     int player = PlaceHighestDominoFromHands();
    //     PlayerTurn = player;
    //     NextPlayer();
    // }
    //
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         NextPlayer();
    //     }
    //
    //     bool rotationPressed = false;
    //     if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
    //     {
    //         _rotateBoard += _rotationSpeedRamp * Time.deltaTime;
    //         rotationPressed = true;
    //     }
    //     if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
    //     {
    //         _rotateBoard -= _rotationSpeedRamp * Time.deltaTime;
    //         rotationPressed = true;
    //     }
    //
    //     _rotateBoard = Mathf.Clamp(_rotateBoard, -_rotationSpeedMax, _rotationSpeedMax);
    //
    //     if (!rotationPressed)
    //     {
    //         _rotateBoard = Mathf.Lerp(_rotateBoard, 0f, _rotationSpeedSlow);
    //         if (Mathf.Abs(_rotateBoard) <= 0.01f)
    //         {
    //             _rotateBoard = 0f;
    //         }
    //     }
    //     
    //     Vector3 euler = _boardParent.localRotation.eulerAngles;
    //     euler.y += _rotateBoard;
    //     _boardParent.localRotation = Quaternion.Euler(euler);
    //     
    //     // Update Ghosts
    //     UpdateGhosts();
    // }
    //
    // // Creates the players for the game
    // private void CreatePlayers(int count)
    // {
    //     for (int i = 0; i < count; i++)
    //     {
    //         Vector3 rotation = Quaternion.LookRotation(GetPlayerHandPosition(i)*1.5f - GetPlayerHandPosition(i), Vector3.up).eulerAngles;
    //         PlayerHand playerHand = Instantiate(_playerHandPrefab, GetPlayerHandPosition(i), Quaternion.Euler(0f, rotation.y, 0f), _handsParent).GetComponent<PlayerHand>();
    //         Player player = new Player(i, _startingHealth, playerHand);
    //         playerHand.SetPlayer(player);
    //         Players.Add(i, player);
    //     }
    // }
    //
    // private void DealDominos(int handSize)
    // {
    //     Camera mainCamera = Camera.main;
    //     Vector3 offscreenBoneyardPosition = mainCamera.ScreenToWorldPoint(new Vector3(-mainCamera.pixelWidth/4f, mainCamera.pixelHeight/2f, 10f));
    //     offscreenBoneyardPosition.y = 0f;
    //     for (int i = 0; i < handSize; i++)
    //     {
    //         for (int p = 0; p < _playerCount; p++)
    //         {
    //             // TODO: Likely need to methodize this code and put it somewhere where it can be used to draw dominos no matter the context
    //             Domino domino = Instantiate(_dominoPrefab, offscreenBoneyardPosition, Quaternion.identity, Players[p].PlayerHand.transform).GetComponent<Domino>();
    //             ValuePair pair = Boneyard.DrawDomino();
    //             domino.SetDomino(pair);
    //             Players[p].AddToHand(domino, i*0.05f);
    //         }
    //     }
    // }
    //
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.cyan;
    //     for (int i = 0; i < _playerCount; i++)
    //     {
    //         Gizmos.DrawSphere(GetPlayerHandPosition(i), 0.25f);
    //     }
    // }
    //
    // private Vector3 GetPlayerHandPosition(int i)
    // {
    //     float playerDegreeSeparation = Mathf.Deg2Rad * (360f / (float)_playerCount);
    //     float playerPositionOffset = 12f;
    //     
    //     Vector3 playerPosition = new Vector3(Mathf.Cos(playerDegreeSeparation * i) * playerPositionOffset, 0f, Mathf.Sin(playerDegreeSeparation * i) * playerPositionOffset);
    //     
    //     return playerPosition;
    // }
    //
    // private void NextPlayer()
    // {
    //     PlayerTurn++;
    //     PlayerTurn %= _playerCount;
    //     Board.CacheOpenInterfaces();
    //     _cameraGroup.DORotate(new Vector3(0, -((360f / (float)_playerCount) * ((float)PlayerTurn) + 90f) , 0f), 0.625f).SetEase(Ease.InOutCubic);
    //     for (int i = 0; i < _playerCount; i++)
    //     {
    //         Players[i].PlayerHand.SetState(i != PlayerTurn);
    //     }
    //
    //     if (!Players[PlayerTurn].PlayerHand.CheckMatchingDominos())
    //     {
    //         // There are no matches in this hand, draw one domino then next turn
    //         Camera mainCamera = Camera.main;
    //         Vector3 offscreenBoneyardPosition = mainCamera.ScreenToWorldPoint(new Vector3(-mainCamera.pixelWidth/4f, mainCamera.pixelHeight/2f, 10f));
    //         offscreenBoneyardPosition.y = 0f;
    //         Domino domino = Instantiate(_dominoPrefab, offscreenBoneyardPosition, Quaternion.identity, Players[PlayerTurn].PlayerHand.transform).GetComponent<Domino>();
    //         domino.gameObject.name = domino.Values.ToString();
    //         ValuePair pair = Boneyard.DrawDomino();
    //         domino.SetDomino(pair);
    //         Players[PlayerTurn].AddToHand(domino);
    //         Invoke("NextPlayer", 2f);
    //     }
    // }
    //
    // // Places the starting domino and then returns the index of the player that held the domino
    // private int PlaceHighestDominoFromHands()
    // {
    //     // First look for the highest double
    //     int player = -1;
    //     Domino highestDomino = null;
    //     for (int i = 0; i < _playerCount; i++)
    //     {
    //         // Get all double value pairs
    //         List<Domino> doubles = Players[i].PlayerHand.Hand.FindAll(d => d.Values.IsDouble);
    //         foreach (var doubleDomino in doubles)
    //         {
    //             if (highestDomino == null || doubleDomino.Values.IsGreater(highestDomino.Values))
    //             {
    //                 highestDomino = doubleDomino;
    //                 player = i;
    //             }
    //         }
    //     }
    //
    //     if (highestDomino == null)
    //     {
    //         for (int i = 0; i < _playerCount; i++)
    //         {
    //             // Get all value pairs
    //             List<Domino> pairs = Players[i].PlayerHand.Hand;
    //             foreach (var pair in pairs)
    //             {
    //                 if (highestDomino == null || pair.Values.IsGreater(highestDomino.Values))
    //                 {
    //                     highestDomino = pair;
    //                     player = i;
    //                 }
    //             }
    //         }
    //     }
    //
    //     Board.PlaceRoot(highestDomino);
    //     highestDomino.RemoveFromPlayerHand();
    //     MoveDominoToPlacement(highestDomino, Vector3.zero, Quaternion.Euler(0f, 0f, 0f));
    //     return player;
    // }
    //
    // public void PlaceDomino(Domino dominoToPlace, DominoInterface connectingInterface)
    // {
    //     // Remove the domino from the player hand
    //     Board.PlaceRoot(dominoToPlace);
    //     dominoToPlace.RemoveFromPlayerHand();
    //
    //     Vector3 interfacePosition = connectingInterface.Parent.GetInterfacePosition(connectingInterface.Side);
    //     MoveDominoToPlacement(dominoToPlace, interfacePosition, Quaternion.LookRotation(connectingInterface.Parent.transform.position - interfacePosition, Vector3.up));
    // }
    //
    // private void MoveDominoToPlacement(Domino domino, Vector3 placement, Quaternion rotation)
    // {
    //     domino.gameObject.layer = 7;
    //     _boardTargetGroup.AddMember(domino.transform, 1f, 1f);
    //     domino.transform.DOKill();
    //     // domino.transform.DOMove(placement, 0.375f).SetEase(Ease.InOutQuad);
    //     // domino.transform.DORotate(rotation.eulerAngles, 0.375f).SetEase(Ease.InOutQuad);
    //     domino.transform.parent = _boardParent;
    //     domino.transform.position = placement;
    //     domino.transform.rotation = rotation;
    //     
    //     domino.UpdatePoints();
    //     domino.ToggleGizmos = true;
    // }
    //
    // private void UpdateGhosts()
    // {
    //     for (int i = _ghosts.Count - 1; i >= 0; i--)
    //     {
    //         Destroy(_ghosts[i], 0f);
    //     }
    //
    //     Domino selectedDomino = Players[PlayerTurn].PlayerHand.DominoSelected();
    //     if (selectedDomino != null)
    //     {
    //         List<DominoInterface> openMatchedInterfaces = Board.GetMatchedOpenInterfaces(selectedDomino.Values.ValueA, selectedDomino.Values.ValueB);
    //
    //         foreach (var openMatchedInterface in openMatchedInterfaces)
    //         {
    //             Vector3 openMatchedInterfacePosition = openMatchedInterface.Parent.GetInterfacePosition(openMatchedInterface.Side);
    //             DominoGhost dominoGhost = Instantiate(_dominoGhostPrefab, openMatchedInterfacePosition, Quaternion.LookRotation(openMatchedInterfacePosition - openMatchedInterface.Parent.transform.position, Vector3.up), _boardParent).GetComponent<DominoGhost>();
    //             dominoGhost.SetDomino(openMatchedInterface.Value);
    //             _ghosts.Add(dominoGhost.gameObject);
    //         }
    //     }
    // }
    //
    // public void RequestPlaceDomino(Domino domino)
    // {
    //     List<DominoInterface> openMatchedInterfaces = Board.GetMatchedOpenInterfaces(domino.Values.ValueA, domino.Values.ValueB);
    //     int randomInterfaceIndex = (int)Mathf.Round(UnityEngine.Random.Range(0, openMatchedInterfaces.Count - 1));
    //
    //     DominoInterface selectedInterface = openMatchedInterfaces[randomInterfaceIndex];
    //     
    //     PlaceDomino(domino, selectedInterface);
    //     Board.AddDomino(domino, domino.GetMatchingInterface(selectedInterface).Side, selectedInterface.Parent, selectedInterface.Side);
    //     
    //     NextPlayer();
    // }
}
