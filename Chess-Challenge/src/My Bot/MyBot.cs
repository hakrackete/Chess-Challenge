﻿using ChessChallenge.API;
using System;
using System.Collections.Generic;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        // alle moves durchsuchen, ob irgendetwas davon checkmate ist

        // Piece values: null, pawn, knight, bishop, rook, queen, king
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

        Move[] moves = board.GetLegalMoves();
        Dictionary<Move,double> moveEval = new Dictionary<Move, double>();

        Random rng = new();
        Move moveToReturn = moves[rng.Next(moves.Length)];

        foreach (Move move in moves)
        {
            if(MoveIsCheckmate(board,move)){
                return move;
            }
            double bestMoveScore = 0;
            double score = evaluateMove(board,move);
            // do something with entry.Value or entry.Key
            if(score > bestMoveScore){
                bestMoveScore = score;
                moveToReturn = move;
            }
        }


        // wenn keine captures, dann versuchen bauern zu advancen?
        int lowestDistance = 7;
        // if(highestCapture == 0){
        //     foreach(Move pawnmove in moves){
        //         Piece myPiece = board.GetPiece(pawnmove.StartSquare);
        //         if(myPiece.IsPawn == false){
        //             continue;
        //         }
        //         if (board.SquareIsAttackedByOpponent(pawnmove.TargetSquare)){
        //             if(MoveIsGuarded(board,pawnmove) == false){
        //                 continue;
        //             }

        //         }
        //         if(pawnmove.IsPromotion){
        //             moveToReturn = pawnmove;
        //             break;
        //         }
        //         Square start = pawnmove.StartSquare;
        //         Square target = pawnmove.TargetSquare;
        //         int distance = 7 - target.Rank;
        //         if(target.Rank - start.Rank < 0){
        //             distance = target.Rank;
        //         }
        //         if(distance < lowestDistance){
        //             lowestDistance = distance;
        //             moveToReturn = pawnmove;
        //         }
        //     }
        // }
        Console.WriteLine(MoveIsGuarded(board,moveToReturn));
        return moveToReturn;
    }
    public bool MoveIsCheckmate(Board board, Move move){
        board.MakeMove(move);
        bool isCheckMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isCheckMate;
    }
    public bool MoveIsCheck(Board board, Move move){
        board.MakeMove(move);
        bool isCheck = board.IsInCheck();
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
    public double evaluateMove(Board board, Move move){
        double score = 0;
        double MoveIsGuardedval = 0;
        if(MoveIsGuarded(board,move)){
            MoveIsGuardedval = 1;
        }
        double MoveIsAttacked = 0;
        if(board.SquareIsAttackedByOpponent(move.TargetSquare)){
            MoveIsAttacked = 1;
        }
        // plus für captures
        score += (int)move.CapturePieceType;

        // minus wenn angegriffen werden kann
        if (board.SquareIsAttackedByOpponent(move.TargetSquare)){
            if(MoveIsGuarded(board,move) == true){
                score -= 0;
            }
            else{
                score -= (int)move.MovePieceType;
            }
        }
        if(MoveIsCheck(board,move)){
            score += 0 + (500 * MoveIsGuardedval);
        }
        if(move.IsPromotion){
            score += (int)move.PromotionPieceType - 100;
            if (board.SquareIsAttackedByOpponent(move.TargetSquare) && !MoveIsGuarded(board,move)){
                score -= (int)move.PromotionPieceType - 100;
            }
            if (board.SquareIsAttackedByOpponent(move.TargetSquare) && MoveIsGuarded(board,move)){
                score -= ((int)move.PromotionPieceType - 100)/2;
            }
        }
        return score;
    }
}