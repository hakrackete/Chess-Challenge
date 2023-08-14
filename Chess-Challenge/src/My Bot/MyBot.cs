using ChessChallenge.API;
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
        int currentscore  = evalBoard(board);
        foreach(Move move in moves){
            int newScore = negamin(board,move,3);
            if(newScore < currentscore){
                currentscore = newScore;
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
    public int evalBoard(Board board){
        // Piece values: null, pawn, knight, bishop, rook, queen, king
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        
        int score = 0;
        foreach (PieceList pieceList in board.GetAllPieceLists()){
            int count = pieceList.Count;
            int value = pieceValues[(int)pieceList.TypeOfPieceInList];
            if(!pieceList.IsWhitePieceList){
                value *= -1;
            }
            score += count * value;
        }
        if(!board.IsWhiteToMove){
            score *= -1;
        }
        return score;

    } 
    public int negamin(Board board,Move move, int Depth){
        int returnValue = 0;
        if(Depth == 0){
            board.MakeMove(move);
            returnValue = evalBoard(board);
            board.UndoMove(move);
        }
        else{
            board.MakeMove(move);
            Move[] allMoves = board.GetLegalMoves();
            foreach (Move newMove in allMoves){
                int newValue = negamin(board, newMove, Depth - 1);
                if(newValue > returnValue){
                    returnValue = newValue; 
                }
            }
            board.UndoMove(move);
        }
        return returnValue;
    }
}