using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace snake
{
    internal class Program
    {
        [DllImport("user32")]
        public static extern UInt16 GetAsyncKeyState(Int32 vKey);   // 현재 키가 눌렸는지 확인
        static int remainKey;   // 입력 키를 기억하는 키

        static void Main(string[] args)
        {
            GameManager gameManager = new GameManager();    // 게임 매니저 생성

        }

        public interface IGameObject    // 게임 오브젝트 인터페이스, 모든 게임 오브젝트는 이 인터페이스를 상속
        {
            void Draw();    // 게임 오브젝트 그리기
            void Calculate();   // 게임 오브젝트 연산 처리
        }

        public class GameManager
        {
            bool isdead = false; // 뱀이 죽었는지 여부
            bool isGameMenu = true; // 게임 메뉴 여부
            bool isGameMode = false;    // 게임 모드 여부

            public GameManager()    // 게임 매니저 생성자
            {
                while (true)    // 프로그램이 실행되고 있는 동안 반복
                {
                    if (isGameMenu) // 게임 메뉴가 참이면
                    {
                        GameMenu(); // 게임 메뉴 실행
                    }

                    if (isGameMode) // 게임 모드가 참이면
                    {
                        GameMode(); // 게임 모드 실행
                    }
                }
            }

            void GameMenu() // 게임 메뉴 함수
            {
                Console.SetWindowSize(100, 60); // 콘솔창 크기 조정
                Console.SetCursorPosition(0, 0);
                Console.WriteLine("게임 메뉴");
                Console.SetCursorPosition(0, 1);
                Console.WriteLine("1. 게임 시작");
                Console.SetCursorPosition(0, 2);
                Console.WriteLine("2. 게임 종료");

                Input input = new Input();  // 인풋 클래스 생성으로 키 입력 받기

                if (input.cki.Key == ConsoleKey.D1) // 1번을 누르면
                {
                    isdead = false; // 뱀이 죽지 않은 상태
                    isGameMenu = false; // 게임 메뉴가 거짓
                    isGameMode = true;  // 게임 모드가 참이 되고 게임 시작
                }

                else if (input.cki.Key == ConsoleKey.D2)    // 2번을 누르면
                {
                    isdead = false; // 뱀이 죽지 않은 상태
                    isGameMenu = false; // 게임 메뉴 거짓
                    isGameMode = false; // 게임 모드가 거짓
                    Environment.Exit(0);    // 프로그램 종료
                }
            }

            void GameMode() // 게임 모드 함수
            {
                Snake snake = new Snake();  // 뱀 생성
                Map map = new Map();    // 맵 생성
                Food food = new Food(); // 음식 생성
                Collision collision = new Collision();  // 충돌 생성

                remainKey = 2; // 위로 이동
                Console.SetWindowSize(100, 60); // 콘솔창 크기 조정


                while (!isdead) // 죽지 않았다면
                {
                    snake.Calculate();  // 뱀 이동 계산
                    if (collision.CheckMapCollision(snake.x, snake.y))  // 뱀이 맵에 부딪혔는지 확인
                    {
                        Dead(); // 데드 함수 실행
                    }

                    if (collision.CheckBodysCollision(snake.x, snake.y, snake.bodys))   // 뱀이 몸에 부딪혔는지 확인
                    {
                        Dead(); // 데드 함수 실행
                    }


                    if (collision.CheckFoodCollision(snake.x, snake.y, food.foodPosX, food.foodPosY, snake.bodys))  // 뱀이 음식에 부딪혔는지 확인
                    {
                        food.Calculate(); // 음식 위치 변경
                        snake.AddBody();    // 몸통 늘어남
                    }

                    Console.Clear();

                    map.Draw(); // 맵을 출력
                    snake.Draw();   // 뱀 머리와 몸을 출력
                    food.Draw();    // 음식을 출력

                    Thread.Sleep(50);   // 0.05초 딜레이   // 업데이트 게임 반복
                }

                GameOver(); // 뱀이 죽고 반복문의 조건이 불만족하면 게임 오버 실행
            }

            public void GameOver()  // 게임 오버 함수
            {
                isGameMenu = true;  // 게임 메뉴가 참이 되고 게임 메뉴 실행
                isGameMode = false; // 게임 모드가 거짓
                //isdead = false;

                Console.Clear();

                Console.SetCursorPosition(30, 10);
                Console.WriteLine("Game Over"); // 게임 오버 출력
            }

            public void Dead()  // 데드 함수
            {
                if (!isdead)    // 뱀이 죽지 않았다면
                {
                    isdead = true;  // 뱀이 죽음
                }

                // else 문은 없어도 될 것 같음
                else  // 뱀이 죽었다면
                {
                    isdead = false; // 뱀이 안 죽음
                }
            }
        }

        public class Input  // 인풋 클래스
        {
            public ConsoleKeyInfo cki;
            
            public Input()  // 인풋 클래스 생성자
            {
                while (true)    // 1번 또는 2번을 입력할 때까지 계속 반복됨
                {
                    cki = Console.ReadKey();

                    if (cki.Key == ConsoleKey.D1)   // 1번 입력 시
                    {
                        break;
                    }
                    else if (cki.Key == ConsoleKey.D2)  // 2번 입력 시
                    {
                        break;
                    }
                }
            }
            public void ResetBuffer()    // 버퍼 초기화
            {
                while (Console.KeyAvailable)
                {
                    Console.ReadKey(false);
                }
                cki = Console.ReadKey();
            }

            public static int ReadKey()    // 키 읽기
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
        }

        public class Collision  // 충돌 클래스
        {
            public bool CheckMapCollision(int x, int y)    // 맵과 충돌 체크
            {
                if (x == 0 || x == 99 || y == 0 || y == 49) // 플레이어의 좌표가 벽이 있는 좌표와 일치하면
                {
                    return true;    // 참을 반환
                }
                else // 충돌하지 않으면
                {
                    return false;   // 거짓을 반환
                }
            }

            public bool CheckBodysCollision(int x, int y, List<int[]> bodys)    // 몸통과 충돌 체크
            {
                for (int i = 0; i < bodys.Count; i++) // 뱀의 길이만큼 반복
                {
                    if (x == bodys[i][0] && y == bodys[i][1]) // 플레이어의 좌표가 뱀의 좌표와 일치하면
                    {
                        return true;    // 참을 반환
                    }
                }
                return false;   // 충돌하지 않으면 거짓을 반환
            }

            public bool CheckFoodCollision(int x, int y, int foodPosX, int foodPosY, List<int[]> bodys) // 음식과 충돌 체크
            {
                if (x == foodPosX && y == foodPosY) // 플레이어의 좌표가 음식이 있는 좌표와 일치하면
                {
                    return true;    // 참을 반환
                }
                else  // 충돌하지 않으면
                {
                    return false;   // 거짓을 반환
                }
            }
        }

        public class Snake : IGameObject    // 뱀 클래스 : 게임 오브젝트 인터페이스 상속
        {
            public List<int[]> bodys = new List<int[]>();   // 뱀 몸 리스트 생성

            public int x, y;    // 뱀의 위치 x,y 좌표를 저장할 변수

            public Snake()  // 뱀 클래스 생성자
            {
                // 최초 뱀의 위치
                x = 50;
                y = 25;

                // 머리 아래에 뱀의 몸통 3개 생성
                bodys.Add(new int[] { 50, 26 });
                bodys.Add(new int[] { 50, 27 });
                bodys.Add(new int[] { 50, 28 });
            }

            public void AddBody()   // 몸통 추가
            {
                bodys.Add(bodys[bodys.Count - 1]); // 뱀의 길이 증가
            }

            void DrawSnakeHead()    // 뱀 머리 출력 함수
            {
                Console.SetCursorPosition(x, y);
                Console.Write("@");
            }

            void DrawSnakeBodys()   // 뱀 몸통 출력 함수
            {
                for (int i = 0; i < bodys.Count; i++)   // 리스트의 길이만큼 반복
                {
                    Console.SetCursorPosition(bodys[i][0], bodys[i][1]);
                    Console.Write("o");
                }
            }

            public void Draw()  // 뱀 출력
            {
                DrawSnakeHead();
                DrawSnakeBodys();
            }

            public void Calculate() // 뱀 이동 계산
            {
                // 자신의 앞에 있는 몸통의 위치를 가져옴
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

                int key = Input.ReadKey();

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
        }

        public class Map : IGameObject  // 맵 클래스 : 게임 오브젝트 인터페이스 상속
        {
            public void Draw()
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

            public void Calculate() // 인터페이스는 무조건 구현해야 함
            {
                // 아무 작업도 하지 않음
            }

        }

        public class Food : IGameObject // 음식 클래스 : 게임 오브젝트 인터페이스를 상속 받음
        {
            public int foodPosX;    // 음식이 생성되는 X 좌표
            public int foodPosY;    // 음식이 생성되는 Y 좌표

            public Food()   // 음식 클래스 생성자
            {
                Calculate();    // 음식 생성 위치 설정
            }

            public void Draw()  // 음식 출력 함수
            {
                Console.SetCursorPosition(foodPosX, foodPosY);
                Console.Write("$"); // 음식 출력
            }

            public void Calculate() // 음식 생성 위치 연산
            {
                Random randomX = new Random();
                Random randomY = new Random();

                foodPosX = randomX.Next(1, 98); // 벽 안쪽의 범위 내에서 X 좌표의 난수를 얻음
                foodPosY = randomY.Next(1, 48); // 벽 안쪽의 범위 내에서 Y 좌표의 난수를 얻음
            }
        }
    }
}