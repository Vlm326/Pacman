using System;
using System.IO;
using System.Runtime.Serialization;
using System.Security.AccessControl;

class Pacman
{

    static void Main(string[] args)
    {

        Console.Title = "Pacman";
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        while (true)
        {
            RunGame();
            Console.WriteLine("Play again? (Y/N)");
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key != ConsoleKey.Y)
            {
                Console.WriteLine("Goodbye!");
                break;
            }
        }
    }


    private static void RunGame()
    {
        

        
        PrintStart();
        for (int i = 0; i < 5; i++){Console.WriteLine();}
        Console.WriteLine("Enter path to the map");

        //Объявление начальных значений
        string? path = Console.ReadLine();
        char[,]? map = null;
        //считываение файла
        if (path != null)
        {
            map = ReadMap(path);
        }
        Console.Clear();
        //начальное значение 
        int currentX = 1, currentY = 1;
        string direction = String.Empty;
        int counter = 0;
        Console.SetCursorPosition(1, 1);
        Console.Write("@");
        
        int ghostX = 10, ghostY = 1;
        int secondGhostX = 15, secondGhostY = 2;
        /*
         * начало основного цикла
         */
        while (true)
        {
            if (map == null)
            {
                break;
            }

            if (!CheckAvailableCoins(map))
            {
                UpdateCoins(map);
                Console.SetCursorPosition(0, 0);
                PrintMap(map);
            }
            Console.SetCursorPosition(0, map.GetLength(1));
            Console.Write($"Score: {counter},    G - Ghost,    @ - Pacman,    Q - Quit");
            
            /*
             * Блок обработки нажатий
             */
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D)
                {
                    direction = "right";
                }
                else if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A)
                {
                    direction = "left";
                }
                else if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                {
                    direction = "up";
                }
                else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                {
                    direction = "down";
                }

                if (key.Key == ConsoleKey.Q)
                {
                    Console.Clear();
                    Console.WriteLine("Goodbye");
                    break;
                }
            }
            
            switch (direction)
            {
                case "right":
                    MoveRight(map, ref currentX, currentY);
                    break;
                case "left":
                    MoveLeft(map, ref currentX, currentY);
                    break;
                case "down":
                    MoveDown(map, currentX, ref currentY);
                    break;
                case "up":
                    MoveUp(map, currentX, ref currentY);
                    break;
            }
            if (CheckCollision(map, currentX, currentY))
            {
                PrintGameOver();
                Console.WriteLine($"YOUR SCORE IS {counter}");
                Thread.Sleep(1000);
                break;
            }
            // Считаем очки
            CoinsCounter(map, currentX, currentY, ref counter);

            // Двигаем призраков после Pacman
            if (counter > 10)
            {
                MoveToPlayer(map, ref ghostX, ref ghostY, currentX, currentY);
                RandomGhost(map, ref secondGhostX, ref secondGhostY);
            }

            // Проверяем после перемещения всех
            if ((currentX == ghostX && currentY == ghostY) || (currentX == secondGhostX && currentY == secondGhostY))
            {
                PrintGameOver();
                Console.WriteLine($"YOUR SCORE IS {counter}");
                Thread.Sleep(1000);
                break;
            }
            Thread.Sleep(150);


        }
    }
    private static char[,] ReadMap(string? path)
    {
        string[] lines = File.ReadAllLines(path);
        char[,] map = new char[lines[0].Length, lines.Length];
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                map[i, j] = lines[j][i];
            }
        }

        return map;
    }

    private static void PrintMap(char[,] map)
    {
        for (int y = 0; y < map.GetLength(1); y++)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                Console.Write(map[x, y]);
            }

            Console.WriteLine();
        }
    }

    private static void UpdateCoins(char[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y] != '#')
                {
                    map[x, y] = '•';
                }
            }
        }
    }

    private static bool CheckAvailableCoins(char[,] map)
    {
        foreach (char c in map)
        {
            if (c == '•')
            {
                return true;
            }
        }
        return false;
    }

    private static void MoveRight(char[,] map, ref int currX, int currY)
    {
        Console.SetCursorPosition(currX, currY);
        Console.Write(" ");
        Console.SetCursorPosition(currX + 1, currY);
        currX += 1;
        Console.Write("@");
    }

    private static void MoveLeft(char[,] map, ref int curr_x, int curr_y)
    {
        Console.SetCursorPosition(curr_x, curr_y);
        Console.Write(" ");
        Console.SetCursorPosition(curr_x - 1, curr_y);
        curr_x -= 1;
        Console.Write("@");
    }

    private static void MoveUp(char[,] map, int curr_x, ref int curr_y)
    {
        Console.SetCursorPosition(curr_x, curr_y);
        Console.Write(" ");
        Console.SetCursorPosition(curr_x, curr_y - 1);
        curr_y -= 1;
        Console.Write("@");
    }

    private static void MoveDown(char[,] map, int curr_x, ref int curr_y)
    {
        Console.SetCursorPosition(curr_x, curr_y);
        Console.Write(" ");
        Console.SetCursorPosition(curr_x, curr_y + 1);
        curr_y += 1;
        Console.Write("@");
    }

    private static bool CheckCollision(char[,] map, int curr_x, int curr_y)
    {
        return map[curr_x, curr_y] == '#';
    }

    private static void CoinsCounter(char[,] map, int currX, int currY, ref int counter)
    {
        if (map[currX, currY] == '•')
        {
            counter++;
            map[currX, currY] = ' ';
        }
    }

    private static void PrintGameOver()
    {
        Console.Clear();
        Console.WriteLine(" ██████╗  █████╗ ███╗   ███╗███████╗     ██████╗ ██╗   ██╗███████╗██████╗");
        Thread.Sleep(200);
        Console.WriteLine("██╔════╝ ██╔══██╗████╗ ████║██╔════╝    ██╔═══██╗██║   ██║██╔════╝██╔══██╗");
        Thread.Sleep(200);
        Console.WriteLine("██║  ███╗███████║██╔████╔██║█████╗      ██║   ██║██║   ██║█████╗  ██████╔╝");
        Thread.Sleep(200);
        Console.WriteLine("██║   ██║██╔══██║██║╚██╔╝██║██╔══╝      ██║   ██║██║   ██║██╔══╝  ██╔══██╗");
        Thread.Sleep(200);
        Console.WriteLine("╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗    ╚██████╔╝╚██████╔╝███████╗██║  ██║");
        Thread.Sleep(200);
        Console.WriteLine("╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝     ╚═════╝  ╚═════╝ ╚══════╝╚═╝  ╚═╝\n");
    }

    private static void PrintStart()
    {
        Console.Clear();
        Console.WriteLine(" ██     ██    ███████    ██           ██████     ███████     ███    ███    ███████ ");
        Thread.Sleep(200);
        Console.WriteLine("██     ██    ██         ██         ██          ██     ██    ████  ████    ██      ");
        Thread.Sleep(200);
        Console.WriteLine("██  █  ██    █████      ██         ██          ██     ██    ██ ████ ██    █████   ");
        Thread.Sleep(200);
        Console.WriteLine("██ ███ ██    ██         ██         ██          ██     ██    ██  ██  ██    ██      ");
        Thread.Sleep(200);
        Console.WriteLine(" ███ ███     ███████     ██████      ██████     ███████     ██      ██    ███████ ");
    }
    // GHOST TEST
     private static void MoveToPlayer(char[,] map, ref int ghostX, ref int ghostY, int playerX, int playerY)
     {
         int deltaX = playerX - ghostX;
         int deltaY = playerY - ghostY;
         
         int stepX = deltaX != 0 ? deltaX/Math.Abs(deltaX) : 0;
         int stepY = deltaY != 0 ? deltaY/Math.Abs(deltaY) : 0;

         if (stepX != 0 && map[ghostX + stepX, ghostY] != '#')
         {
             Console.SetCursorPosition(ghostX, ghostY);
             Console.Write(" ");
             ghostX = ghostX + stepX;
             Console.SetCursorPosition(ghostX, ghostY);
             Console.Write("G");
         }
         else if(stepY != 0 && map[ghostX, ghostY + stepY] != '#')
         {
             Console.SetCursorPosition(ghostX, ghostY);
             Console.Write(" ");
             ghostY = ghostY + stepY;
             Console.SetCursorPosition(ghostX, ghostY);
             Console.Write("G");
         }
     }

     private static void RandomGhost(char[,] map, ref int secondGhostX, ref int secondGhostY)
     {
         Random random = new Random();
         List<(int dx, int dy)> directions = new()
         {
             (1, 0),
             (-1, 0),
             (0, 1),
             (0, -1)
         };
         directions = directions.OrderBy(d => random.Next()).ToList();
         foreach ((int dx, int dy) valueTuple in directions)
         {
             int newX = secondGhostX + valueTuple.dx;
             int newY = secondGhostY + valueTuple.dy;
             if (newX >= 0 && newX < map.GetLength(0) &&
                  newY >= 0 && newY < map.GetLength(1) &&
                  map[newX, newY] != '#')
             {
                 // Стираем старое положение
                 Console.SetCursorPosition(secondGhostX, secondGhostY);
                 Console.Write(" ");

                 // Обновляем координаты
                 secondGhostX = newX;
                 secondGhostY = newY;

                 // Отображаем призрака
                 Console.SetCursorPosition(secondGhostX, secondGhostY);
                 Console.Write("G");
                 break;
             }
         }


     }
}


    