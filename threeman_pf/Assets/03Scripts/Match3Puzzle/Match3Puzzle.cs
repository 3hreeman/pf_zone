using System;

public class Match3Puzzle {
    private const int Rows = 8;
    private const int Columns = 8;
    private const int MatchLength = 3;
    private int[,] board = new int[Rows, Columns];

    // 새로운 보드 초기화
    public void InitializeBoard() {
        Random rand = new Random();
        for (int i = 0; i < Rows; i++) {
            for (int j = 0; j < Columns; j++) {
                board[i, j] = rand.Next(1, 5); // 예: 1부터 4까지의 숫자를 사용
            }
        }
    }

    // 3매치 검사
    public bool CheckForMatches() {
        bool isMatchFound = false;

        // 가로 검사
        for (int i = 0; i < Rows; i++) {
            for (int j = 0; j < Columns - MatchLength + 1; j++) {
                if (board[i, j] == board[i, j + 1] && board[i, j] == board[i, j + 2]) {
                    // 3매치 발견
                    isMatchFound = true;
                    for (int k = 0; k < MatchLength; k++) {
                        board[i, j + k] = 0; // 매치된 아이템 제거
                    }
                }
            }
        }

        // 세로 검사
        for (int j = 0; j < Columns; j++) {
            for (int i = 0; i < Rows - MatchLength + 1; i++) {
                if (board[i, j] == board[i + 1, j] && board[i, j] == board[i + 2, j]) {
                    // 3매치 발견
                    isMatchFound = true;
                    for (int k = 0; k < MatchLength; k++) {
                        board[i + k, j] = 0; // 매치된 아이템 제거
                    }
                }
            }
        }

        return isMatchFound;
    }

    // 빈 칸 채우기
    public void FillEmptySpaces() {
        Random rand = new Random();
        for (int j = 0; j < Columns; j++) {
            for (int i = Rows - 1; i >= 0; i--) {
                if (board[i, j] == 0) // 빈 칸 발견
                {
                    int k = i - 1;
                    while (k >= 0 && board[k, j] == 0) k--;
                    if (k >= 0) {
                        board[i, j] = board[k, j];
                        board[k, j] = 0;
                    }
                    else {
                        board[i, j] = rand.Next(1, 5); // 새 아이템으로 채우기
                    }
                }
            }
        }
    }
}