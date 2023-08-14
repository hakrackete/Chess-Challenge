﻿using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        // alle moves durchsuchen, ob irgendetwas davon checkmate ist

        // Piece values: null, pawn, knight, bishop, rook, queen, king
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        Move[] moves = board.GetLegalMoves();
        Random rng = new();
        Move moveToReturn = moves[rng.Next(moves.Length)];
        int highestCapture = 0;

        foreach (Move move in moves)
        {
            if(MoveIsCheckmate(board,move)){
                moveToReturn = move;
                break;
            }

        // wenn keine, dann durchsuchen, welche Moves ein pieve capturen, dabei den höchsten pieceValue beachten
            if(pieceValues[(int)move.CapturePieceType] > highestCapture ){
                highestCapture = pieceValues[(int)move.CapturePieceType];
                Console.WriteLine(highestCapture);
                moveToReturn = move;
            }

        }
        // wenn keine captures, dann versuchen bauern zu advancen?
        int lowestDistance = 7;
        if(highestCapture == 0){
            foreach(Move pawnmove in moves){
                Piece myPiece = board.GetPiece(pawnmove.StartSquare);
                if(myPiece.IsPawn == false){
                    continue;
                }
                if (board.SquareIsAttackedByOpponent(pawnmove.TargetSquare)){
                    if(MoveIsGuarded(board,pawnmove) == false){
                        continue;
                    }

                }
                if(pawnmove.IsPromotion){
                    moveToReturn = pawnmove;
                    break;
                }
                Square start = pawnmove.StartSquare;
                Square target = pawnmove.TargetSquare;
                int distance = 7 - target.Rank;
                if(target.Rank - start.Rank < 0){
                    distance = target.Rank;
                }
                if(distance < lowestDistance){
                    lowestDistance = distance;
                    moveToReturn = pawnmove;
                }
            }
        }
        Console.WriteLine(MoveIsGuarded(board,moveToReturn));
        return moveToReturn;
    }
    public bool MoveIsCheckmate(Board board, Move move){
        board.MakeMove(move);
        bool isCheck = board.IsInCheckmate();
        board.UndoMove(move);
        return isCheck;
    }

    public bool MoveIsGuarded(Board board, Move move){
        Square target = move.TargetSquare;
        board.MakeMove(move);
        bool isGuarded = board.SquareIsAttackedByOpponent(target);
        board.UndoMove(move);
        return isGuarded;
        

    }
}