using static SDL2.SDL;

namespace RayCasting
{
    internal class Program
    {
        const int WindowWidth = 1000;
        const int WindowHeight = 1000;

        private bool IsRunning { get; set; }
        private int[,] Map { get; set; }
        private float PlayerX { get; set; }
        private float PlayerY { get; set; }
        private float PlayerAngle { get; set; }
        private float PlayerSpeed { get; set; }
        private int Fov { get; set; }
        private nint Window { get; set; }
        private nint Renderer { get; set; }

        Program()
        {
            IsRunning = true;
            Map = new int[,] {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,0,1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,1},
                {1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,1},
                {1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,0,1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,0,0,0,1,0,1,0,1,0,1,0,1,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,0,0,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1,0,1},
                {1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,0,1,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,1},
                {1,0,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            };
            PlayerX = 150.0f;
            PlayerY = 500.0f;
            PlayerAngle = 360.0f;
            PlayerSpeed = 10.0f;
            Fov = 80;
        }

        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run();
        }

        private void Run()
        {
            SDL_Init(SDL_INIT_VIDEO);

            Window = SDL_CreateWindow(
                "Ray Casting - Matheus Faustino",
                SDL_WINDOWPOS_CENTERED,
                SDL_WINDOWPOS_CENTERED,
                WindowWidth,
                WindowHeight,
                SDL_WindowFlags.SDL_WINDOW_SHOWN
            );

            Renderer = SDL_CreateRenderer(
                Window,
                1,
                SDL_RendererFlags.SDL_RENDERER_SOFTWARE
            );

            while (IsRunning)
            {
                SDL_SetRenderDrawColor(Renderer, 128, 128, 128, 255);
                SDL_RenderClear(Renderer);

                HandleInput();
                DrawMap();
                CastRays();

                SDL_RenderPresent(Renderer);
            }

            SDL_DestroyRenderer(Renderer);
            SDL_DestroyWindow(Window);
            SDL_Quit();
        }

        private void HandleInput()
        {
            SDL_Event e;

            while (SDL_PollEvent(out e) != 0)
            {
                if (e.type == SDL_EventType.SDL_QUIT)
                {
                    IsRunning = false;
                }

                if (e.type == SDL_EventType.SDL_KEYDOWN)
                {
                    if (e.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                    {
                        IsRunning = false;
                    }

                    float newX;
                    float newY;

                    if (e.key.keysym.sym == SDL_Keycode.SDLK_w || e.key.keysym.sym == SDL_Keycode.SDLK_UP)
                    {
                        newX = PlayerX + MathF.Cos(DegreesToRadians(PlayerAngle)) * PlayerSpeed;
                        newY = PlayerY + MathF.Sin(DegreesToRadians(PlayerAngle)) * PlayerSpeed;

                        if (!CheckCollision(newX, newY))
                        {
                            PlayerX = newX;
                            PlayerY = newY;
                        }
                    }

                    if (e.key.keysym.sym == SDL_Keycode.SDLK_s || e.key.keysym.sym == SDL_Keycode.SDLK_DOWN)
                    {
                        newX = PlayerX - MathF.Cos(DegreesToRadians(PlayerAngle)) * PlayerSpeed;
                        newY = PlayerY - MathF.Sin(DegreesToRadians(PlayerAngle)) * PlayerSpeed;

                        if (!CheckCollision(newX, newY))
                        {
                            PlayerX = newX;
                            PlayerY = newY;
                        }
                    }

                    if (e.key.keysym.sym == SDL_Keycode.SDLK_a || e.key.keysym.sym == SDL_Keycode.SDLK_LEFT)
                    {
                        PlayerAngle -= 1.0f;
                        PlayerAngle = (PlayerAngle + 360) % 360;
                    }

                    if (e.key.keysym.sym == SDL_Keycode.SDLK_d || e.key.keysym.sym == SDL_Keycode.SDLK_RIGHT)
                    {
                        PlayerAngle += 1.0f;
                        PlayerAngle = (PlayerAngle + 360) % 360;
                    }
                }
            }
        }

        private void DrawMap()
        {
            for (int row = 0; row < Map.GetLength(0); row++)
            {
                for (int col = 0; col < Map.GetLength(1); col++)
                {
                    if (Map[row, col] != 0)
                    {
                        SDL_SetRenderDrawColor(Renderer, 255, 255, 255, 255);

                        SDL_FRect cell = new SDL_FRect
                        {
                            x = WindowWidth / Map.GetLength(1) * col,
                            y = WindowHeight / Map.GetLength(0) * row,
                            w = WindowWidth / Map.GetLength(1),
                            h = WindowWidth / Map.GetLength(0),
                        };

                        SDL_RenderFillRectF(Renderer, ref cell);
                    }
                }
            }
        }

        private void CastRays()
        {
            for (int i = 0; i < (Fov - 1); i++)
            {
                float rayAngle = PlayerAngle - (Fov / 2) + i;

                float rayTargetX = PlayerX + MathF.Cos(DegreesToRadians(rayAngle)) * WindowWidth;
                float rayTargetY = PlayerY + MathF.Sin(DegreesToRadians(rayAngle)) * WindowWidth;

                var collision = GetCollision(rayTargetX, rayTargetY);

                SDL_SetRenderDrawColor(Renderer, 0, 255, 255, 255);
                SDL_RenderDrawLineF(Renderer, PlayerX, PlayerY, collision.x, collision.y);
            }
        }

        // Utiliza DDA (Digital Differential Analyzer) para determinar os intervalos para checar a colisão com as "paredes" do mapa. 
        private (float x, float y) GetCollision(float rayTargetX, float rayTargetY)
        {
            float diffX = rayTargetX - PlayerX;
            float diffY = rayTargetY - PlayerY;

            int steps = (int)MathF.Max(MathF.Abs(diffX), MathF.Abs(diffY));

            float incrementX = diffX / steps;
            float incrementY = diffY / steps;

            float x = PlayerX;
            float y = PlayerY;

            for (int i = 0; i < steps; i++)
            {
                if (CheckCollision(x, y))
                    break;

                x += incrementX;
                y += incrementY;
            }

            return (x, y);
        }

        private bool CheckCollision(float x, float y)
        {
            int row = (int)y / (WindowHeight / Map.GetLength(0));
            int col = (int)x / (WindowWidth / Map.GetLength(1));

            return Map[row, col] != 0;
        }

        private float DegreesToRadians(float degrees)
        {
            return degrees * (MathF.PI / 180);
        }
    }
}
