﻿using System;
using System.Text;
using System.Threading;
using System.Linq;
using System.Runtime.CompilerServices;

int width = 50;
int height = 30;
const int spaceWidth = 36;
int windowWidth;
int windowHeight;
char[,] scene;
int score = 0;
int shipPosition;
int shipVelocity;
bool gameRunning;
bool keepPlaying = true;
bool consoleSizeError = false;
int previousSpaceUpdate = 0;
Random rnd = new Random();

Console.CursorVisible = false;
List<LaserBolt> LaserBoltList = [];
List<Asteroid> AsteroidList = [];

try
{
	Initialize();
	StartScreen();
	while (keepPlaying)
	{
		InitializeScene();
		while (gameRunning)
		{
			if (Console.WindowHeight < height || Console.WindowWidth < width)
			{
				consoleSizeError = true;
				keepPlaying = false;
				break;
			}
			HandleInput();
			Update();
			Render();
			if (gameRunning)
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(68));
			}
		}
		if (keepPlaying)
		{
			GameOverScreen();
		}
	}
	Console.Clear();
	if (consoleSizeError)
	{
		Console.WriteLine("Console window is too small.");
		Console.WriteLine("Increase the size of the console window.");
	}
	Console.WriteLine("Space shooter was closed.");
}
finally
{
	Console.CursorVisible = true;
}

void Initialize()
{
	windowWidth = Console.WindowWidth;
	windowHeight = Console.WindowHeight;
	if (OperatingSystem.IsWindows())
	{
		if (windowWidth < width && OperatingSystem.IsWindows())
		{
			windowWidth = Console.WindowWidth = width + 1;
		}
		if (windowHeight < height && OperatingSystem.IsWindows())
		{
			windowHeight = Console.WindowHeight = height + 1;
		}
		Console.BufferWidth = windowWidth;
		Console.BufferHeight = windowHeight;
	}
}

void StartScreen()
{
	Console.Clear();
	Console.WriteLine("Welcome to Space shooter game");
	Console.WriteLine();
	Console.WriteLine("Use a,d,w or arrows to control your movement.");
	Console.WriteLine();
	Console.WriteLine("Use spacebar to shoot");
	Console.WriteLine();
	Console.Write("Press enter or space to start...");
	PressEnterToContinue();
}

void InitializeScene()
{
	gameRunning = true;
	shipPosition = width / 2;
	shipVelocity = 0;
	int leftEdge = (width - spaceWidth) / 2;
	int rightEdge = leftEdge + spaceWidth + 1;
	scene = new char[height, width];
	for (int i = 0; i < height; i++)
	{
		for (int j = 0; j < width; j++)
		{
			if (j < leftEdge || j > rightEdge)
			{
				scene[i, j] = '.';
			}
			else
			{
				scene[i, j] = ' ';
			}
		}
	}
}

void Render()
{
	StringBuilder stringBuilder = new(width * height);
	for (int i = height - 1; i >= 0; i--)
	{
		for (int j = 0; j < width; j++)
		{
			if (i is 1 && j == shipPosition)
			{
				stringBuilder.Append(
					!gameRunning ? 'X' :
					shipVelocity < 0 ? '<' :
					shipVelocity > 0 ? '>' :
					'^');
			}
			else
			{
				stringBuilder.Append(scene[i, j]);
			}
		}
		if (i > 0)
		{
			stringBuilder.AppendLine();
		}
	}


	Console.SetCursorPosition(0, 0);
	Console.Write(stringBuilder);
}

void HandleInput()
{
	while (Console.KeyAvailable)
	{
		ConsoleKey key = Console.ReadKey(true).Key;
		switch (key)
		{
			case ConsoleKey.A or ConsoleKey.LeftArrow:
				shipVelocity = -1;
				break;
			case ConsoleKey.D or ConsoleKey.RightArrow:
				shipVelocity = +1;
				break;
			case ConsoleKey.W or ConsoleKey.UpArrow or ConsoleKey.S or ConsoleKey.DownArrow:
				shipVelocity = 0;
				break;
			case ConsoleKey.Escape:
				gameRunning = false;
				keepPlaying = false;
				break;
			case ConsoleKey.Spacebar:
				shipVelocity = 0;
				LaserBolt bolt = new(0, shipPosition);
				LaserBoltList.Add(bolt);
				break;
			case ConsoleKey.Enter:
				Console.ReadLine();
				break;
		}
	}
}

void GameOverScreen()
{
	Console.SetCursorPosition(0, 0);
	Console.WriteLine("Game Over");
	Console.WriteLine($"Score: {score}");
	Console.WriteLine($"Play Again (Y/N)?");
GetInput:
	ConsoleKey key = Console.ReadKey(true).Key;
	switch (key)
	{
		case ConsoleKey.Y:
			keepPlaying = true;
			break;
		case ConsoleKey.N or ConsoleKey.Escape:
			keepPlaying = false;
			break;
		default:
			goto GetInput;
	}
}

