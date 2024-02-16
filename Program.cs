﻿using System;
using System.Text;
using System.Threading;

int width = 50;
int height = 30;
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

Console.CursorVisible = false;
List<laserBolt> laserBoltList = new List<laserBolt>();

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
				Thread.Sleep(TimeSpan.FromMilliseconds(60));
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
	const int spaceWidth = 36;
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
				laserBolt bolt = new laserBolt(1, shipPosition);
				laserBoltList.Add(bolt);
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
	laserBoltList.RemoveAll(elem => !elem.isActive());

	for (int i = 0; i < height - 1; i++)
	{
		for (int j = 0; j < width; j++)
		{
			if (scene[i + 1, j] != '|')
				scene[i, j] = scene[i + 1, j];
		}
	}
	int spaceUpdate =
		Random.Shared.Next(5) < 4 ? previousSpaceUpdate :
		Random.Shared.Next(3) - 1;
	if (spaceUpdate is -1 && scene[height - 1, 0] is ' ') spaceUpdate = 1;
	if (spaceUpdate is 1 && scene[height - 1, width - 1] is ' ') spaceUpdate = -1;
	switch (spaceUpdate)
	{
		case -1: // left
			for (int i = 0; i < width - 1; i++)
			{
				scene[height - 1, i] = scene[height - 1, i + 1];
			}
			scene[height - 1, width - 1] = '.';
			break;
		case 1: // right
			for (int i = width - 1; i > 0; i--)
			{
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

	foreach (var bolt in laserBoltList)
	{
		scene[bolt.getY(), bolt.getX()] = ' ';
		bolt.travel(height - 1, scene);
		if (bolt.isActive())
			scene[bolt.getY(), bolt.getX()] = '|';
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



class laserBolt
{
	int yPos;
	int xPos;
	bool active;

	public laserBolt(int y, int x, bool act = true)
	{
		yPos = y;
		xPos = x;
		active = act;
	}

	internal int getX()
	{
		return xPos;
	}

	internal int getY()
	{
		return yPos;
	}
	internal void travel(int height, char [,] scene)
	{
		yPos += 2;
		if(yPos >= height || scene[yPos, xPos] == '.' || scene[yPos - 1, xPos] == '.')
		{
			this.active = false;
		} 
	}

	internal bool isActive()
	{
		return active;
	}
}

