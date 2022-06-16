using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EnemyPath2D : Path2D
{
    private NavigationAgent2D? _navAgent;
    private List<Vector2> _pathList = new List<Vector2>();

    [Export]
    NodePath? _tileMapPath;
    private TileMap? _tileMap;

    public sealed override void _Ready()
    {
        _tileMap = GetNode<TileMap>(_tileMapPath);
        // GeneratePath();

        // BuildPath2D(_pathList.ToArray());

        GeneratePath();
    }

    public sealed override void _Process(float delta)
    {
        //GeneratePath();
    }

    // Returns the x index of the free tile at the top and bottom of the game screen. This exists so the path2d can move around if an initial or final tile of it gets blocked.
    private Vector2i? GetInitialAndFinalPositions(AStarPathFinder aspf)
    {
        int topIndex = 0;
        int bottomIndex = 0;

        int j = 4;
        for (float i = 0; i < 9; i++)
        {
            j += (int)(Math.Pow(-1, i) * i);
            if (aspf.Grid[j, 0].IsAccessible)
            {
                topIndex = j;
                break;
            }
        }

        j = 4;
        for (int i = 0; i < 9; i++)
        {
            j += (int)(Math.Pow(-1, i) * i);
            if (aspf.Grid[j, 13].IsAccessible)
            {
                bottomIndex = j;
                break;
            }
        }
        return new Vector2i(topIndex, bottomIndex);
    }

    // Returns whether the the path could reach its destination or not
    public bool GeneratePath()
    {
        _pathList.Clear();
        AStarPathFinder aspf = new AStarPathFinder(_tileMap!);
        aspf.GenerateNavMesh();
        Vector2i? startingPos = GetInitialAndFinalPositions(aspf);
        if (startingPos == null)
        {
            GD.PrintErr("Failed to get (starting/ending) index position of EnemyPath2D");
            return false;
        }
        var path = aspf.FindPath(
            aspf.Grid[startingPos.Value.x, 0],
            aspf.Grid[startingPos.Value.y, 13]
        );

        if (path == null)
        {
            return false;
        }

        _pathList.Add(path![0].Position - new Vector2(0, 64));

        foreach (var item in path)
        {
            _pathList.Add(item.Position);
        }
        _pathList.Add(path![path.Count - 1].Position - new Vector2(0, -64));
        return true;
    }

    public override void _Draw()
    {
        if (_pathList.ToArray().Length >= 2)
        {
            DrawPolyline(_pathList.ToArray(), new Color(150, 0, 0), 0.8f, true);
        }
    }

    public void BuildPath2D()
    {
        Curve2D temp = new Curve2D();
        foreach (var item in _pathList.ToArray())
        {
            temp.AddPoint(item);
        }
        Curve = temp;
        Update();
    }
}
