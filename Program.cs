using System;

namespace Chess_Rewriting
{
    class ChessGameLauncher
    {
        static void Main(string[] args)
        {
            new ChessGame().play();
        }
    }
    class ChessGame
    {
        public ChessPiece[,] board = new ChessPiece[8, 8];
        public ChessPiece[][,] boardsHistory = new ChessPiece[50][,];
        public string[] castlingStatusHistory = new string[50];
        public int boardsHistoryAndCastlingHistoryIndex;
        public bool isWhiteTurn;
        public Move move;
        string input;
        bool isWhiteKingOrRightRookMoved;
        bool isWhiteKingOrLeftRookMoved;
        bool isBlackKingOrRightRookMoved;
        bool isBlackKingOrLeftRookMoved;

        public void play()
        {
            board = firstTimeBoardFill();
            boardsHistory[0] = firstTimeBoardFill();
            castlingStatusHistory[0] = getCastlingStatus();
            isWhiteTurn = true;

            Console.WriteLine("WELCOME !!! Let's play some chess!!!\n\n" +
                "But first, a few details. \n" +
                "Every chess piece is represented by 2 letters. The first letter represents the color of the chess piece.\n" +
                "The second letter represents the name of the piece. For example, \"WQ\" means White Queen. \"BN\" means Black Knight.\nAnd so on...\n" +
                "\nEvery move should be entered by the followi6ng rules: \nThe move should be 5 letters long.\n" +
                "First two letters are the FILE and the RANK of the square from which you wish to move your piece.\n" +
                "The FILE is represented with a letter from A to H. The RANK is represented with a number from 1 to 8. \n" +
                "After those two letters enter a SPACE.\nThen enter the FILE and the RANK of the destination square.\n" +
                "You can enter your move either with lowercase or upper case.\n" +
                "For example, if you want to move yor piece from C3 to E5, your input can look like this:\nC3 E5\nor like this: \nc3 e5\n" +
                "Both are fine. Finish by pressing ENTER.\n\n" +
                "Every player, when it's his turn, can enter the word \"DRAW\" if he wants to end the game with a draw.\n" +
                "In that case, the second player would have to agree to a draw.\nIf he agrees - the game will end. Otherwise, the game will continue.\n" +
                "\nEvery player, when it's his turn, can do a CASTLING, by entering 2 words:\n" +
                "\"LITTLE CASTLING\" for a castling towards the G FILE or \"BIG CASTLING\" towards the C FILE.\nFinish by pressing ENTER.\n\n" +
                "OK, players, ready to play ?");

            printBoard();

            string relevantPlayer = isWhiteTurn ? "White Player" : "Black Player";
            do
            {
                Console.WriteLine(relevantPlayer + ", enter your next move and then press enter.");
                input = (Console.ReadLine().ToUpper()).Trim();
                if (input == "DRAW")
                {
                    Console.WriteLine(relevantPlayer + " ,Do you agree to a DRAW ?" +
                        "If yes - enter the letter Y. If no - enter anything else. Then press ENTER");
                    input = ((Console.ReadLine()).ToUpper()).Trim();
                    if (input == "Y")
                    {
                        Console.WriteLine("\nGAME OVER !!! It's a DRAW\n");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nNO DRAW. The game will continue !!!\n");
                        continue;
                    }
                }
                makeMove();
                printBoard();
                isWhiteTurn = isWhiteTurn ? false : true;
                relevantPlayer = isWhiteTurn ? "White Player" : "Black Player";

                if (isCheck())
                {
                    if (isStalemate())
                    {
                        Console.WriteLine("It's checkmate. " + relevantPlayer + ", you lose.\nGAME OVER !!!");
                        break;
                    }

                    else Console.WriteLine(relevantPlayer + " ,CHECK !!!");
                }
                if (isInsufficientMaterial())
                {
                    Console.WriteLine("There is insufficient material on the board to continue the game.\nIt's a DRAW.\nGAME OVER !!!");
                    break;
                }
                if (isStalemate())
                {
                    Console.WriteLine("It's a STALEMATE. \nGAME OVER");
                    break;
                }
                if (isFiftyMovesRule())
                {
                    Console.WriteLine("There hasn't been any irreversible moves for the last 50 moves.\nIt's a DRAW.\nGAME OVER");
                    break;
                }
                if (isThreefoldRepetition())
                {
                    Console.WriteLine(relevantPlayer + " ,the chess board setting has repeated for 3 times.\n" +
                        "You have an option to end this game with a DRAW. Do you agree to end the game with a DRAW ?\n" +
                        "If yes - enter the letter Y. If no - enter anything else. Then press ENTER");
                    move.moveInput = ((Console.ReadLine()).ToUpper()).Trim();
                    if (move.moveInput == "Y")
                    {
                        Console.WriteLine("\nGAME OVER !!! It's a DRAW");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("\nNO DRAW. The game will continue !!!\n");
                        continue;
                    }
                }
            } while (true);
        }
        public ChessPiece[,] firstTimeBoardFill()
        {
            //יצירה של לוח שחמט ואתחול שלו באיך שהלוח אמור להראות בתחילת המשחק
            ChessPiece[,] result = new ChessPiece[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++) result[i, j] = new EmptySquare();
            for (int i = 0; i < 8; i++) result[1, i] = new Pawn(true);
            for (int i = 0; i < 8; i++) result[6, i] = new Pawn(false);
            result[0, 0] = new Rook(true); result[0, 1] = new Knight(true);
            result[0, 2] = new Bishop(true); result[0, 3] = new Queen(true);
            result[0, 4] = new King(true); result[0, 5] = new Bishop(true);
            result[0, 6] = new Knight(true); result[0, 7] = new Rook(true);
            result[7, 0] = new Rook(false); result[7, 1] = new Knight(false);
            result[7, 2] = new Bishop(false); result[7, 3] = new Queen(false);
            result[7, 4] = new King(false); result[7, 5] = new Bishop(false);
            result[7, 6] = new Knight(false); result[7, 7] = new Rook(false);
            return result;
        }
        public void printBoard()
        {
            //פונקציה שמדפיסה את הלוח
            Console.WriteLine();
            for (int row = 7; row >= 0; row--)
            {
                for (int column = 0; column < 8; column++)
                {
                    if (column == 0) Console.Write((row + 1) + "| ");
                    Console.Write(board[row, column] + " | ");
                }
                Console.WriteLine();
                Console.WriteLine("------------------------------------------");
            }
            Console.WriteLine("   A    B    C    D    E    F    G    H   ");
            Console.WriteLine();
        }
        public void makeMove()
        {
            string relevantPlayer = isWhiteTurn ? "White Player" : "Black Player";
            do
            {
                if (input == "LITTLE CASTLING")
                {
                    if (!isLittleCastlingPossible())
                    {
                        Console.WriteLine(relevantPlayer + " ,this move can't be done. Please try another one");
                        input = (Console.ReadLine().ToUpper()).Trim();
                        continue;
                    }
                    break;
                }
                if (input == "BIG CASTLING")
                {
                    if (!isBigCastlingPossible())
                    {
                        Console.WriteLine(relevantPlayer + " ,this move can't be done. Please try another one");
                        input = (Console.ReadLine().ToUpper()).Trim();
                        continue;
                    }
                    break;
                }
                if (!isInputLegal(input))            //בדיקה האם הקלט תקין
                {
                    Console.WriteLine(relevantPlayer + " ,your move is invalid. Please try again according to the rules above");
                    input = (Console.ReadLine().ToUpper()).Trim();
                    continue;
                }

                move = new Move(input);

                if (!board[move.initialRowNumber, move.initialColumnNumber].isMoveLegit(this))           //בדיקה האם המהלך חוקי
                {
                    Console.WriteLine(relevantPlayer + " ,this move can't be done. Please try another one ");
                    input = ((Console.ReadLine()).ToUpper()).Trim();
                    continue;
                }
                if (willBeCheck())  //בדיקה האם יהיה עלי שח אם אבצע את המהלך הזה
                {
                    if (isCheck())
                    {
                        Console.WriteLine(relevantPlayer + " ,this move can't be done, because your King is in danger. " +
                        "You must save him. Please try another move.");
                        input = ((Console.ReadLine()).ToUpper()).Trim();
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(relevantPlayer + " ,you putting your king in danger. You can't do that. Please try another move");
                        input = ((Console.ReadLine()).ToUpper()).Trim();
                        continue;
                    }
                }
                break;
            } while (true);

            if (input == "LITTLE CASTLING")
            {
                if (isWhiteTurn)
                {
                    board[0, 6] = new King(true);
                    board[0, 5] = new Rook(true);
                    board[0, 4] = new EmptySquare();
                    board[0, 7] = new EmptySquare();
                }
                else
                {
                    board[7, 6] = new King(false);
                    board[7, 5] = new Rook(false);
                    board[7, 4] = new EmptySquare();
                    board[7, 7] = new EmptySquare();
                }
            }
            else if (input == "BIG CASTLING")
            {
                if (isWhiteTurn)
                {
                    board[0, 2] = new King(true);
                    board[0, 3] = new Rook(true);
                    board[0, 4] = new EmptySquare();
                    board[0, 0] = new EmptySquare();
                }
                else
                {
                    board[7, 2] = new King(false);
                    board[7, 3] = new Rook(false);
                    board[7, 4] = new EmptySquare();
                    board[7, 0] = new EmptySquare();
                }
            }
            else
            {      // בודק אם המהלך שהיה עכשיו הוא הכאה דרך הילוכו
                if (move.initialColumnNumber != move.finalColumnNumber && board[move.finalRowNumber, move.finalColumnNumber] is EmptySquare &&
                    board[move.initialRowNumber, move.initialColumnNumber] is Pawn)
                {
                    if (isWhiteTurn) board[move.finalRowNumber - 1, move.finalColumnNumber] = new EmptySquare();
                    else board[move.finalRowNumber + 1, move.finalColumnNumber] = new EmptySquare();
                }
                // עושים את המהלך בפועל אחרי כל הבדיקות
                board[move.finalRowNumber, move.finalColumnNumber] = board[move.initialRowNumber, move.initialColumnNumber];
                board[move.initialRowNumber, move.initialColumnNumber] = new EmptySquare();
                // בודק האם יש רגלי שצריך לעשות לו הכתרה. אם כן - שולח לפונקציה של הכתרה
                if (board[move.finalRowNumber, move.finalColumnNumber] is Pawn && move.finalRowNumber == (isWhiteTurn ? 7 : 0))
                    makeCoronation(move.finalRowNumber, move.finalColumnNumber);
            }
            if (board[0, 4] is EmptySquare || board[0, 7] is EmptySquare) isWhiteKingOrRightRookMoved = true;
            if (board[0, 4] is EmptySquare || board[0, 0] is EmptySquare) isWhiteKingOrLeftRookMoved = true;
            if (board[7, 4] is EmptySquare || board[7, 7] is EmptySquare) isBlackKingOrRightRookMoved = true;
            if (board[7, 4] is EmptySquare || board[7, 0] is EmptySquare) isBlackKingOrLeftRookMoved = true;

            updateEnPassantStatus();
            updateBoardsHistory();
            castlingStatusHistory[boardsHistoryAndCastlingHistoryIndex] = getCastlingStatus();
        }
        public void updateEnPassantStatus()
        {
            if (isWhiteTurn)
            {
                for (int i = 0; i < 8; i++) if (board[3, i] is Pawn) ((Pawn)board[3, i]).isEnPassantOnMePossible = false;
                if (move.initialRowNumber == 1 && move.finalRowNumber == 3 && board[move.finalRowNumber, move.finalColumnNumber] is Pawn)
                    ((Pawn)board[move.finalRowNumber, move.finalColumnNumber]).isEnPassantOnMePossible = true;
            }
            else
            {
                for (int i = 0; i < 8; i++) if (board[4, i] is Pawn) ((Pawn)board[4, i]).isEnPassantOnMePossible = false;
                if (move.initialRowNumber == 6 && move.finalRowNumber == 4 && board[move.finalRowNumber, move.finalColumnNumber] is Pawn)
                    ((Pawn)board[move.finalRowNumber, move.finalColumnNumber]).isEnPassantOnMePossible = true;
            }
        }
        public void updateBoardsHistory()
        {
            bool thereWasNoCapturingAndNoPawnMovement = true;
            if (getTotalNumberOfPiecesOnBoard(board) != getTotalNumberOfPiecesOnBoard(boardsHistory[boardsHistoryAndCastlingHistoryIndex]))
            {     // אם היתה הכאה - מתחילים את מערך ההיסטוריה מהתחלה
                boardsHistoryAndCastlingHistoryIndex = 0;
                thereWasNoCapturingAndNoPawnMovement = false;
            }
            else
            { // אם הייתה הסעה של רגלי - מתחילים את מערך את ההסטוריה מהתחלה
                for (int i = 0; i < 8; i++)
                    for (int j = 0; j < 8; j++)
                        if (board[i, j] is Pawn)
                            if (!(boardsHistory[boardsHistoryAndCastlingHistoryIndex][i, j] is Pawn))
                            {
                                boardsHistoryAndCastlingHistoryIndex = 0;
                                thereWasNoCapturingAndNoPawnMovement = false;
                            }
            }
            if (thereWasNoCapturingAndNoPawnMovement) boardsHistoryAndCastlingHistoryIndex++;
            // שומרים בפועל את הלוח הנוכחי בהסטוריית הלוחות
            boardsHistory[boardsHistoryAndCastlingHistoryIndex] = new ChessPiece[8, 8];
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    boardsHistory[boardsHistoryAndCastlingHistoryIndex][i, j] = board[i, j];
        }
        public string getCastlingStatus()
        {
            string castlingStatus = "";
            if (isWhiteTurn)
            {
                castlingStatus += (isLittleCastlingPossible() ? "t" : "f");
                castlingStatus += (isBigCastlingPossible() ? "t" : "f");
                isWhiteTurn = false;
                castlingStatus += (isLittleCastlingPossible() ? "t" : "f");
                castlingStatus += (isBigCastlingPossible() ? "t" : "f");
                isWhiteTurn = true;
            }
            else
            {
                isWhiteTurn = true;
                castlingStatus += (isLittleCastlingPossible() ? "t" : "f");
                castlingStatus += (isBigCastlingPossible() ? "t" : "f");
                isWhiteTurn = false;
                castlingStatus += (isLittleCastlingPossible() ? "t" : "f");
                castlingStatus += (isBigCastlingPossible() ? "t" : "f");
            }
            return castlingStatus;
        }
        public static int getTotalNumberOfPiecesOnBoard(ChessPiece[,] board)
        {
            int totalNumberOfPieces = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (!(board[i, j] is EmptySquare)) totalNumberOfPieces++;
            return totalNumberOfPieces;
        }
        public static bool isInputLegal(string input)
        {
            // פונקציה שבודקת האם הקלט בכלל תקין, עוד לפני שבודקים האם המהלך חוקי
            input = input.Trim();
            if (input.Length != 5) return false;
            if ((int)input[1] > 56 || (int)input[1] < 49 || (int)input[4] > 56 || (int)input[4] < 49) return false;
            if ((int)input[0] > 72 || (int)input[0] < 65 || (int)input[3] > 72 || (int)input[3] < 65) return false;
            if (input[2] != ' ') return false;
            return true;
        }
        public bool isLittleCastlingPossible()
        {
            if (isWhiteTurn)
            {
                if (isWhiteKingOrRightRookMoved) return false;    //האם המלך או הצריח הימני זזו
                if (!(board[0, 5] is EmptySquare) || !(board[0, 6] is EmptySquare)) return false; // בדיקה האם יש כלים בין מלך לצריח
                if (isCheck()) return false;  // בדיקה האם אני במצב של שח עכשיו
                move = new Move("E1 G1");
                if (willBeCheck()) return false; // בדיקה האם אני אהיה בשח בסוף המהלך
                move = new Move("E1 F1");
                if (willBeCheck()) return false; // בדיקה האם אני עובר במשבצת מאוימת
            }
            else
            {
                if (isBlackKingOrRightRookMoved) return false;
                if (!(board[7, 5] is EmptySquare) || !(board[7, 6] is EmptySquare)) return false;
                if (isCheck()) return false;
                move = new Move("E8 G8");
                if (willBeCheck()) return false;
                move = new Move("E8 F8");
                if (willBeCheck()) return false;
            }
            return true;
        }
        public bool isBigCastlingPossible()
        {
            if (isWhiteTurn)
            {
                if (isWhiteKingOrLeftRookMoved) return false;
                if (!(board[0, 1] is EmptySquare) || !(board[0, 2] is EmptySquare) || !(board[0, 3] is EmptySquare))
                    return false; // בדיקה האם יש כלים בין מלך לצריח
                if (isCheck()) return false;  // בדיקה האם אני במצב של שח עכשיו
                move = new Move("E1 C1");
                if (willBeCheck()) return false; // בדיקה האם אני אהיה בשח בסוף המהלך
                move = new Move("E1 D1");
                if (willBeCheck()) return false;  // בדיקה האם אני עובר במשבצת מאוימת          
            }
            else
            {
                if (isBlackKingOrLeftRookMoved) return false;
                if (!(board[7, 1] is EmptySquare) || !(board[7, 2] is EmptySquare) || !(board[7, 3] is EmptySquare)) return false;
                if (isCheck()) return false;
                move = new Move("E8 C8");
                if (willBeCheck()) return false;
                move = new Move("E8 D8");
                if (willBeCheck()) return false;
            }
            return true;
        }
        public bool isInsufficientMaterial()
        {
            //  בדיקה האם נשארו מלך מול מלך \ מלך ופרש מול מלך \ מלך ורץ מול מלך \ מלך ופרש מול מלך ורץ
            int numberOfWhitePieces = 0, numberOfBlackPieces = 0;
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] is Queen || board[i, j] is Rook || board[i, j] is Pawn) return false;
                    if (board[i, j].isWhite) numberOfWhitePieces++;
                    if (!board[i, j].isWhite) numberOfBlackPieces++;
                }
            if (numberOfWhitePieces + numberOfBlackPieces > 4) return false;
            if (numberOfWhitePieces > 2 || numberOfBlackPieces > 2) return false;
            return true;
        }
        public bool isStalemate()
        {
            for (int initialRowNumber = 0; initialRowNumber < 8; initialRowNumber++)
            {
                for (int initialColumnNumber = 0; initialColumnNumber < 8; initialColumnNumber++)
                {
                    for (int finalRowNumber = 0; finalRowNumber < 8; finalRowNumber++)
                    {
                        for (int finalColumnNumber = 0; finalColumnNumber < 8; finalColumnNumber++)
                        {
                            move = convertCoordinatesToMove(initialRowNumber, initialColumnNumber, finalRowNumber, finalColumnNumber);
                            if (board[initialRowNumber, initialColumnNumber].isMoveLegit(this))
                                if (!willBeCheck()) return false;
                        }
                    }
                }
            }
            return true;
        }
        public bool isFiftyMovesRule()
        {
            if (boardsHistoryAndCastlingHistoryIndex == 49) return true;
            return false;
        }
        public bool isThreefoldRepetition()
        {
            if (boardsHistoryAndCastlingHistoryIndex < 7) return false;
            bool theSameBoard;
            int howManySameBoards = 0;
            // פונקציה שעוברת אחורה על כל הלוחות, עד לתחילת המשחק, ואם מוצאת לפחות 2 לוחות שהם אותו דבר כמו הלוח הנוכחי  
            //   אצל אותו שחקן תחזיר הודעה לשחקן שיש לו אפשרות לבקש תיקו עקב חוק שלושה מסעים חוזרים
            for (int historyIndex = boardsHistoryAndCastlingHistoryIndex - 2; historyIndex >= 0; historyIndex -= 2)
            {
                theSameBoard = true;
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if (board[i, j].ToString() != boardsHistory[historyIndex][i, j].ToString())
                        {
                            theSameBoard = false;
                            break;
                        }
                    }
                    if (!theSameBoard) break;
                }
                if (!theSameBoard) continue;
                // אחרי שמצאנו אותו לוח כמו הלוח הנוכחי - בודקים סטטוס הצרחות
                if (getCastlingStatus() == castlingStatusHistory[historyIndex]) howManySameBoards++;
                if (howManySameBoards == 2) return true; // אם מצאנו עוד שתי לוחות שהיו כמו הלוח הנוכחי - המשחק נגמר עקב 3 מסעים חוזרים
            }
            return false;
        }
        public static Move convertCoordinatesToMove(int initialRowNumber, int initialColumnNumber, int finalRowNumber, int finalColumnNumber)
        {
            string moveInput = "" + (char)(initialColumnNumber + 65);
            moveInput += initialRowNumber + 1;
            moveInput += " ";
            moveInput += (char)(finalColumnNumber + 65);
            moveInput += finalRowNumber + 1;
            return new Move(moveInput);
        }
        public bool isCheck()
        {
            char myChessColor = (isWhiteTurn ? 'W' : 'B');
            //בודק באיזה משבצת נמצא המלך בצבע הרלוונטי
            int kingRow = 0, kingColumn = 0;
            bool kingFound = false;
            for (int i = 0; i < 8 && !kingFound; i++)
                for (int j = 0; j < 8 && !kingFound; j++)
                    if (board[i, j].ToString() == myChessColor + "K")
                    {
                        kingRow = i;
                        kingColumn = j;
                        kingFound = true;
                    }
            Move tempMove = move;
            isWhiteTurn = isWhiteTurn ? false : true;
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    move = convertCoordinatesToMove(row, column, kingRow, kingColumn);
                    if (board[row, column].isMoveLegit(this))
                    {
                        isWhiteTurn = isWhiteTurn ? false : true;
                        move = tempMove;
                        return true;
                    }
                }
            }
            isWhiteTurn = isWhiteTurn ? false : true;
            move = tempMove;
            return false;
        }
        public bool willBeCheck()
        {
            ChessPiece tempPiece = board[move.finalRowNumber, move.finalColumnNumber];
            board[move.finalRowNumber, move.finalColumnNumber] = board[move.initialRowNumber, move.initialColumnNumber];
            board[move.initialRowNumber, move.initialColumnNumber] = new EmptySquare();
            bool willBeCheck = isCheck();
            board[move.initialRowNumber, move.initialColumnNumber] = board[move.finalRowNumber, move.finalColumnNumber];
            board[move.finalRowNumber, move.finalColumnNumber] = tempPiece;
            return willBeCheck;
        }
        public void makeCoronation(int finalRowNumber, int finalColumnNumber)
        {
            printBoard();
            Console.WriteLine((finalRowNumber == 7 ? "White" : "Black") + " Player, congratulations for reaching the final row !!! " +
              "What chess piece would you like instead of the pawn ? \nPlease press a single letter and than press ENTER");
            Console.WriteLine("Q for Queen \nB for Bishop \nN for Knight \nR for Rook");
            string newChessPieceType = Console.ReadLine().Trim().ToUpper();
            bool isPawnWhite = finalRowNumber == 7 ? true : false;
            bool validChoice;
            do
            {
                validChoice = true;
                switch (newChessPieceType)
                {
                    case "Q":
                        board[finalRowNumber, finalColumnNumber] = new Queen(isPawnWhite);
                        break;
                    case "B":
                        board[finalRowNumber, finalColumnNumber] = new Bishop(isPawnWhite);
                        break;
                    case "N":
                        board[finalRowNumber, finalColumnNumber] = new Knight(isPawnWhite);
                        break;
                    case "R":
                        board[finalRowNumber, finalColumnNumber] = new Rook(isPawnWhite);
                        break;
                    default:
                        {
                            Console.WriteLine("\nInvalid choice. Please select a valid chess piece from the given choices");
                            Console.WriteLine("Q for Queen \nB for Bishop \nN for Knight \nR for Rook");
                            newChessPieceType = Console.ReadLine().Trim().ToUpper();
                            validChoice = false;
                            break;
                        }
                }
            } while (!validChoice);
        }
    }
    class ChessPiece
    {
        public bool isWhite;
        public ChessPiece() { }
        public ChessPiece(bool isWhite)
        {
            this.isWhite = isWhite;
        }
        public override string ToString()
        {
            return base.ToString();
        }
        public virtual bool isMoveLegit(ChessGame game)
        {
            ChessPiece initialSqaurePiece = game.board[game.move.initialRowNumber, game.move.initialColumnNumber]; // כלי במשבצת התחלתית
            ChessPiece finalSqaurePiece = game.board[game.move.finalRowNumber, game.move.finalColumnNumber]; // כלי במשבצת היעד

            if (initialSqaurePiece.ToString()[0] == finalSqaurePiece.ToString()[0])
                return false;      //אם במשבצת היעד עומד כלי באותו צבע כמו במשבצת התחלתית
            if (game.move.moveInput[0] == game.move.moveInput[3] && game.move.moveInput[1] == game.move.moveInput[4])
                return false;   //אם המהלך הוא להשאר באותה משבצת
            if ((game.isWhiteTurn && !initialSqaurePiece.isWhite) || (!game.isWhiteTurn && initialSqaurePiece.isWhite))
                return false;   //בודק שהשחור באמת משחק עם השחורים או הלבן אם הלבנים. אחרת המהלך לא חוקי
            return true;
        }
    }
    class Rook : ChessPiece
    {
        public Rook(bool isWhite) : base(isWhite) { }
        public override string ToString()
        {
            return (isWhite ? "W" : "B") + "R";
        }
        public override bool isMoveLegit(ChessGame game)
        {

            if (!base.isMoveLegit(game)) return false;
            bool isMoveLegit = true;
            if (game.move.initialColumnNumber == game.move.finalColumnNumber)
            {
                if (game.move.initialRowNumber < game.move.finalRowNumber)    //אם הולך קדימה
                {
                    for (int i = game.move.initialRowNumber + 1; i < game.move.finalRowNumber; i++)
                        if (!(game.board[i, game.move.initialColumnNumber] is EmptySquare)) isMoveLegit = false;
                }
                else    //אם הולך אחורה
                {
                    for (int i = game.move.initialRowNumber - 1; i > game.move.finalRowNumber; i--)
                        if (!(game.board[i, game.move.initialColumnNumber] is EmptySquare)) isMoveLegit = false;
                }
            }
            else if (game.move.initialRowNumber == game.move.finalRowNumber)
            {
                if (game.move.initialColumnNumber < game.move.finalColumnNumber)  //אם הולך ימינה
                {
                    for (int i = game.move.initialColumnNumber + 1; i < game.move.finalColumnNumber; i++)
                        if (!(game.board[game.move.initialRowNumber, i] is EmptySquare)) isMoveLegit = false;
                }
                else  //אם הולך שמאלה
                {
                    for (int i = game.move.initialColumnNumber - 1; i > game.move.finalColumnNumber; i--)
                        if (!(game.board[game.move.initialRowNumber, i] is EmptySquare)) isMoveLegit = false;
                }
            }
            else return false;
            return isMoveLegit;
        }
    }
    class Bishop : ChessPiece
    {
        public Bishop(bool isWhite) : base(isWhite) { }
        public override string ToString()
        {
            return (isWhite ? "W" : "B") + "B";
        }
        public override bool isMoveLegit(ChessGame game)
        {
            if (!base.isMoveLegit(game)) return false;
            Move tempMove = new Move(game.move.moveInput);
            //הפרש בין מספר שורה סופית ומספר שורה התחלתית חייב להיות תואם להפרש בין עמודה סופית לעמודה התחלתית 
            if (tempMove.initialRowNumber < tempMove.finalRowNumber) //אם הולך למעלה
            {
                if (tempMove.initialColumnNumber < tempMove.finalColumnNumber) //למעלה וימינה
                {
                    if ((tempMove.finalRowNumber - tempMove.initialRowNumber) != (tempMove.finalColumnNumber - tempMove.initialColumnNumber))
                        return false;
                    tempMove.initialRowNumber++;
                    tempMove.initialColumnNumber++;
                    while (tempMove.initialRowNumber < tempMove.finalRowNumber && tempMove.initialColumnNumber < tempMove.finalColumnNumber)
                    {
                        if (!(game.board[tempMove.initialRowNumber, tempMove.initialColumnNumber] is EmptySquare)) return false;
                        tempMove.initialRowNumber++;
                        tempMove.initialColumnNumber++;
                    }
                }
                else   //למעלה ומשאלה
                {
                    if ((tempMove.finalRowNumber - tempMove.initialRowNumber) != (tempMove.initialColumnNumber - tempMove.finalColumnNumber))
                        return false;
                    tempMove.initialRowNumber++;
                    tempMove.initialColumnNumber--;
                    while (tempMove.initialRowNumber < tempMove.finalRowNumber && tempMove.initialColumnNumber > tempMove.finalColumnNumber)
                    {
                        if (!(game.board[tempMove.initialRowNumber, tempMove.initialColumnNumber] is EmptySquare)) return false;
                        tempMove.initialRowNumber++;
                        tempMove.initialColumnNumber--;
                    }
                }
            }
            else   //אם הולך למטה
            {
                if (tempMove.initialColumnNumber < tempMove.finalColumnNumber) //למטה וימינה
                {
                    if ((tempMove.initialRowNumber - tempMove.finalRowNumber) != (tempMove.finalColumnNumber - tempMove.initialColumnNumber))
                        return false;
                    tempMove.initialRowNumber--;
                    tempMove.initialColumnNumber++;
                    while (tempMove.initialRowNumber > tempMove.finalRowNumber && tempMove.initialColumnNumber < tempMove.finalColumnNumber)
                    {
                        if (!(game.board[tempMove.initialRowNumber, tempMove.initialColumnNumber] is EmptySquare)) return false;
                        tempMove.initialRowNumber--;
                        tempMove.initialColumnNumber++;
                    }
                }
                else   //למטה ומשאלה
                {
                    if ((tempMove.initialRowNumber - tempMove.finalRowNumber) != (tempMove.initialColumnNumber - tempMove.finalColumnNumber))
                        return false;
                    tempMove.initialRowNumber--;
                    tempMove.initialColumnNumber--;
                    while (tempMove.initialRowNumber > tempMove.finalRowNumber && tempMove.initialColumnNumber > tempMove.finalColumnNumber)
                    {
                        if (!(game.board[tempMove.initialRowNumber, tempMove.initialColumnNumber] is EmptySquare)) return false;
                        tempMove.initialRowNumber--;
                        tempMove.initialColumnNumber--;
                    }
                }
            }
            return true;
        }
    }
    class Pawn : ChessPiece
    {
        public bool isEnPassantOnMePossible;
        public Pawn(bool isWhite) : base(isWhite) { }
        public override string ToString()
        {
            return (isWhite ? "W" : "B") + "P";
        }
        public override bool isMoveLegit(ChessGame game)
        {
            if (!base.isMoveLegit(game)) return false;
            if (game.board[game.move.initialRowNumber, game.move.initialColumnNumber].isWhite)  //אם זה רגלי לבן
            {
                if (game.move.initialColumnNumber == game.move.finalColumnNumber && game.move.finalRowNumber - game.move.initialRowNumber == 1 &&
                    game.board[game.move.finalRowNumber, game.move.finalColumnNumber] is EmptySquare)
                    return true;
                if (game.move.initialColumnNumber == game.move.finalColumnNumber && game.move.finalRowNumber - game.move.initialRowNumber == 2 &&
                    game.move.initialRowNumber == 1 && game.board[game.move.finalRowNumber, game.move.finalColumnNumber] is EmptySquare &&
                    game.board[game.move.finalRowNumber - 1, game.move.finalColumnNumber] is EmptySquare)
                    return true;
                // בודק באיזה תנאים רגלי יכול ללכת באלכסון : כשמספר שורה עולה, כשהמשבצת לא ריקה
                if (game.move.finalRowNumber - game.move.initialRowNumber == 1 &&
                    !(game.board[game.move.finalRowNumber, game.move.finalColumnNumber] is EmptySquare) &&
                    (game.move.finalColumnNumber - game.move.initialColumnNumber == 1 ||
                    game.move.initialColumnNumber - game.move.finalColumnNumber == 1))
                    return true;
                //  בדיקה של הכאה דרך הילוכו
                if (game.move.finalRowNumber == 5 && game.move.initialRowNumber == 4 && game.board[5, game.move.finalColumnNumber] is EmptySquare &&
                    game.board[6, game.move.finalColumnNumber] is EmptySquare &&
                    (game.move.finalColumnNumber - game.move.initialColumnNumber == 1 ||
                    game.move.initialColumnNumber - game.move.finalColumnNumber == 1) &&
                    game.board[game.move.finalRowNumber - 1, game.move.finalColumnNumber].ToString() == "BP" &&
                    ((Pawn)game.board[game.move.finalRowNumber - 1, game.move.finalColumnNumber]).isEnPassantOnMePossible)
                    return true;
            }
            else   // אם זה רגלי שחור
            {
                if (game.move.initialColumnNumber == game.move.finalColumnNumber && game.move.initialRowNumber - game.move.finalRowNumber == 1 &&
                    game.board[game.move.finalRowNumber, game.move.finalColumnNumber] is EmptySquare)
                    return true;
                if (game.move.initialColumnNumber == game.move.finalColumnNumber && game.move.initialRowNumber - game.move.finalRowNumber == 2 &&
                    game.move.initialRowNumber == 6 && game.board[game.move.finalRowNumber, game.move.finalColumnNumber] is EmptySquare &&
                    game.board[game.move.finalRowNumber + 1, game.move.finalColumnNumber] is EmptySquare)
                    return true;
                // בודק באיזה תנאים רגלי יכול ללכת באלכסון : כשמספר שורה עולה, כשהמשבצת לא ריקה
                if (game.move.initialRowNumber - game.move.finalRowNumber == 1 &&
                    !(game.board[game.move.finalRowNumber, game.move.finalColumnNumber] is EmptySquare) &&
                    (game.move.finalColumnNumber - game.move.initialColumnNumber == 1 ||
                    game.move.initialColumnNumber - game.move.finalColumnNumber == 1))
                    return true;
                //  בדיקה של הכאה דרך הילוכו
                if (game.move.finalRowNumber == 2 && game.move.initialRowNumber == 3 && game.board[2, game.move.finalColumnNumber] is EmptySquare &&
                    game.board[1, game.move.finalColumnNumber] is EmptySquare &&
                    (game.move.finalColumnNumber - game.move.initialColumnNumber == 1 ||
                    game.move.initialColumnNumber - game.move.finalColumnNumber == 1) &&
                    game.board[game.move.finalRowNumber + 1, game.move.finalColumnNumber].ToString() == "WP" &&
                    ((Pawn)game.board[game.move.finalRowNumber + 1, game.move.finalColumnNumber]).isEnPassantOnMePossible)
                    return true;
            }
            return false;
        }
    }
    class Knight : ChessPiece
    {
        public Knight(bool isWhite) : base(isWhite) { }
        public override string ToString()
        {
            return (isWhite ? "W" : "B") + "N";
        }
        public override bool isMoveLegit(ChessGame game)
        {
            if (!base.isMoveLegit(game)) return false;
            if ((game.move.finalRowNumber - game.move.initialRowNumber == 2 && game.move.finalColumnNumber - game.move.initialColumnNumber == 1) ||
                (game.move.finalRowNumber - game.move.initialRowNumber == 2 && game.move.initialColumnNumber - game.move.finalColumnNumber == 1) ||
                (game.move.finalRowNumber - game.move.initialRowNumber == 1 && game.move.finalColumnNumber - game.move.initialColumnNumber == 2) ||
                (game.move.finalRowNumber - game.move.initialRowNumber == 1 && game.move.initialColumnNumber - game.move.finalColumnNumber == 2) ||
                (game.move.initialRowNumber - game.move.finalRowNumber == 2 && game.move.finalColumnNumber - game.move.initialColumnNumber == 1) ||
                (game.move.initialRowNumber - game.move.finalRowNumber == 2 && game.move.initialColumnNumber - game.move.finalColumnNumber == 1) ||
                (game.move.initialRowNumber - game.move.finalRowNumber == 1 && game.move.finalColumnNumber - game.move.initialColumnNumber == 2) ||
                (game.move.initialRowNumber - game.move.finalRowNumber == 1 && game.move.initialColumnNumber - game.move.finalColumnNumber == 2))
                return true;
            return false;
        }
    }
    class Queen : ChessPiece
    {
        public Queen(bool isWhite) : base(isWhite) { }
        public override string ToString()
        {
            return (isWhite ? "W" : "B") + "Q";
        }
        public override bool isMoveLegit(ChessGame game)
        {
            Bishop queenAsBishop = new Bishop(this.isWhite ? true : false);
            Rook queenAsRook = new Rook(this.isWhite ? true : false);
            return queenAsBishop.isMoveLegit(game) || queenAsRook.isMoveLegit(game);
        }
    }
    class King : ChessPiece
    {
        public King(bool isWhite) : base(isWhite) { }
        public override string ToString()
        {
            return (isWhite ? "W" : "B") + "K";
        }
        public override bool isMoveLegit(ChessGame game)
        {
            if (!base.isMoveLegit(game)) return false;
            if ((game.move.initialRowNumber == game.move.finalRowNumber && game.move.initialColumnNumber - game.move.finalColumnNumber == 1) ||
                (game.move.initialRowNumber == game.move.finalRowNumber && game.move.finalColumnNumber - game.move.initialColumnNumber == 1) ||
                (game.move.initialColumnNumber == game.move.finalColumnNumber && game.move.initialRowNumber - game.move.finalRowNumber == 1) ||
                (game.move.initialColumnNumber == game.move.finalColumnNumber && game.move.finalRowNumber - game.move.initialRowNumber == 1) ||
                (game.move.finalRowNumber - game.move.initialRowNumber == 1 && game.move.finalColumnNumber - game.move.initialColumnNumber == 1) ||
                (game.move.finalRowNumber - game.move.initialRowNumber == 1 && game.move.initialColumnNumber - game.move.finalColumnNumber == 1) ||
                (game.move.initialRowNumber - game.move.finalRowNumber == 1 && game.move.finalColumnNumber - game.move.initialColumnNumber == 1) ||
                game.move.initialRowNumber - game.move.finalRowNumber == 1 && game.move.initialColumnNumber - game.move.finalColumnNumber == 1)
                return true;
            return false;
        }
    }
    class EmptySquare : ChessPiece
    {
        public EmptySquare() { }
        public override bool isMoveLegit(ChessGame game)
        {
            return false;
        }
        public override string ToString()
        {
            return "  ";
        }
    }
    class Move
    {
        public string moveInput;
        public int initialRowNumber;
        public int initialColumnNumber;
        public int finalRowNumber;
        public int finalColumnNumber;
        public Move(string moveInput)
        {
            this.moveInput = moveInput;
            this.initialRowNumber = int.Parse(this.moveInput[1] + "") - 1;
            this.initialColumnNumber = (int)moveInput[0] - 65;
            this.finalRowNumber = int.Parse(moveInput[4] + "") - 1;
            this.finalColumnNumber = (int)moveInput[3] - 65;
        }
    }
}