void Update()
{
	foreach (var asteroid in AsteroidList)
	{
		//scene[asteroid.GetY(), asteroid.GetX()] = ' ';
		asteroid.Move();
		if (asteroid.CollideWithPlayer(shipPosition))
		{
			gameRunning = false;
			asteroid.Destroy(scene);
		}
		else if (asteroid.CollideWithLaser(scene))
		{
			LaserBoltList.RemoveAll(elem => elem.GetX() == asteroid.GetX() && 
				(elem.GetY() + 1 == asteroid.GetY() || 
				elem.GetY() + 2 == asteroid.GetY()));
			asteroid.Destroy(scene);
			score++;
		}
		else if (asteroid.IsOutside())
		{
			asteroid.Destroy(scene);
		}
	}

	AsteroidList.RemoveAll(elem => !elem.IsActive());
	LaserBoltList.RemoveAll(elem => !elem.IsActive());
	Dictionary<int, int> wages = [];
	

	for (int i = (width - spaceWidth) / 2 + 1; i < (width + spaceWidth) / 2 - 1; i++)
	{
		wages.Add(i, rnd.Next(1, 10));
	}

	List<int> asteroidX = 
		(from wage in wages
			where wage.Value > 8
			select wage.Key
		).ToList();
	wages.Clear();

	for (int i = 0; i < height - 1; i++)
	{
		for (int j = 0; j < width; j++)
		{
			if (scene[i + 1, j] != '|')
				scene[i, j] = scene[i + 1, j];
		}
	}
	int spaceUpdate =
		rnd.Next(5) < 4 ? previousSpaceUpdate :
		rnd.Next(3) - 1;
	if (spaceUpdate is -1 && scene[height - 1, 0] is ' ') spaceUpdate = 1;
	if (spaceUpdate is 1 && scene[height - 1, width - 1] is ' ') spaceUpdate = -1;
	switch (spaceUpdate)
	{
		case -1: // left
			for (int i = 0; i < width - 1; i++)
			{
				if (scene[height - 1, i + 1] != '|' && scene[height - 1, i + 1] != '*')
					scene[height - 1, i] = scene[height - 1, i + 1];
			}
			scene[height - 1, width - 1] = '.';
			break;
		case 1: // right
			for (int i = width - 1; i > 0; i--)
			{
				if (scene[height - 1, i - 1] != '|' && scene[height - 1, i - 1] != '*')
					scene[height - 1, i] = scene[height - 1, i - 1];
			}
			scene[height - 1, 0] = '.';
			break;
	}
	previousSpaceUpdate = spaceUpdate;
	shipPosition += shipVelocity;
	if (shipPosition < 0 || shipPosition >= width || scene[1, shipPosition] is not ' ')
	{
		gameRunning = false;
	}

	foreach (var x in asteroidX)
	{
		if (scene[height - 1, x] != '.' && 
		scene[height - 1, x] != '|' && 
		scene[height - 2, x] != '|' && rnd.Next(1,10) > 8)
		{
			Asteroid ast = new(height - 1, x);
			AsteroidList.Add(ast);
			scene[height - 1, x] = '*';
		}
		
	}

	foreach (var bolt in LaserBoltList)
	{
		scene[bolt.GetY(), bolt.GetX()] = ' ';
		bolt.Travel(height - 1, scene);
		if (bolt.IsActive())
			scene[bolt.GetY(), bolt.GetX()] = '|';
		//else
		//	gameRunning = false;
	}
}

void PressEnterToContinue()
{
GetInput:
	ConsoleKey key = Console.ReadKey(true).Key;
	switch (key)
	{
		case ConsoleKey.Enter:
			break;
		case ConsoleKey.Spacebar:
			break;
		case ConsoleKey.Escape:
			keepPlaying = false;
			break;
		default: goto GetInput;
	}
}



class LaserBolt
{
	int yPos;
	int xPos;
	bool active;

	public LaserBolt(int y, int x, bool act = true)
	{
		yPos = y;
		xPos = x;
		active = act;
	}

	internal int GetX()
	{
		return xPos;
	}

	internal int GetY()
	{
		return yPos;
	}
	internal void Travel(int height, char [,] scene)
	{
		yPos += 2;
		if(yPos >= height || scene[yPos, xPos] == '.' || scene[yPos - 1, xPos] == '.')
		{
			this.active = false;
		} 
	}

	internal bool IsActive()
	{
		return active;
	}

	//internal void Destroy();
}

interface IObstacle
{
	void Move();
	int GetY();
	int GetX();
	bool CollideWithPlayer(int xPlayer);
	bool IsOutside();
	bool CollideWithLaser(char [,] scene);
	bool IsActive();
	void Destroy(char [,] scene);
}

class Asteroid : IObstacle
{
	int yPos;
	int xPos;
	bool isActive;
	public Asteroid (int yPos, int xPos, bool active = true)
	{
		this.yPos = yPos;
		this.xPos = xPos;
		isActive = active;
	}
	public void Move()
	{
		yPos -= 1;
	}

	public int GetY()
	{
		return yPos;
	}

	public int GetX()
	{
		return xPos;
	}

	public bool CollideWithPlayer(int xPlayer)
	{
		return yPos == 2 && xPos == xPlayer;
	}

	public bool IsOutside()
	{
		return (yPos <= 1);
	}

	public bool CollideWithLaser(char [,] scene)
	{
		if (yPos >= 3)
			return scene[yPos, xPos] == '|' || 
			scene[yPos - 1, xPos] == '|' || 
			scene[yPos - 2, xPos] == '|' ||
			scene[yPos - 3, xPos] == '|';
		return false;
	}

	public void Destroy(char [,] scene)
	{
		isActive = false;
		scene[yPos, xPos] = ' ';
	}

	public bool IsActive()
	{
		return isActive;
	}

}

