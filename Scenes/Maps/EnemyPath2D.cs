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

    private Vector2i? GetInitialAndFinalPositions(AStarPathFinder aspf)
    {
        // for (int k = 1; k < 5; k++)
        // {
        //     if (_tileMap!.GetCellSourceId(k, new Vector2i(i, j), true) != -1)
        //     {
        //         aspf.Grid[i, j].IsAccessible = false;
        //     }
        // }
        return null;
    }

    // Returns whether the the path could reach its destination or not
    public bool GeneratePath()
    {
        _pathList.Clear();
        AStarPathFinder aspf = new AStarPathFinder(_tileMap!);
        aspf.GenerateNavMesh();
        GetInitialAndFinalPositions(aspf);
        var path = aspf.FindPath(aspf.Grid[4, 0], aspf.Grid[4, 13]);

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
