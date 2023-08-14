
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
        int searchdepth = 3;
        Console.WriteLine(timer);

        Move[] moves = board.GetLegalMoves();

        Random rng = new();
        Move moveToReturn = moves[rng.Next(moves.Length)];
        int currentscore  = board.IsWhiteToMove ? Int32.MinValue : Int32.MaxValue;

            if(board.IsWhiteToMove){
                currentscore = Int32.MinValue;
                foreach (Move newMove in board.GetLegalMoves()){
                    board.MakeMove(newMove);
                    int newValue = minmax(board,false,searchdepth);
                    if(newValue > currentscore){
                        currentscore = newValue; 
                        moveToReturn = newMove;
                    }
                    board.UndoMove(newMove);
                }
            }
            else{
                currentscore = Int32.MaxValue;
                foreach (Move newMove in board.GetLegalMoves()){
                    board.MakeMove(newMove);
                    int newValue = minmax(board,true,searchdepth);
                    if(newValue < currentscore){
                        currentscore = newValue; 
                        moveToReturn = newMove;
                    }
                    board.UndoMove(newMove);
                }
            }
        Console.WriteLine($"expected score{currentscore}");
        Console.WriteLine(evalBoard(board));
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
        return score;

    } 
    public int minmax(Board board, bool PlayertoMaximize, int Depth){
        int returnValue = 0;
        if(Depth == 0){
            returnValue = evalBoard(board);
        }
        else{
            if(PlayertoMaximize){
                returnValue = Int32.MinValue;
                foreach (Move newMove in board.GetLegalMoves()){
                    board.MakeMove(newMove);
                    int newValue = minmax(board,false,Depth-1);
                    if(newValue > returnValue){
                        returnValue = newValue; 
                    }
                    board.UndoMove(newMove);
                }
                
            }
            else{
                returnValue = Int32.MaxValue;
                foreach (Move newMove in board.GetLegalMoves()){
                    board.MakeMove(newMove);
                    int newValue = minmax(board,true,Depth-1);
                    if(newValue < returnValue){
                        returnValue = newValue; 
                    }
                    board.UndoMove(newMove);
                }
            }
        }
    return returnValue;
    }

}