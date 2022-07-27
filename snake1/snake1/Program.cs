using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;


namespace snake1
{
    internal class Program
    {
        [DllImport("user32")]
        public static extern UInt16 GetAsyncKeyState(Int32 vKey);   // 현재 키가 눌렸는지 확인
        static bool isdead = false; // 뱀이 죽었는지 여부
        static int x, y;  // 플레이어의 좌표

        static int remainKey;   // 입력 키를 기억하는 키
        static int foodPosX;    // 음식이 생성되는 X 좌표
        static int foodPosY;    // 음식이 생성되는 Y 좌표

        static List<int[]> bodys = new List<int[]>();   // 뱀 몸 리스트 생성

        static void Main(string[] args)
        {
            ConsoleKeyInfo cki; // 콘솔 키 입력 받기

            while (true)    // 프로그램이 계속 반복됨
            {
                StartGame();    // 스타트 게임 함수 실행

                while (!isdead) // 죽지 않았다면
                {
                    UpdateGame();   // 업데이트 게임 반복
                }

                Console.Clear();    // 화면 초기화
                Console.SetCursorPosition(0, 50);   // 커서 위치 변경
                Console.WriteLine("게임을 끝내려면 Q를 누르세요. 다시하려면 아무 키나 입력.\n");

                while (Console.KeyAvailable)    // 버퍼 초기화
                {
                    Console.ReadKey(false);
                }

                cki = Console.ReadKey();    // 키 읽기

                if (cki.Key == ConsoleKey.Q)    // Q를 누르면 while문 종료
                {
                    break;
                }
                else // 그렇지 않으면
                {
                    isdead = false; // 반복문 다시 실행
                }
            }
            Console.SetCursorPosition(0, 52);
        }

        private static void StartGame() // 최초 1회 시작되는 함수
        {
            x = 50; // 플레이어 X 좌표
            y = 25; // 플레이어 Y 좌표
            remainKey = 2; // 위로 이동 

            bodys.Clear();  // 뱀 몸 리스트 초기화
            bodys.Add(new int[] { 50, 26 });
            bodys.Add(new int[] { 50, 27 });
            bodys.Add(new int[] { 50, 28 });

            Console.SetWindowSize(100, 60); // 콘솔창 크기 조정

            SetFoodPosition();   // 음식 위치 변경
        }

        private static void UpdateGame()    // 프레임마다 업데이트 되는 함수
        {
            MoveSnake();
            CheckCollision();

            Console.Clear();

            DrawMap();
            DrawSnakeHead();
            DrawSnakeBodys();
            DrawFood();

            Thread.Sleep(50);   // 0.05초 딜레이
        }

        private static void DrawMap()   // 맵 생성
        {
            for (int mapX = 0; mapX < 100; mapX++)   // x를 0부터 100까지 반복
            {
                for (int mapY = 0; mapY < 50; mapY++)    // y를 0부터 50까지 반복
                {
                    if (mapX == 0 || mapX == 99 || mapY == 0 || mapY == 49) // 좌표가 모서리의 네 면일 경우에만
                    {
                        Console.SetCursorPosition(mapX, mapY);
                        Console.Write("#");  // 벽 출력
                    }
                }
            }
        }

        private static void DrawSnakeHead()     // 뱀 머리 출력
        {
            Console.SetCursorPosition(x, y);    // 플레이어 좌표 x,y
            Console.Write("@"); // 플레이어 출력
        }

        private static void DrawSnakeBodys()    // 뱀 몸통 리스트 출력
        {
            for (int i = 0; i < bodys.Count; i++)   // 리스트의 길이만큼 반복
            {
                Console.SetCursorPosition(bodys[i][0], bodys[i][1]);
                Console.Write("o");
            }
        }


        private static void MoveSnake() // 뱀 이동
        {
            for (int i = bodys.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    bodys[i] = new int[] { x, y };
                }
                else
                {
                    bodys[i] = bodys[i - 1];
                }
            }

            int key = ReadKey();

            switch (key != 0 ? key : remainKey) // 키가 입력되면 입력한 키를 반환, 아니면 이전 키를 반환
            {
                case 1: x--; break; // 좌
                case 2: y--; break; // 상
                case 3: x++; break; // 우
                case 4: y++; break; // 하
            }

            if (key != 0)   // 키 미입력 시
            {
                remainKey = key;    // 현재 키를 이전 키에 저장
            }
        }

        private static int ReadKey()    // 키 읽기
        {
            int returnKey = 0;

            if (GetAsyncKeyState(0x25) != 0 && remainKey != 3)  // 좌
            {
                returnKey = 1;
            }

            if (GetAsyncKeyState(0x26) != 0 && remainKey != 4)  // 상
            {
                returnKey = 2;
            }

            if (GetAsyncKeyState(0x27) != 0 && remainKey != 1)  // 우
            {
                returnKey = 3;
            }

            if (GetAsyncKeyState(0x28) != 0 && remainKey != 2)  // 하
            {
                returnKey = 4;
            }

            return returnKey;
        }

        private static void CheckCollision()    // 충돌 체크
        {
            if (x == 0 || x == 99 || y == 0 || y == 49) // 플레이어의 좌표가 벽이 있는 좌표와 일치하면
            {
                Dead(); //  사망 함수 실행
            }

            for (int i = 0; i < bodys.Count; i++) // 뱀의 길이만큼 반복
            {
                if (x == bodys[i][0] && y == bodys[i][1]) // 플레이어의 좌표가 뱀의 좌표와 일치하면
                {
                    Dead(); // 사망 함수 실행
                }
            }

            if (x == foodPosX && y == foodPosY) // 플레이어의 좌표가 음식이 있는 좌표와 일치하면
            {
                SetFoodPosition(); // 음식 위치 변경
                bodys.Add(bodys[bodys.Count - 1]); // 뱀의 길이 증가
            }
        }

        private static void Dead()  // 데드
        {
            isdead = true;  // 죽음 여부를 참으로 변경
        }

        private static void SetFoodPosition()    // 음식 위치 랜덤
        {
            Random randomX = new Random();
            Random randomY = new Random();

            foodPosX = randomX.Next(1, 98); // 벽 안쪽의 범위 내에서 X 좌표의 난수를 얻음
            foodPosY = randomY.Next(1, 48); // 벽 안쪽의 범위 내에서 Y 좌표의 난수를 얻음
        }

        private static void DrawFood() // 음식 스폰
        {
            Console.SetCursorPosition(foodPosX, foodPosY);
            Console.Write("$"); // 음식 출력
        }
    }
}