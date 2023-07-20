using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour
{
	private int[,] pawnBaseValue =	  {{ 100, 50, 10, 5, 0,  5,  5,0},
										{100, 50, 10, 5, 0, -5, 10,0},
										{100, 50, 20,10, 0,-10, 10,0},
										{100, 50, 30,25,20,  0,-20,0},
										{100, 50, 30,25,20,  0,-20,0},
										{100, 50, 20,10, 0,-10, 10,0},
										{100, 50, 10, 5, 0, -5, 10,0},
										{100, 50, 10, 5, 0,  5,  5,0}};
	private int[,] rookBaseValue =		{	{   0,  5,  -5,  -5,  -5,  -5,  -5,  0},
											{   0, 10,   0,   0,   0,   0,   0,  0},
											{   0, 10,   0,   0,   0,   0,   0,  0},
											{   0, 10,   0,   0,   0,   0,   0,  5},
											{   0, 10,   0,   0,   0,   0,   0,  5},
											{   0, 10,   0,   0,   0,   0,   0,  0},
											{   0, 10,   0,   0,   0,   0,   0,  0},
											{   0,  5,  -5,  -5,  -5,  -5,  -5,  0}};
	private int[,] knightBaseValue =     {	{  -50,-40,-30,-30,-30,-30,-40,-50},
											{  -40,-20,  0,  0,  0,  5,-20,-40},
											{  -30,  0, 10, 15, 15, 10,  0,-30},
											{  -30,  0, 15, 20, 20, 15,  5,-30},
											{  -30,  0, 15, 20, 20, 15,  5,-30},
											{  -30,  0, 10, 15, 15, 10,  0,-30},
											{  -40,-20,  0,  0,  0,  5,-20,-40},
											{  -50,-40,-30,-30,-30,-30,-40,-50}};
	private int[,] bishopBaseValue =     {  {  -20,-10,-10,-10,-10,-10,-10,-20},
											{  -10,  0,  0,  5,  0, 10,  5,-10},
											{  -10,  0,  5,  5, 10, 10,  0,-10},
											{  -10,  0, 10, 10, 10, 10,  0,-10},
											{  -10,  0, 10, 10, 10, 10,  0,-10},
											{  -10,  0,  5,  5, 10, 10,  0,-10},
											{  -10,  0,  0,  5,  0, 10,  5,-10},
											{  -20,-10,-10,-10,-10,-10,-10,-20}};
	private int[,] queenBaseValue =      {	{  -20,-10,-10, -5, -5,-10,-10,-20},
											{  -10,  0,  0,  0,  0,  0,  0,-10},
											{  -10,  0,  5,  5,  5,  5,  0,-10},
											{   -5,  0,  5,  5,  5,  5,  0, -5},
											{   -5,  0,  5,  5,  5,  5,  0, -5},
											{  -10,  0,  5,  5,  5,  5,  5,-10},
											{  -10,  0,  0,  0,  0,  5,  0,-10},
											{  -20,-10,-10, -5,  0,-10,-10,-20}};
	private int[,] kingBaseValue =       {  {  -30,-30,-30,-30,-20,-10, 20, 20},
											{  -40,-40,-40,-40,-30,-20, 20, 30},
											{  -40,-40,-40,-40,-30,-20,  0, 10},
											{  -50,-50,-50,-50,-40,-20,  0,  0},
											{  -50,-50,-50,-50,-40,-20,  0,  0},
											{  -40,-40,-40,-40,-30,-20,  0, 10},
											{  -40,-40,-40,-40,-30,-20, 20, 30},
											{  -30,-30,-30,-30,-20,-10, 20, 20}};
	public GameManager gameManager;
	public PieceManager pieceManager;
	private List<BasePiece> aiPawns = new List<BasePiece>();
	private List<BasePiece> aiRoyals = new List<BasePiece>();
	private King aiKing;
	private List<BasePiece> aiAllPieces = new List<BasePiece>();

	private List<BasePiece> playerPawns = new List<BasePiece>();
	private List<BasePiece> playerRoyals = new List<BasePiece>();
	private King playerKing;
	public void AIMove()
	{
		aiKing.CheckPath();
		Cell castle = aiKing.CanCastle();
		if (castle != null) 
		{
			aiKing.targetCell = castle;
			aiKing.MoveHelper();
			pieceManager.SwitchSide(Color.black);
			return;
		}
		aiKing.CleanCells();
	}
}